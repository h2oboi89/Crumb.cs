using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Parsing;
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

    public static readonly ReadOnlyDictionary<string, Func<int, List<AstNode>, Scope, Node>> NativeFunctions = new Dictionary<string, Func<int, List<AstNode>, Scope, Node>>
    {
        // IO
        { Names.Print, Print },
        //public const string PrintLine = "printLine";
        //public const string Input = "input";
        { Names.InputLine, InputLine },
        //public const string Rows = "rows";
        //public const string Columns = "columns";
        //public const string ReadFile = "read";
        //public const string WriteFile = "write";
        //// TODO: event ??
        //public const string Use = "use";

        // defining values
        { Names.Define, Define },
        //public const string Function = "fun";

        // comparisons
        //public const string Is = "is";
        //public const string LessThan = "<";
        //public const string GreaterThan = ">";
        //public const string Equal = "=";

        // logic operators^
        //public const string Not = "not";
        //public const string And = "and";
        //public const string Or = "or";

        // arithmetic
        { Names.Add, Add },
        { Names.Subtract, Subtract },
        { Names.Multiply, Multiply },
        { Names.Divide, Divide },
        //public const string Remainder = "%";
        //public const string Power = "^";
        //public const string Random = "random";

        // TODO: bitwise? <<, <<<, >>, >>>, &, |, ~, ^

        // control
        //public const string For = "for";
        //public const string While = "while";
        //public const string If = "if";
        //public const string Wait = "wait";

        // types
        //public const string Integer = "integer";
        //public const string Float = "float";
        //public const string String = "string";
        //public const string List = "list";
        //// TODO: type ??

        // list and string methods
        //public const string Length = "length";
        //public const string Join = "join";
        //public const string Get = "get";
        //public const string Set = "set";
        //public const string Head = "head";
        //public const string Tail = "tail";
        //public const string Insert = "insert";
        //public const string Delete = "delete";
        //public const string Map = "map";
        //public const string Reduce = "reduce";
        //public const string Range = "range";
        //public const string Find = "find";
}.AsReadOnly();

    #region Native Functions
    private static VoidNode Print(int lineNumber, List<AstNode> args, Scope scope)
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
                var escapeSequence = GetEscapeSequence(line, i + 1);

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

        Console.Write(outline.ToString());

        return VoidNode.GetInstance();
    }

    private static StringNode InputLine(int lineNumber, List<AstNode> args, Scope scope)
    {
        var line = Console.ReadLine();

        return line == null ? throw new RuntimeException(lineNumber, $"{Names.InputLine}: unable to get input.") : new StringNode(line);
    }

    private static VoidNode Define(int lineNumber, List<AstNode> args, Scope scope)
    {
        ValidateArgCount(lineNumber, args, 2, 2, Names.Define);

        var identifier = args[0];
        var value = args[1];

        if (identifier.OpCode != OpCodes.Identifier)
        {
            throw new RuntimeException(lineNumber, $"{Names.Define} requires valid identifier, got {identifier.OpCode}.");
        }

        scope.Set(identifier.Value, Interpreter.Evaluate(value, scope));

        // TODO: return evaluated value?
        return VoidNode.GetInstance();
    }

    private static Node Add(int lineNumber, List<AstNode> args, Scope scope)
    {
        ValidateMinArgCount(lineNumber, args, 2, Names.Add);

        var numbers = Interpreter.EvaluateArguments(args, scope);
        
        ValidateNumber(lineNumber, numbers, Names.Add);

        if (CheckForFloat(numbers))
        {
            var result = GetFloatValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result += GetFloatValue(numbers[i]);
            }

            return new FloatNode(result);
        }
        else
        {
            var result = GetIntegerValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result += GetIntegerValue(numbers[i]);
            }

            return new IntegerNode(result);
        }
    }

    private static Node Subtract(int lineNumber, List<AstNode> args, Scope scope)
    {
        ValidateMinArgCount(lineNumber, args, 2, Names.Subtract);

        var numbers = Interpreter.EvaluateArguments(args, scope);

        ValidateNumber(lineNumber, numbers, Names.Subtract);

        if (CheckForFloat(numbers))
        {
            var result = GetFloatValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result -= GetFloatValue(numbers[i]);
            }

            return new FloatNode(result);
        }
        else
        {
            var result = GetIntegerValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result -= GetIntegerValue(numbers[i]);
            }

            return new IntegerNode(result);
        }
    }

    private static Node Multiply(int lineNumber, List<AstNode> args, Scope scope)
    {
        ValidateMinArgCount(lineNumber, args, 2, Names.Multiply);

        var numbers = Interpreter.EvaluateArguments(args, scope);

        ValidateNumber(lineNumber, numbers, Names.Multiply);

        if (CheckForFloat(numbers))
        {
            var result = GetFloatValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result *= GetFloatValue(numbers[i]);
            }

            return new FloatNode(result);
        }
        else
        {
            var result = GetIntegerValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result *= GetIntegerValue(numbers[i]);
            }

            return new IntegerNode(result);
        }
    }

    private static Node Divide(int lineNumber, List<AstNode> args, Scope scope)
    {
        ValidateMinArgCount(lineNumber, args, 2, Names.Divide);

        var numbers = Interpreter.EvaluateArguments(args, scope);

        ValidateNumber(lineNumber, numbers, Names.Divide);

        if (CheckForFloat(numbers))
        {
            var result = GetFloatValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result /= GetFloatValue(numbers[i]);
            }

            return new FloatNode(result);
        }
        else
        {
            var result = GetIntegerValue(numbers[0]);

            for (var i = 1; i < numbers.Count; i++)
            {
                result /= GetIntegerValue(numbers[i]);
            }

            return new IntegerNode(result);
        }
    }
    #endregion

    #region Helper Methods
    private static void ValidateArgCount<T>(int lineNumber, List<T> args, int min, int max, string name)
    {
        ValidateMinArgCount(lineNumber, args, min, name);
        ValidateMaxArgCount(lineNumber, args, max, name);
    }

    private static void ValidateMinArgCount<T>(int lineNumber, List<T> args, int min, string name)
    {
        if (args.Count < min)
        {
            throw new RuntimeException(lineNumber, $"{name} requires at least {min} arguments, got {args.Count}.");
        }
    }

    private static void ValidateMaxArgCount<T>(int lineNumber, List<T> args, int max, string name)
    {
        if (args.Count > max)
        {
            throw new RuntimeException(lineNumber, $"{name} requires at most {max} arguments, got {args.Count}.");
        }
    }

    private static void ValidateArgsTypes(int lineNumber, List<Node> args, string name, params NodeTypes[] types) =>
        args.ForEach(a => ValidateArgType(lineNumber, a, name, types));

    private static void ValidateArgType(int lineNumber, Node arg, string name, params NodeTypes[] expected)
    {
        if (!expected.Contains(arg.Type))
        {
            throw new RuntimeException(lineNumber, $"{name}: unexpected type {arg.Type}, expected one of [ {string.Join(", ", expected)} ].");
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

    private static double GetFloatValue(Node node) => node.Type switch
    {
        NodeTypes.Float => ((FloatNode)node).Value,
        NodeTypes.Integer => ((IntegerNode)node).Value,
        _ => throw UnreachableCode,
    };

    private static int GetIntegerValue(Node node) => node.Type switch
    {
    NodeTypes.Integer => ((IntegerNode)node).Value,
    _ => throw UnreachableCode,
    };

    private static NotImplementedException UnreachableCode =>
        new("unreachable code");
    #endregion
}
