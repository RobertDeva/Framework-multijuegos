namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>
/// Plans an action from an explicit context. Implementations must be deterministic
/// and must not mutate the context or external game state. State changes are returned
/// as typed effects for a separate effect-application stage.
/// </summary>
public interface IActionHandler<TContext, TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    ActionHandlerResult<TContext, TEvent, TEffect> Execute(TContext context);
}
