using Crumb.Core.Parsing;

namespace Crumb.Core.Evaluating.Nodes;
internal class NativeFunctionNode : Node
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
    public new Func<int, List<AstNode>, Scope, Node> Value => (Func<int, List<AstNode>, Scope, Node>)base.Value;
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public NativeFunctionNode(Func<int, List<AstNode>, Scope, Node> value) : base(NodeTypes.NativeFunction, value) { }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    protected override bool ValueEquals(object? otherValue) =>
        ReferenceEquals(Value, (Func<int, List<AstNode>, Scope, Node>)otherValue);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => "[NativeFunction]";
}
