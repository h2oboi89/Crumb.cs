using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal partial class NativeFunctions
{
    internal static BooleanNode Equal(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 2, Names.Equal);

        var a = args[0];
        var b = args[1];

        if (HelperMethods.AreAllArgs(args, NodeTypes.Integer, NodeTypes.Float))
        {
            var aValue = HelperMethods.GetFloatValue(lineNumber, a);
            var bValue = HelperMethods.GetFloatValue(lineNumber, b);

            return BooleanNode.GetInstance(aValue == bValue);
        }

        // anything else
        return BooleanNode.GetInstance(a == b);
    }

    internal static BooleanNode LessThan(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 2, Names.LessThan);
        HelperMethods.ValidateNumber(lineNumber, args, Names.LessThan);

        var a = HelperMethods.GetFloatValue(lineNumber, args[0]);
        var b = HelperMethods.GetFloatValue(lineNumber, args[1]);

        return BooleanNode.GetInstance(a < b);
    }
}
