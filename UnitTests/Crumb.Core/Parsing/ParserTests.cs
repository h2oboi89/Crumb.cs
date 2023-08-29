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

    [Test]
    public static void Application_CanBeNested()
    {
        var tokens = Lexer.Lex("( ( 1 ) ( 2 ) )");

        var expected = """
            1| Statement
                1| Application
                    1| Application
                        1| Integer 1
                    1| Application
                        1| Integer 2
            """;

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }

    [Test]
    public static void Return()
    {
        var tokens = Lexer.Lex("<- 42 ");

        var expected = """
            1| Statement
                1| Return
                    1| Integer 42
            """;

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }

    [Test]
    public static void Assignment()
    {
        var tokens = Lexer.Lex("a = ( add 1 2 )");

        var expected = """
            1| Statement
                1| Assignment
                    1| Identifier a
                    1| Application
                        1| Identifier add
                        1| Integer 1
                        1| Integer 2
            """;

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }

    [Test]
    public static void Function_NoArguments()
    {
        var tokens = Lexer.Lex("foo = { <- \"bar\" } ");

        var expected = """
            1| Statement
                1| Assignment
                    1| Identifier foo
                    1| Function
                        1| Statement
                            1| Return
                                1| String bar
            """;

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }

    [Test]
    public static void Function_WithArguments()
    {
        var tokens = Lexer.Lex("+ = { a b -> <- ( add a b ) } ");

        var expected = """
            1| Statement
                1| Assignment
                    1| Identifier +
                    1| Function
                        1| Identifier a
                        1| Identifier b
                        1| Statement
                            1| Return
                                1| Application
                                    1| Identifier add
                                    1| Identifier a
                                    1| Identifier b
            """;

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }

    [Test]
    public static void EmptyInput()
    {
        var tokens = Lexer.Lex(string.Empty);

        var expected = "No Program";

        Assert.That(AstNode.Print(Parser.Parse(tokens)), Is.EqualTo(expected));
    }
}
