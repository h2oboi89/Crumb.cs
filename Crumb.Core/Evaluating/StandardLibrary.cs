using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Utility;
using System.Collections.ObjectModel;
using System.Text;

namespace Crumb.Core.Evaluating;
public static class StandardLibrary
{
    public static IConsole Console { private get; set; } = new SystemConsole();

    // TODO: use english names for all and then add aliases?
    // TODO: resolve aliases until nothing found, use last valid result
    private static class Names
    {
        // IO
        public const string Print = "print";
        public const string PrintLine = "printLine";
        public const string Input = "input";
        public const string InputLine = "inputLine";
        public const string Rows = "rows";
        public const string Columns = "columns";
        public const string ReadFile = "read";
        public const string WriteFile = "write";
        // TODO: event ??
        public const string Use = "use";

        // ??
        public const string Define = "def";         // define variable
        public const string Function = "fun";       // define function

        // comparisons
        public const string Is = "is";
        public const string LessThan = "<";         // alias with lt, <
        public const string GreaterThan = ">";      // alias with gt, >
        public const string Equal = "=";            // alias with eq, =

        // logic operators^
        public const string Not = "not";            // replace with !
        public const string And = "and";            // replace with &&
        public const string Or = "or";              // replace with ||

        // arithmetic
        public const string Add = "+";
        public const string Subtract = "-";
        public const string Multiply = "*";
        public const string Divide = "/";
        public const string Remainder = "%";
        public const string Power = "^";
        public const string Random = "random";

        // TODO: bitwise? <<, <<<, >>, >>>, &, |, ~, ^

        // control
        public const string For = "for";
        public const string While = "while";
        public const string If = "if";
        public const string Wait = "wait";

        // types
        public const string Integer = "integer";
        public const string Float = "float";
        public const string String = "string";
        public const string List = "list";
        // TODO: type ??

        // list and string methods
        public const string Length = "length";              // alias with len
        public const string Join = "join";
        public const string Get = "get";
        public const string Set = "set";
        public const string Head = "head";                  // alias with car
        public const string Tail = "tail";                  // alias with cdr
        public const string Insert = "insert";
        public const string Delete = "delete";
        public const string Map = "map";
        public const string Reduce = "reduce";
        public const string Range = "range";
        public const string Find = "find";
    }

    public static readonly ReadOnlyDictionary<string, Func<int, List<Node>, Scope, Node>> NativeFunctions = new Dictionary<string, Func<int, List<Node>, Scope, Node>>
    {
        // IO
        { Names.Print, Print },
        // TODO: input (single char)
        { Names.InputLine, InputLine }
        // TODO: rows
        // TODO: columns
        // TODO: read_file
        // TODO: write_file
        // TODO: event ??
        // TODO: use

        // comparisons
        // TODO: is
        // TODO: less_than
        // TODO: greater_than

        // logic operators
        // TODO: not
        // TODO: and
        // TODO: or

        // arithmetic
        // TODO: add
        // TODO: subtract
        // TODO: multiply
        // TODO: divide
        // TODO: remainder
        // TODO: power
        // TODO: random

        // control
        // TODO: loop (for)
        // TODO: until (while)
        // TODO: if
        // TODO: wait (thread.sleep)

        // types
        // TODO: integer
        // TODO: float
        // TOOD: string
        // TODO: list
        // TODO: type ??

        // list and string methods
        // TODO: length
        // TODO: join
        // TODO: get (get @ index)
        // TODO: set (set @ index)
        // TODO: insert (insert @ index)
        // TODO: delete (remove @ index)
        // TODO: map
        // TODO: reduce
        // TODO: range (getRange)
        // TODO: find
    }.AsReadOnly();

    #region Native Functions
    private static VoidNode Print(int lineNumber, List<Node> args, Scope scope)
    {
        var line = string.Join(string.Empty, args.Select(a => a.ToString()));

        var outline = new StringBuilder();

        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '\\')
            {
                var escapeSequence = GetEscapeSequence(line, i + 1);

                if (escapeSequence == null)
                {
                    throw new RuntimeException(lineNumber, $"{Names.Print}: invalid escape sequence");
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

        Console.Write(outline.ToString());

        return VoidNode.GetInstance();
    }

    private static StringNode InputLine(int lineNumber, List<Node> args, Scope scope)
    {
        var line = Console.ReadLine();

        return line == null ? throw new RuntimeException(lineNumber, $"{Names.InputLine}: unable to get input") : new StringNode(line);
    }
    #endregion

    #region Helper Methods
    private static void ValidateArgCount(int lineNumber, List<Node> args, int min, int max, string name)
    {
        ValidateMinArgCount(lineNumber, args, min, name);
        ValidateMaxArgCount(lineNumber, args, max, name);
    }

    private static void ValidateMinArgCount(int lineNumber, List<Node> args, int min, string name)
    {
        if (args.Count < min)
        {
            throw new RuntimeException(lineNumber, $"{name} requires at least {min} arguments, got {args.Count}");
        }
    }

    private static void ValidateMaxArgCount(int lineNumber, List<Node> args, int max, string name)
    {
        if (args.Count > max)
        {
            throw new RuntimeException(lineNumber, $"{name} requires at most {max} arguments, got {args.Count}");
        }
    }

    private static void ValidateArgsTypes(int lineNumber, List<Node> args, string name, params NodeTypes[] types) =>
        args.ForEach(a => ValidateArgType(lineNumber, a, name, types));

    private static void ValidateArgType(int lineNumber, Node arg, string name, params NodeTypes[] expected)
    {
        if (!expected.Contains(arg.Type))
        {
            throw new RuntimeException(lineNumber, $"{name}: unexpected type {arg.Type}, expected one of [ {string.Join(", ", expected)} ]");
        }
    }
    private static void ValidateNumber(int lineNumber, List<Node> args, string name) =>
        ValidateArgsTypes(lineNumber, args, name, NodeTypes.Integer, NodeTypes.Float);

    private static void ValidateFunction(int lineNumber, Node arg, string name) =>
        ValidateArgType(lineNumber, arg, name, NodeTypes.Function, NodeTypes.NativeFunction);

    private static char? GetEscapeSequence(string line, int i)
    {
        if (i == line.Length)
        {
            return null;
        }

        return line[i] switch
        {
            '\'' => '\'',
            '"' => '"',
            '\\' => '\\',
            '0' => '\0',
            'a' => '\a',
            'b' => '\b',
            'f' => '\f',
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            'v' => '\v',
            _ => null,
        };
    }

    private static bool CheckForFloat(List<Node> args) => args.Any(a => a.Type == NodeTypes.Float);

    //private static double GetFloatValue(Node node) => node.Type switch
    //{
    //    NodeTypes.Float => ((FloatNode)node).Value,
    //    NodeTypes.Integer => ((IntegerNode)node).Value,
    //    _ => throw UnreachableCode,
    //};

    //private static int GetIntegerValue(Node node) => node.Type switch
    //{
    //    NodeTypes.Integer => ((IntegerNode)node).Value,
    //    _ => throw UnreachableCode,
    //};

    private static NotImplementedException UnreachableCode =>
        new("unreachable code");
    #endregion
}
