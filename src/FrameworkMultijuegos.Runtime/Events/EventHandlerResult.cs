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
        var eventSnapshot = Snapshot(
            followUpEvents,
            EventDispatchLimits.MaximumSupportedEvents,
            nameof(followUpEvents));
        var effectSnapshot = Snapshot(
            effects,
            EventDispatchLimits.MaximumSupportedEffects,
            nameof(effects));

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

    private static T[] Snapshot<T>(IEnumerable<T> values, int maximumCount, string parameterName)
        where T : notnull
    {
        var snapshot = new List<T>();
        foreach (var value in values)
        {
            if (value is null)
                throw new ArgumentException("Output collections cannot contain null entries.", parameterName);
            if (snapshot.Count >= maximumCount)
                throw new ArgumentException("The handler output collection exceeds the supported limit.", parameterName);

            snapshot.Add(value);
        }

        return snapshot.ToArray();
    }
}
