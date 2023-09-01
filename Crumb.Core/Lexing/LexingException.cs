namespace Crumb.Core.Lexing;
public class LexingException : Exception
{
    private LexingException(string? message) : base(message) { }

    public LexingException(int lineNumber, string error) : this($"Syntax error @ line {lineNumber}: {error}") { }
}
