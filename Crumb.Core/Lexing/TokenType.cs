namespace Crumb.Core.Lexing;

public enum TokenType
{
    Start,
    BlockStart,
    BlockEnd,
    ApplyStart,
    ApplyEnd,
    ListStart,
    ListEnd,
    String,
    Float,
    Integer,
    Identifier,
    End,
}