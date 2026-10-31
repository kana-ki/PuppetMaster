using System;

namespace PuppetMaster;

public class PlayerId : IEquatable<PlayerId>
{
    public string Name { get; init; } = string.Empty;
    public string World { get; init; } = string.Empty;

    public bool Matches(string name, string world) =>
        Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
        World.Equals(world, StringComparison.OrdinalIgnoreCase);

    public static bool operator == (PlayerId? left, PlayerId? right) =>
        (left is null && right is null) || 
        (left is not null && left.Equals(right));
    
    public static bool operator != (PlayerId? left, PlayerId? right) =>
        !(left == right);
    
    public bool Equals(PlayerId? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && World == other.World;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((PlayerId)obj);
    }

    public override int GetHashCode() =>
        HashCode.Combine(Name, World);
}
