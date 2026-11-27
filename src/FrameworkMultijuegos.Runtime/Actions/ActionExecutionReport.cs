using FrameworkMultijuegos.Core.Actions;

namespace FrameworkMultijuegos.Runtime.Actions;

/// <summary>
/// Immutable report for an action tree. Outputs are published only when the whole
/// tree succeeds; attempted action identifiers remain available for diagnostics.
/// </summary>
public sealed class ActionExecutionReport<TEvent, TEffect>
    where TEvent : notnull
    where TEffect : notnull
{
    internal ActionExecutionReport(
        IEnumerable<ActionId> attemptedActions,
        IEnumerable<TEvent> events,
        IEnumerable<TEffect> effects,
        ActionExecutionFailure? failure)
    {
        AttemptedActions = Array.AsReadOnly(attemptedActions.ToArray());
        Events = Array.AsReadOnly(events.ToArray());
        Effects = Array.AsReadOnly(effects.ToArray());
        Failure = failure;
    }

    public bool IsSuccess => Failure is null;
    public IReadOnlyList<ActionId> AttemptedActions { get; }
    public IReadOnlyList<TEvent> Events { get; }
    public IReadOnlyList<TEffect> Effects { get; }
    public ActionExecutionFailure? Failure { get; }
}
