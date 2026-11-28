namespace FrameworkMultijuegos.Runtime.Events;

/// <summary>
/// Immutable trace and planned outputs from an event-dispatch cascade.
/// Follow-up events and effects are exposed only when the whole cascade succeeds.
/// </summary>
public sealed class EventDispatchReport<TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    internal EventDispatchReport(
        IEnumerable<TEvent> dispatchedEvents,
        IEnumerable<TEvent> followUpEvents,
        IEnumerable<TEffect> effects,
        EventDispatchFailure? failure)
    {
        DispatchedEvents = Array.AsReadOnly(dispatchedEvents.ToArray());
        FollowUpEvents = Array.AsReadOnly(followUpEvents.ToArray());
        Effects = Array.AsReadOnly(effects.ToArray());
        Failure = failure;
    }

    public bool IsSuccess => Failure is null;
    public IReadOnlyList<TEvent> DispatchedEvents { get; }
    public IReadOnlyList<TEvent> FollowUpEvents { get; }
    public IReadOnlyList<TEffect> Effects { get; }
    public EventDispatchFailure? Failure { get; }
}
