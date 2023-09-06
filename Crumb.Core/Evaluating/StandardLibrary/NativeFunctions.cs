using Crumb.Core.Evaluating.Nodes;
using System.Text;

namespace Crumb.Core.Evaluating.StandardLibrary;

// TODO: make partial class and split up functions according to grouping in Names?
internal class NativeFunctions
{
#pragma warning disable IDE0060 // Remove unused parameter
    internal static VoidNode Print(int lineNumber, List<Node> args, Scope scope)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        // TODO: should strings get quotes when printed? - Yes
        var line = string.Join(string.Empty, args.Select(a => a.ToString()));

        var outline = new StringBuilder();

        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '\\')
            {
                var escapeSequence = HelperMethods.GetEscapeSequence(line, i + 1);

                if (escapeSequence == null)
                {
                    throw new RuntimeException(lineNumber, $"{Names.Print}: invalid escape sequence.");
                }
                else
                {
                    outline.Append(escapeSequence.Value);
                    i++;
                }
            }
            else
            {
                outline.Append(line[i]);
            }
        }

        BuiltIns.Console.Write(outline.ToString());

        return VoidNode.GetInstance();
    }

#pragma warning disable IDE0060 // Remove unused parameter
    internal static StringNode InputLine(int lineNumber, List<Node> args, Scope scope)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 0, 0, Names.InputLine);

        var line = BuiltIns.Console.ReadLine();

        return line == null ? throw new RuntimeException(lineNumber, $"{Names.InputLine}: unable to get input.") : new StringNode(line);
    }

    internal static Node Add(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Add);

    internal static Node Subtract(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Subtract);

    internal static Node Multiply(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Multiply);

    internal static Node Divide(int lineNumber, List<Node> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Divide);

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

        for(var i = 0; i < list.Value.Count; i++)
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
