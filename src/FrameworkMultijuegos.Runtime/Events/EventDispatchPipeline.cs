using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Runtime.Events;

/// <summary>
/// Dispatches an explicit event batch in FIFO order. Every registered handler is
/// evaluated in registration order for each event; applicable handlers may plan
/// follow-up events and effect intents.
/// </summary>
public sealed class EventDispatchPipeline<TContext, TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    private readonly IReadOnlyList<IEventHandler<TContext, TEvent, TEffect>> _handlers;
    private readonly EventDispatchLimits _limits;

    public EventDispatchPipeline(
        IEnumerable<IEventHandler<TContext, TEvent, TEffect>> handlers,
        EventDispatchLimits? limits = null)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        _limits = limits ?? EventDispatchLimits.Default;

        var handlerSnapshot = handlers.ToArray();
        if (handlerSnapshot.Any(handler => handler is null))
            throw new ArgumentException("Event handlers cannot contain null entries.", nameof(handlers));

        _handlers = Array.AsReadOnly(handlerSnapshot);
    }

    public EventDispatchReport<TEvent, TEffect> Dispatch(
        TContext context,
        IEnumerable<TEvent> initialEvents)
    {
        ArgumentNullException.ThrowIfNull(initialEvents);

        var pendingEvents = new Queue<TEvent>();
        var scheduledEventCount = 0;
        foreach (var initialEvent in initialEvents)
        {
            if (initialEvent is null)
                throw new ArgumentException("Initial events cannot contain null entries.", nameof(initialEvents));

            if (scheduledEventCount >= _limits.MaximumEvents)
            {
                return Failed(
                    [],
                    new EventDispatchFailure(
                        new GameError(
                            "event.dispatch_limit_exceeded",
                            "The maximum number of events in this dispatch cascade was exceeded."),
                        scheduledEventCount,
                        null));
            }

            pendingEvents.Enqueue(initialEvent);
            scheduledEventCount++;
        }

        var dispatchedEvents = new List<TEvent>();
        var followUpEvents = new List<TEvent>();
        var effects = new List<TEffect>();
        var handlerInvocations = 0;

        while (pendingEvents.Count > 0)
        {
            var currentEvent = pendingEvents.Dequeue();
            dispatchedEvents.Add(currentEvent);
            var eventIndex = dispatchedEvents.Count - 1;

            for (var handlerIndex = 0; handlerIndex < _handlers.Count; handlerIndex++)
            {
                if (handlerInvocations >= _limits.MaximumHandlerInvocations)
                {
                    return Failed(
                        dispatchedEvents,
                        new EventDispatchFailure(
                            new GameError(
                                "event.handler_invocation_limit_exceeded",
                                "The maximum number of event-handler invocations was exceeded."),
                            eventIndex,
                            handlerIndex));
                }

                handlerInvocations++;
                var handler = _handlers[handlerIndex];

                bool canHandle;
                try
                {
                    canHandle = handler.CanHandle(context, currentEvent);
                }
                catch (Exception)
                {
                    return Failed(
                        dispatchedEvents,
                        new EventDispatchFailure(
                            new GameError(
                                "event.handler_filter_failed",
                                "An event handler could not evaluate whether it applies."),
                            eventIndex,
                            handlerIndex));
                }

                if (!canHandle)
                    continue;

                EventHandlerResult<TEvent, TEffect> handlerResult;
                try
                {
                    handlerResult = handler.Handle(context, currentEvent);
                }
                catch (Exception)
                {
                    return Failed(
                        dispatchedEvents,
                        new EventDispatchFailure(
                            new GameError(
                                "event.handler_failed",
                                "An event handler failed unexpectedly."),
                            eventIndex,
                            handlerIndex));
                }

                if (handlerResult is null)
                {
                    return Failed(
                        dispatchedEvents,
                        new EventDispatchFailure(
                            new GameError(
                                "event.handler_returned_null",
                                "An event handler returned a null result."),
                            eventIndex,
                            handlerIndex));
                }

                if (!handlerResult.IsSuccess)
                {
                    return Failed(
                        dispatchedEvents,
                        new EventDispatchFailure(
                            handlerResult.Error ?? new GameError(
                                "event.handler_failed",
                                "An event handler reported failure without an error."),
                            eventIndex,
                            handlerIndex));
                }

                if (handlerResult.FollowUpEvents.Count > _limits.MaximumEvents - scheduledEventCount)
                {
                    return Failed(
                        dispatchedEvents,
                        new EventDispatchFailure(
                            new GameError(
                                "event.dispatch_limit_exceeded",
                                "The maximum number of scheduled events in this dispatch cascade would be exceeded."),
                            eventIndex,
                            handlerIndex));
                }

                if (handlerResult.Effects.Count > _limits.MaximumEffects - effects.Count)
                {
                    return Failed(
                        dispatchedEvents,
                        new EventDispatchFailure(
                            new GameError(
                                "event.effect_limit_exceeded",
                                "The maximum number of effect intents in this dispatch cascade was exceeded."),
                            eventIndex,
                            handlerIndex));
                }

                foreach (var followUpEvent in handlerResult.FollowUpEvents)
                {
                    followUpEvents.Add(followUpEvent);
                    pendingEvents.Enqueue(followUpEvent);
                    scheduledEventCount++;
                }

                effects.AddRange(handlerResult.Effects);
            }
        }

        return new EventDispatchReport<TEvent, TEffect>(
            dispatchedEvents,
            followUpEvents,
            effects,
            null);
    }

    private static EventDispatchReport<TEvent, TEffect> Failed(
        IEnumerable<TEvent> dispatchedEvents,
        EventDispatchFailure failure) =>
        new(dispatchedEvents, [], [], failure);
}
