namespace Crumb.Core.Evaluating.Nodes;
internal class NativeFunctionNode : Node
{
    public new Func<int, List<Node>, Scope, Node> Value => (Func<int, List<Node>, Scope, Node>)base.Value;

    public NativeFunctionNode(Func<int, List<Node>, Scope, Node> value) : base(NodeTypes.NativeFunction, value) { }

    protected override bool ValueEquals(object otherValue) =>
        ReferenceEquals(Value, (Func<int, List<Node>, Scope, Node>)otherValue);

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => "<NativeFunction>";
}
