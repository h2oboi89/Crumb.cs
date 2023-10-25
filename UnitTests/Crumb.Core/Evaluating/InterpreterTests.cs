using NSubstitute;

namespace UnitTests.Crumb.Core.Evaluating;
internal static class InterpreterTests
{
    #region Tests
    [Test]
    public static void EmptyApply_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                "{ ( ) }",
                HelperMethods.RuntimeErrorOnLine1("empty apply.")
            )
        );
    }

    [Test]
    public static void ApplyInvalidFunction_Throws()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
                "{ ( 1 ) }",
                HelperMethods.RuntimeErrorOnLine1("expected function, got '1'.")
            ),
            (
                """
                {
                    ( def foo 1 )
                    ( foo )
                }
                """,
                HelperMethods.RuntimeErrorOnLineN("expected function name, got Integer.", 3)
            )
        );
    }

    [Test]
    public static void RecursionDepthTest()
    {
        HelperMethods.ExecuteForRuntimeError(
            (
            """
            {
                ( def f ( fun { ( f ) } ) )
                ( f )
            }
            """,
            HelperMethods.RuntimeErrorOnLineN("exceeded recursion limit.", 2)
            )
        );
    }

    [Test]
    public static void CanRedefineVariable()
    {
        var input = """
            {
                ( def a 1 )
                ( print a )
                ( def a 2 )
                ( print a )
            }
            """;

        var testConsole = HelperMethods.CaptureOutput();

        HelperMethods.Execute(input);

        Received.InOrder(() =>
        {
            testConsole.Write("1");
            testConsole.Write("2");
        });
    }
    #endregion
}
