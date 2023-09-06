using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal partial class NativeFunctions
{
#pragma warning disable IDE0060 // Remove unused parameter
    internal static IntegerNode Length(int lineNumber, List<Node> args, Scope scope)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 1, 1, Names.Length);
        HelperMethods.ValidateArgType(lineNumber, args[0], Names.Length, NodeTypes.List, NodeTypes.String);

        return args[0].Type switch
        {
            NodeTypes.List => new IntegerNode(((ListNode)args[0]).Value.Count),
            NodeTypes.String => new IntegerNode(((StringNode)args[0]).Value.Length),
            _ => throw HelperMethods.UnreachableCode($"{Names.Length}: expected string or list"),
        };
    }

    internal static ListNode Map(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 2, 2, Names.Map);

        HelperMethods.ValidateArgType(lineNumber, args[0], Names.Map, NodeTypes.List);
        var list = (ListNode)args[0];

        HelperMethods.ValidateArgType(lineNumber, args[1], Names.Map, NodeTypes.Function, NodeTypes.NativeFunction);
        var function = args[1];

        var result = new List<Node>();

        for (var i = 0; i < list.Value.Count; i++)
        {
            var mapFuncArgs = new List<Node>
            {
                list.Value[i],
                new IntegerNode(i),
            };

            result.Add(Interpreter.ExecuteFunction(lineNumber, function, mapFuncArgs, scope));
        }

        return new ListNode(result);
    }

    internal static Node Reduce(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 2, 3, Names.Reduce);

        HelperMethods.ValidateArgType(lineNumber, args[0], Names.Map, NodeTypes.List);
        var list = (ListNode)args[0];

        HelperMethods.ValidateArgType(lineNumber, args[1], Names.Map, NodeTypes.Function, NodeTypes.NativeFunction);
        var function = args[1];

        var accumulator = (Node)VoidNode.GetInstance();

        if (args.Count == 3)
        {
            accumulator = args[2];
        }

        for (var i = 0; i < list.Value.Count; i++)
        {
            var reduceFuncArgs = new List<Node>
            {
                accumulator,
                list.Value[i],
                new IntegerNode(i),
            };

            accumulator = Interpreter.ExecuteFunction(lineNumber, function, reduceFuncArgs, scope);
        }

        return accumulator;
    }
}
