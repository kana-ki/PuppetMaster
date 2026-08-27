using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PuppetMaster;

public class Reaction
{
    public bool Enabled { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public string TriggerPhrase { get; set; } = string.Empty;
    public bool MotionOnly { get; set; } = true;
    public bool UseRegex { get; set; } = false;
    public string CustomPhrase { get; set; } = string.Empty;
    public string TestInput { get; set; } = string.Empty;
    public List<int> EnabledChannels { get; set; } = [];
    public bool ReplaceBrackets { get; set; } = true;
    public CommandFilterMode FilterMode { get; set; } = CommandFilterMode.AllowAll;
    public List<string> CommandWhitelist { get; set; } = [];
    public List<string> CommandBlacklist { get; set; } = [];
    public SenderFilterMode SenderFilterMode { get; set; } = SenderFilterMode.AllowEveryone;
    public List<PlayerId> AllowedSenders { get; set; } = [];
    public Regex? Rx;
    public Regex? CustomRx;

    //---- Deprecated, migrated to FilterMode and CommandWhitelist
    public bool AllowSit { get; set; } = false;
    public bool AllowAllCommands { get; set; } = false;

    public Reaction Clone() => new()
    {
        Enabled = Enabled,
        Name = Name,
        TriggerPhrase = TriggerPhrase,
        MotionOnly = MotionOnly,
        UseRegex = UseRegex,
        CustomPhrase = CustomPhrase,
        TestInput = TestInput,
        FilterMode = FilterMode,
        ReplaceBrackets = ReplaceBrackets,
        SenderFilterMode = SenderFilterMode,
        EnabledChannels = [.. EnabledChannels],
        CommandWhitelist = [.. CommandWhitelist],
        CommandBlacklist = [.. CommandBlacklist],
        AllowedSenders = [.. AllowedSenders.Select(p => new PlayerId { Name = p.Name, World = p.World })],
    };
}
