namespace FrameworkMultijuegos.Core.Actions;

/// <summary>Stable identifier for an action definition within a game.</summary>
public readonly record struct ActionId
{
    public ActionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim();
    }

    public string Value { get; }
    public bool IsValid => !string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;
}
