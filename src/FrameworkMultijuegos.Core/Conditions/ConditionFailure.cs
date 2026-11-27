using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Core.Conditions;

/// <summary>A failed condition with a stable code and human-readable explanation.</summary>
public sealed record ConditionFailure
{
    public ConditionFailure(GameError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Error = error;
    }

    public GameError Error { get; }
}
