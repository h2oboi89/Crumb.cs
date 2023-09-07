namespace Crumb.Core.Evaluating.StandardLibrary;
internal static class Names
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

    // scope state
    public const string Define = "def";
    public const string Mutate = "mut";
    public const string Function = "fun";

    // comparisons
    public const string Is = "is";
    public const string LessThan = "<";
    public const string GreaterThan = ">";
    public const string Equal = "=";

    // logic operators^
    public const string Not = "not";
    public const string And = "and";
    public const string Or = "or";

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
    // TODO: type ?? (boolean, ...)

    // list and string methods
    public const string Length = "len";
    public const string Join = "join";
    public const string Get = "get";
    public const string Set = "set";
    public const string Head = "head";                  // alias with car?
    public const string Tail = "tail";                  // alias with cdr?
    public const string Insert = "insert";
    public const string Append = "append";
    public const string Delete = "delete";
    public const string Map = "map";
    public const string Reduce = "reduce";
    public const string Range = "range";
    public const string Find = "find";
    public const string Flatten = "flatten";
}
