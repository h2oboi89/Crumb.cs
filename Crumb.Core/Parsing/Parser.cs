using Crumb.Core.Lexing;

namespace Crumb.Core.Parsing;
public static class Parser
{
    // Parse Program
    // EBNF: Start, Statement, End
    public static AstNode? Parse(List<Token> tokens)
    {
        var token = tokens[0];

        if (token.Type != TokenType.Start)
        {
            throw new ParsingException($"Syntax error @ Line {token.LineNumber}: Missing start token.");
        }



        return null;
    }
}
