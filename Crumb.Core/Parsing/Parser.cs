using Crumb.Core.Lexing;

namespace Crumb.Core.Parsing;
public static class Parser
{

    public static AstNode? Parse(List<Token> tokens) => ParseProgram(tokens);

    // parse program
    // ebnf: program = start, statement, end;
    private static AstNode ParseProgram(List<Token> tokens)
    {
        var index = 0;

        var token = tokens[index++];

        if (token.Type != TokenType.Start)
        {
            throw new ParsingException($"Syntax error @ Line {token.LineNumber}: Missing start token.");
        }

        var ast = ParseStatement(tokens, ref index);

        token = tokens[index++];

        if (token.Type != TokenType.End)
        {
            throw new ParsingException($"Syntax error @ Line {token.LineNumber}: Missing end token.");
        }

        return ast;
    }

    // parse statement
    // ebnf: statement = {return | assignment | value};
    // precedence: return, assignment, value
    private static AstNode ParseStatement(List<Token> tokens, ref int index)
    {
        var token = tokens[index++];

        var ast = new AstNode(null, OpCodes.Statement, token.LineNumber);

        token = tokens[index];

        switch(token.Type)
        {
            case TokenType.Return:
                ast.Append(ParseReturn(tokens, ref index));
                break;
            case TokenType.Assignment:
                ast.Append(ParseAssignment(tokens, ref index));
                break;
            default:
                ast.Append(ParseValue(tokens, ref index));
                break;
        }

        return ast;
    }

    // parse return
    // ebnf: return = "<-", value;
    private static AstNode ParseReturn(List<Token> tokens, ref int index)
    {
        throw new NotImplementedException();
    }

    // parse assignment
    // ebnf: assignment = identifier, "=", value;
    private static AstNode ParseAssignment(List<Token> tokens, ref int index)
    {
        throw new NotImplementedException();
    }

    // parse value
    // ebnf: value = application | function | int | float | string | identifier;
    private static AstNode ParseValue(List<Token> tokens, ref int index)
    {
        throw new NotImplementedException();
    }

    // parse application
    // ebnf: application = "(", value, ")";
    private static AstNode ParseApplication(List<Token> tokens, ref int index)
    {
        throw new NotImplementedException();
    }
}
