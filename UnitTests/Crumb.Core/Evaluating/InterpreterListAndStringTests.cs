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
}
