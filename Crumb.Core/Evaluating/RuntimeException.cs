namespace Crumb.Core.Evaluating;

public class RuntimeException : Exception
{
    private RuntimeException(string? message, Exception? innerException) : base(message, innerException) { }

    public RuntimeException(int lineNumber, string error, Exception? innerException = null) : this($"Runtime error @ line {lineNumber}: {error}", innerException) { }
}