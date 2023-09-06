using Crumb.Core.Evaluating.StandardLibrary;

namespace Crumb.Core.Evaluating.Nodes;
internal class FunctionNode : Node
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
    public new DefinedFunction Value => (DefinedFunction)base.Value;
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public FunctionNode(DefinedFunction value) : base(NodeTypes.Function, value) { }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    protected override bool ValueEquals(object? otherValue) =>
        ReferenceEquals(Value, (DefinedFunction)otherValue);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => "[Function]";
}
