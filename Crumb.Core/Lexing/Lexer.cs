namespace Crumb.Core.Lexing;
public static class Lexer
{
    private const string WHITESPACE = " \n\r\t\f\v";

    public static List<Token> Tokenize(string code)
    {
        code += '\0';

        var tokens = new List<Token>();

        var lineNumber = 1;

        tokens.Add(ConsumeToken(TokenType.Start, lineNumber));

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
            }
            else if (WHITESPACE.Contains(c))
            {
                // do nothing
            }
            else if (IsComment(c, code[i + 1]))
            {
                ConsumeComment(code, ref lineNumber, ref i);
            }
            else if (c == '{')
            {
                tokens.Add(ConsumeToken(TokenType.BlockStart, lineNumber));
            }
            else if (c == '}')
            {
                tokens.Add(ConsumeToken(TokenType.BlockEnd, lineNumber));
            }
            else if (c == '(')
            {
                tokens.Add(ConsumeToken(TokenType.ApplyStart, lineNumber));
            }
            else if (c == ')')
            {
                tokens.Add(ConsumeToken(TokenType.ApplyEnd, lineNumber));
            }
            else if (c == '[')
            {
                tokens.Add(ConsumeToken(TokenType.ListStart, lineNumber));
            }
            else if (c == ']')
            {
                tokens.Add(ConsumeToken(TokenType.ListEnd, lineNumber));
            }
            else if (c == '"')
            {
                tokens.Add(ConsumeString(code, lineNumber, ref i));
            }
            else if (char.IsDigit(c) || (c == '-' && char.IsDigit(code[i + 1])))
            {
                tokens.Add(ConsumeNumber(code, lineNumber, ref i));
            }
            else
            {
                tokens.Add(ConsumeIdentifier(code, lineNumber, ref i));
            }

            i++;
        }

        tokens.Add(ConsumeToken(TokenType.End, lineNumber));

        return tokens;
    }

    #region Helper Methods
    private static bool IsComment(char current, char next) => current == '/' && next == '/';

    private static Token ConsumeToken(TokenType tokenType, int lineNumber) =>
        new(null, tokenType, lineNumber);

    private static void ConsumeComment(string code, ref int lineNumber, ref int i)
    {
        while (i < code.Length && code[i] != '\n')
        {
            i++;
        }
        lineNumber++;
    }

    private static Token ConsumeString(string code, int lineNumber, ref int i)
    {
        var stringStart = ++i;

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

        return new Token(value, TokenType.String, lineNumber);
    }

    private static Token ConsumeNumber(string code, int lineNumber, ref int i)
    {
        var numberStart = i++;

        var isFloat = false;

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
        i--;

        return new Token(value, isFloat ? TokenType.Float : TokenType.Integer, lineNumber);
    }

    private static Token ConsumeIdentifier(string code, int lineNumber, ref int i)
    {
        var identifierStart = i++;

        while (
            i < code.Length &&
            !WHITESPACE.Contains(code[i]) &&
            i + 1 < code.Length &&
            !IsComment(code[i], code[i + 1]))
        {
            i++;
        }

        var value = code[identifierStart..i];
        i--;

        return new Token(value, TokenType.Identifier, lineNumber);
    }
    #endregion
}
