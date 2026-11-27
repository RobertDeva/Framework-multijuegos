using FrameworkMultijuegos.Core.Actions;
using FrameworkMultijuegos.Core.Conditions;
using FrameworkMultijuegos.Core.Primitives;
using FrameworkMultijuegos.Runtime.Actions;

namespace FrameworkMultijuegos.Runtime.Tests.Actions;

public sealed class ActionExecutionPipelineTests
{
    [Fact]
    public void ExecutesMultipleNestedPostactionsInDeterministicDepthFirstOrder()
    {
        var pipeline = new ActionExecutionPipeline<int, string, string>(
        [
            Registration("root", _ => ActionHandlerResult<int, string, string>.Success(
                [
                    Invoke("child-a", 2),
                    Invoke("child-b", 4)
                ],
                ["event.root"],
                ["effect.root"])),
            Registration("child-a", _ => ActionHandlerResult<int, string, string>.Success(
                [Invoke("grandchild", 3)],
                ["event.child-a"],
                ["effect.child-a"])),
            Registration("grandchild", _ => ActionHandlerResult<int, string, string>.Success(
                events: ["event.grandchild"],
                effects: ["effect.grandchild"])),
            Registration("child-b", _ => ActionHandlerResult<int, string, string>.Success(
                events: ["event.child-b"],
                effects: ["effect.child-b"]))
        ]);

        var report = pipeline.Execute(Invoke("root", 1));

        Assert.True(report.IsSuccess);
        Assert.Equal(
            ["root", "child-a", "grandchild", "child-b"],
            report.AttemptedActions.Select(id => id.Value));
        Assert.Equal(
            ["event.root", "event.child-a", "event.grandchild", "event.child-b"],
            report.Events);
        Assert.Equal(
            ["effect.root", "effect.child-a", "effect.grandchild", "effect.child-b"],
            report.Effects);
        Assert.Null(report.Failure);
    }

    [Fact]
    public void RepeatedSiblingActionsAreAllowed()
    {
        var pipeline = new ActionExecutionPipeline<int, string, string>(
        [
            Registration("root", _ => ActionHandlerResult<int, string, string>.Success(
                [Invoke("child", 2), Invoke("child", 3)])),
            Registration("child", _ => ActionHandlerResult<int, string, string>.Success())
        ]);

        var report = pipeline.Execute(Invoke("root", 1));

        Assert.True(report.IsSuccess);
        Assert.Equal(["root", "child", "child"], report.AttemptedActions.Select(id => id.Value));
    }

    [Fact]
    public void RejectsAnActionRepeatedInItsActiveAncestorPath()
    {
        var pipeline = new ActionExecutionPipeline<int, string, string>(
        [
            Registration("root", _ => ActionHandlerResult<int, string, string>.Success([Invoke("child", 2)])),
            Registration("child", _ => ActionHandlerResult<int, string, string>.Success([Invoke("root", 3)]))
        ]);

        var report = pipeline.Execute(Invoke("root", 1));

        Assert.False(report.IsSuccess);
        Assert.Equal("action.cycle_detected", report.Failure!.Error.Code);
        Assert.Equal(2, report.Failure.Depth);
        Assert.Empty(report.Events);
        Assert.Empty(report.Effects);
    }

    [Fact]
    public void EnforcesMaximumNestedDepth()
    {
        var pipeline = new ActionExecutionPipeline<int, string, string>(
        [
            Registration("root", _ => ActionHandlerResult<int, string, string>.Success([Invoke("child", 2)])),
            Registration("child", _ => ActionHandlerResult<int, string, string>.Success([Invoke("grandchild", 3)])),
            Registration("grandchild", _ => ActionHandlerResult<int, string, string>.Success())
        ],
        new ActionExecutionLimits(maximumDepth: 1, maximumInvocations: 10));

        var report = pipeline.Execute(Invoke("root", 1));

        Assert.False(report.IsSuccess);
        Assert.Equal("action.depth_limit_exceeded", report.Failure!.Error.Code);
        Assert.Empty(report.Events);
    }

    [Fact]
    public void EnforcesMaximumInvocationCount()
    {
        var pipeline = new ActionExecutionPipeline<int, string, string>(
        [
            Registration("root", _ => ActionHandlerResult<int, string, string>.Success([Invoke("child", 2)])),
            Registration("child", _ => ActionHandlerResult<int, string, string>.Success([Invoke("grandchild", 3)])),
            Registration("grandchild", _ => ActionHandlerResult<int, string, string>.Success())
        ],
        new ActionExecutionLimits(maximumDepth: 10, maximumInvocations: 2));

        var report = pipeline.Execute(Invoke("root", 1));

        Assert.False(report.IsSuccess);
        Assert.Equal("action.invocation_limit_exceeded", report.Failure!.Error.Code);
        Assert.Empty(report.Effects);
    }

