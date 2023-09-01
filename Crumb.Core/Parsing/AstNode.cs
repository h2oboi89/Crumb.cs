using System.Text;

namespace Crumb.Core.Parsing;
public class AstNode
{
    private const string INDENT = "    ";

    public List<AstNode> Children { get; private set; } = new();
    public OpCodes OpCode { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public int LineNumber { get; private set; }

    public AstNode(OpCodes opCode, int lineNumber, string? value = null)
    {
        OpCode = opCode;
        LineNumber = lineNumber;

        if (!string.IsNullOrEmpty(value))
        {
            Value = value;
        }
    }

    public void Append(AstNode child) => Children.Add(child);

    public string ToString(int depth)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < depth; i++)
        {
            sb.Append(INDENT);
        }

        sb.Append($"{LineNumber} | {OpCode}{(string.IsNullOrEmpty(Value) ? string.Empty : $" '{Value}'")}");

        var strings = new List<string> { sb.ToString() };

        strings.AddRange(Children.Select(c => c.ToString(depth + 1)));

        return string.Join(Environment.NewLine, strings);
    }

    public override string ToString() => ToString(0);
}
