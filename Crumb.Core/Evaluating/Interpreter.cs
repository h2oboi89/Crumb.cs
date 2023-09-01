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

    private static Node Evaluate(AstNode node, Scope scope)
    {
        return node.OpCode switch
        {
            OpCodes.Block => EvaluateBlock(node, scope),
            OpCodes.Apply => EvaluateApply(node, scope),
            OpCodes.List => throw new NotImplementedException(),
            OpCodes.Integer => throw new NotImplementedException(),
            OpCodes.Float => throw new NotImplementedException(),
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

        throw new RuntimeException(node.LineNumber, $"expected block, got {node.OpCode}");
    }

    private static Node EvaluateApply(AstNode node, Scope scope)
    {
        if (node.Children.Count == 0)
        {
            throw new RuntimeException(node.LineNumber, "empty application.");
        }

        var function = node.Children[0].OpCode switch
        {
            OpCodes.Apply => EvaluateApply(node.Children[0], scope),
            OpCodes.Identifier => EvaluateIdentifier(node.Children[0], scope),
            _ => throw new RuntimeException(node.LineNumber, $"expected function, got {node.Children[0].Value}"),
        };

        var args = node.Children.Skip(1).Select(a => Evaluate(a, scope)).ToList();

        return function switch
        {
            //FunctionNode functionNode => EvaluateFunction(node.LineNumber, functionNode, args, scope),
            NativeFunctionNode nativeFunctionNode => EvaluateNativeFunction(node.LineNumber, nativeFunctionNode, args, scope),
            _ => throw new RuntimeException(node.LineNumber, $"expected function name, got {function.Type}."),
        };

        throw new NotImplementedException();
    }

    private static StringNode EvaluateString(AstNode node) => new(node.Value);

    private static Node EvaluateIdentifier(AstNode node, Scope scope)
    {
        var value = scope.Get(node.Value);

        return value ?? throw new RuntimeException(node.LineNumber, $"undefined reference to '{node.Value}'");
    }

    private static Node EvaluateNativeFunction(int lineNumber, NativeFunctionNode node, List<Node> args, Scope scope) =>
        node.Value(lineNumber, args, new Scope(scope));
}
