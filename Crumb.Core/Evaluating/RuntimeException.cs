namespace Crumb.Core.Evaluating;

public class RuntimeException : Exception
{
    private RuntimeException(string? message, Exception? innerException) : base(message, innerException) { }

    public RuntimeException(int lineNumber, string error, Exception? innerException = null) : this($"Runtime error @ line {lineNumber}: {error}", innerException) { }

    public static RuntimeException UndefinedReference(int lineNumber, string name) =>
        new(lineNumber, $"undefined reference to '{name}'.");

    internal static NotImplementedException UnreachableCode(int lineNumber, string error) =>
        new($"reached unreachable code @ line {lineNumber}: {error}");
}