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
            var a = HelperMethods.GetFloatValue(lineNumber, args[0]);
            var b = HelperMethods.GetFloatValue(lineNumber, args[1]);
            return new FloatNode(a % b);
        }
        else
        {
            var a = HelperMethods.GetIntegerValue(lineNumber, args[0]);
            var b = HelperMethods.GetIntegerValue(lineNumber, args[1]);
            return new IntegerNode(a % b);
        }
    }

    internal static FloatNode Power(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 2, Names.Power);

        HelperMethods.ValidateNumber(lineNumber, args, Names.Power);

        var a = HelperMethods.GetFloatValue(lineNumber, args[0]);
        var b = HelperMethods.GetFloatValue(lineNumber, args[1]);

        return new FloatNode(Math.Pow(a, b));
    }

    // TODO: figure out how to make this return a function with embedded Random.cs
    // ( def r ( random seed ) ) OR ( def r ( random ) ) <- sets r to be a random function
    // ( r ) <- returns a random value
    internal static FloatNode Random(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateNoArgs(lineNumber, args, Names.Random);

        return new FloatNode(random.NextDouble());
    }

    // TODO: get rid of this once we figure out Random
    internal static VoidNode Seed(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 1, Names.Seed);

        HelperMethods.ValidateArgType(lineNumber, args[0], Names.Seed, NodeTypes.Integer);

        var seed = ((IntegerNode)args[0]).Value;

        random = new Random(seed);

        return VoidNode.GetInstance();
    }

    internal static IntegerNode Floor(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 1, Names.Floor);

        HelperMethods.ValidateNumber(lineNumber, args, Names.Floor);

        var value = HelperMethods.GetFloatValue(lineNumber, args[0]);

        return new IntegerNode((int)Math.Floor(value));
    }

    internal static IntegerNode Ceiling(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 1, Names.Ceiling);

        HelperMethods.ValidateNumber(lineNumber, args, Names.Ceiling);

        var value = HelperMethods.GetFloatValue(lineNumber, args[0]);

        return new IntegerNode((int)Math.Ceiling(value));
    }

    internal static IntegerNode Round(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateExactArgCount(lineNumber, args, 1, Names.Round);

        HelperMethods.ValidateNumber(lineNumber, args, Names.Round);

        var value = HelperMethods.GetFloatValue(lineNumber, args[0]);

        return new IntegerNode((int)Math.Round(value));
    }
#pragma warning restore IDE0060 // Remove unused parameter

    private static Node ExecuteBasicMathFunction(int lineNumber, List<Node> args, string name)
    {
        HelperMethods.ValidateMinArgCount(lineNumber, args, 2, name);

        HelperMethods.ValidateNumber(lineNumber, args, name);

        if (HelperMethods.CheckForFloat(args))
        {
            return new FloatNode(args.Skip(1).Aggregate(
                HelperMethods.GetFloatValue(lineNumber, args[0]),
                (acc, node) =>
                {
                    return name switch
                    {
                        Names.Add => acc + HelperMethods.GetFloatValue(lineNumber, node),
                        Names.Subtract => acc - HelperMethods.GetFloatValue(lineNumber, node),
                        Names.Multiply => acc * HelperMethods.GetFloatValue(lineNumber, node),
                        Names.Divide => acc / HelperMethods.GetFloatValue(lineNumber, node),
                        _ => throw RuntimeException.UnreachableCode(lineNumber, $"invalid math operation {name}")
                    };
                }
            ));
        }
        else
        {
            return new IntegerNode(args.Skip(1).Aggregate(
                HelperMethods.GetIntegerValue(lineNumber, args[0]),
                (acc, node) =>
                {
                    return name switch
                    {
                        Names.Add => acc + HelperMethods.GetIntegerValue(lineNumber, node),
                        Names.Subtract => acc - HelperMethods.GetIntegerValue(lineNumber, node),
                        Names.Multiply => acc * HelperMethods.GetIntegerValue(lineNumber, node),
                        Names.Divide => acc / HelperMethods.GetIntegerValue(lineNumber, node),
                        _ => throw RuntimeException.UnreachableCode(lineNumber, $"invalid math operation {name}")
                    };
                }
            ));
        }
    }
}
