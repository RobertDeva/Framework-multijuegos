namespace FrameworkMultijuegos.Core.Conditions;

/// <summary>
/// Evaluates conditions in declaration order and collects every rejection.
/// Evaluation does not short-circuit; conditions must not mutate the context.
/// </summary>
public sealed class ConditionSet<TContext>
{
    private readonly IGameCondition<TContext>[] _conditions;
    private readonly IReadOnlyList<IGameCondition<TContext>> _readOnlyConditions;

    public ConditionSet(IEnumerable<IGameCondition<TContext>> conditions)
    {
        ArgumentNullException.ThrowIfNull(conditions);
        _conditions = conditions.ToArray();

        if (_conditions.Any(condition => condition is null))
            throw new ArgumentException("A condition set cannot contain null entries.", nameof(conditions));

        _readOnlyConditions = Array.AsReadOnly(_conditions);
    }

    public IReadOnlyList<IGameCondition<TContext>> Conditions => _readOnlyConditions;

    public ConditionEvaluation Evaluate(TContext context)
    {
        var failures = new List<ConditionFailure>();

        foreach (var condition in _conditions)
        {
            var result = condition.Evaluate(context) ??
                throw new InvalidOperationException("A condition returned a null evaluation.");

            failures.AddRange(result.Failures);
        }

        return ConditionEvaluation.FromFailures(failures);
    }
}
