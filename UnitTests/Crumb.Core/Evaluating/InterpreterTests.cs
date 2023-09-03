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
            )
        );
    }
    #endregion
}
