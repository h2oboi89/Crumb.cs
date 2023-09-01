using Crumb.Core.Evaluating.Nodes;

namespace Crumb.Core.Evaluating;
public class Scope
{
    public readonly Scope? Parent;

    public Dictionary<string, Node> Values = new();

    public Scope(Scope? parent) { Parent = parent; }

    public void Set(string name, Node value) => Values[name] = value;

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

        foreach (var (name, function) in StandardLibrary.NativeFunctions)
        {
            scope.Set(name, new NativeFunctionNode(function));
        }

        return scope;
    }
}
