namespace Crumb.Core.Evaluating.Nodes;
public class BooleanNode : Node
{
    public static readonly BooleanNode _trueInstance = new(true);
    public static readonly BooleanNode _falseInstance = new(false);

    public new bool Value => (bool)base.Value;

    private BooleanNode(bool value) : base(NodeTypes.Boolean, value) { }

    protected override bool ValueEquals(object otherValue) =>
        Value == (bool)otherValue;

    protected override int ValueGetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString().ToLower();

    public static BooleanNode GetTrueInstance() => _trueInstance;
    public static BooleanNode GetFalseInstance() => _falseInstance;
}