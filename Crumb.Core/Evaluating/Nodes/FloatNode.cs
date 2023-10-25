namespace Crumb.Core.Evaluating.Nodes;
internal class FloatNode : Node
{
    public new double Value => (double)base.Value;

    public FloatNode(double value) : base(NodeTypes.Float, value) { }

    protected override bool ValueEquals(object otherValue) =>
        Value == (double)otherValue;

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}

