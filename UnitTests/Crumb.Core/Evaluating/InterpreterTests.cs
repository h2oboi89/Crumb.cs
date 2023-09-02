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

        var testConsole = CaptureOutputAndExecute(input);

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

        var testConsole = CaptureOutputAndExecute(input);

        testConsole.Received().Write(expected);
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

        var testConsole = CaptureOutput();

        testConsole.ReadLine().Returns("Bob");

        Execute(input);

        testConsole.Received().ReadLine();
        testConsole.Received().Write(expected);
    }

    [Test]
    public static void SimpleDefine()
    {
        var input = """
            { 
                ( def foo 1 )
                ( print foo )
            }
            """;

        var expected = "1";

        var testConsole = CaptureOutput();

        Execute(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void SimpleIntegerMath()
    {
        var input = """
            {
                ( print ( - ( / ( * 5 8 ) ( + 1 3 ) ) 4 ) )
            }
            """;

        var expected = "6";

        var testConsole = CaptureOutput();

        Execute(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void SimpleFloatMath()
    {
        var input = """
            {
                ( print ( - ( / ( * 5.0 8.0 ) ( + 1.5 2.5 ) ) 4.5 ) )
            }
            """;

        var expected = "5.5";

        var testConsole = CaptureOutput();

        Execute(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void SimpleMixedMath()
    {
        var input = """
            {
                ( print ( - ( / ( * 5 8 ) ( + 1.5 2.5 ) ) 4.5 ) )
            }
            """;

        var expected = "5.5";

        var testConsole = CaptureOutput();

        Execute(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void EmptyApply_Throws()
    {
        var input = "{ ( ) }";

        Assert.That(() => Execute(input), Throws.TypeOf<RuntimeException>()
            .With.Message.EqualTo("Runtime error @ line 1: empty apply."));
    }

    [Test]
    public static void ApplyInvalidFunction_Throws()
    {
        var input = "{ ( 1 ) }";

        Assert.That(() => Execute(input), Throws.TypeOf<RuntimeException>()
            .With.Message.EqualTo("Runtime error @ line 1: expected function, got '1'."));
    }

    #region Helper Methods
    private static IConsole CaptureOutput()
    {
        var testConsole = Substitute.For<IConsole>();

        StandardLibrary.Console = testConsole;

        return testConsole;
    }

    private static void Execute(string input)
    {
        var ast = Parser.Parse(Lexer.Tokenize(input));

        Interpreter.Evaluate(Array.Empty<string>(), ast);
    }

    private static IConsole CaptureOutputAndExecute(string input)
    {
        var testConsole = CaptureOutput();

        Execute(input);

        return testConsole;
    }

    [TearDown]
    public void FreeConsole()
    {
        StandardLibrary.Console = new SystemConsole();
    }
    #endregion
}
