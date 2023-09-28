using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;

[TestFixture]
internal class InterpreterComparisonTests
{
    [Test]
    public static void Equal_InvalidArgCount()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( = 1 ) }""",
                HelperMethods.RuntimeErrorOnLine1("= requires exactly 2 arguments, got 1.")
            ),
            (
                """{ ( = 1 2 3) }""",
                HelperMethods.RuntimeErrorOnLine1("= requires exactly 2 arguments, got 3.")
            )
        );
    }

    [Test]
    public static void Equal_Numbers()
    {
        var input = """
            {
                ( print ( = 1 1 ) )
                ( print ( = 1 2 ) )

                ( print ( = 1.1 1.1 ) )
                ( print ( = 1.1 2.2 ) )

                ( print ( = 1 1.0 ) )
                ( print ( = 1 1.1 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");
        });
    }

    [Test]
    public static void Equal_SameTypes()
    {
        var input = """
            {
                ( print ( = true true ) )
                ( print ( = false false ) )
                ( print ( = true false ) )

                ( print ( = "foo" "foo" ) )
                ( print ( = "foo" "bar" ) )

                ( print ( = map map ) )
                ( print ( = map reduce ) )

                ( print ( = ( fun {} ) ( fun {} ) ) )
                ( print ( = ( fun {} ) ( fun [] {} ) ) )
                ( print ( = ( fun {} ) ( fun [ a ] {} ) ) )

                ( print ( = [] [] ) )
                ( print ( = [ 1 true ] [ 1 true ] ) )
                ( print ( = [ 1 true ] [ 2 false ] ) )

                ( print ( = void void ) )
                
                ( def r1 ( random ) )
                ( def r2 ( random ) )
                ( print ( = r1 r1 ) )
                ( print ( = r1 r2 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("true");
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("true");

            testConsole.Write("true");
            testConsole.Write("false");
        });
    }

    [Test]
    public static void Equal_MixedTypes()
    {
        var input = """
            {
                ( print ( = 1 "1" ) )
                ( print ( = 1.0 "1.1" ) )

                ( print ( = true 1 ) )
                ( print ( = false 1.0 ) )

                ( print ( = "foo" true ) )
                ( print ( = "foo" 1 ) )

                ( print ( = map ( fun {} ) ) )
                ( print ( = map "foo" ) )

                ( print ( = [] -1 ) )
                ( print ( = [ 1 true ] ( fun {} ) ) )
                ( print ( = [ 1 true ] "fun" ) )

                ( print ( = void 0 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("false");
            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("false");
        });
    }

    [Test]
    public static void LessThan_InvalidArgCount()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( < 1 ) }""",
                HelperMethods.RuntimeErrorOnLine1("< requires exactly 2 arguments, got 1.")
            ),
            (
                """{ ( < 1 2 3) }""",
                HelperMethods.RuntimeErrorOnLine1("< requires exactly 2 arguments, got 3.")
            )
        );
    }

    [Test]
    public static void LessThan_InvalidArgType()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( < true false ) }""",
                HelperMethods.RuntimeErrorOnLine1("< unexpected Boolean, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void LessThan_ValidArgs()
    {
        var input = """
            {
                ( print ( < 0 1 ) )
                ( print ( < 1 0 ) )
                ( print ( < 1 1 ) )

                ( print ( < 0.0 1.0 ) )
                ( print ( < 1.0 0.0 ) )
                ( print ( < 1.0 1.0 ) )

                ( print ( < 0 1.0 ) )
                ( print ( < 1 0.0 ) )
                ( print ( < 1 1.0 ) )

                ( print ( < 0.0 1 ) )
                ( print ( < 1.0 0 ) )
                ( print ( < 1.0 1 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("true");
            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");
            testConsole.Write("false");

            testConsole.Write("true");
            testConsole.Write("false");
            testConsole.Write("false");
        });
    }

    [Test]
    public static void GreaterThan_InvalidArgCount()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( > 1 ) }""",
                HelperMethods.RuntimeErrorOnLine1("> requires exactly 2 arguments, got 1.")
            ),
            (
                """{ ( > 1 2 3) }""",
                HelperMethods.RuntimeErrorOnLine1("> requires exactly 2 arguments, got 3.")
            )
        );
    }

    [Test]
    public static void GreaterThan_InvalidArgType()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( > true false ) }""",
                HelperMethods.RuntimeErrorOnLine1("> unexpected Boolean, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void GreaterThan_ValidArgs()
    {
        var input = """
            {
                ( print ( > 0 1 ) )
                ( print ( > 1 0 ) )
                ( print ( > 1 1 ) )

                ( print ( > 0.0 1.0 ) )
                ( print ( > 1.0 0.0 ) )
                ( print ( > 1.0 1.0 ) )
                
                ( print ( > 0 1.0 ) )
                ( print ( > 1 0.0 ) )
                ( print ( > 1 1.0 ) )
                
                ( print ( > 0.0 1 ) )
                ( print ( > 1.0 0 ) )
                ( print ( > 1.0 1 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("false");
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("false");
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("false");
            testConsole.Write("true");
            testConsole.Write("false");

            testConsole.Write("false");
            testConsole.Write("true");
            testConsole.Write("false");
        });
    }
}
