using Crumb.Core.Lexing;

namespace Crumb.Core.Parsing;
public static class Parser
{
    public static AstNode Parse(List<Token> tokens) => ParseProgram(tokens);

    private static AstNode ParseProgram(List<Token> tokens)
    {
        var index = 0;

        ConsumeToken(tokens, TokenType.Start, ref index);

        var ast = ParseBlock(tokens, ref index);

        ConsumeToken(tokens, TokenType.End, ref index);

        return ast;
    }

    private static AstNode ParseBlock(List<Token> tokens, ref int index)
    {
        var token = tokens[index];

        ConsumeToken(tokens, TokenType.BlockStart, ref index);

        var ast = new AstNode(OpCodes.Block, token.LineNumber);

        while (tokens[index].Type != TokenType.BlockEnd)
        {
            switch(tokens[index].Type)
            {
                case TokenType.ApplyStart:
                    ast.Append(ParseApply(tokens, ref index));
                    break;
                default:
                    ast.Append(ParseAtom(tokens, ref index));
                    break;
            }
        }

        ConsumeToken(tokens, TokenType.BlockEnd, ref index);

        return ast;
    }

    private static AstNode ParseApply(List<Token> tokens, ref int index)
    {
        var token = tokens[index];

        ConsumeToken(tokens, TokenType.ApplyStart, ref index);

        var ast = new AstNode(OpCodes.Apply, token.LineNumber);

        while (tokens[index].Type != TokenType.ApplyEnd)
        {
            switch (tokens[index].Type)
            {
                case TokenType.ApplyStart:
                    ast.Append(ParseApply(tokens, ref index));
                    break;
                default:
                    ast.Append(ParseAtom(tokens, ref index));
                    break;
            }
        }

        ConsumeToken(tokens, TokenType.ApplyEnd, ref index);

        return ast;
    }

    private static AstNode ParseAtom(List<Token> tokens, ref int index)
    {
        var token = tokens[index];

        return token.Type switch
        {
            TokenType.BlockStart => ParseBlock(tokens, ref index),
            TokenType.ListStart => ParseList(tokens, ref index),
            TokenType.String => ConsumeToken(tokens, OpCodes.String, ref index),
            TokenType.Float => ConsumeToken(tokens, OpCodes.Float, ref index),
            TokenType.Integer => ConsumeToken(tokens, OpCodes.Integer, ref index),
            TokenType.Identifier => ConsumeToken(tokens, OpCodes.Identifier, ref index),
            _ => throw new ParsingException(token, $"unexpected token {token.Type} for atom."),
        };
    }

    private static AstNode ParseList(List<Token> tokens, ref int index)
    {
        var token = tokens[index];

        ConsumeToken(tokens, TokenType.ListStart, ref index);

        var ast = new AstNode(OpCodes.List, token.LineNumber);

        while (tokens[index].Type != TokenType.ListEnd)
        {
            ast.Append(ParseAtom(tokens, ref index));
        }

        ConsumeToken(tokens, TokenType.ListEnd, ref index);

        return ast;
    }

    #region Helper Methods
    private static Token ConsumeToken(List<Token> tokens, TokenType expected, ref int index)
    {
        var token = tokens[index++];

        if (token.Type != expected)
        {
            throw new ParsingException(token, $"expected {expected}, but got {token.Type}.");
        }

        return token;
    }

    private static AstNode ConsumeToken(List<Token> tokens, OpCodes opCode, ref int index)
    {
        var token = tokens[index++];

        return new AstNode(opCode, token.LineNumber, token.Value);
    }
    #endregion
}

