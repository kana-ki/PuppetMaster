using System.Collections.Generic;
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
    public string ReplaceMatch { get; set; } = string.Empty;
    public string TestInput { get; set; } = string.Empty;
    public List<int> EnabledChannels { get; set; } = [];
    public CommandFilterMode FilterMode { get; set; } = CommandFilterMode.AllowAll;
    public List<string> CommandWhitelist { get; set; } = [];
    public List<string> CommandBlacklist { get; set; } = [];
    public Regex? Rx;
    public Regex? CustomRx;

    //---- Deprecated, migrated to FilterMode and CommandWhitelist
    public bool AllowSit { get; set; } = false;
    public bool AllowAllCommands { get; set; } = false;
}
