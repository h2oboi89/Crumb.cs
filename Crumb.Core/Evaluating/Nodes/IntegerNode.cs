namespace Crumb.Core.Evaluating.Nodes;
public class IntegerNode : Node
{
#pragma warning disable CS8605 // Unboxing a possibly null value.
    public new int Value => (int)base.Value;
#pragma warning restore CS8605 // Unboxing a possibly null value.

    public IntegerNode(int value) : base(NodeTypes.Integer, value) { }

#pragma warning disable CS8605 // Unboxing a possibly null value.
    protected override bool ValueEquals(object? otherValue) =>
        Value == (int)otherValue;
#pragma warning restore CS8605 // Unboxing a possibly null value.

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}