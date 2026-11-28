using FrameworkMultijuegos.Core.Conditions;

namespace FrameworkMultijuegos.Core.Actions;

/// <summary>
/// Describes an action and its preconditions. It does not execute the action
/// or mutate game state; execution belongs to a runtime or game-specific handler.
/// </summary>
public sealed class ActionDefinition<TContext>
{
    public ActionDefinition(ActionId id, ConditionSet<TContext> conditions)
    {
        if (!id.IsValid)
            throw new ArgumentException("A valid action identifier is required.", nameof(id));

        ArgumentNullException.ThrowIfNull(conditions);

        Id = id;
        Conditions = conditions;
    }

    public ActionId Id { get; }
    public ConditionSet<TContext> Conditions { get; }

    public ConditionEvaluation Validate(TContext context) => Conditions.Evaluate(context);
}
