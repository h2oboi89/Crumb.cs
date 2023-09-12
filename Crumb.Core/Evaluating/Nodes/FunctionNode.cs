using Crumb.Core.Evaluating.StandardLibrary;

namespace Crumb.Core.Evaluating.Nodes;
internal class FunctionNode : Node
{
    public new DefinedFunction Value => (DefinedFunction)base.Value;

    public FunctionNode(DefinedFunction value) : base(NodeTypes.Function, value) { }

    protected override bool ValueEquals(object otherValue) =>
        Value == (DefinedFunction)otherValue;

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => "<Function>";
}
