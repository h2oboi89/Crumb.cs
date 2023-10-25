namespace Crumb.Core.Evaluating.Nodes;
internal class StringNode : Node
{
    public new string Value => (string)base.Value;

    public StringNode(string value) : base(NodeTypes.String, value) { }

    protected override bool ValueEquals(object otherValue) =>
        Value == (string)otherValue;

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
