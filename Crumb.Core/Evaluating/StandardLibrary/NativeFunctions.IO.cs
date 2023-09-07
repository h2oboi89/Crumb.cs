using Crumb.Core.Evaluating.Nodes;
using System.Text;

namespace Crumb.Core.Evaluating.StandardLibrary;

internal partial class NativeFunctions
{
#pragma warning disable IDE0060 // Remove unused parameter
    internal static VoidNode Print(int lineNumber, List<Node> args, Scope scope)
    {
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

    internal static StringNode InputLine(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 0, 0, Names.InputLine);

        var line = BuiltIns.Console.ReadLine();

        return line == null ? throw new RuntimeException(lineNumber, $"{Names.InputLine}: unable to get input.") : new StringNode(line);
    }

    internal static IntegerNode Rows(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 0, 0, Names.Rows);

        return new IntegerNode(BuiltIns.Console.WindowHeight);
    }

    internal static IntegerNode Columns(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 0, 0, Names.Columns);

        return new IntegerNode(BuiltIns.Console.WindowWidth);
    }

    internal static VoidNode Clear(int lineNumber, List<Node> args, Scope scope)
    {
        HelperMethods.ValidateArgCount(lineNumber, args, 0, 0, Names.Clear);

        BuiltIns.Console.Clear();

        return VoidNode.GetInstance();
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
