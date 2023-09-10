using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Evaluating.StandardLibrary;
using Crumb.Core.Parsing;

namespace Crumb.Core.Evaluating;
public static class Interpreter
{
    private static int depth = 0;
    // TODO: improve performance to increase recursion limit
    private const int RECURSION_LIMIT = 500; 

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
            OpCodes.List => EvaluateList(node, scope),
            OpCodes.Integer => EvaluateInteger(node),
            OpCodes.Float => EvaluateFloat(node),
            OpCodes.String => EvaluateString(node),
            OpCodes.Identifier => EvaluateIdentifier(node, scope),
            _ => throw RuntimeException.UnreachableCode(node.LineNumber, $"unknown op code {node.OpCode}"),
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

            depth--;

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

        // TODO: make it so you can get string from another evaluation and use it?
        if (node.Children[0].OpCode == OpCodes.Identifier)
        {
            switch (node.Children[0].Value)
            {
                case Names.Define:
                    return Define(node.LineNumber, node.Children.Skip(1).ToList(), scope);
                case Names.Mutate:
                    return Mutate(node.LineNumber, node.Children.Skip(1).ToList(), scope);
                case Names.Function:
                    return Function(node.LineNumber, node.Children.Skip(1).ToList());
            }
        }

        var function = node.Children[0].OpCode switch
        {
            OpCodes.Apply => EvaluateApply(node.Children[0], scope),
            OpCodes.Identifier => EvaluateIdentifier(node.Children[0], scope),
            _ => throw new RuntimeException(node.LineNumber, $"expected function, got '{node.Children[0].Value}'."),
        };

        var args = node.Children.Skip(1).Select(a => Evaluate(a, scope)).ToList();

        return function switch
        {
            FunctionNode functionNode => EvaluateFunction(node.LineNumber, functionNode, args, scope),
            NativeFunctionNode nativeFunctionNode => EvaluateNativeFunction(node.LineNumber, nativeFunctionNode, args, scope),
            _ => throw new RuntimeException(node.LineNumber, $"expected function name, got {function.Type}."),
        };
    }

    private static VoidNode Define(int lineNumber, List<AstNode> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 2, Names.Define);

        var identifier = args[0];
        var value = args[1];

        ValidateArgType(lineNumber, identifier, Names.Define, OpCodes.Identifier);

        scope.Set(identifier.Value, Evaluate(value, scope));

        return VoidNode.GetInstance();
    }

    private static VoidNode Mutate(int lineNumber, List<AstNode> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 2, Names.Mutate);

        var identifier = args[0];
        var value = args[1];

        ValidateArgType(lineNumber, identifier, Names.Mutate, OpCodes.Identifier);

        if (!scope.Update(identifier.Value, Evaluate(value, scope)))
        {
            throw RuntimeException.UndefinedReference(lineNumber, identifier.Value);
        }

        return VoidNode.GetInstance();
    }

    private static FunctionNode Function(int lineNumber, List<AstNode> args)
    {
        HelperMethods.ValidateRangeArgCount(lineNumber, args, 1, 2, Names.Function);

        var functionArguments = new List<AstNode>();
        var bodyArg = 0;

        if (args.Count == 2)
        {
            ValidateArgType(lineNumber, args[0], Names.Function, OpCodes.List);

            ValidateArgsTypes(lineNumber, args[0].Children, Names.Function, OpCodes.Identifier);

            functionArguments.AddRange(args[0].Children);

            bodyArg++;
        }

        ValidateArgType(lineNumber, args[bodyArg], Names.Function, OpCodes.Block);

        var body = args[bodyArg];

        var function = new DefinedFunction(functionArguments, body);

        return new FunctionNode(function);
    }

    private static ListNode EvaluateList(AstNode node, Scope scope)
    {
        var values = new List<Node>();

        foreach (var child in node.Children)
        {
            values.Add(Evaluate(child, scope));
        }

        return new ListNode(values);
    }

    private static IntegerNode EvaluateInteger(AstNode node)
    {
        if (int.TryParse(node.Value, out var value))
        {
            return new IntegerNode(value);
        }
        else
        {
            // should be handled by lexer
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
            // should be handled by lexer
            throw new RuntimeException(node.LineNumber, $"invalid value for integer: '{node.Value}'");
        }
    }

    private static StringNode EvaluateString(AstNode node) => new(node.Value);

    private static Node EvaluateIdentifier(AstNode node, Scope scope)
    {
        var value = scope.Get(node.Value);

        // should be handled by lexer
        return value ?? throw RuntimeException.UndefinedReference(node.LineNumber, node.Value);
    }

    internal static Node ExecuteFunction(int lineNumber, Node node, List<Node> args, Scope scope)
    {
        if (node is NativeFunctionNode nativeFunction)
        {
            return EvaluateNativeFunction(lineNumber, nativeFunction, args, scope);
        }
        if (node is FunctionNode function)
        {
            return EvaluateFunction(lineNumber, function, args, scope);
        }

        throw new RuntimeException(lineNumber, $"Expected function, got {node.Type}");
    }

    private static Node EvaluateNativeFunction(int lineNumber, NativeFunctionNode node, List<Node> args, Scope scope) =>
        node.Value(lineNumber, args, scope);

    private static Node EvaluateFunction(int lineNumber, FunctionNode node, List<Node> args, Scope scope)
    {
        var localScope = new Scope(scope);

        var funcArgs = node.Value.Arguments;

        if (args.Count != funcArgs.Count)
        {
            throw new RuntimeException(lineNumber, $"invalid number of arguments: require {node.Value.Arguments.Count}, got {args.Count}.");
        }

        for (var i = 0; i < node.Value.Arguments.Count; i++)
        {
            localScope.Set(funcArgs[i].Value, args[i]);
        }

        return EvaluateBlock(node.Value.Body, localScope);
    }

    private static void ValidateArgType(int lineNumber, AstNode arg, string name, params OpCodes[] expected)
    {
        if (!expected.Contains(arg.OpCode))
        {
            throw new RuntimeException(lineNumber, $"{name} unexpected {arg.OpCode}, expected one of [ {string.Join(", ", expected)} ].");
        }
    }

    private static void ValidateArgType(int lineNumber, AstNode arg, string name, OpCodes expected)
    {
        if (arg.OpCode != expected)
        {
            throw new RuntimeException(lineNumber, $"{name} unexpected {arg.OpCode}, expected {expected}.");
        }
    }

    private static void ValidateArgsTypes(int lineNumber, List<AstNode> args, string name, params OpCodes[] types) =>
        args.ForEach(a => ValidateArgType(lineNumber, a, name, types));
}
