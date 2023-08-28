using System.Text;

namespace crumb.core.Parsing;
internal class AstNode
{
    private const string INDENT = "  ";

    public List<AstNode> Children { get; private set; } = new();
    public OpCodes OpCode { get; private set; }
    public string? Value { get; private set; }
    public int LineNumber { get; private set; }

    public AstNode(OpCodes opCode, string? value, int lineNumber)
    {
        OpCode = opCode;
        Value = value;
        LineNumber = lineNumber;
    }

    public void Append(AstNode child) => Children.Add(child);

    public AstNode Clone()
    {
        var copy = new AstNode(OpCode, Value, LineNumber)
        {
            Children = new(Children.Select(c => c.Clone()))
        };

        return copy;
    }

    public string ToString(int depth)
    {
        var sb = new StringBuilder();

        for(var i = 0; i < depth ; i++)
        {
            sb.Append(INDENT);
        }

        sb.AppendLine($"{LineNumber}| {OpCode}{(string.IsNullOrEmpty(Value) ? string.Empty : $" {Value}")}");

        foreach(var child in Children)
        {
            sb.AppendLine(child.ToString(depth + 1));
        }

        return sb.ToString();
    }

    public override string ToString() => ToString(0);
}
