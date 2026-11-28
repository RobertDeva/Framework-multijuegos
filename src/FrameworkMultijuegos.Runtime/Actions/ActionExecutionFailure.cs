using FrameworkMultijuegos.Core.Actions;
using FrameworkMultijuegos.Core.Conditions;
using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>Failure information for an action invocation in an execution tree.</summary>
public sealed record ActionExecutionFailure
{
    public ActionExecutionFailure(
        ActionId actionId,
        GameError error,
        int depth,
        ConditionEvaluation? conditionEvaluation = null)
    {
        if (!actionId.IsValid)
            throw new ArgumentException("A valid action identifier is required.", nameof(actionId));
        ArgumentNullException.ThrowIfNull(error);
        ArgumentOutOfRangeException.ThrowIfNegative(depth);

        ActionId = actionId;
        Error = error;
        Depth = depth;
        ConditionEvaluation = conditionEvaluation;
    }

    public ActionId ActionId { get; }
    public GameError Error { get; }
    public int Depth { get; }
    public ConditionEvaluation? ConditionEvaluation { get; }
}
