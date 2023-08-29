namespace Crumb.Core.Lexing;

public class LexingException : Exception
{
    public LexingException(string? message) : base(message) { }

    public LexingException(int lineNumber, string error) : this($"Syntax error @ Line {lineNumber}: {error}") { }
}