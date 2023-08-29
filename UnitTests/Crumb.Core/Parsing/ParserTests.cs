using Crumb.Core.Lexing;
using Crumb.Core.Parsing;

namespace UnitTests.Crumb.Core.Parsing;

[TestFixture]
internal class ParserTests
{
    [Test]
    public static void SimpleValues()
    {
        var tokens = Lexer.Lex("""
            1 
            3.14 
            "foo" 
            bar
            """
        );

        var expected = """
            1| Statement
                1| Integer 1
            2| Statement
                2| Float 3.14
            3| Statement
                3| String foo
            4| Statement
                4| Identifier bar
            """;

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }

    [Test]
    public static void Application_Valid()
    {
        var tokens = Lexer.Lex("( 1 )");

        var expected = """
            1| Statement
                1| Application
                    1| Integer 1
            """;

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }

    [Test]
    public static void Application_MissingClose()
    {
        var tokens = Lexer.Lex("( 1 ");

        var expected = "Syntax error @ line 1: unexpected token End.";

        Assert.That(() => Parser.Parse(tokens), Throws.TypeOf<ParsingException>()
            .With.Message.EqualTo(expected));
    }
}
