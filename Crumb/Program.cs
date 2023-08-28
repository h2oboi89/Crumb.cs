using crumb.core.Lexing;

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

if (debug)
{
    Console.WriteLine();
    Console.WriteLine($"CODE");
}

var code = File.ReadAllText(codePath) + '\0';

var tokens = Lexer.Lex(code);

if (debug)
{
    foreach(var token in tokens)
    {
        Console.WriteLine(token);
    }
    Console.WriteLine($"Token Count: {tokens.Count}");
}