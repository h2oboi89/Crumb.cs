using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal partial class NativeFunctions
{
    internal static Random random = new();

#pragma warning disable IDE0060 // Remove unused parameter
    internal static Node Add(int lineNumber, List<Node> args, Scope scope) =>
        ExecuteBasicMathFunction(lineNumber, args, Names.Add);

    internal static Node Subtract(int lineNumber, List<Node> args, Scope scope) =>
        ExecuteBasicMathFunction(lineNumber, args, Names.Subtract);

    internal static Node Multiply(int lineNumber, List<Node> args, Scope scope) =>
        ExecuteBasicMathFunction(lineNumber, args, Names.Multiply);

    internal static Node Divide(int lineNumber, List<Node> args, Scope scope) =>
        ExecuteBasicMathFunction(lineNumber, args, Names.Divide);

    internal static Node Remainder(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 2, Names.Remainder);

        HelperMethods.ValidateNumber(lineNumber, args, Names.Remainder);

        if (HelperMethods.CheckForFloat(args))
        {
            return new FloatNode(HelperMethods.GetFloatValue(args[0]) % HelperMethods.GetFloatValue(args[1]));
        }
        else
        {
            return new IntegerNode(HelperMethods.GetIntegerValue(args[0]) % HelperMethods.GetIntegerValue(args[1]));
        }
    }

    internal static FloatNode Power(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 2, Names.Power);

        HelperMethods.ValidateNumber(lineNumber, args, Names.Power);

        var a = HelperMethods.GetFloatValue(args[0]);
        var b = HelperMethods.GetFloatValue(args[1]);

        return new FloatNode(Math.Pow(a, b));
    }

    internal static FloatNode Random(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateNoArgs(lineNumber, args, Names.Random);

        return new FloatNode(random.NextDouble());
    }

    internal static VoidNode Seed(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 1, Names.Seed);

        HelperMethods.ValidateArgType(lineNumber, args[0], Names.Seed, NodeTypes.Integer);

        var seed = ((IntegerNode)args[0]).Value;

        random = new Random(seed);

        return VoidNode.GetInstance();
    }
#pragma warning restore IDE0060 // Remove unused parameter

    private static Node ExecuteBasicMathFunction(int lineNumber, List<Node> args, string name)
    {
        HelperMethods.ValidateMinArgCount(lineNumber, args, 2, name);

        HelperMethods.ValidateNumber(lineNumber, args, name);

        if (HelperMethods.CheckForFloat(args))
        {
            return new FloatNode(args.Skip(1).Aggregate(
                HelperMethods.GetFloatValue(args[0]),
                (acc, node) =>
                {
                    return name switch
                    {
                        Names.Add => acc + HelperMethods.GetFloatValue(node),
                        Names.Subtract => acc - HelperMethods.GetFloatValue(node),
                        Names.Multiply => acc * HelperMethods.GetFloatValue(node),
                        Names.Divide => acc / HelperMethods.GetFloatValue(node),
                        _ => throw HelperMethods.UnreachableCode($"invalid math operation {name}")
                    };
                }
            ));
        }
        else
        {
            return new IntegerNode(args.Skip(1).Aggregate(
                HelperMethods.GetIntegerValue(args[0]),
                (acc, node) =>
                {
                    return name switch
                    {
                        Names.Add => acc + HelperMethods.GetIntegerValue(node),
                        Names.Subtract => acc - HelperMethods.GetIntegerValue(node),
                        Names.Multiply => acc * HelperMethods.GetIntegerValue(node),
                        Names.Divide => acc / HelperMethods.GetIntegerValue(node),
                        _ => throw HelperMethods.UnreachableCode($"invalid math operation {name}")
                    };
                }
            ));
        }
    }
}
