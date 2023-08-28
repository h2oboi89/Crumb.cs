using System.Text;

namespace crumb.core.Lexing;

public class Token
{
    public string? Value { get; private set; }
    public TokenType Type { get; private set; }
    public int LineNumber { get; private set; }

    public Token(string? value, TokenType type, int lineNumber)
    {
        Value = value;
        Type = type;
        LineNumber = lineNumber;
    }

    public override string ToString() => $"{LineNumber}| {Type}{(string.IsNullOrEmpty(Value) ? string.Empty : $" {Value}")}";

    public static string Print(IEnumerable<Token> tokens)
    {
        var sb = new StringBuilder();

        foreach (var token in tokens)
        {
            sb.AppendLine(token.ToString());
        }

        return sb.ToString();
    }
}
