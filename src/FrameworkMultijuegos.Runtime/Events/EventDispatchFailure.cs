using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Runtime.Events;

/// <summary>Failure information for one event-dispatch cascade.</summary>
public sealed record EventDispatchFailure
{
    public EventDispatchFailure(GameError error, int eventIndex, int? handlerIndex)
    {
        ArgumentNullException.ThrowIfNull(error);
        ArgumentOutOfRangeException.ThrowIfNegative(eventIndex);
        if (handlerIndex.HasValue)
            ArgumentOutOfRangeException.ThrowIfNegative(handlerIndex.Value);

        Error = error;
        EventIndex = eventIndex;
        HandlerIndex = handlerIndex;
    }

    public GameError Error { get; }
    public int EventIndex { get; }
    public int? HandlerIndex { get; }
}
