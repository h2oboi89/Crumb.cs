﻿using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;

[TestFixture]
internal class InterpreterListAndStringTests
{
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
}
