using Crumb.Core.Lexing;

namespace UnitTests.Crumb.Core.Lexing;

[TestFixture]
internal static class LexerTests
{

    [Test]
    public static void NullCharacter_MarksEndOfInput()
    {
        var input = "\0abcd";

        var expected = """
            [0] : 1 | Start
            [1] : 1 | End
            """;

        var tokens = Lexer.Tokenize(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void NewLine_IncrementsLineNumber()
    {
        var input = "\n";

        var tokens = Lexer.Tokenize(input);

        var expected = """
            [0] : 1 | Start
            [1] : 2 | End
            """;

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Whitespace_IsIgnored()
    {
        var input = " \r\t\f\v";

        var tokens = Lexer.Tokenize(input);

        var expected = """
            [0] : 1 | Start
            [1] : 1 | End
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
                [0] : 1 | Start
                [1] : 1 | Identifier 'abc'
                [2] : 2 | End
                """
            ),
            (
                "abc // foo\n bar\n",
                """
                [0] : 1 | Start
                [1] : 1 | Identifier 'abc'
                [2] : 2 | Identifier 'bar'
                [3] : 3 | End
                """
            )
        };

        foreach (var (input, expected) in values)
        {
            var tokens = Lexer.Tokenize(input);

            Assert.That(Token.Print(tokens), Is.EqualTo(expected));
        }
    }

    [Test]
    public static void SimpleTokens()
    {
        var input = "{} () []";

        var expected = """
            [0] : 1 | Start
            [1] : 1 | BlockStart
            [2] : 1 | BlockEnd
            [3] : 1 | ApplyStart
            [4] : 1 | ApplyEnd
            [5] : 1 | ListStart
            [6] : 1 | ListEnd
            [7] : 1 | End
            """;

        var tokens = Lexer.Tokenize(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Strings_ThrowExceptionsIfNotClosedProperly()
    {
        var values = new (string input, string expected)[]
        {
            (
                """
                "I like
                strings"
                """,
                "Syntax error @ line 1: unexpected new line before string closed."
            ),
            (
                """
                "I like // strings
                """,
                "Syntax error @ line 1: unexpected end of file before string closed."
            )
        };

        foreach (var (input, expected) in values)
        {
            Assert.That(() => Lexer.Tokenize(input), Throws.TypeOf<LexingException>()
                .With.Message.EqualTo(expected));
        }
    }

    [Test]
    public static void String_Simple()
    {
        var input = """
            "I like strings"
            """;

        var expected = """
            [0] : 1 | Start
            [1] : 1 | String 'I like strings'
            [2] : 1 | End
            """;

        var tokens = Lexer.Tokenize(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Strings_CanHaveEscapeSequences()
    {
        var input = """ " \\ \" " """;

        var expected = """
            [0] : 1 | Start
            [1] : 1 | String ' \\ \" '
            [2] : 1 | End
            """;

        var tokens = Lexer.Tokenize(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Numbers_ThrowExceptionsIfTheyHaveMultipleDecimals()
    {
        var input = "1.2.3";

        Assert.That(() => Lexer.Tokenize(input), Throws.TypeOf<LexingException>()
            .With.Message.EqualTo("Syntax error @ line 1: multiple decimal points in single number."));
    }


    [Test]
    public static void Numbers_CanBeIntegers()
    {
        var input = "1 2 3 314 90000 -1 -0";

        var expected = """
            [0] : 1 | Start
            [1] : 1 | Integer '1'
            [2] : 1 | Integer '2'
            [3] : 1 | Integer '3'
            [4] : 1 | Integer '314'
            [5] : 1 | Integer '90000'
            [6] : 1 | Integer '-1'
            [7] : 1 | Integer '-0'
            [8] : 1 | End
            """;

        var tokens = Lexer.Tokenize(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }

    [Test]
    public static void Numbers_CanBeFloats()
    {
        var input = "1.23 3.14159 90.000 -1.0 -0.0";

        var expected = """
            [0] : 1 | Start
            [1] : 1 | Float '1.23'
            [2] : 1 | Float '3.14159'
            [3] : 1 | Float '90.000'
            [4] : 1 | Float '-1.0'
            [5] : 1 | Float '-0.0'
            [6] : 1 | End
            """;

        var tokens = Lexer.Tokenize(input);

        Assert.That(Token.Print(tokens), Is.EqualTo(expected));
    }


    [Test]
    public static void ValidPrograms()
    {
        var values = new (string input, string expected)[]
        {
            (
                "{ ( print \"Hello, World!\" ) }",
                """
                [0] : 1 | Start
                [1] : 1 | BlockStart
                [2] : 1 | ApplyStart
                [3] : 1 | Identifier 'print'
                [4] : 1 | String 'Hello, World!'
                [5] : 1 | ApplyEnd
                [6] : 1 | BlockEnd
                [7] : 1 | End
                """
            ),
            (
                """
                {
                    ( def foo ( fun [ a b ] { + a b } ) )
                    ( def bar ( fun [ a b ] { - a b } ) )
                    ( def baz ( fun [ a b ] { ( print ( * ( foo a b ) ( bar a b ) ) ) } ) )
                    ( def qux ( fun ( baz 7 3 ) ) )
                    
                    ( qux ) // prints 40
                }
                """,
                """
                [0] : 1 | Start
                [1] : 1 | BlockStart
                [2] : 2 | ApplyStart
                [3] : 2 | Identifier 'def'
                [4] : 2 | Identifier 'foo'
                [5] : 2 | ApplyStart
                [6] : 2 | Identifier 'fun'
                [7] : 2 | ListStart
                [8] : 2 | Identifier 'a'
                [9] : 2 | Identifier 'b'
                [10] : 2 | ListEnd
                [11] : 2 | BlockStart
                [12] : 2 | Identifier '+'
                [13] : 2 | Identifier 'a'
                [14] : 2 | Identifier 'b'
                [15] : 2 | BlockEnd
                [16] : 2 | ApplyEnd
                [17] : 2 | ApplyEnd
                [18] : 3 | ApplyStart
                [19] : 3 | Identifier 'def'
                [20] : 3 | Identifier 'bar'
                [21] : 3 | ApplyStart
                [22] : 3 | Identifier 'fun'
                [23] : 3 | ListStart
                [24] : 3 | Identifier 'a'
                [25] : 3 | Identifier 'b'
                [26] : 3 | ListEnd
                [27] : 3 | BlockStart
                [28] : 3 | Identifier '-'
                [29] : 3 | Identifier 'a'
                [30] : 3 | Identifier 'b'
                [31] : 3 | BlockEnd
                [32] : 3 | ApplyEnd
                [33] : 3 | ApplyEnd
                [34] : 4 | ApplyStart
                [35] : 4 | Identifier 'def'
                [36] : 4 | Identifier 'baz'
                [37] : 4 | ApplyStart
                [38] : 4 | Identifier 'fun'
                [39] : 4 | ListStart
                [40] : 4 | Identifier 'a'
                [41] : 4 | Identifier 'b'
                [42] : 4 | ListEnd
                [43] : 4 | BlockStart
                [44] : 4 | ApplyStart
                [45] : 4 | Identifier 'print'
                [46] : 4 | ApplyStart
                [47] : 4 | Identifier '*'
                [48] : 4 | ApplyStart
                [49] : 4 | Identifier 'foo'
                [50] : 4 | Identifier 'a'
                [51] : 4 | Identifier 'b'
                [52] : 4 | ApplyEnd
                [53] : 4 | ApplyStart
                [54] : 4 | Identifier 'bar'
                [55] : 4 | Identifier 'a'
                [56] : 4 | Identifier 'b'
                [57] : 4 | ApplyEnd
                [58] : 4 | ApplyEnd
                [59] : 4 | ApplyEnd
                [60] : 4 | BlockEnd
                [61] : 4 | ApplyEnd
                [62] : 4 | ApplyEnd
                [63] : 5 | ApplyStart
                [64] : 5 | Identifier 'def'
                [65] : 5 | Identifier 'qux'
                [66] : 5 | ApplyStart
                [67] : 5 | Identifier 'fun'
                [68] : 5 | ApplyStart
                [69] : 5 | Identifier 'baz'
                [70] : 5 | Integer '7'
                [71] : 5 | Integer '3'
                [72] : 5 | ApplyEnd
                [73] : 5 | ApplyEnd
                [74] : 5 | ApplyEnd
                [75] : 7 | ApplyStart
                [76] : 7 | Identifier 'qux'
                [77] : 7 | ApplyEnd
                [78] : 8 | BlockEnd
                [79] : 8 | End
                """
            ),
        };

        foreach (var (input, expected) in values)
        {
            var tokens = Lexer.Tokenize(input);

            Assert.That(Token.Print(tokens), Is.EqualTo(expected));
        }
    }
}
