using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Parsing;

namespace Crumb.Core.Evaluating;
public static class Interpreter
{
    private static int depth = 0;
    private const int RECURSION_LIMIT = 20_000;

    public static Node Evaluate(string[] args, AstNode node)
    {
        var scope = Scope.CreateGlobal(args);

        return EvaluateBlock(node, scope);
    }

    internal static Node Evaluate(AstNode node, Scope scope)
    {
        return node.OpCode switch
        {
            OpCodes.Block => EvaluateBlock(node, scope),
            OpCodes.Apply => EvaluateApply(node, scope),
            OpCodes.List => throw new NotImplementedException(),
            OpCodes.Integer => EvaluateInteger(node),
            OpCodes.Float => EvaluateFloat(node),
            OpCodes.String => EvaluateString(node),
            OpCodes.Identifier => EvaluateIdentifier(node, scope),
            _ => throw new RuntimeException(node.LineNumber, $"unknown op code {node.OpCode}"),
        };
    }

    private static Node EvaluateBlock(AstNode node, Scope scope)
    {
        if (depth++ == RECURSION_LIMIT)
        {
            throw new RuntimeException(node.LineNumber, "exceeded recursion limit."); ;
        }

        if (node.OpCode == OpCodes.Block)
        {
            var localScope = new Scope(scope);

            var result = (Node)VoidNode.GetInstance();

            foreach (var child in node.Children)
            {
                result = Evaluate(child, localScope);
            }

            return result;
        }

        throw new RuntimeException(node.LineNumber, $"expected block, got {node.OpCode}.");
    }

    private static Node EvaluateApply(AstNode node, Scope scope)
    {
        if (node.Children.Count == 0)
        {
            throw new RuntimeException(node.LineNumber, "empty apply.");
        }

        var function = node.Children[0].OpCode switch
        {
            OpCodes.Apply => EvaluateApply(node.Children[0], scope),
            OpCodes.Identifier => EvaluateIdentifier(node.Children[0], scope),
            _ => throw new RuntimeException(node.LineNumber, $"expected function, got '{node.Children[0].Value}'."),
        };

        var args = node.Children.Skip(1).ToList();

        return function switch
        {
            //FunctionNode functionNode => EvaluateFunction(node.LineNumber, functionNode, args, scope),
            NativeFunctionNode nativeFunctionNode => EvaluateNativeFunction(node.LineNumber, nativeFunctionNode, args, scope),
            _ => throw new RuntimeException(node.LineNumber, $"expected function name, got {function.Type}."),
        };

        throw new NotImplementedException();
    }

    private static IntegerNode EvaluateInteger(AstNode node)
    {
        if (int.TryParse(node.Value, out var value))
        {
            return new IntegerNode(value);
        }
        else
        {
            throw new RuntimeException(node.LineNumber, $"invalid value for integer: '{node.Value}'.");
        }
    }

    private static FloatNode EvaluateFloat(AstNode node)
    {
        if (double.TryParse(node.Value, out var value))
        {
            return new FloatNode(value);
        }
        else
        {
            throw new RuntimeException(node.LineNumber, $"invalid value for integer: '{node.Value}'");
        }
    }

    private static StringNode EvaluateString(AstNode node) => new(node.Value);

    internal static List<Node> EvaluateArguments(IEnumerable<AstNode> nodes, Scope scope) =>
        nodes.Select(n => Evaluate(n, scope)).ToList();

    private static Node EvaluateIdentifier(AstNode node, Scope scope)
    {
        var value = scope.Get(node.Value);

        return value ?? throw RuntimeException.UndefinedReference(node.LineNumber, node.Value);
    }

    private static Node EvaluateNativeFunction(int lineNumber, NativeFunctionNode node, List<AstNode> args, Scope scope) =>
        node.Value(lineNumber, args, scope);
}
