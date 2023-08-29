using System.Text;

namespace Crumb.Core.Lexing;

public readonly record struct Token
{
    public readonly string? Value;
    public readonly TokenType Type;
    public readonly int LineNumber;

    public Token(string? value, TokenType type, int lineNumber)
    {
        Value = value;
        Type = type;
        LineNumber = lineNumber;
    }

    public override string ToString() => $"{LineNumber}| {Type}{(string.IsNullOrEmpty(Value) ? string.Empty : $" {Value}")}";

    public static string Print(IEnumerable<Token> tokens) =>
        string.Join(Environment.NewLine, tokens.Select((t, i) => $"[{i}]: {t}"));
}
