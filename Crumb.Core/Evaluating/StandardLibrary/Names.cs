namespace Crumb.Core.Evaluating.StandardLibrary;
internal static class Names
{
    // constants
    public const string Void = "void";
    public const string True = "true";
    public const string False = "false";

    // IO
    public const string Print = "print";
    public const string PrintLine = "printLine";
    public const string Input = "input";
    public const string InputLine = "inputLine";
    public const string Rows = "rows";
    public const string Columns = "columns";
    public const string Clear = "clear";
    public const string ReadFile = "read";
    public const string WriteFile = "write";

    // scope state
    public const string Define = "def";
    public const string Mutate = "mut";
    public const string Function = "fun";
    public const string Import = "import";
    // NOTE: These are defined in the Interpreter directly
    // as they need access to internals and operate differently
    // than other functions

    // comparisons
    public const string Is = "is";
    public const string LessThan = "<";
    public const string LessThanOrEqual = "<=";
    public const string Equal = "=";
    public const string GreaterThanOrEqual = ">=";
    public const string GreaterThan = ">";

    // logic operators^
    public const string Not = "not";
    public const string And = "and";
    public const string Or = "or";

    // math
    public const string Add = "+";
    public const string Subtract = "-";
    public const string Multiply = "*";
    public const string Divide = "/";
    public const string Remainder = "%";
    public const string Power = "^";
    public const string Random = "random";
    public const string Floor = "floor";
    public const string Ceiling = "ceiling";
    public const string Round = "round";

    // TODO: bitwise? <<, <<<, >>, >>>, &, |, ~, ^

    // control
    public const string For = "for";
    public const string While = "while";
    public const string If = "if";
    public const string Wait = "wait";

    // types
    public const string IntegerType = "integer";
    public const string FloatType = "float";
    public const string StringType = "string";
    public const string BooleanType = "boolean";
    public const string ListType = "list";
    public const string FunctionType = "function";

    // list and string methods
    public const string Length = "len";
    public const string Join = "join";
    public const string Get = "get";
    public const string Set = "set";
    public const string Insert = "insert";
    public const string Append = "append";
    public const string Delete = "delete";
    public const string Map = "map";
    public const string Reduce = "reduce";
    public const string Range = "range";
    public const string Find = "find";
    public const string Flatten = "flatten";
}
