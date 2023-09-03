using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;
internal static class InterpreterTests
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
        HelperMethods.ExecuteForError(
            (
                """{ ( print "\d" ) }""",
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
    public static void SimpleDefine()
    {
        var input = """
            { 
                ( def foo 1 )
                ( print foo )
            }
            """;

        var expected = "1";

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void InvalidDefineTarget_Throws()
    {
        HelperMethods.ExecuteForError(
            (
                "{ ( def 1 2 ) }",
                HelperMethods.RuntimeErrorOnLine1("def: unexpected Integer '1', expected Identifier.")
            )
        );
    }

    [Test]
    public static void Define_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForError(
            (
                "{ ( def foo ) }",
                HelperMethods.RuntimeErrorOnLine1("def requires at least 2 arguments, got 1.")
            ),
            (
                "{ ( def foo 1 2 ) }",
                HelperMethods.RuntimeErrorOnLine1("def requires at most 2 arguments, got 3.")
            )
        );
    }

    [Test]
    public static void SimpleIntegerMath()
    {
        var input = """
            {
                ( print ( + 1 2 ) )
                ( print " " )
                ( print ( - 9 5 ) )
                ( print " " )
                ( print ( * 7 6 ) )
                ( print " " )
                ( print ( / 6 2 ) )
                ( print " " )
                ( print ( / 5 3 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("3");
            testConsole.Write(" ");
            testConsole.Write("4");
            testConsole.Write(" ");
            testConsole.Write("42");
            testConsole.Write(" ");
            testConsole.Write("3");
            testConsole.Write(" ");
            testConsole.Write("1");
        });
    }

    [Test]
    public static void SimpleFloatMath()
    {
        var input = """
            {
                ( print ( + 2.3 3.1 ) )
                ( print " " )
                ( print ( - 3.14 0.25 ) )
                ( print " " )
                ( print ( * 3.3 2.0 ) )
                ( print " " )
                ( print ( / 5.0 3.0 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("5.4");
            testConsole.Write(" ");
            testConsole.Write("2.89");
            testConsole.Write(" ");
            testConsole.Write("6.6");
            testConsole.Write(" ");
            testConsole.Write("1.6666666666666667");
        });
    }

    [Test]
    public static void SimpleMixedMath()
    {
        var input = """
            {
                ( print ( / 5 3 ) )
                ( print " " )
                ( print ( / 5 3.0 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
            testConsole.Write(" ");
            testConsole.Write("1.6666666666666667");
        });
    }

    [Test]
    public static void NestedMath()
    {
        var input = """
            {
                ( print 
                    ( 
                        / 
                        ( 
                            +
                            ( * 5 7 ) // 35
                            ( - 8 3 ) // 5
                        )             // 40
                        4 
                    )                 // 10
                )
            }
            """;

        var expected = "10";

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        testConsole.Received().Write(expected);
    }

    [Test]
    public static void BasicMath_LessThanMinArgs_Throws()
    {
        HelperMethods.ExecuteForError(
            (
                "{ ( + 1 ) }", 
                HelperMethods.RuntimeErrorOnLine1("+ requires at least 2 arguments, got 1.")
            ),
            (
                "{ ( - 1 ) }", 
                HelperMethods.RuntimeErrorOnLine1("- requires at least 2 arguments, got 1.")
            ),
            (
                "{ ( * 1 ) }", 
                HelperMethods.RuntimeErrorOnLine1("* requires at least 2 arguments, got 1.")
            ),
            (
                "{ ( / 1 ) }", 
                HelperMethods.RuntimeErrorOnLine1("/ requires at least 2 arguments, got 1.")
            )
        );
    }

    [Test]
    public static void EmptyApply_Throws()
    {
        HelperMethods.ExecuteForError(
            (
                "{ ( ) }",
                HelperMethods.RuntimeErrorOnLine1("empty apply.")
            )
        );
    }

    [Test]
    public static void ApplyInvalidFunction_Throws()
    {
        HelperMethods.ExecuteForError(
            (
                "{ ( 1 ) }",
                HelperMethods.RuntimeErrorOnLine1("expected function, got '1'.")
            )
        );
    }
    #endregion
}
