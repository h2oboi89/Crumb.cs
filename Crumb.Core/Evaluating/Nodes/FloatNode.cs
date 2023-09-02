namespace Crumb.Core.Evaluating.Nodes;
internal class FloatNode : Node
{
#pragma warning disable CS8605 // Unboxing a possibly null value.
    public new double Value => (double)base.Value;
#pragma warning restore CS8605 // Unboxing a possibly null value.

    public FloatNode(double value) : base(NodeTypes.Float, value) { }

#pragma warning disable CS8605 // Unboxing a possibly null value.
    protected override bool ValueEquals(object? otherValue) =>
        Value == (double)otherValue;
#pragma warning restore CS8605 // Unboxing a possibly null value.

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}

