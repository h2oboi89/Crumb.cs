namespace Crumb.Core.Parsing;
public class ParsingException : Exception
{
    public ParsingException(string? message) : base(message) { }

    public ParsingException(Lexing.Token token, string error) : this($"Syntax error @ line {token.LineNumber}: {error}") { }
}