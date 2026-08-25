using Dalamud.Game.Text;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace PuppetMaster;

internal class ReactionService
{
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly IChatGui chatGui;

    public Configuration Configuration { get; private set; } = null!;
    public Semaphore Semaphore { get; } = new(initialCount: 1, maximumCount: 1);

    private const uint CHANNEL_COUNT = 23;

    public ReactionService(IDalamudPluginInterface pluginInterface, IChatGui chatGui)
    {
        this.pluginInterface = pluginInterface;
        this.chatGui = chatGui;
        InitializeConfig();
    }

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

    private static void MigrateConfiguration(Configuration configuration)
    {
        // Version 0 to 1 migration
        if (configuration.Version == 0)
        {
            var enabledChannels = new List<int>();
            foreach (var channel in configuration.EnabledChannels)
            {
                if (channel.Enabled)
                    enabledChannels.Add(channel.ChatType);
            }

            configuration.Reactions =
            [
                new()
                {
                    Enabled = true,
                    Name = "Reaction",
                    TriggerPhrase = configuration.TriggerPhrase,
                    AllowSit = configuration.AllowSit,
                    MotionOnly = configuration.MotionOnly,
                    AllowAllCommands = configuration.AllowAllCommands,
                    UseRegex = configuration.UseRegex,
                    CustomPhrase = configuration.CustomPhrase,
                    ReplaceMatch = configuration.ReplaceMatch,
                    TestInput = configuration.TestInput,
                    EnabledChannels = enabledChannels,
                }
            ];
            configuration.Version = 1;
        }
    }

    private void InitializeConfig()
    {
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(pluginInterface);

        if (Configuration.Version < ConfigVersion.CURRENT)
        {
            MigrateConfiguration(Configuration);
        }

        if (Configuration.EnabledChannels.Count != CHANNEL_COUNT)
        {
            Configuration.EnabledChannels =
            [
                new() { ChatType = (int)XivChatType.CrossLinkShell1, Name = "CWLS1" },
                new() { ChatType = (int)XivChatType.CrossLinkShell2, Name = "CWLS2" },
                new() { ChatType = (int)XivChatType.CrossLinkShell3, Name = "CWLS3" },
                new() { ChatType = (int)XivChatType.CrossLinkShell4, Name = "CWLS4" },
                new() { ChatType = (int)XivChatType.CrossLinkShell5, Name = "CWLS5" },
                new() { ChatType = (int)XivChatType.CrossLinkShell6, Name = "CWLS6" },
                new() { ChatType = (int)XivChatType.CrossLinkShell7, Name = "CWLS7" },
                new() { ChatType = (int)XivChatType.CrossLinkShell8, Name = "CWLS8" },
                new() { ChatType = (int)XivChatType.Ls1, Name = "LS1" },
                new() { ChatType = (int)XivChatType.Ls2, Name = "LS2" },
                new() { ChatType = (int)XivChatType.Ls3, Name = "LS3" },
                new() { ChatType = (int)XivChatType.Ls4, Name = "LS4" },
                new() { ChatType = (int)XivChatType.Ls5, Name = "LS5" },
                new() { ChatType = (int)XivChatType.Ls6, Name = "LS6" },
                new() { ChatType = (int)XivChatType.Ls7, Name = "LS7" },
                new() { ChatType = (int)XivChatType.Ls8, Name = "LS8" },
                new() { ChatType = (int)XivChatType.TellIncoming, Name = "Tell" },
                new() { ChatType = (int)XivChatType.Say, Name = "Say" },
                new() { ChatType = (int)XivChatType.Party, Name = "Party" },
                new() { ChatType = (int)XivChatType.Yell, Name = "Yell" },
                new() { ChatType = (int)XivChatType.Shout, Name = "Shout" },
                new() { ChatType = (int)XivChatType.FreeCompany, Name = "Free Company" },
                new() { ChatType = (int)XivChatType.Alliance, Name = "Alliance" }
            ];
        }

        InitializeRegex();

        if (Configuration.Reactions.Count == 0)
        {
            Configuration.Reactions.Add(new Reaction() { Name = "Reaction" });
        }

        if (Configuration.CustomChannels.Count == 0)
        {
            Configuration.CustomChannels.Add(new ChannelSetting() { Name = "SystemMessage", ChatType = 57 });
        }

        // Always set to false on load
        Configuration.DebugLogTypes = false;

        Configuration.Save();
    }
}
