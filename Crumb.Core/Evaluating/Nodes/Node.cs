namespace Crumb.Core.Evaluating.Nodes;
public abstract class Node : IEquatable<Node>
{
    public readonly NodeTypes Type;
    public readonly object Value;

    public Node(NodeTypes type, object value)
    {
        Type = type;
        Value = value;
    }

    public bool Equals(Node? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Type == other.Type &&
            ValueEquals(other.Value);
    }

    protected abstract bool ValueEquals(object otherValue);

    public override bool Equals(object? obj) => obj is Node other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Type);
        hashCode.Add(ValueGetHashCode());
        return hashCode.ToHashCode();
    }

    protected abstract int ValueGetHashCode();

    public static bool operator ==(Node left, Node right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Node left, Node right) => !(left == right);
}
