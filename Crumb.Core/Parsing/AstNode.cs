using System.Text;

namespace Crumb.Core.Parsing;
public class AstNode : IEquatable<AstNode>
{
    private const string INDENT = "    ";

    public List<AstNode> Children { get; private set; } = new();
    public OpCodes OpCode { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public int LineNumber { get; private set; }

    public AstNode(OpCodes opCode, int lineNumber, string? value = null)
    {
        OpCode = opCode;
        LineNumber = lineNumber;

        if (!string.IsNullOrEmpty(value))
        {
            Value = value;
        }
    }

    public void Append(AstNode child) => Children.Add(child);

    // TODO: implement GetHashCode?

    public string ToString(int depth)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < depth; i++)
        {
            sb.Append(INDENT);
        }

        sb.Append($"{LineNumber} | {OpCode}{(string.IsNullOrEmpty(Value) ? string.Empty : $" '{Value}'")}");

        var strings = new List<string> { sb.ToString() };

        strings.AddRange(Children.Select(c => c.ToString(depth + 1)));

        return string.Join(Environment.NewLine, strings);
    }

    public override string ToString() => ToString(0);

    public bool Equals(AstNode? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return OpCode == other.OpCode &&
            Value == other.Value &&
            Enumerable.SequenceEqual(Children, other.Children);
    }

    public override bool Equals(object? obj) => obj is AstNode other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(OpCode.GetHashCode());
        hashCode.Add(Value.GetHashCode());
        foreach(var child in Children)
        {
            hashCode.Add(child.GetHashCode());
        }
        return hashCode.ToHashCode();
    }

    public static bool operator ==(AstNode left, AstNode right) => left.Equals(right);

    public static bool operator !=(AstNode left, AstNode right) => !(left == right);
}
