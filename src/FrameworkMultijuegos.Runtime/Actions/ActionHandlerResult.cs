using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>
/// Immutable outputs from planning one action: nested postactions, events and effect intents.
/// A failed result cannot publish partial outputs.
/// </summary>
public sealed class ActionHandlerResult<TContext, TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    private ActionHandlerResult(
        bool isSuccess,
        GameError? error,
        IEnumerable<ActionInvocation<TContext>> postActions,
        IEnumerable<TEvent> events,
        IEnumerable<TEffect> effects)
    {
        var postActionSnapshot = postActions.ToArray();
        var eventSnapshot = events.ToArray();
        var effectSnapshot = effects.ToArray();

        if (postActionSnapshot.Any(item => item is null))
            throw new ArgumentException("Postactions cannot contain null entries.", nameof(postActions));
        if (eventSnapshot.Any(item => item is null))
            throw new ArgumentException("Events cannot contain null entries.", nameof(events));
        if (effectSnapshot.Any(item => item is null))
            throw new ArgumentException("Effects cannot contain null entries.", nameof(effects));

        IsSuccess = isSuccess;
        Error = error;
        PostActions = Array.AsReadOnly(postActionSnapshot);
        Events = Array.AsReadOnly(eventSnapshot);
        Effects = Array.AsReadOnly(effectSnapshot);
    }

    public bool IsSuccess { get; }
    public GameError? Error { get; }
    public IReadOnlyList<ActionInvocation<TContext>> PostActions { get; }
    public IReadOnlyList<TEvent> Events { get; }
    public IReadOnlyList<TEffect> Effects { get; }

    public static ActionHandlerResult<TContext, TEvent, TEffect> Success(
        IEnumerable<ActionInvocation<TContext>>? postActions = null,
        IEnumerable<TEvent>? events = null,
        IEnumerable<TEffect>? effects = null) =>
        new(true, null, postActions ?? [], events ?? [], effects ?? []);

    public static ActionHandlerResult<TContext, TEvent, TEffect> Failure(GameError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(false, error, [], [], []);
    }
}
