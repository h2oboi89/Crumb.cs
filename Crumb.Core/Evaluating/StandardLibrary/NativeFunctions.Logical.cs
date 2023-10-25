using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal partial class NativeFunctions
{
#pragma warning disable IDE0060 // Remove unused parameter
    internal static BooleanNode Or(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateMinArgCount(lineNumber, args, 2, Names.Or);
        HelperMethods.ValidateArgsTypes(lineNumber, args, Names.Or, NodeTypes.Boolean);

        return BooleanNode.GetInstance(
            args.Select(a => ((BooleanNode)a).Value)
                .Aggregate(false, (acc, v) => acc || v)
        );
    }

    internal static BooleanNode And(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateMinArgCount(lineNumber, args, 2, Names.And);
        HelperMethods.ValidateArgsTypes(lineNumber, args, Names.And, NodeTypes.Boolean);

        return BooleanNode.GetInstance(
            args.Select(a => ((BooleanNode)a).Value)
                .Aggregate(true, (acc, v) => acc && v)
        );
    }

    internal static BooleanNode Not(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 1, Names.Not);
        HelperMethods.ValidateArgsTypes(lineNumber, args, Names.Not, NodeTypes.Boolean);

        return BooleanNode.GetInstance(!((BooleanNode)args[0]).Value);
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
