using FrameworkMultijuegos.Core.Conditions;
using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Core.Tests.Conditions;

public sealed class ConditionSetTests
{
    [Fact]
    public void EmptyConditionSet_IsSatisfied()
    {
        var result = new ConditionSet<int>([]).Evaluate(42);

        Assert.True(result.IsSatisfied);
        Assert.Empty(result.Failures);
    }

    [Fact]
    public void AllConditions_AreEvaluatedInDeclarationOrder()
    {
        var calls = new List<int>();
        var set = new ConditionSet<int>(
        [
            new RecordingCondition(1, calls, succeeds: false),
            new RecordingCondition(2, calls, succeeds: true),
            new RecordingCondition(3, calls, succeeds: false)
        ]);

        var result = set.Evaluate(0);

        Assert.Equal([1, 2, 3], calls);
        Assert.False(result.IsSatisfied);
        Assert.Equal(["condition.1", "condition.3"], result.Failures.Select(x => x.Error.Code));
    }

    [Fact]
    public void FailureCollection_IsReadOnlySnapshot()
    {
        var source = new List<ConditionFailure>
        {
            new(new GameError("condition.failed", "Rejected"))
        };

        var evaluation = ConditionEvaluation.FromFailures(source);
        source.Clear();

        Assert.Single(evaluation.Failures);
    }

    [Fact]
    public void NullCondition_IsRejected()
    {
        IGameCondition<int>[] conditions = [null!];

        Assert.Throws<ArgumentException>(() => new ConditionSet<int>(conditions));
    }

    [Fact]
    public void NullEvaluation_IsRejected()
    {
        var set = new ConditionSet<int>([new NullEvaluationCondition()]);

        Assert.Throws<InvalidOperationException>(() => set.Evaluate(0));
    }

    private sealed class RecordingCondition(int number, List<int> calls, bool succeeds)
        : IGameCondition<int>
    {
        public ConditionEvaluation Evaluate(int context)
        {
            calls.Add(number);
            return succeeds
                ? ConditionEvaluation.Satisfied()
                : ConditionEvaluation.Rejected(new ConditionFailure(
                    new GameError($"condition.{number}", $"Condition {number} failed.")));
        }
    }

    private sealed class NullEvaluationCondition : IGameCondition<int>
    {
        public ConditionEvaluation Evaluate(int context) => null!;
    }
}
