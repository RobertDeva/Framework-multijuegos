using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Runtime.Events;

/// <summary>Immutable outputs planned while handling one event.</summary>
public sealed class EventHandlerResult<TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    private EventHandlerResult(
        bool isSuccess,
        GameError? error,
        IEnumerable<TEvent> followUpEvents,
        IEnumerable<TEffect> effects)
    {
        var eventSnapshot = followUpEvents.ToArray();
        var effectSnapshot = effects.ToArray();

        if (eventSnapshot.Any(item => item is null))
            throw new ArgumentException("Follow-up events cannot contain null entries.", nameof(followUpEvents));
        if (effectSnapshot.Any(item => item is null))
            throw new ArgumentException("Effects cannot contain null entries.", nameof(effects));

        IsSuccess = isSuccess;
        Error = error;
        FollowUpEvents = Array.AsReadOnly(eventSnapshot);
        Effects = Array.AsReadOnly(effectSnapshot);
    }

    public bool IsSuccess { get; }
    public GameError? Error { get; }
    public IReadOnlyList<TEvent> FollowUpEvents { get; }
    public IReadOnlyList<TEffect> Effects { get; }

    public static EventHandlerResult<TEvent, TEffect> Success(
        IEnumerable<TEvent>? followUpEvents = null,
        IEnumerable<TEffect>? effects = null) =>
        new(true, null, followUpEvents ?? [], effects ?? []);

    public static EventHandlerResult<TEvent, TEffect> Failure(GameError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(false, error, [], []);
    }
}
