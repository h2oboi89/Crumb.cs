namespace Crumb.Core.Evaluating.Nodes;
internal class ListNode : Node
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
    public new List<Node> Value => (List<Node>)base.Value;
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public ListNode(List<Node> value) : base(NodeTypes.List, value) { }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
    protected override bool ValueEquals(object? otherValue) =>
        Value.SequenceEqual((List<Node>)otherValue);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    protected override int ValueGetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var node in Value)
        {
            hashCode.Add(node.GetHashCode());
        }
        return hashCode.ToHashCode();
    }

    public override string ToString() =>
        $"[ {string.Join(", ", Value.Select(n => n.ToString()))} ]";
}
