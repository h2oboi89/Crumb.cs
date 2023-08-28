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
                // comments
                while (code[i] != '\n' && i < code.Length)
                {
                    i++;
                }
                lineNumber++;
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
                var stringStart = i + 1;

                i++;

                while (code[i] != '"')
                {
                    if (code[i] == '\n')
                    {
                        throw new LexingException($"Syntax error @ Line {lineNumber}: unexpected new line before string closed.");
                    }

                    if (code[i] == '\0')
                    {
                        throw new LexingException($"Syntax error @ Line {lineNumber}: unexpected end of file before string closed.");
                    }

                    if (code[i] == '\\')
                    {
                        i++;
                    }

                    i++;
                }

                var value = code[stringStart..i];

                tokens.Add(new Token(value, TokenType.String, lineNumber));
            }
            else if (char.IsDigit(c) || (c == '-' && char.IsDigit(code[i + 1])))
            {
                var numberStart = i;

                var isFloat = false;

                i++;

                while (char.IsDigit(code[i]) || code[i] == '.')
                {
                    if (code[i] == '.')
                    {
                        if (isFloat)
                        {
                            throw new LexingException($"Syntax error @ {lineNumber}: Multiple decimal points in single number.");
                        }

                        isFloat = true;
                    }

                    i++;
                }

                var value = code[numberStart..i];

                tokens.Add(new Token(value, isFloat ? TokenType.Float : TokenType.Integer, lineNumber));

                continue;
            }
            else
            {
                var identifierStart = i;

                while (
                    !WHITESPACE.Contains(code[i]) &&
                    !RESERVED.Contains(code[i]) &&
                    !IsArrow(code[i], code[i + 1]) &&
                    !IsReturn(code[i], code[i + 1]) &&
                    !IsComment(code[i], code[i + 1]))
                {
                    i++;
                }

                var value = code[identifierStart..i];

                tokens.Add(new Token(value, TokenType.Identifier, lineNumber));

                continue;
            }

            i++;
        }

        tokens.Add(new Token(null, TokenType.End, lineNumber));

        return tokens;
    }
}
