namespace FrameworkMultijuegos.Core.Conditions;

/// <summary>
/// A predicate over an explicit context. Implementations should be deterministic
/// and side-effect-free; state changes belong to action execution.
/// </summary>
public interface IGameCondition<in TContext>
{
    ConditionEvaluation Evaluate(TContext context);
}
