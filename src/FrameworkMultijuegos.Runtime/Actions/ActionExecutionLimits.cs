namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>Safety limits for one depth-first action execution tree.</summary>
public sealed record ActionExecutionLimits
{
    public const int MaximumSupportedDepth = 64;
    public const int MaximumSupportedInvocations = 10_000;

    public ActionExecutionLimits(int maximumDepth = 32, int maximumInvocations = 256)
    {
        if (maximumDepth < 0 || maximumDepth > MaximumSupportedDepth)
            throw new ArgumentOutOfRangeException(nameof(maximumDepth),
                $"Maximum depth must be between 0 and {MaximumSupportedDepth}.");
        if (maximumInvocations < 1 || maximumInvocations > MaximumSupportedInvocations)
            throw new ArgumentOutOfRangeException(nameof(maximumInvocations),
                $"Maximum invocations must be between 1 and {MaximumSupportedInvocations}.");

        MaximumDepth = maximumDepth;
        MaximumInvocations = maximumInvocations;
    }

    public int MaximumDepth { get; }
    public int MaximumInvocations { get; }

    public static ActionExecutionLimits Default { get; } = new();
}
