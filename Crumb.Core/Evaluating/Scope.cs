using Crumb.Core.Evaluating.Nodes;
using Crumb.Core.Lexing;
using Crumb.Core.Parsing;

namespace Crumb.Core.Evaluating;
public class Scope
{
    public readonly Scope? Parent;

    public Dictionary<string, Node> Values = new();

    public Scope(Scope? parent) { Parent = parent; }

    public void Set(string name, Node value) => Values[name] = value;

    public bool Update(string name, Node value)
    {
        if (Values.ContainsKey(name))
        {
            Values[name] = value;
            return true;
        }

        if (Parent != null)
        {
            return Parent.Update(name, value);
        }

        return false;
    }

    public Node? Get(string name)
    {
        if (Values.TryGetValue(name, out Node? value)) return value;

        if (Parent != null) return Parent.Get(name);

        return null;
    }

    public static Scope CreateGlobal(IEnumerable<string> args)
    {
        var scope = new Scope(null);

        // TODO: pass along command line args

        // constants
        scope.Set("void", VoidNode.GetInstance());

        foreach (var (name, function) in StandardLibrary.BuiltIns.NativeFunctions)
        {
            scope.Set(name, new NativeFunctionNode(function));
        }

        foreach (var (name, function) in StandardLibrary.BuiltIns.Functions)
        {
            scope.Set(name, CreateFunction(function, scope));
        }

        return scope;
    }

    private static FunctionNode CreateFunction(string input, Scope scope)
    {
        var tokens = Lexer.Tokenize(input);
        var ast = Parser.Parse(tokens);
        var function = Interpreter.Evaluate(ast, scope);

        return (FunctionNode)function;
    }
}
