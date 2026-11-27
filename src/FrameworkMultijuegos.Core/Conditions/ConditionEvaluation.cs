namespace FrameworkMultijuegos.Core.Conditions;

/// <summary>Immutable snapshot of the outcome of evaluating one or more conditions.</summary>
public sealed class ConditionEvaluation
{
    private readonly IReadOnlyList<ConditionFailure> _failures;

    private ConditionEvaluation(IEnumerable<ConditionFailure> failures)
    {
        var snapshot = failures.ToArray();
        if (snapshot.Any(failure => failure is null))
            throw new ArgumentException("A condition evaluation cannot contain null failures.", nameof(failures));

        _failures = Array.AsReadOnly(snapshot);
    }

    public bool IsSatisfied => _failures.Count == 0;
    public IReadOnlyList<ConditionFailure> Failures => _failures;

    public static ConditionEvaluation Satisfied() => new([]);

    public static ConditionEvaluation Rejected(ConditionFailure failure)
    {
        ArgumentNullException.ThrowIfNull(failure);
        return new([failure]);
    }

    public static ConditionEvaluation FromFailures(IEnumerable<ConditionFailure> failures)
    {
        ArgumentNullException.ThrowIfNull(failures);
        return new(failures);
    }
}
