using FrameworkMultijuegos.Core.Actions;
using FrameworkMultijuegos.Core.Conditions;
using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>
/// Executes a bounded, deterministic depth-first tree of actions and postactions.
/// Handlers plan outputs; this pipeline does not dispatch events or apply effects.
/// </summary>
public sealed class ActionExecutionPipeline<TContext, TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    private readonly IReadOnlyDictionary<ActionId, ActionRegistration<TContext, TEvent, TEffect>> _registrations;
    private readonly ActionExecutionLimits _limits;

    public ActionExecutionPipeline(
        IEnumerable<ActionRegistration<TContext, TEvent, TEffect>> registrations,
        ActionExecutionLimits? limits = null)
    {
        ArgumentNullException.ThrowIfNull(registrations);
        _limits = limits ?? ActionExecutionLimits.Default;

        var map = new Dictionary<ActionId, ActionRegistration<TContext, TEvent, TEffect>>();
        foreach (var registration in registrations)
        {
            ArgumentNullException.ThrowIfNull(registration);
            if (!map.TryAdd(registration.Definition.Id, registration))
                throw new ArgumentException(
                    $"An action with identifier '{registration.Definition.Id}' is already registered.",
                    nameof(registrations));
        }

        _registrations = map;
    }

    public ActionExecutionReport<TEvent, TEffect> Execute(ActionInvocation<TContext> root)
    {
        ArgumentNullException.ThrowIfNull(root);

        var attemptedActions = new List<ActionId>();
        var events = new List<TEvent>();
        var effects = new List<TEffect>();
        var activePath = new HashSet<ActionId>();
        var invocationCount = 0;

        var failure = ExecuteNode(
            root,
            depth: 0,
            attemptedActions,
            events,
            effects,
            activePath,
            ref invocationCount);

        return failure is null
            ? new ActionExecutionReport<TEvent, TEffect>(attemptedActions, events, effects, null)
            : new ActionExecutionReport<TEvent, TEffect>(
                attemptedActions,
                [],
                [],
                failure);
    }

    private ActionExecutionFailure? ExecuteNode(
        ActionInvocation<TContext> invocation,
        int depth,
        List<ActionId> attemptedActions,
        List<TEvent> events,
        List<TEffect> effects,
        HashSet<ActionId> activePath,
        ref int invocationCount)
    {
        var actionId = invocation.ActionId;

        if (depth > _limits.MaximumDepth)
        {
            attemptedActions.Add(actionId);
            return Failure(actionId, "action.depth_limit_exceeded",
                "The nested action depth limit was exceeded.", depth);
        }

        if (invocationCount >= _limits.MaximumInvocations)
        {
            attemptedActions.Add(actionId);
            return Failure(actionId, "action.invocation_limit_exceeded",
                "The maximum number of action invocations was exceeded.", depth);
        }

        if (!activePath.Add(actionId))
        {
            attemptedActions.Add(actionId);
            return Failure(actionId, "action.cycle_detected",
                "The action identifier already exists in the active postaction path.", depth);
        }

        invocationCount++;
        attemptedActions.Add(actionId);

        try
        {
            if (!_registrations.TryGetValue(actionId, out var registration))
            {
                return Failure(actionId, "action.not_registered",
                    "No handler is registered for this action identifier.", depth);
            }

            ConditionEvaluation validation;
            try
            {
                validation = registration.Definition.Validate(invocation.Context);
                if (validation is null)
                    throw new InvalidOperationException("An action definition returned a null condition evaluation.");
            }
            catch (Exception)
            {
                return Failure(actionId, "action.condition_evaluation_failed",
                    "The action preconditions could not be evaluated.", depth);
            }

            if (!validation.IsSatisfied)
            {
                return new ActionExecutionFailure(
                    actionId,
                    new GameError("action.preconditions_failed", "One or more action preconditions were not satisfied."),
                    depth,
                    validation);
            }

            ActionHandlerResult<TContext, TEvent, TEffect> handlerResult;
            try
            {
                handlerResult = registration.Handler.Execute(invocation.Context);
                if (handlerResult is null)
                    throw new InvalidOperationException("An action handler returned a null result.");
            }
            catch (Exception)
            {
                return Failure(actionId, "action.handler_failed",
                    "The action handler failed unexpectedly.", depth);
            }

            if (!handlerResult.IsSuccess)
            {
                return new ActionExecutionFailure(
                    actionId,
                    handlerResult.Error ?? new GameError("action.handler_failed", "The action handler reported a failure without an error."),
                    depth);
            }

            events.AddRange(handlerResult.Events);
            effects.AddRange(handlerResult.Effects);

            foreach (var postAction in handlerResult.PostActions)
            {
                var postActionFailure = ExecuteNode(
                    postAction,
                    depth + 1,
                    attemptedActions,
                    events,
                    effects,
                    activePath,
                    ref invocationCount);

                if (postActionFailure is not null)
                    return postActionFailure;
            }

            return null;
        }
        finally
        {
            activePath.Remove(actionId);
        }
    }

    private static ActionExecutionFailure Failure(
        ActionId actionId,
        string code,
        string message,
        int depth) =>
        new(actionId, new GameError(code, message), depth);
}
