using FrameworkMultijuegos.Core.Actions;
using FrameworkMultijuegos.Core.Conditions;
using FrameworkMultijuegos.Core.Primitives;

namespace FrameworkMultijuegos.Core.Tests.Actions;

public sealed class ActionDefinitionTests
{
    [Fact]
    public void ActionDefinition_ExposesIdentifierAndValidatesPreconditions()
    {
        var conditions = new ConditionSet<int>([new MinimumValueCondition(10)]);
        var action = new ActionDefinition<int>(new ActionId("combat.heavy_attack"), conditions);

        var accepted = action.Validate(12);
        var rejected = action.Validate(4);

        Assert.Equal("combat.heavy_attack", action.Id.Value);
        Assert.True(accepted.IsSatisfied);
        Assert.False(rejected.IsSatisfied);
        Assert.Equal("condition.minimum_not_met", rejected.Failures[0].Error.Code);
    }

    [Fact]
    public void InvalidIdentifier_IsRejected()
    {
        Assert.Throws<ArgumentException>(() =>
            new ActionDefinition<int>(default, new ConditionSet<int>([])));
    }

    [Fact]
    public void ActionId_TrimsWhitespace()
    {
        Assert.Equal("combat.attack", new ActionId(" combat.attack ").Value);
    }

    private sealed class MinimumValueCondition(int minimum) : IGameCondition<int>
    {
        public ConditionEvaluation Evaluate(int context) =>
            context >= minimum
                ? ConditionEvaluation.Satisfied()
                : ConditionEvaluation.Rejected(new ConditionFailure(
                    new GameError("condition.minimum_not_met", "The minimum value was not met.")));
    }
}
