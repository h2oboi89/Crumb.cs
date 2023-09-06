using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal partial class NativeFunctions
{
    internal static Node Add(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Add);

    internal static Node Subtract(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Subtract);

    internal static Node Multiply(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Multiply);

    internal static Node Divide(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Divide);
}
