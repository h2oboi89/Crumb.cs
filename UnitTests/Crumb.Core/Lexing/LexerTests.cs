﻿using Crumb.Core.Lexing;

namespace UnitTests.Crumb.Core.Lexing;

[TestFixture]
internal class LexerTests
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
                "\"I like\nstrings\"",
                "Syntax error @ line 1: unexpected new line before string closed."
            ),
            (
                "\"I like\0strings\"",
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
        var input = "\"I like strings\"";

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
                    ( def foo [ a b ] { + a b } )
                    ( def bar [ a b ] { - a b } )
                    ( def baz [ a b ] { ( print ( * ( foo a b ) ( bar a b ) ) ) } )
                    ( def qux ( baz 7 3 ) )
                    
                    ( qux ) // prints 40
                }
                """,
                """
                [0] : 1 | Start
                [1] : 1 | BlockStart
                [2] : 2 | ApplyStart
                [3] : 2 | Identifier 'def'
                [4] : 2 | Identifier 'foo'
                [5] : 2 | ListStart
                [6] : 2 | Identifier 'a'
                [7] : 2 | Identifier 'b'
                [8] : 2 | ListEnd
                [9] : 2 | BlockStart
                [10] : 2 | Identifier '+'
                [11] : 2 | Identifier 'a'
                [12] : 2 | Identifier 'b'
                [13] : 2 | BlockEnd
                [14] : 2 | ApplyEnd
                [15] : 3 | ApplyStart
                [16] : 3 | Identifier 'def'
                [17] : 3 | Identifier 'bar'
                [18] : 3 | ListStart
                [19] : 3 | Identifier 'a'
                [20] : 3 | Identifier 'b'
                [21] : 3 | ListEnd
                [22] : 3 | BlockStart
                [23] : 3 | Identifier '-'
                [24] : 3 | Identifier 'a'
                [25] : 3 | Identifier 'b'
                [26] : 3 | BlockEnd
                [27] : 3 | ApplyEnd
                [28] : 4 | ApplyStart
                [29] : 4 | Identifier 'def'
                [30] : 4 | Identifier 'baz'
                [31] : 4 | ListStart
                [32] : 4 | Identifier 'a'
                [33] : 4 | Identifier 'b'
                [34] : 4 | ListEnd
                [35] : 4 | BlockStart
                [36] : 4 | ApplyStart
                [37] : 4 | Identifier 'print'
                [38] : 4 | ApplyStart
                [39] : 4 | Identifier '*'
                [40] : 4 | ApplyStart
                [41] : 4 | Identifier 'foo'
                [42] : 4 | Identifier 'a'
                [43] : 4 | Identifier 'b'
                [44] : 4 | ApplyEnd
                [45] : 4 | ApplyStart
                [46] : 4 | Identifier 'bar'
                [47] : 4 | Identifier 'a'
                [48] : 4 | Identifier 'b'
                [49] : 4 | ApplyEnd
                [50] : 4 | ApplyEnd
                [51] : 4 | ApplyEnd
                [52] : 4 | BlockEnd
                [53] : 4 | ApplyEnd
                [54] : 5 | ApplyStart
                [55] : 5 | Identifier 'def'
                [56] : 5 | Identifier 'qux'
                [57] : 5 | ApplyStart
                [58] : 5 | Identifier 'baz'
                [59] : 5 | Integer '7'
                [60] : 5 | Integer '3'
                [61] : 5 | ApplyEnd
                [62] : 5 | ApplyEnd
                [63] : 7 | ApplyStart
                [64] : 7 | Identifier 'qux'
                [65] : 7 | ApplyEnd
                [66] : 8 | BlockEnd
                [67] : 8 | End
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
