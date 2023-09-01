namespace Crumb.Core.Evaluating.Nodes;
internal class VoidNode : Node
{
    private static readonly VoidNode _instance = new();

    private VoidNode() : base(NodeTypes.Void, null) { }

    protected override bool ValueEquals(object? otherValue) => true;

    protected override int ValueGetHashCode() => new HashCode().ToHashCode();

    public override string ToString() => "[Void]";

    public static VoidNode GetInstance() => _instance;
}