    [Fact]
    public void FailedPreconditionsPreventHandlerExecution()
    {
        var handlerCalls = 0;
        var registration = new ActionRegistration<int, string, string>(
            new ActionDefinition<int>(
                new ActionId("guarded"),
                new ConditionSet<int>([new MinimumValueCondition(5)])),
            new DelegateHandler(_ =>
            {
                handlerCalls++;
                return ActionHandlerResult<int, string, string>.Success();
            }));
        var pipeline = new ActionExecutionPipeline<int, string, string>([registration]);

        var report = pipeline.Execute(Invoke("guarded", 3));

        Assert.False(report.IsSuccess);
        Assert.Equal("action.preconditions_failed", report.Failure!.Error.Code);
        Assert.NotNull(report.Failure.ConditionEvaluation);
        Assert.Single(report.Failure.ConditionEvaluation!.Failures);
        Assert.Equal(0, handlerCalls);
    }

    [Fact]
    public void FailureInNestedActionDiscardsAllPlannedEventsAndEffects()
    {
        var pipeline = new ActionExecutionPipeline<int, string, string>(
        [
            Registration("root", _ => ActionHandlerResult<int, string, string>.Success(
                [Invoke("child", 2)],
                ["event.root"],
                ["effect.root"])),
            Registration("child", _ => ActionHandlerResult<int, string, string>.Failure(
                new GameError("game.child_failed", "The child action failed.")))
        ]);

        var report = pipeline.Execute(Invoke("root", 1));

        Assert.False(report.IsSuccess);
        Assert.Equal("game.child_failed", report.Failure!.Error.Code);
        Assert.Empty(report.Events);
        Assert.Empty(report.Effects);
        Assert.Equal(["root", "child"], report.AttemptedActions.Select(id => id.Value));
    }

    [Fact]
    public void ConvertsUnexpectedHandlerExceptionToFailure()
    {
        var pipeline = new ActionExecutionPipeline<int, string, string>(
        [
            Registration("throws", _ => throw new InvalidOperationException("private detail"))
        ]);

        var report = pipeline.Execute(Invoke("throws", 1));

        Assert.False(report.IsSuccess);
        Assert.Equal("action.handler_failed", report.Failure!.Error.Code);
        Assert.DoesNotContain("private detail", report.Failure.Error.Message);
    }

    [Fact]
    public void RejectsDuplicateRegistrationsAndUnknownInvocations()
    {
        var registration = Registration("same", _ => ActionHandlerResult<int, string, string>.Success());

        Assert.Throws<ArgumentException>(() =>
            new ActionExecutionPipeline<int, string, string>([registration, registration]));

        var pipeline = new ActionExecutionPipeline<int, string, string>([]);
        var report = pipeline.Execute(Invoke("missing", 1));

        Assert.False(report.IsSuccess);
        Assert.Equal("action.not_registered", report.Failure!.Error.Code);
    }

    [Fact]
    public void RejectsUnsafeExecutionLimits()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ActionExecutionLimits(-1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ActionExecutionLimits(ActionExecutionLimits.MaximumSupportedDepth + 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ActionExecutionLimits(0, 0));
    }

    private static ActionInvocation<int> Invoke(string id, int context) =>
        new(new ActionId(id), context);

    private static ActionRegistration<int, string, string> Registration(
        string id,
        Func<int, ActionHandlerResult<int, string, string>> execute) =>
        new(
            new ActionDefinition<int>(new ActionId(id), new ConditionSet<int>([])),
            new DelegateHandler(execute));

    private sealed class DelegateHandler(
        Func<int, ActionHandlerResult<int, string, string>> execute)
        : IActionHandler<int, string, string>
    {
        public ActionHandlerResult<int, string, string> Execute(int context) => execute(context);
    }

    private sealed class MinimumValueCondition(int minimum) : IGameCondition<int>
    {
        public ConditionEvaluation Evaluate(int context) =>
            context >= minimum
                ? ConditionEvaluation.Satisfied()
                : ConditionEvaluation.Rejected(new ConditionFailure(
                    new GameError("condition.minimum_not_met", "The minimum value was not met.")));
    }
}
