using Crumb.Core.Lexing;

namespace Crumb.Core.Parsing;
public static class Parser
{
    public static List<AstNode> Parse(List<Token> tokens) => ParseProgram(tokens);

    // parse program
    // ebnf: program = start, statement, end;
    private static List<AstNode> ParseProgram(List<Token> tokens)
    {
        var index = 0;

        ConsumeToken(tokens, TokenType.Start, ref index);

        var ast = new List<AstNode>();

        while (tokens[index].Type != TokenType.End)
        {
            ast.Add(ParseStatement(tokens, ref index));
        }

        ConsumeToken(tokens, TokenType.End, ref index);

        return ast;
    }

    // parse statement
    // ebnf: statement = {return | assignment | value};
    // precedence: return, assignment, value
    private static AstNode ParseStatement(List<Token> tokens, ref int index)
    {
        var token = tokens[index];

        var ast = new AstNode(null, OpCodes.Statement, token.LineNumber);

        if (token.Type == TokenType.Return)
        {
            ast.Append(ParseReturn(tokens, ref index));
        }
        else if (token.Type == TokenType.Identifier &&
            tokens[index + 1].Type == TokenType.Assignment)
        {
            ast.Append(ParseAssignment(tokens, ref index));
        }
        else
        {
            ast.Append(ParseValue(tokens, ref index));
        }

        return ast;
    }

    // parse return
    // ebnf: return = "<-", value;
    private static AstNode ParseReturn(List<Token> tokens, ref int index)
    {
        ConsumeToken(tokens, TokenType.Return, ref index);

        return ParseValue(tokens, ref index);
    }

    // parse assignment
    // ebnf: assignment = identifier, "=", value;
    private static AstNode ParseAssignment(List<Token> tokens, ref int index)
    {
        var identifier = ConsumeToken(tokens, OpCodes.Identifier, ref index);

        var ast = new AstNode(null, OpCodes.Assignment, identifier.LineNumber);

        ast.Append(identifier);

        ConsumeToken(tokens, TokenType.Assignment, ref index);

        ast.Append(ParseValue(tokens, ref index));

        return ast;
    }

    // parse value
    // ebnf: value = application | function | int | float | string | identifier;
    private static AstNode ParseValue(List<Token> tokens, ref int index)
    {
        var token = tokens[index];

        return token.Type switch
        {
            TokenType.ApplyOpen => ParseApplication(tokens, ref index),
            TokenType.FunctionOpen => ParseFunction(tokens, ref index),
            TokenType.Integer => ConsumeToken(tokens, OpCodes.Integer, ref index),
            TokenType.Float => ConsumeToken(tokens, OpCodes.Float, ref index),
            TokenType.String => ConsumeToken(tokens, OpCodes.String, ref index),
            TokenType.Identifier => ConsumeToken(tokens, OpCodes.Identifier, ref index),
            _ => throw new ParsingException($"Unexpected token {token}."),
        };
    }

    // parse application
    // ebnf: application = "(", {value}, ")";
    private static AstNode ParseApplication(List<Token> tokens, ref int index)
    {
        var token = ConsumeToken(tokens, TokenType.ApplyOpen, ref index);

        var ast = new AstNode(null, OpCodes.Application, token.LineNumber);

        while (tokens[index].Type != TokenType.ApplyClose)
        {
            ast.Append(ParseValue(tokens, ref index));
        }

        ConsumeToken(tokens, TokenType.ApplyClose, ref index);

        return ast;
    }

    // parse function
    // ebnf: "{", [{identifier}, "->"], statement, "}";
    private static AstNode ParseFunction(List<Token> tokens, ref int index)
    {
        var token = ConsumeToken(tokens, TokenType.FunctionOpen, ref index);

        var ast = new AstNode(null, OpCodes.Function, token.LineNumber);

        token = tokens[index];

        if (token.Type == TokenType.Identifier && tokens[index + 1].Type != TokenType.Assignment)
        {
            while (tokens[index].Type != TokenType.Arrow)
            {
                token = ConsumeToken(tokens, TokenType.Identifier, ref index);

                ast.Append(new AstNode(token.Value, OpCodes.Identifier, token.LineNumber));
            }

            ConsumeToken(tokens, TokenType.Arrow, ref index);
        }

        while (tokens[index].Type != TokenType.FunctionClose)
        {
            ast.Append(ParseStatement(tokens, ref index));
        }

        ConsumeToken(tokens, TokenType.FunctionClose, ref index);

        return ast;
    }

    private static AstNode ConsumeToken(List<Token> tokens, OpCodes opCode, ref int index)
    {
        var token = tokens[index++];

        return new AstNode(token.Value, opCode, token.LineNumber);
    }

    private static Token ConsumeToken(List<Token> tokens, TokenType expected, ref int index)
    {
        var token = tokens[index++];

        if (token.Type != expected)
        {
            throw new ParsingException(token, $"Expected {expected}, but got {token.Type}");
        }

        return token;
    }
}
