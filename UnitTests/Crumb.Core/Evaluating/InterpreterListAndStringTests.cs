using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;

[TestFixture]
internal class InterpreterListAndStringTests
{
    [Test]
    public static void Length_List_ReturnsLength()
    {
        var input = """
            {
                ( def l [ 1 2 3 4 ] )
                ( print ( len l ) )
                ( mut l [ ] )
                ( print ( len l ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("4");
            testConsole.Write("0");
        });
    }

    [Test]
    public static void Length_String_ReturnsLength()
    {
        var input = """
            {
                ( def s "foo the bar" )
                ( print ( len s ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        testConsole.Received().Write("11");
    }

    [Test]
    public static void Length_InvalidType_Throws()
    {
        var input = "{ ( len 1 ) }";
        var expected = HelperMethods.RuntimeErrorOnLine1("len: unexpected Integer, expected one of [ List, String ].");

        HelperMethods.ExecuteForRuntimeError((input, expected));
    }

    [Test]
    public static void Length_InvalidArgCount_Throws()
    {
        var values = new (string input, string expected)[]
        {
            (
                "{ ( len ) } ",
                HelperMethods.RuntimeErrorOnLine1("len requires at least 1 arguments, got 0.")
            ),
            (
                "{ ( len 1 2 ) } ",
                HelperMethods.RuntimeErrorOnLine1("len requires at most 1 arguments, got 2.")
            )
        };

        foreach (var value in values)
        {
            HelperMethods.ExecuteForRuntimeError(value);
        }
    }

    [Test]
    public static void Join_Lists_MergesLists()
    {
        var input = """
            {
                ( print ( join [ 1 ] [ 2 ] [ 3 ] [ 4 ] ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        testConsole.Received().Write("[ 1, 2, 3, 4 ]");
    }

    [Test]
    public static void Join_Strings_MergesStrings()
    {
        var input = """
            {
                ( print ( join "foo" " " "bar" ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        testConsole.Received().Write("foo bar");
    }

    [Test]
    public static void Join_InvalidTypes_Throws()
    {
        var input = """{ ( join [ ] "foo" ) }""";
        var expected = HelperMethods.RuntimeErrorOnLine1("join: expected all lists or all strings.");

        HelperMethods.ExecuteForRuntimeError((input, expected));
    }

    [Test]
    public static void Join_InvalidArgCount_Throws()
    {
        var input = "{ ( join ) }";
        var expected = HelperMethods.RuntimeErrorOnLine1("join requires at least 2 arguments, got 0.");

        HelperMethods.ExecuteForRuntimeError((input, expected));
    }

    [Test]
    public static void Map_IteratesList()
    {
        var input = """
            {
                ( map [ 1 2 3 4 ] ( fun [ item i ] { ( print item ) } ) )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
            testConsole.Write("2");
            testConsole.Write("3");
            testConsole.Write("4");
        });
    }

    [Test]
    public static void Map_InvalidArgTypes_Throws()
    {
        var values = new (string input, string expected)[]
        {
            (
                "{ ( map 1 ( fun [ ] { } ) ) } ",
                HelperMethods.RuntimeErrorOnLine1("map: unexpected Integer, expected List.")
            ),
            (
                "{ ( map [ ] 1 ) } ",
                HelperMethods.RuntimeErrorOnLine1("map: unexpected Integer, expected one of [ Function, NativeFunction ].")
            )
        };

        foreach (var value in values)
        {
            HelperMethods.ExecuteForRuntimeError(value);
        }
    }

    [Test]
    public static void Map_InvalidArgCount_Throws()
    {
        var values = new (string input, string expected)[]
        {
            (
                "{ ( map 1 ) } ",
                HelperMethods.RuntimeErrorOnLine1("map requires at least 2 arguments, got 1.")
            ),
            (
                "{ ( map 1 2 3 ) } ",
                HelperMethods.RuntimeErrorOnLine1("map requires at most 2 arguments, got 3.")
            )
        };

        foreach (var value in values)
        {
            HelperMethods.ExecuteForRuntimeError(value);
        }
    }

    [Test]
    public static void Reduce_IteratesList()
    {
        var input = """
            {
                ( print
                    (
                        reduce
                        [ 1 2 3 4 ]
                        ( fun [ acc item i ] { ( + acc item ) } )
                        0
                    )
                )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        testConsole.Received().Write("10");
    }

    [Test]
    public static void Reduce_InvalidArgTypes_Throws()
    {
        var values = new (string input, string expected)[]
        {
            (
                "{ ( reduce 1 ( fun [ ] { } ) 0 ) } ",
                HelperMethods.RuntimeErrorOnLine1("reduce: unexpected Integer, expected List.")
            ),
            (
                "{ ( reduce [ ] 1 0 ) } ",
                HelperMethods.RuntimeErrorOnLine1("reduce: unexpected Integer, expected one of [ Function, NativeFunction ].")
            )
        };

        foreach (var value in values)
        {
            HelperMethods.ExecuteForRuntimeError(value);
        }
    }

    [Test]
    public static void Reduce_InvalidArgCount_Throws()
    {
        var values = new (string input, string expected)[]
        {
            (
                "{ ( reduce 1 ) } ",
                HelperMethods.RuntimeErrorOnLine1("reduce requires at least 2 arguments, got 1.")
            ),
            (
                "{ ( reduce 1 2 3 4 ) } ",
                HelperMethods.RuntimeErrorOnLine1("reduce requires at most 3 arguments, got 4.")
            )
        };

        foreach (var value in values)
        {
            HelperMethods.ExecuteForRuntimeError(value);
        }
    }
}
