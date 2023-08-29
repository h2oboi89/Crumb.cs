namespace Crumb.Core.Lexing;

public static class Lexer
{
    private const string WHITESPACE = " \r\t\f\v";
    private const string RESERVED = "{}()\"=";

    private static bool IsArrow(char c, char c1) => c == '-' && c1 == '>';
    private static bool IsReturn(char c, char c1) => c == '<' && c1 == '-';
    private static bool IsComment(char c, char c1) => c == '/' && c1 == '/';

    public static List<Token> Lex(string code)
    {
        var tokens = new List<Token>();

        var lineNumber = 1;

        tokens.Add(new Token(null, TokenType.Start, lineNumber));

        var i = 0;

        while (i < code.Length)
        {
            var c = code[i];

            if (c == '\0')
            {
                break;
            }

            if (c == '\n')
            {
                lineNumber++;
                i++;
                continue;
            }

            if (WHITESPACE.Contains(c))
            {
                i++;
                continue;
            }

            if (IsComment(c, code[i + 1]))
            {
                ConsumeComment(code, ref lineNumber, ref i);
                continue;
            }
            else if (c == '(')
            {
                tokens.Add(new Token(null, TokenType.ApplyOpen, lineNumber));
            }
            else if (c == ')')
            {
                tokens.Add(new Token(null, TokenType.ApplyClose, lineNumber));
            }
            else if (c == '=')
            {
                tokens.Add(new Token(null, TokenType.Assignment, lineNumber));
            }
            else if (c == '{')
            {
                tokens.Add(new Token(null, TokenType.FunctionOpen, lineNumber));
            }
            else if (c == '}')
            {
                tokens.Add(new Token(null, TokenType.FunctionClose, lineNumber));
            }
            else if (IsArrow(c, code[i + 1]))
            {
                tokens.Add(new Token(null, TokenType.Arrow, lineNumber));
                i++;
            }
            else if (IsReturn(c, code[i + 1]))
            {
                tokens.Add(new Token(null, TokenType.Return, lineNumber));
                i++;
            }
            else if (c == '"')
            {
                tokens.Add(ConsumeString(code, lineNumber, ref i));
                continue;
            }
            else if (char.IsDigit(c) || (c == '-' && char.IsDigit(code[i + 1])))
            {
                tokens.Add(ConsumeNumber(code, lineNumber, ref i));
                continue;
            }
            else
            {
                tokens.Add(ConsumeIdentifier(code, lineNumber, ref i));
                continue;
            }

            i++;
        }

        tokens.Add(new Token(null, TokenType.End, lineNumber));

        return tokens;
    }

    private static void ConsumeComment(string code, ref int lineNumber, ref int i)
    {
        while (i < code.Length && code[i] != '\n')
        {
            i++;
        }
        lineNumber++;
        i++;
    }

    private static Token ConsumeIdentifier(string code, int lineNumber, ref int i)
    {
        var identifierStart = i;

        while (
            i < code.Length &&
            !WHITESPACE.Contains(code[i]) &&
            !RESERVED.Contains(code[i]) &&
            i + 1 < code.Length &&
            !IsArrow(code[i], code[i + 1]) &&
            !IsReturn(code[i], code[i + 1]) &&
            !IsComment(code[i], code[i + 1]))
        {
            i++;
        }

        var value = code[identifierStart..i];

        return new Token(value, TokenType.Identifier, lineNumber);
    }

    private static Token ConsumeNumber(string code, int lineNumber, ref int i)
    {
        var numberStart = i;

        var isFloat = false;

        i++;

        while (char.IsDigit(code[i]) || 
            (code[i] == '.' && (i + 1 < code.Length) && char.IsDigit(code[i + 1])))
        {
            if (code[i] == '.')
            {
                if (isFloat)
                {
                    throw new LexingException(lineNumber, "multiple decimal points in single number.");
                }

                isFloat = true;
            }

            i++;
        }

        var value = code[numberStart..i];

        return new Token(value, isFloat ? TokenType.Float : TokenType.Integer, lineNumber);
    }

    private static Token ConsumeString(string code, int lineNumber, ref int i)
    {
        var stringStart = i + 1;

        i++;

        while (code[i] != '"')
        {
            if (code[i] == '\n')
            {
                throw new LexingException(lineNumber, "unexpected new line before string closed.");
            }

            if (code[i] == '\0')
            {
                throw new LexingException(lineNumber, "unexpected end of file before string closed.");
            }

            if (code[i] == '\\')
            {
                i++;
            }

            i++;
        }

        var value = code[stringStart..i];

        i++;

        return new Token(value, TokenType.String, lineNumber);
    }
}
