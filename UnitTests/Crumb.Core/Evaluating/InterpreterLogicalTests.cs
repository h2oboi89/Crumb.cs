using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;

[TestFixture]
internal class InterpreterLogicalTests
{
    [Test]
    public static void InvalidArgCount()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( or true ) }""",
                HelperMethods.RuntimeErrorOnLine1("or requires at least 2 arguments, got 1.")
            ),
            (
                """{ ( and false ) }""",
                HelperMethods.RuntimeErrorOnLine1("and requires at least 2 arguments, got 1.")
            ),
            (
                """{ ( not ) }""",
                HelperMethods.RuntimeErrorOnLine1("not requires exactly 1 arguments, got 0.")
            )
        );
    }

    [Test]
    public static void InvalidArgType()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( or 1 false ) }""",
                HelperMethods.RuntimeErrorOnLine1("or unexpected Integer, expected one of [ Boolean ].")
            ),
            (
                """{ ( and 1 true ) }""",
                HelperMethods.RuntimeErrorOnLine1("and unexpected Integer, expected one of [ Boolean ].")
            ),
            (
                """{ ( not 0 ) }""",
                HelperMethods.RuntimeErrorOnLine1("not unexpected Integer, expected one of [ Boolean ].")
            )
        );
    }

    [Test]
    public static void ValidArgs()
    {
        var input = """
            {
                ( print ( or false false ) )
                ( print ( or false true ) )
                ( print ( or true false ) )
                ( print ( or true true ) )
                ( print ( or true false true false ) )

                ( print ( and false false ) )
                ( print ( and false true ) )
                ( print ( and true false ) )
                ( print ( and true true ) )
                ( print ( and true false true false ) )

                ( print ( not false ) )
                ( print ( not true ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("false");
            testConsole.Write("true");
            testConsole.Write("true");
            testConsole.Write("true");
            testConsole.Write("true");

            testConsole.Write("false");
            testConsole.Write("false");
            testConsole.Write("false");
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");
        });
    }
}
