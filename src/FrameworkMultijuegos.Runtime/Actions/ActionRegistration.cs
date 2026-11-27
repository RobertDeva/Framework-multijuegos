using FrameworkMultijuegos.Core.Actions;

namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>Pairs an action's precondition definition with its execution handler.</summary>
public sealed class ActionRegistration<TContext, TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    public ActionRegistration(
        ActionDefinition<TContext> definition,
        IActionHandler<TContext, TEvent, TEffect> handler)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(handler);

        Definition = definition;
        Handler = handler;
    }

    public ActionDefinition<TContext> Definition { get; }
    public IActionHandler<TContext, TEvent, TEffect> Handler { get; }
}
