using FrameworkMultijuegos.Core.Actions;

namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>A single invocation of a registered action with its explicit context.</summary>
public sealed class ActionInvocation<TContext>
{
    public ActionInvocation(ActionId actionId, TContext context)
    {
        if (!actionId.IsValid)
            throw new ArgumentException("A valid action identifier is required.", nameof(actionId));

        ActionId = actionId;
        Context = context;
    }

    public ActionId ActionId { get; }
    public TContext Context { get; }
}
