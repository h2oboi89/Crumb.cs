using Crumb.Core.Evaluating;
using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Lexing;
using Crumb.Core.Parsing;

var debug = false;

const string MISSING_FILE = "Error: Supply file path to read from.";

var fileArg = 0;

if (args.Length > 0 && args[0] == "-d")
{
    debug = true;
    fileArg = 1;
}

if (args.Length < fileArg + 1)
{
    Console.Error.WriteLine(MISSING_FILE);
    Environment.Exit(1);
}

var code = File.ReadAllText(args[fileArg]);

if (debug)
{
    Console.WriteLine($"CODE");
    Console.WriteLine(code);
}

var tokens = Lexer.Tokenize(code);

if (debug)
{
    Console.WriteLine();
    Console.WriteLine($"TOKENS");
    Console.WriteLine(Token.Print(tokens));
    Console.WriteLine($"Token Count: {tokens.Count}");
}

var program = (AstNode?)null;

try
{
    program = Parser.Parse(tokens);
}
catch (ParsingException e)
{
    Console.Error.WriteLine(e.Message);
    Environment.Exit(1);
}

if (debug)
{
    Console.WriteLine();
    Console.WriteLine($"AST");
    Console.WriteLine(program.ToString());
}

if (debug)
{
    Console.WriteLine();
    Console.WriteLine("EVAL");
}

try
{
    var result = Interpreter.Evaluate(args, program);

    //if (result is IntegerNode exitCodeNode)
    //{
    //    Environment.Exit(exitCodeNode.Value);
    //}
}
catch (RuntimeException e)
{
    Console.Error.WriteLine(e.Message);
    Environment.Exit(1);
}