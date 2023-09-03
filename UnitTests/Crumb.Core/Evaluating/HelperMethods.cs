using Crumb.Core.Evaluating;
using Crumb.Core.Lexing;
using Crumb.Core.Parsing;
using Crumb.Core.Utility;
using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;
internal static class HelperMethods
{
    public static IConsole CaptureOutput()
    {
        var testConsole = Substitute.For<IConsole>();

        StandardLibrary.Console = testConsole;

        return testConsole;
    }

    public static void Execute(string input)
    {
        var ast = Parser.Parse(Lexer.Tokenize(input));

        Interpreter.Evaluate(Array.Empty<string>(), ast);
    }

    public static void ExecuteForError(params (string input, string error)[] values)
    {
        foreach (var (input, error) in values)
        {
            Assert.That(() => Execute(input), Throws.TypeOf<RuntimeException>()
                .With.Message.EqualTo(error));
        }
    }

    public static string RuntimeErrorOnLine1(string error) => RuntimeErrorOnLineN(error, 1);

    public static string RuntimeErrorOnLineN(string error, int n) => $"Runtime error @ line {n}: {error}";

    public static IConsole CaptureOutputAndExecute(string input)
    {
        var testConsole = CaptureOutput();

        Execute(input);

        return testConsole;
    }

    public static void ResetConsole()
    {
        StandardLibrary.Console = new SystemConsole();
    }
}
