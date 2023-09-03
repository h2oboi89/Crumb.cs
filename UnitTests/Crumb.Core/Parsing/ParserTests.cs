using Crumb.Core.Lexing;
using Crumb.Core.Parsing;

namespace UnitTests.Crumb.Core.Parsing;

[TestFixture]
internal static class ParserTests
{
    [Test]
    public static void EmptyString_ThrowsParsingException()
    {
        var tokens = Lexer.Tokenize(string.Empty);

        Assert.That(() => Parser.Parse(tokens), Throws.TypeOf<ParsingException>()
            .With.Message.EqualTo("Syntax error @ line 1: expected BlockStart, but got End."));
    }

    [Test]
    public static void MinimalProgram()
    {
        var tokens = Lexer.Tokenize("{ }");

        var expected = """
            1 | Block
            """;

        var ast = Parser.Parse(tokens);

        Assert.That(ast.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public static void SimpleAtoms()
    {
        var tokens = Lexer.Tokenize("""
            { 
                1 
                2.3 
                "foo" 
                bar 
            } 
            """);

        var expected = """
            1 | Block
                2 | Integer '1'
                3 | Float '2.3'
                4 | String 'foo'
                5 | Identifier 'bar'
            """;

        var ast = Parser.Parse(tokens);

        Assert.That(ast.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public static void NestedBlocks()
    {
        var tokens = Lexer.Tokenize("""
            {
                {
                    7
                }
            }
            """);

        var expected = """
            1 | Block
                2 | Block
                    3 | Integer '7'
            """;

        var ast = Parser.Parse(tokens);

        Assert.That(ast.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public static void SimpleListAtom()
    {
        var tokens = Lexer.Tokenize("""
            {
                [ -42 "a" 3.14 pi ]
            }
            """);

        var expected = """
            1 | Block
                2 | List
                    2 | Integer '-42'
                    2 | String 'a'
                    2 | Float '3.14'
                    2 | Identifier 'pi'
            """;

        var ast = Parser.Parse(tokens);

        Assert.That(ast.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public static void NestLists()
    {
        var tokens = Lexer.Tokenize("""
            {
                [ 
                    [ 1 2 3 4 ]
                    [ "i" "like" "lists" ]
                ]
            }
            """);

        var expected = """
            1 | Block
                2 | List
                    3 | List
                        3 | Integer '1'
                        3 | Integer '2'
                        3 | Integer '3'
                        3 | Integer '4'
                    4 | List
                        4 | String 'i'
                        4 | String 'like'
                        4 | String 'lists'
            """;

        var ast = Parser.Parse(tokens);

        Assert.That(ast.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public static void AtomInvalidToken()
    {
        var tokens = Lexer.Tokenize("""
            {
                [ ( foo ]
            }
            """);

        Assert.That(() => Parser.Parse(tokens), Throws.TypeOf<ParsingException>()
            .With.Message.EqualTo("Syntax error @ line 2: unexpected token ApplyStart for atom."));
    }

    [Test]
    public static void SimpleApply()
    {
        var tokens = Lexer.Tokenize("""
            {
                ( print "Hello, World!" )
            }
            """);

        var expected = """
            1 | Block
                2 | Apply
                    2 | Identifier 'print'
                    2 | String 'Hello, World!'
            """;

        var ast = Parser.Parse(tokens);

        Assert.That(ast.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public static void NestedApply()
    {
        var tokens = Lexer.Tokenize("""
            {
                ( 
                    ( fun [ a b ] { 
                        ( + a b ) 
                    } )
                    1
                    2 
                )
            }
            """);

        var expected = """
            1 | Block
                2 | Apply
                    3 | Apply
                        3 | Identifier 'fun'
                        3 | List
                            3 | Identifier 'a'
                            3 | Identifier 'b'
                        3 | Block
                            4 | Apply
                                4 | Identifier '+'
                                4 | Identifier 'a'
                                4 | Identifier 'b'
                    6 | Integer '1'
                    7 | Integer '2'
            """;

        var ast = Parser.Parse(tokens);

        Assert.That(ast.ToString(), Is.EqualTo(expected));
    }
}
