namespace FrameworkMultijuegos.Runtime.Events;

/// <summary>Safety limits for one FIFO event-dispatch cascade.</summary>
public sealed record EventDispatchLimits
{
    public const int MaximumSupportedEvents = 10_000;
    public const int MaximumSupportedHandlerInvocations = 100_000;

    public EventDispatchLimits(int maximumEvents = 1_024, int maximumHandlerInvocations = 10_000)
    {
        if (maximumEvents < 1 || maximumEvents > MaximumSupportedEvents)
            throw new ArgumentOutOfRangeException(nameof(maximumEvents),
                $"Maximum events must be between 1 and {MaximumSupportedEvents}.");
        if (maximumHandlerInvocations < 1 ||
            maximumHandlerInvocations > MaximumSupportedHandlerInvocations)
            throw new ArgumentOutOfRangeException(nameof(maximumHandlerInvocations),
                $"Maximum handler invocations must be between 1 and {MaximumSupportedHandlerInvocations}.");

        MaximumEvents = maximumEvents;
        MaximumHandlerInvocations = maximumHandlerInvocations;
    }

    public int MaximumEvents { get; }
    public int MaximumHandlerInvocations { get; }

    public static EventDispatchLimits Default { get; } = new();
}
