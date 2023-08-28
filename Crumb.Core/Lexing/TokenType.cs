namespace Crumb.Core.Lexing;

public enum TokenType
{
    ApplyOpen,
    ApplyClose,
    Assignment,
    FunctionOpen,
    FunctionClose,
    Comma, // unused
    Arrow,
    Return,
    Identifier,
    Integer,
    Float,
    String,
    End,
    Start,
}
