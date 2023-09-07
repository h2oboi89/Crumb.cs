using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal partial class NativeFunctions
{
#pragma warning disable IDE0060 // Remove unused parameter
    internal static Node Add(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, Names.Add);

    internal static Node Subtract(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, Names.Subtract);

    internal static Node Multiply(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, Names.Multiply);

    internal static Node Divide(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, Names.Divide);

    internal static Node Remainder(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 2, 2, Names.Remainder);

        HelperMethods.ValidateNumber(lineNumber, args, Names.Remainder);

        if (HelperMethods.CheckForFloat(args))
        {
            return new FloatNode(HelperMethods.GetFloatValue(args[0]) % HelperMethods.GetFloatValue(args[1]));
        }
        else
        {
            return new IntegerNode(HelperMethods.GetIntegerValue(args[0]) % HelperMethods.GetIntegerValue(args[1]));
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
