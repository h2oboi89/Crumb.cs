using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Parsing;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal class HelperMethods
{
    internal static void ValidateArgCount<T>(int lineNumber, List<T> args, int min, int max, string name)
    {
        ValidateMinArgCount(lineNumber, args, min, name);
        ValidateMaxArgCount(lineNumber, args, max, name);
    }

    internal static void ValidateMinArgCount<T>(int lineNumber, List<T> args, int min, string name)
    {
        if (args.Count < min)
        {
            throw new RuntimeException(lineNumber, $"{name} requires at least {min} arguments, got {args.Count}.");
        }
    }

    internal static void ValidateMaxArgCount<T>(int lineNumber, List<T> args, int max, string name)
    {
        if (args.Count > max)
        {
            throw new RuntimeException(lineNumber, $"{name} requires at most {max} arguments, got {args.Count}.");
        }
    }

    internal static void ValidateArgType(int lineNumber, AstNode arg, string name, params OpCodes[] expected)
    {
        if (!expected.Contains(arg.OpCode))
        {
            throw new RuntimeException(lineNumber, $"{name}: unexpected {arg.OpCode} '{arg.Value}', expected one of [ {string.Join(", ", expected)} ].");
        }
    }

    internal static void ValidateArgType(int lineNumber, AstNode arg, string name, OpCodes expected)
    {
        if (arg.OpCode != expected)
        {
            throw new RuntimeException(lineNumber, $"{name}: unexpected {arg.OpCode} '{arg.Value}', expected {expected}.");
        }
    }

    internal static void ValidateArgsTypes(int lineNumber, List<AstNode> args, string name, params OpCodes[] types) =>
        args.ForEach(a => ValidateArgType(lineNumber, a, name, types));

    internal static void ValidateArgsTypes(int lineNumber, List<Node> args, string name, params NodeTypes[] types) =>
        args.ForEach(a => ValidateArgType(lineNumber, a, name, types));

    internal static void ValidateArgType(int lineNumber, Node arg, string name, params NodeTypes[] expected)
    {
        if (!expected.Contains(arg.Type))
        {
            throw new RuntimeException(lineNumber, $"{name}: unexpected {arg.Type}, expected one of [ {string.Join(", ", expected)} ].");
        }
    }

    internal static void ValidateArgType(int lineNumber, Node arg, string name, NodeTypes expected)
    {
        if (arg.Type != expected)
        {
            throw new RuntimeException(lineNumber, $"{name}: unexpected {arg.Type}, expected {expected}.");
        }
    }

    internal static void ValidateNumber(int lineNumber, List<Node> args, string name) =>
        ValidateArgsTypes(lineNumber, args, name, NodeTypes.Integer, NodeTypes.Float);

    internal static char? GetEscapeSequence(string line, int i)
    {
        return line[i] switch
        {
            '\'' => '\'',
            '"' => '"',
            '\\' => '\\',
            '0' => '\0',
            //'a' => '\a',
            'b' => '\b',
            //'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            //'v' => '\v',
            _ => null,
        };
    }

    internal static bool CheckForFloat(List<Node> args) => args.Any(a => a.Type == NodeTypes.Float);

    internal static double GetFloatValue(Node node) => node.Type switch
    {
        NodeTypes.Float => ((FloatNode)node).Value,
        NodeTypes.Integer => ((IntegerNode)node).Value,
        _ => throw UnreachableCode($"expected float or integer, got {node.Type}"),
    };

    internal static int GetIntegerValue(Node node) => node.Type switch
    {
        NodeTypes.Integer => ((IntegerNode)node).Value,
        _ => throw UnreachableCode($"expected integer, got {node.Type}"),
    };

    internal static Node ExecuteBasicMathFunction(int lineNumber, List<Node> args, Scope scope, string name)
    {
        ValidateMinArgCount(lineNumber, args, 2, name);

        ValidateNumber(lineNumber, args, name);

        if (CheckForFloat(args))
        {
            return new FloatNode(args.Skip(1).Aggregate(
                GetFloatValue(args[0]),
                (acc, node) => {
                    return name switch
                    {
                        Names.Add => acc + GetFloatValue(node),
                        Names.Subtract => acc - GetFloatValue(node),
                        Names.Multiply => acc * GetFloatValue(node),
                        Names.Divide => acc / GetFloatValue(node),
                        _ => throw UnreachableCode($"invalid math operation {name}")
                    };
                }
            ));
        }
        else
        {
            return new IntegerNode(args.Skip(1).Aggregate(
                GetIntegerValue(args[0]),
                (acc, node) => {
                    return name switch
                    {
                        Names.Add => acc + GetIntegerValue(node),
                        Names.Subtract => acc - GetIntegerValue(node),
                        Names.Multiply => acc * GetIntegerValue(node),
                        Names.Divide => acc / GetIntegerValue(node),
                        _ => throw UnreachableCode($"invalid math operation {name}")
                    };
                }
            ));
        }
    }

    internal static NotImplementedException UnreachableCode(string error) =>
        new($"unreachable code : {error}");
}
