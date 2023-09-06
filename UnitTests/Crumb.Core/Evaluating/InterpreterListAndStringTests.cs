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
