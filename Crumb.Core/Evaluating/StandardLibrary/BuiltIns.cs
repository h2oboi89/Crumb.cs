using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Utility;
using System.Collections.ObjectModel;

namespace Crumb.Core.Evaluating.StandardLibrary;
public static class BuiltIns
{
    public static IConsole Console { internal get; set; } = new SystemConsole();

    internal static readonly ReadOnlyDictionary<string, Func<int, List<Node>, Scope, Node>> NativeFunctions = new Dictionary<string, Func<int, List<Node>, Scope, Node>>
    {
        // meta
        // help: prints function info? (args, types expected, etc?)

        // IO
        { Names.Print, StandardLibrary.NativeFunctions.Print },
        //public const string Input = "input";
        { Names.InputLine, StandardLibrary.NativeFunctions.InputLine },
        { Names.Rows, StandardLibrary.NativeFunctions.Rows },
        { Names.Columns, StandardLibrary.NativeFunctions.Columns },
        { Names.Clear, StandardLibrary.NativeFunctions.Clear },
        //public const string ReadFile = "read";
        //public const string WriteFile = "write";

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
        { Names.Add, StandardLibrary.NativeFunctions.Add },
        { Names.Subtract, StandardLibrary.NativeFunctions.Subtract },
        { Names.Multiply, StandardLibrary.NativeFunctions.Multiply },
        { Names.Divide, StandardLibrary.NativeFunctions.Divide },
        { Names.Remainder, StandardLibrary.NativeFunctions.Remainder },
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
        //// TODO: type ??

        // list and string methods
        { Names.Length, StandardLibrary.NativeFunctions.Length },
        { Names.Join, StandardLibrary.NativeFunctions.Join },
        //public const string Get = "get";
        //public const string Set = "set";
        //public const string Head = "head";
        //public const string Tail = "tail";
        //public const string Insert = "insert";
        //public const string Delete = "delete";
        { Names.Map, StandardLibrary.NativeFunctions.Map },
        { Names.Reduce, StandardLibrary.NativeFunctions.Reduce },
        //public const string Range = "range";
        //public const string Find = "find";
        //public const string Flatten = "flatten";
    }.AsReadOnly();

    internal static readonly ReadOnlyDictionary<string, string> Functions = new Dictionary<string, string>
    {
        { Names.PrintLine, StandardLibrary.Functions.PrintLine },
    }.AsReadOnly();
}
