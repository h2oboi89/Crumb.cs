using Crumb.Core.Lexing;

namespace UnitTests.Crumb.Core.Lexing;

[TestFixture]
internal class LexerTests
{
    [Test]
    public static void NullCharacter_Terminates()
    {
        var input = "\0abcd";

        var tokens = Lexer.Lex(input);

        var expected = """
            [0]: 1| Start
            [1]: 1| End
            """;

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void NewLine_IncrementsLineNumber()
    {
        var input = "\n";

        var tokens = Lexer.Lex(input);

        var expected = """
            [0]: 1| Start
            [1]: 2| End
            """;

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Whitespace_IsIgnored()
    {
        var input = " \r\t\f\v";

        var tokens = Lexer.Lex(input);

        var expected = """
            [0]: 1| Start
            [1]: 1| End
            """;

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Comments_RunUntilTheEndOfTheCurrentLine()
    {
        var values = new (string input, string expected)[]
        {
            (
                "abc // foo bar",
                """
                [0]: 1| Start
                [1]: 1| Identifier abc
                [2]: 2| End
                """
            ),
            (
                "abc // foo \n bar",
                """
                [0]: 1| Start
                [1]: 1| Identifier abc
                [2]: 2| Identifier bar
                [3]: 2| End
                """
            )
        };

        foreach (var (input, expected) in values)
        {
            var tokens = Lexer.Lex(input);

            Assert.That(Token.Print(tokens), Is.EqualTo(expected));
        }
    }

    [Test]
    public static void SingleCharacterTokens()
    {
        var input = "() = {}";

        var expected = """
            [0]: 1| Start
            [1]: 1| ApplyOpen
            [2]: 1| ApplyClose
            [3]: 1| Assignment
            [4]: 1| FunctionOpen
            [5]: 1| FunctionClose
            [6]: 1| End
            """;

        var tokens = Lexer.Lex(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void MultiCharacterTokens()
    {
        var input = "<- ->";

        var expected = """
            [0]: 1| Start
            [1]: 1| Return
            [2]: 1| Arrow
            [3]: 1| End
            """;

        var tokens = Lexer.Lex(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void PartialMultiCharacterTokensAtEndOfCode()
    {
        var values = new (string input, string expected)[]
        {
            (
                "-",
                """
                [0]: 1| Start
                [1]: 1| Identifier -
                [2]: 1| End
                """
            ),
            (
                "<",
                """
                [0]: 1| Start
                [1]: 1| Identifier <
                [2]: 1| End
                """
            ),
            (
                "/",
                """
                [0]: 1| Start
                [1]: 1| Identifier /
                [2]: 1| End
                """
            ),
        };

        foreach (var (input, expected) in values)
        {
            var tokens = Lexer.Lex(input);

            Assert.That(Token.Print(tokens), Is.EqualTo(expected));
        }
    }

    [Test]
    public static void Strings_ThrowExceptionsIfNotClosedProperly()
    {
        var values = new (string input, string expected)[]
        {
            (
                "\"I like\nstrings\"",
                "Syntax error @ line 1: unexpected new line before string closed."
            ),
            (
                "\"I like\0strings\"",
                "Syntax error @ line 1: unexpected end of file before string closed."
            )
        };
        
        foreach(var (input, expected) in values)
        {
            Assert.That(() => Lexer.Lex(input), Throws.TypeOf<LexingException>()
                .With.Message.EqualTo(expected));
        }
    }

    [Test]
    public static void String_Simple()
    {
        var input = "\"I like strings\"";

        var expected = """
            [0]: 1| Start
            [1]: 1| String I like strings
            [2]: 1| End
            """;

        var tokens = Lexer.Lex(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Strings_CanHaveEscapeSequences()
    {
        var input = """ " \\ \" " """;

        var expected = """
            [0]: 1| Start
            [1]: 1| String  \\ \" 
            [2]: 1| End
            """;

        var tokens = Lexer.Lex(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Numbers_ThrowExceptionsIfTheyHaveMultipleDecimals()
    {
        var input = "1.2.3";

        Assert.That(() => Lexer.Lex(input), Throws.TypeOf<LexingException>()
            .With.Message.EqualTo("Syntax error @ line 1: multiple decimal points in single number."));
    }


    [Test]
    public static void Numbers_CanBeIntegers()
    {
        var input = "1 2 3 314 90000 -1 -0";

        var expected = """
            [0]: 1| Start
            [1]: 1| Integer 1
            [2]: 1| Integer 2
            [3]: 1| Integer 3
            [4]: 1| Integer 314
            [5]: 1| Integer 90000
            [6]: 1| Integer -1
            [7]: 1| Integer -0
            [8]: 1| End
            """;

        var tokens = Lexer.Lex(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Numbers_CanBeFloats()
    {
        var input = "1.23 3.14159 90.000 -1.0 -0.0";

        var expected = """
            [0]: 1| Start
            [1]: 1| Float 1.23
            [2]: 1| Float 3.14159
            [3]: 1| Float 90.000
            [4]: 1| Float -1.0
            [5]: 1| Float -0.0
            [6]: 1| End
            """;

        var tokens = Lexer.Lex(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }
}
