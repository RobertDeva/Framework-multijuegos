namespace FrameworkMultijuegos.Runtime.Events;

/// <summary>
/// Evaluates and handles an event against an explicit context. Implementations must be
/// deterministic and must not mutate the context or external state.
/// </summary>
public interface IEventHandler<TContext, TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    /// <summary>Returns whether this handler applies to the supplied event and context.</summary>
    bool CanHandle(TContext context, TEvent @event);

    /// <summary>Plans follow-up events and effect intents for an applicable event.</summary>
    EventHandlerResult<TEvent, TEffect> Handle(TContext context, TEvent @event);
}
