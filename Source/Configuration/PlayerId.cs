using System;

namespace PuppetMaster;

public class PlayerId
{
    public string Name { get; set; } = string.Empty;
    public string World { get; set; } = string.Empty;

    public bool Matches(string name, string world) =>
        Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
        World.Equals(world, StringComparison.OrdinalIgnoreCase);
}
