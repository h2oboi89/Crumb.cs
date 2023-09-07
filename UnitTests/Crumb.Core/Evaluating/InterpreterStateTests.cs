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
    public static void Define_Update_Get()
    {
        var input = """
            { 
                ( def foo 1 )
                ( print foo )
                ( mut foo 2 )
                ( print foo )

                {
                    ( mut foo 3 )
                    ( print foo )
                }

                ( print foo )
            }
            """;

        var testConsole = HelperMethods.CaptureOutputAndExecute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
            testConsole.Write("2");
            testConsole.Write("3");
            testConsole.Write("3");
        });
    }

    [Test]
    public static void InvalidDefineTarget_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                "{ ( def 1 2 ) }",
                HelperMethods.RuntimeErrorOnLine1("def unexpected Integer '1', expected Identifier.")
            )
        );
    }

    [Test]
    public static void UndefinedReference_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                "{ ( print foo ) }",
                HelperMethods.RuntimeErrorOnLine1("undefined reference to 'foo'.")
            )
        );
    }

    [Test]
    public static void Mutate_Undefined_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                "{ ( mut foo 1 ) }",
                HelperMethods.RuntimeErrorOnLine1("undefined reference to 'foo'.")
            )
        );
    }

    [Test]
    public static void Mutate_NestedBlocks_Undefined_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                "{ { ( mut foo 1 ) } }",
                HelperMethods.RuntimeErrorOnLine1("undefined reference to 'foo'.")
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
