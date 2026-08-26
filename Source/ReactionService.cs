using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System;
using System.Text.RegularExpressions;

namespace PuppetMaster;

internal class ReactionService
{
    private readonly ConfigurationProvider configurationProvider;
    private readonly IChatGui chatGui;

    public Configuration Configuration => configurationProvider.Configuration;

    public ReactionService(ConfigurationProvider configurationProvider, IChatGui chatGui)
    {
        this.configurationProvider = configurationProvider;
        this.chatGui = chatGui;
        InitializeRegex();
    }

    public Reaction NewReaction() => configurationProvider.NewReaction();

    public void SetEnabledAll(bool enabled = true)
    {
        for (var i = 0; i < Configuration.Reactions.Count; i++)
            Configuration.Reactions[i].Enabled = enabled;
        Configuration.Save();
#if DEBUG
        if (Configuration.Reactions.Count > 0)
            chatGui.Print("[PuppetMaster] " + (enabled ? "Enabled" : "Disabled") + $" {Configuration.Reactions.Count} reaction" + (Configuration.Reactions.Count > 1 ? "s" : ""));
#endif
    }

    public void SetEnabled(string name, bool enabled = true, StringComparison sc = StringComparison.Ordinal)
    {
#if DEBUG
        var found = 0;
#endif
        for (var i = 0; i < Configuration.Reactions.Count; i++)
        {
            if (Configuration.Reactions[i].Name.Equals(name, sc))
            {
                Configuration.Reactions[i].Enabled = enabled;
#if DEBUG
                found++;
#endif
            }
        }
#if DEBUG
        if (found > 0)
        {
            chatGui.Print("[PuppetMaster] " + (enabled ? "Enabled" : "Disabled") + $" {found} reaction" + (found > 1 ? "s" : "") + $" with name={name}");
        }
#endif
        Configuration.Save();
    }

    public bool IsValidReactionIndex(int index)
    {
        return 0 <= index && index < Configuration.Reactions.Count;
    }

    public string GetDefaultRegex(int index)
    {
        return IsValidReactionIndex(index) && !Configuration.Reactions[index].TriggerPhrase.IsNullOrWhitespace()
                   ? @"(?i)\b(?:" + Configuration.Reactions[index].TriggerPhrase + @")\s+(?:\((.*?)\)|(\w+))"
                   : @"";
    }

    public string GetDefaultReplaceMatch()
    {
        return @"/$1$2";
    }

    private void InitializeRegex()
    {
        for (var i = 0; i < Configuration.Reactions.Count; i++)
            InitializeRegex(i);
    }

    public void InitializeRegex(int index, bool reload = false)
    {
        if (Configuration.Reactions[index].UseRegex && (reload || Configuration.Reactions[index].CustomRx == null))
            try
            {
                Configuration.Reactions[index].CustomRx = new Regex(Configuration.Reactions[index].CustomPhrase);
            }
            catch (Exception) { }
        else if (reload || Configuration.Reactions[index].Rx == null)
            try
            {
                Configuration.Reactions[index].Rx = new Regex(GetDefaultRegex(index));
            }
            catch (Exception) { }
    }

    public TextCommand GetTestInputCommand(int index)
    {
        TextCommand result = new();

        if (!IsValidReactionIndex(index) ||
            Configuration.Reactions[index].TestInput.IsNullOrWhitespace()) return result;

        var usingRegex = (Configuration.Reactions[index].UseRegex && Configuration.Reactions[index].CustomRx != null);

        if ((usingRegex && Configuration.Reactions[index].CustomRx!.ToString().IsNullOrWhitespace()) ||
            (!usingRegex && Configuration.Reactions[index].Rx!.ToString().IsNullOrWhitespace()))
        {
            return result;
        }

        var matches = usingRegex
                          ? Configuration.Reactions[index].CustomRx!.Matches(Configuration.Reactions[index].TestInput)
                          : Configuration.Reactions[index].Rx!.Matches(Configuration.Reactions[index].TestInput);
        if (matches.Count != 0)
        {
            result.Args = matches[0].ToString();
            try
            {
                result.Main = usingRegex
                                  ? Configuration.Reactions[index].CustomRx!.Replace(
                                      matches[0].Value, Configuration.Reactions[index].ReplaceMatch)
                                  : Configuration.Reactions[index].Rx!.Replace(
                                      matches[0].Value, GetDefaultReplaceMatch());
            }
            catch (Exception) { }
        }

        result.Main = new TextCommand(result.Main).ToString();
        return result;
    }
}
