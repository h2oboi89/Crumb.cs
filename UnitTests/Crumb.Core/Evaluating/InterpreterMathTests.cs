using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;

[TestFixture]
internal class InterpreterMathTests
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
    public static void SimpleIntegerMath()
    {
        var input = """
            {
                ( print ( + 1 2 ) )
                ( print ( - 9 5 ) )
                ( print ( * 7 6 ) )
                ( print ( / 6 2 ) )
                ( print ( / 5 3 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("3");
            testConsole.Write("4");
            testConsole.Write("42");
            testConsole.Write("3");
            testConsole.Write("1");
        });
    }

    [Test]
    public static void SimpleFloatMath()
    {
        var input = """
            {
                ( print ( + 2.3 3.1 ) )
                ( print ( - 3.14 0.25 ) )
                ( print ( * 3.3 2.0 ) )
                ( print ( / 5.0 3.0 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("5.4");
            testConsole.Write("2.89");
            testConsole.Write("6.6");
            testConsole.Write("1.6666666666666667");
        });
    }

    [Test]
    public static void SimpleMixedMath()
    {
        var input = """
            {
                ( print ( / 5 3 ) )
                ( print ( / 5 3.0 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
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
        HelperMethods.ExecuteForRuntimeError(
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
    public static void BasicMath_InvalidArgs_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( + "foo" "bar" ) }""",
                HelperMethods.RuntimeErrorOnLine1("+ unexpected String, expected one of [ Integer, Float ].")
            ),
            (
                """{ ( - "foo" "bar" ) }""",
                HelperMethods.RuntimeErrorOnLine1("- unexpected String, expected one of [ Integer, Float ].")
            ),
            (
                """{ ( * "foo" "bar" ) }""",
                HelperMethods.RuntimeErrorOnLine1("* unexpected String, expected one of [ Integer, Float ].")
            ),
            (
                """{ ( / "foo" "bar" ) }""",
                HelperMethods.RuntimeErrorOnLine1("/ unexpected String, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void BasicMath_SupportsMultipleArgs()
    {
        var input = """
            {
                ( print ( + 1 2 3 4 ) )
                ( print ( - 10 4 3 2 ) )
                ( print ( * 1 2 3 4 ) )
                ( print ( / 24 4 3 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("10");
            testConsole.Write("1");
            testConsole.Write("24");
            testConsole.Write("2");
        });
    }

    [Test]
    public static void Remainder()
    {
        var input = """
            {
                ( print ( % 5 3 ) )
                ( print ( % 5.2 3.0 ) )
                ( print ( % 6.2 3 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("2");
            testConsole.Write("2.2");
            testConsole.Write("0.20000000000000018");
        });
    }

    [Test]
    public static void Remainder_InvalidArg_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( % 1 "bar" ) }""",
                HelperMethods.RuntimeErrorOnLine1("% unexpected String, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void Remainder_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( % 1 ) }""",
                HelperMethods.RuntimeErrorOnLine1("% requires exactly 2 arguments, got 1.")
            ),
            (
                """{ ( % 1 2 3) }""",
                HelperMethods.RuntimeErrorOnLine1("% requires exactly 2 arguments, got 3.")
            )
        );
    }

    [Test]
    public static void Power()
    {
        var input = """
            {
                ( print ( ^ 2 3 ) )
                ( print ( ^ 16 0.5 ) )
                ( print ( ^ 2.5 2 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("8");
            testConsole.Write("4");
            testConsole.Write("6.25");
        });
    }

    [Test]
    public static void Power_InvalidArg_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( ^ 1 "bar" ) }""",
                HelperMethods.RuntimeErrorOnLine1("^ unexpected String, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void Power_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( ^ 1 ) }""",
                HelperMethods.RuntimeErrorOnLine1("^ requires exactly 2 arguments, got 1.")
            ),
            (
                """{ ( ^ 1 2 3) }""",
                HelperMethods.RuntimeErrorOnLine1("^ requires exactly 2 arguments, got 3.")
            )
        );
    }

    private static bool IsDouble(string x) => double.TryParse(x, out var _);

    [Test]
    public static void Random()
    {
        var input = """
            {
                ( def r0 ( random 0 ) )
                ( print ( r0 ) )
                ( print ( r0 ) )
                ( print ( r0 ) )

                ( def r1 ( random 1 ) )
                ( print ( r1 ) )
                ( print ( r1 ) )
                ( print ( r1 ) )

                ( def r2 ( random 0 ) )
                ( print ( r2 ) )
                ( print ( r2 ) )
                ( print ( r2 ) )

                ( def r3 ( random ) )
                ( print ( r3 ) )
                ( print ( r3 ) )
                ( print ( r3 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("0.7262432699679598");
            testConsole.Write("0.8173253595909687");
            testConsole.Write("0.7680226893946634");

            testConsole.Write("0.24866858415709278");
            testConsole.Write("0.11074397718102856");
            testConsole.Write("0.46701067987224587");

            testConsole.Write("0.7262432699679598");
            testConsole.Write("0.8173253595909687");
            testConsole.Write("0.7680226893946634");

            testConsole.Write(Arg.Is<string>(x => IsDouble(x)));
            testConsole.Write(Arg.Is<string>(x => IsDouble(x)));
            testConsole.Write(Arg.Is<string>(x => IsDouble(x)));
        });
    }

    [Test]
    public static void Random_InvalidArg_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( random "1" ) }""",
                HelperMethods.RuntimeErrorOnLine1("random unexpected String, expected Integer.")
            )
        );
    }

    [Test]
    public static void Random_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( random 1 2 ) }""",
                HelperMethods.RuntimeErrorOnLine1("random requires at most 1 arguments, got 2.")
            )
        );
    }

    [Test]
    public static void Floor()
    {
        var input = """
            {
                ( print ( floor 1.0 ) )
                ( print ( floor 1.4 ) )
                ( print ( floor 1.5 ) )
                ( print ( floor 1.6 ) )
                ( print ( floor -1.4 ) )
                ( print ( floor -1.5 ) )
                ( print ( floor -1.6 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
            testConsole.Write("1");
            testConsole.Write("1");
            testConsole.Write("1");
            testConsole.Write("-2");
            testConsole.Write("-2");
            testConsole.Write("-2");
        });
    }

    [Test]
    public static void Floor_InvalidArg_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( floor "foo" ) }""",
                HelperMethods.RuntimeErrorOnLine1("floor unexpected String, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void Floor_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( floor ) }""",
                HelperMethods.RuntimeErrorOnLine1("floor requires exactly 1 arguments, got 0.")
            ),
            (
                """{ ( floor 1 2 ) }""",
                HelperMethods.RuntimeErrorOnLine1("floor requires exactly 1 arguments, got 2.")
            )
        );
    }

    [Test]
    public static void Ceiling()
    {
        var input = """
            {
                ( print ( ceiling 1.0 ) )
                ( print ( ceiling 1.4 ) )
                ( print ( ceiling 1.5 ) )
                ( print ( ceiling 1.6 ) )
                ( print ( ceiling -1.4 ) )
                ( print ( ceiling -1.5 ) )
                ( print ( ceiling -1.6 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
            testConsole.Write("2");
            testConsole.Write("2");
            testConsole.Write("2");
            testConsole.Write("-1");
            testConsole.Write("-1");
            testConsole.Write("-1");
        });
    }

    [Test]
    public static void Ceiling_InvalidArg_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( ceiling "foo" ) }""",
                HelperMethods.RuntimeErrorOnLine1("ceiling unexpected String, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void Ceiling_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( ceiling ) }""",
                HelperMethods.RuntimeErrorOnLine1("ceiling requires exactly 1 arguments, got 0.")
            ),
            (
                """{ ( ceiling 1 2 ) }""",
                HelperMethods.RuntimeErrorOnLine1("ceiling requires exactly 1 arguments, got 2.")
            )
        );
    }

    [Test]
    public static void Round()
    {
        var input = """
            {
                ( print ( round 1.0 ) )
                ( print ( round 1.4 ) )
                ( print ( round 1.5 ) )
                ( print ( round 1.6 ) )
                ( print ( round -1.4 ) )
                ( print ( round -1.5 ) )
                ( print ( round -1.6 ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
            testConsole.Write("1");
            testConsole.Write("2");
            testConsole.Write("2");
            testConsole.Write("-1");
            testConsole.Write("-2");
            testConsole.Write("-2");
        });
    }

    [Test]
    public static void Round_InvalidArg_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( round "foo" ) }""",
                HelperMethods.RuntimeErrorOnLine1("round unexpected String, expected one of [ Integer, Float ].")
            )
        );
    }

    [Test]
    public static void Round_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                """{ ( round ) }""",
                HelperMethods.RuntimeErrorOnLine1("round requires exactly 1 arguments, got 0.")
            ),
            (
                """{ ( round 1 2 ) }""",
                HelperMethods.RuntimeErrorOnLine1("round requires exactly 1 arguments, got 2.")
            )
        );
    }
    #endregion
}
