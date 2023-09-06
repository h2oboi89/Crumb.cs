using Crumb.Core.Parsing;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal class DefinedFunction
{
    public readonly List<AstNode> Arguments = new();
    public readonly AstNode Body;

    public DefinedFunction(List<AstNode> arguments, AstNode body)
    {
        Arguments = arguments;
        Body = body;
    }
}
