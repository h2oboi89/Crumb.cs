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

var codePath = args[fileArg];


var code = File.ReadAllText(codePath) + '\0';

if (debug)
{
    Console.WriteLine();
    Console.WriteLine($"CODE");
    Console.WriteLine(code);
}

var tokens = Lexer.Lex(code);

if (debug)
{
    Console.WriteLine();
    Console.WriteLine($"TOKENS");
    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }
    Console.WriteLine($"Token Count: {tokens.Count}");
}

var ast = Parser.Parse(tokens);

if (debug)
{
    Console.WriteLine();
    Console.WriteLine($"AST");
    Console.WriteLine(ast?.ToString() ?? "NULL");
}