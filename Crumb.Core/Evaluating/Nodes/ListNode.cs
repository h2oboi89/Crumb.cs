namespace Crumb.Core.Evaluating.Nodes;
internal class ListNode : Node
{
    public new List<Node> Value => (List<Node>)base.Value;

    public ListNode(List<Node> value) : base(NodeTypes.List, value) { }

    protected override bool ValueEquals(object otherValue) =>
        Value.SequenceEqual((List<Node>)otherValue);

    protected override int ValueGetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var node in Value)
        {
            hashCode.Add(node.GetHashCode());
        }
        return hashCode.ToHashCode();
    }

    // TODO: remove commas to match user style
    public override string ToString() =>
        $"[ {string.Join(", ", Value.Select(n => n.ToString()))} ]";
}
