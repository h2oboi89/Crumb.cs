using Crumb.Core.Evaluating;
using Crumb.Core.Lexing;
using Crumb.Core.Parsing;
using Crumb.Core.Utility;
using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;
internal class InterpreterTests
{
    [Test]
    public static void HelloWorld()
    {
        var input = """
            { 
                ( print "Hello, World!" )
            }
            """;

        var expected = "Hello, World!";

        var testConsole = ExecuteWithNoArgsAndCaptureOutput(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void PrintWithNewLine()
    {
        var input = """
            {
                ( print "Hello\r\nWorld!" )
            }
            """;

        var expected = """
            Hello
            World!
            """;

        var testConsole = ExecuteWithNoArgsAndCaptureOutput(input);

        testConsole.Received().Write(expected);
    }

    #region Helper Methods
    private static IConsole CaptureOutput()
    {
        var testConsole = Substitute.For<IConsole>();

        StandardLibrary.Console = testConsole;

        return testConsole;
    }

    private static IConsole ExecuteWithNoArgsAndCaptureOutput(string input)
    {
        var ast = Parser.Parse(Lexer.Tokenize(input));

        var testConsole = CaptureOutput();

        Interpreter.Evaluate(Array.Empty<string>(), ast);

        return testConsole;
    }

    [TearDown]
    public void FreeConsole()
    {
        StandardLibrary.Console = new SystemConsole();
    }
    #endregion
}
