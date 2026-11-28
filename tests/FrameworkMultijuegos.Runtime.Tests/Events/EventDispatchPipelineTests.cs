using FrameworkMultijuegos.Core.Primitives;
using FrameworkMultijuegos.Runtime.Events;

namespace FrameworkMultijuegos.Runtime.Tests.Events;

public sealed class EventDispatchPipelineTests
{
    [Fact]
    public void DispatchesInitialAndFollowUpEventsInFifoAndRegistrationOrder()
    {
        var calls = new List<string>();
        var pipeline = new EventDispatchPipeline<int, string, string>(
        [
            Handler(
                (_, eventName) => eventName == "root-a",
                (_, eventName) =>
                {
                    calls.Add($"first:{eventName}");
                    return EventHandlerResult<string, string>.Success(
                        followUpEvents: ["child"],
                        effects: ["effect.root-a"]);
                }),
            Handler(
                (_, _) => true,
                (_, eventName) =>
                {
                    calls.Add($"second:{eventName}");
                    return EventHandlerResult<string, string>.Success();
                })
        ]);

        var report = pipeline.Dispatch(0, ["root-a", "root-b"]);

        Assert.True(report.IsSuccess);
        Assert.Equal(
            ["first:root-a", "second:root-a", "second:root-b", "second:child"],
            calls);
        Assert.Equal(["root-a", "root-b", "child"], report.DispatchedEvents);
        Assert.Equal(["child"], report.FollowUpEvents);
        Assert.Equal(["effect.root-a"], report.Effects);
        Assert.Null(report.Failure);
    }

    [Fact]
    public void FailureDiscardsAllPlannedFollowUpEventsAndEffects()
    {
        var pipeline = new EventDispatchPipeline<int, string, string>(
        [
            Handler(
                (_, _) => true,
                (_, _) => EventHandlerResult<string, string>.Success(
                    followUpEvents: ["planned"],
                    effects: ["planned-effect"])),
            Handler(
                (_, _) => true,
                (_, _) => EventHandlerResult<string, string>.Failure(
                    new GameError("game.event_rejected", "The event was rejected.")))
        ]);

        var report = pipeline.Dispatch(0, ["input"]);

        Assert.False(report.IsSuccess);
        Assert.Equal("game.event_rejected", report.Failure!.Error.Code);
        Assert.Equal(["input"], report.DispatchedEvents);
        Assert.Empty(report.FollowUpEvents);
        Assert.Empty(report.Effects);
    }

    [Fact]
    public void BoundsSelfProducingEventChains()
    {
        var pipeline = new EventDispatchPipeline<int, string, string>(
        [
            Handler(
                (_, _) => true,
                (_, _) => EventHandlerResult<string, string>.Success(
                    followUpEvents: ["next"],
                    effects: ["planned-effect"]))
        ],
        new EventDispatchLimits(maximumEvents: 3, maximumHandlerInvocations: 10));

        var report = pipeline.Dispatch(0, ["start"]);

        Assert.False(report.IsSuccess);
        Assert.Equal("event.dispatch_limit_exceeded", report.Failure!.Error.Code);
        Assert.Equal(3, report.Failure.EventIndex);
        Assert.Null(report.Failure.HandlerIndex);
        Assert.Equal(["start", "next", "next"], report.DispatchedEvents);
        Assert.Empty(report.FollowUpEvents);
        Assert.Empty(report.Effects);
    }

    [Fact]
    public void BoundsHandlerInvocationsAcrossOneEvent()
    {
        var calls = 0;
        var pipeline = new EventDispatchPipeline<int, string, string>(
        [
            Handler(
                (_, _) => true,
                (_, _) =>
                {
                    calls++;
                    return EventHandlerResult<string, string>.Success(effects: ["planned-effect"]);
                }),
            Handler(
                (_, _) => true,
                (_, _) =>
                {
                    calls++;
                    return EventHandlerResult<string, string>.Success();
                })
        ],
        new EventDispatchLimits(maximumEvents: 10, maximumHandlerInvocations: 1));

        var report = pipeline.Dispatch(0, ["input"]);

        Assert.False(report.IsSuccess);
        Assert.Equal("event.handler_invocation_limit_exceeded", report.Failure!.Error.Code);
        Assert.Equal(0, report.Failure.EventIndex);
        Assert.Equal(1, report.Failure.HandlerIndex);
        Assert.Equal(1, calls);
        Assert.Empty(report.Effects);
    }

    [Fact]
    public void ConvertsFilterExceptionsToStableFailures()
    {
        var pipeline = new EventDispatchPipeline<int, string, string>(
        [
            Handler(
                (_, _) => throw new InvalidOperationException("private filter detail"),
                (_, _) => EventHandlerResult<string, string>.Success())
        ]);

        var report = pipeline.Dispatch(0, ["input"]);

        Assert.False(report.IsSuccess);
        Assert.Equal("event.handler_filter_failed", report.Failure!.Error.Code);
        Assert.DoesNotContain("private filter detail", report.Failure.Error.Message);
    }

    [Fact]
    public void ConvertsHandlerExceptionsToStableFailures()
    {
        var pipeline = new EventDispatchPipeline<int, string, string>(
        [
            Handler(
                (_, _) => true,
                (_, _) => throw new InvalidOperationException("private handler detail"))
        ]);

        var report = pipeline.Dispatch(0, ["input"]);

        Assert.False(report.IsSuccess);
        Assert.Equal("event.handler_failed", report.Failure!.Error.Code);
        Assert.DoesNotContain("private handler detail", report.Failure.Error.Message);
    }

    [Fact]
    public void EmptyInputSucceedsWithoutInvokingHandlers()
    {
        var calls = 0;
        var pipeline = new EventDispatchPipeline<int, string, string>(
        [
            Handler(
                (_, _) =>
                {
                    calls++;
                    return true;
                },
                (_, _) => EventHandlerResult<string, string>.Success())
        ]);

        var report = pipeline.Dispatch(0, []);

        Assert.True(report.IsSuccess);
        Assert.Empty(report.DispatchedEvents);
        Assert.Empty(report.FollowUpEvents);
        Assert.Empty(report.Effects);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void RejectsUnsafeDispatchLimitsAndNullEntries()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EventDispatchLimits(0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EventDispatchLimits(EventDispatchLimits.MaximumSupportedEvents + 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EventDispatchLimits(1, 0));

        Assert.Throws<ArgumentException>(() =>
            new EventDispatchPipeline<int, string, string>([null!]));

        Assert.Throws<ArgumentException>(() =>
            EventHandlerResult<string, string>.Success(followUpEvents: [null!]));

        var pipeline = new EventDispatchPipeline<int, string, string>([]);
        Assert.Throws<ArgumentException>(() => pipeline.Dispatch(0, [null!]));
    }

    private static DelegateEventHandler Handler(
        Func<int, string, bool> canHandle,
        Func<int, string, EventHandlerResult<string, string>> handle) =>
        new(canHandle, handle);

    private sealed class DelegateEventHandler(
        Func<int, string, bool> canHandle,
        Func<int, string, EventHandlerResult<string, string>> handle)
        : IEventHandler<int, string, string>
    {
        public bool CanHandle(int context, string @event) => canHandle(context, @event);

        public EventHandlerResult<string, string> Handle(int context, string @event) =>
            handle(context, @event);
    }
}
