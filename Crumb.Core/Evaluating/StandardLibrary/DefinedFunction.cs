using Crumb.Core.Parsing;

namespace Crumb.Core.Evaluating.StandardLibrary;
internal class DefinedFunction : IEquatable<DefinedFunction>
{
    public readonly List<AstNode> Arguments = new();
    public readonly AstNode Body;

    public DefinedFunction(List<AstNode> arguments, AstNode body)
    {
        Arguments = arguments;
        Body = body;
    }
    public bool Equals(DefinedFunction? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Enumerable.SequenceEqual(Arguments, other.Arguments) && Body == other.Body;
    }

    public override bool Equals(object? obj) => obj is DefinedFunction other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach(var arg in Arguments)
        {
            hashCode.Add(arg.GetHashCode());
        }
        hashCode.Add(Body);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(DefinedFunction left, DefinedFunction right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(DefinedFunction left, DefinedFunction right) => !(left == right);
}
