using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal class HelperMethods
{
    internal static void ValidateRangeArgCount<T>(int lineNumber, List<T> args, int min, int max, string name)
    {
        ValidateMinArgCount(lineNumber, args, min, name);
        ValidateMaxArgCount(lineNumber, args, max, name);
    }

    internal static void ValidateExactArgCount<T>(int lineNumber, List<T> args, int exact, string name) {
        if (args.Count != exact)
        {
            throw new RuntimeException(lineNumber, $"{name} requires exactly {exact} arguments, got {args.Count}.");
        }
    }

    internal static void ValidateNoArgs<T>(int lineNumber, List<T> args, string name)
    {
        if (args.Count > 0)
        {
            throw new RuntimeException(lineNumber, $"{name} takes no arguments, got {args.Count}.");
        }
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

    internal static void ValidateArgsTypes(int lineNumber, List<Node> args, string name, params NodeTypes[] types) =>
        args.ForEach(a => ValidateArgType(lineNumber, a, name, types));

    internal static void ValidateArgType(int lineNumber, Node arg, string name, params NodeTypes[] expected)
    {
        if (!expected.Contains(arg.Type))
        {
            throw new RuntimeException(lineNumber, $"{name} unexpected {arg.Type}, expected one of [ {string.Join(", ", expected)} ].");
        }
    }

    internal static bool AreAllArgs(List<Node> args, NodeTypes expected) =>
        args.All(a => a.Type == expected);

    internal static void ValidateArgType(int lineNumber, Node arg, string name, NodeTypes expected)
    {
        if (arg.Type != expected)
        {
            throw new RuntimeException(lineNumber, $"{name} unexpected {arg.Type}, expected {expected}.");
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

    internal static double GetFloatValue(int lineNumber, Node node) => node.Type switch
    {
        NodeTypes.Float => ((FloatNode)node).Value,
        NodeTypes.Integer => ((IntegerNode)node).Value,
        _ => throw RuntimeException.UnreachableCode(lineNumber, $"expected float or integer, got {node.Type}"),
    };

    internal static int GetIntegerValue(int lineNumber, Node node) => node.Type switch
    {
        NodeTypes.Integer => ((IntegerNode)node).Value,
        _ => throw RuntimeException.UnreachableCode(lineNumber, $"expected integer, got {node.Type}"),
    };
}
