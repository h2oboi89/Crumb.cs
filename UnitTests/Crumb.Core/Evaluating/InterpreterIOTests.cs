using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;

[TestFixture]
internal static class InterpreterIOTests
{
    #region Setup / TearDown
    [TearDown]
    public static void Reset()
    {
        HelperMethods.ResetConsole();
    }
    #endregion

    #region Tests
    [Test]
    public static void HelloWorld()
    {
        var input = """
            { 
                ( print "Hello, World!" )
            }
            """;

        var expected = "Hello, World!";

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

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

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void PrintInvalidEscapeSequence_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( print "\d" ) }""",
                HelperMethods.RuntimeErrorOnLine1("print: invalid escape sequence.")
            ),
            (
                """{ ( print "\ " ) }""",
                HelperMethods.RuntimeErrorOnLine1("print: invalid escape sequence.")
            )
        );
    }

    [Test]
    public static void ReadInputAndPrint()
    {
        var input = """
            {
                ( print "Hello " ( inputLine ) "!" )
            }
            """;

        var expected = "Hello Bob!";

        var testConsole = HelperMethods.CaptureOutput();

        testConsole.ReadLine().Returns("Bob");

        HelperMethods.Execute(input);

        testConsole.Received().ReadLine();
        testConsole.Received().Write(expected);
    }
    #endregion
}
