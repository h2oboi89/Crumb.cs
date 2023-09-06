using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Parsing;
using System.Text;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal class NativeFunctions
{
    internal static VoidNode Print(int lineNumber, List<AstNode> args, Scope scope)
    {
        var line = string.Join(string.Empty,
            Interpreter.EvaluateArguments(args, scope)
            .Select(a => a.ToString())
        );

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

    // TODO: move to wrap entire file?
#pragma warning disable IDE0060 // Remove unused parameter
    internal static StringNode InputLine(int lineNumber, List<AstNode> args, Scope scope)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var line = BuiltIns.Console.ReadLine();

        return line == null ? throw new RuntimeException(lineNumber, $"{Names.InputLine}: unable to get input.") : new StringNode(line);
    }

    internal static VoidNode Define(int lineNumber, List<AstNode> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 2, 2, Names.Define);

        var identifier = args[0];
        var value = args[1];

        HelperMethods.ValidateArgType(lineNumber, identifier, Names.Define, OpCodes.Identifier);

        scope.Set(identifier.Value, Interpreter.Evaluate(value, scope));

        return VoidNode.GetInstance();
    }

    internal static VoidNode Mutate(int lineNumber, List<AstNode> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 2, 2, Names.Mutate);

        var identifier = args[0];
        var value = args[1];

        HelperMethods.ValidateArgType(lineNumber, identifier, Names.Mutate, OpCodes.Identifier);

        if (!scope.Update(identifier.Value, Interpreter.Evaluate(value, scope)))
        {
            throw RuntimeException.UndefinedReference(lineNumber, identifier.Value);
        }

        return VoidNode.GetInstance();
    }

    internal static Node Add(int lineNumber, List<AstNode> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Add);

    internal static Node Subtract(int lineNumber, List<AstNode> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Subtract);

    internal static Node Multiply(int lineNumber, List<AstNode> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Multiply);

    internal static Node Divide(int lineNumber, List<AstNode> args, Scope scope) =>
        HelperMethods.ExecuteBasicMathFunction(lineNumber, args, scope, Names.Divide);
}
