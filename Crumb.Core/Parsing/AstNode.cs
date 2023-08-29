using System.Text;

namespace Crumb.Core.Parsing;
public class AstNode
{
    private const string INDENT = "    ";

    public List<AstNode> Children { get; private set; } = new();
    public OpCodes OpCode { get; private set; }
    public string? Value { get; private set; }
    public int LineNumber { get; private set; }

    public AstNode(string? value, OpCodes opCode, int lineNumber)
    {
        OpCode = opCode;
        Value = value;
        LineNumber = lineNumber;
    }

    public void Append(AstNode child) => Children.Add(child);

    public AstNode Clone()
    {
        var copy = new AstNode(Value, OpCode, LineNumber)
        {
            Children = new(Children.Select(c => c.Clone()))
        };

        return copy;
    }

    public string ToString(int depth)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < depth; i++)
        {
            sb.Append(INDENT);
        }

        sb.Append($"{LineNumber}| {OpCode}{(string.IsNullOrEmpty(Value) ? string.Empty : $" {Value}")}");

        var strings = new List<string> { sb.ToString() };

        strings.AddRange(Children.Select(c => c.ToString(depth + 1)));

        return string.Join(Environment.NewLine, strings);
    }

    public override string ToString() => ToString(0);

    public static string Print(List<AstNode> program) =>
        program.Count > 0 ? string.Join(Environment.NewLine, program.Select(a => a.ToString())) : "No Program";
}
