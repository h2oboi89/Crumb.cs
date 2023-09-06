﻿using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Parsing;
using Crumb.Core.Utility;
using System.Collections.ObjectModel;

namespace Crumb.Core.Evaluating.StandardLibrary;
public static class BuiltIns
{
    public static IConsole Console { internal get; set; } = new SystemConsole();

    internal static readonly ReadOnlyDictionary<string, Func<int, List<AstNode>, Scope, Node>> NativeFunctions = new Dictionary<string, Func<int, List<AstNode>, Scope, Node>>
    {
        // IO
        { Names.Print, StandardLibrary.NativeFunctions.Print },
        //public const string PrintLine = "printLine";
        //public const string Input = "input";
        { Names.InputLine, StandardLibrary.NativeFunctions.InputLine },
        //public const string Rows = "rows";
        //public const string Columns = "columns";
        //public const string ReadFile = "read";
        //public const string WriteFile = "write";
        //// TODO: event ??
        //public const string Use = "use";

        // scope state
        { Names.Define, StandardLibrary.NativeFunctions.Define },
        { Names.Mutate, StandardLibrary.NativeFunctions.Mutate },
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
        { Names.Add, StandardLibrary.NativeFunctions.Add },
        { Names.Subtract, StandardLibrary.NativeFunctions.Subtract },
        { Names.Multiply, StandardLibrary.NativeFunctions.Multiply },
        { Names.Divide, StandardLibrary.NativeFunctions.Divide },
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
}