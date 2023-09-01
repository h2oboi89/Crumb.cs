namespace Crumb.Core.Evaluating.Nodes;
internal class StringNode : Node
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
    public new string Value => (string)base.Value;
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public StringNode(string value) : base(NodeTypes.Integer, value) { }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    protected override bool ValueEquals(object? otherValue) =>
        Value == (string)otherValue;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
