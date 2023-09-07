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
    public static void Print_NoArgs_PrintsNothing()
    {
        var input = "{ ( print ) }";

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        testConsole.Received().Write(string.Empty);
    }

    [Test]
    public static void PrintWithEscapeSequences()
    {
        var values = new (string input, string expected)[]
        {
            (
            """ { ( print "Hello\r\nWorld!" ) } """,
            """
            Hello
            World!
            """
            ),
            (
            """ { ( print " \t \" \' \\ \b \0" ) } """,
            " \t \" \' \\ \b \0"
            )
        };

        foreach (var (input, expected) in values)
        {
            var testConsole = HelperMethods.CaptureOutputAndExecute(input);

            testConsole.Received().Write(expected);
        }
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

    [Test]
    public static void InputLine_InvalidArgCount_Throws()
    {
        var input = "{ ( inputLine 1 ) }";

        var expected = HelperMethods.RuntimeErrorOnLine1("inputLine requires at most 0 arguments, got 1.");

        HelperMethods.ExecuteForRuntimeError((input, expected));
    }

    [Test]
    public static void PrintLine()
    {
        var input = """
            {
                ( print "Hello, World!" )
                ( printLine "Hello, World!" )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("Hello, World!");
            testConsole.Write("Hello, World!");
            testConsole.Write($"{Environment.NewLine}");
        });
    }

    [Test]
    public static void Printing_Types()
    {
        var values = new (string input, string expected)[]
        {
            ( "{ ( print void ) }" , "void"),
            ( "{ ( print 1 ) }", "1"),
            ( "{ ( print 3.14159 ) }", "3.14159"),
            ( "{ ( print \"foo\" ) }", "foo"),
            ( "{ ( print ( fun { } ) ) }", "<Function>"),
            ( "{ ( print print ) }", "<NativeFunction>"),
            ( "{ ( print [ void 1 2.5 \"bar\" map ] ) }", "[ void, 1, 2.5, bar, <NativeFunction> ]"),
        };

        foreach (var (input, expected) in values)
        {
            var testConsole = HelperMethods.CaptureOutputAndExecute(input);

            testConsole.Received().Write(expected);
        }
    }

    [Test]
    public static void RowsColumnsClear()
    {
        var input = """
            {
                ( print ( rows ) )
                ( print ( columns ) )
                ( clear )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        testConsole.WindowHeight.Returns(10);
        testConsole.WindowWidth.Returns(20);

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            var r = testConsole.WindowHeight;
            testConsole.Write("10");
            var c = testConsole.WindowWidth;
            testConsole.Write("20");
            testConsole.Clear();
        });
    }
    #endregion
}
