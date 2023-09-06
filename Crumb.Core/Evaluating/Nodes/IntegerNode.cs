namespace Crumb.Core.Evaluating.Nodes;
public class IntegerNode : Node
{
    public new int Value => (int)base.Value;

    public IntegerNode(int value) : base(NodeTypes.Integer, value) { }

    protected override bool ValueEquals(object otherValue) =>
        Value == (int)otherValue;

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}