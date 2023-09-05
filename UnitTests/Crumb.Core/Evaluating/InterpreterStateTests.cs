using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;

[TestFixture]
internal static class InterpreterStateTests
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
        HelperMethods.ExecuteForRuntimeError(
            (
                "{ ( def 1 2 ) }",
                HelperMethods.RuntimeErrorOnLine1("def: unexpected Integer '1', expected Identifier.")
            )
        );
    }

    [Test]
    public static void Define_InvalidArgCount_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
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
    #endregion
}
