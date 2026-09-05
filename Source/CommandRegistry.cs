using System;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace PuppetMaster;

internal class CommandRegistry
{
    private readonly IPluginLog logger;
    private HashSet<string> emotes = [];
    private static readonly string[] MotionlessEmotes = ["/cpose"];
    private static readonly string[] DestructiveCommands = [
        "/say",
        "/s",
        "/yell",
        "/y",
        "/shout",
        "/sh",
        "/tell",
        "/t",
        "/reply",
        "/r",
        "/party",
        "/p",
        "/alliance",
        "/a",
        "/freecompany",
        "/fc",
        "/pvpteam",
        "/pt",
        "/cwlinkshell",
        "/cwl",
        "/cwlinkshell1",
        "/cwl1",
        "/cwlinkshell2",
        "/cwl2",
        "/cwlinkshell3",
        "/cwl3",
        "/cwlinkshell4",
        "/cwl4",
        "/cwlinkshell5",
        "/cwl5",
        "/cwlinkshell6",
        "/cwl6",
        "/cwlinkshell7",
        "/cwl7",
        "/cwlinkshell8",
        "/cwl8",
        "/linkshell",
        "/l",
        "/linkshell1",
        "/l1",
        "/linkshell2",
        "/l2",
        "/linkshell3",
        "/l3",
        "/linkshell4",
        "/l4",
        "/linkshell5",
        "/l5",
        "/linkshell6",
        "/l6",
        "/linkshell7",
        "/l7",
        "/linkshell8",
        "/l8",
        "/novice",
        "/n",
        "/emote",
        "/em",
        "/friendlist",
        "/flist",
        "/blacklist",
        "/blist",
        "/hotbar",
        "/pvphotbar",
        "/crosshotbar",
        "/pvpcrosshotbar",
        "/hud ",
        "/hudreset",
        "/uireset",
        "/uiscale",
        "/searchcomment",
        "/graphicpresets",
        "/gpresets",
        "/puppet",
        "/puppetmaster"
    ];

    public CommandRegistry(IDataManager dataManager, IPluginLog logger)
    {
        this.logger = logger;
        LoadEmotes(dataManager);
    }

    private void LoadEmotes(IDataManager dataManager)
    {
        var sheet = dataManager.GetExcelSheet<Emote>();
        foreach (var emote in sheet)
        {
            AddCommand(emote.TextCommand.ValueNullable?.Command.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.ShortCommand.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.Alias.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.ShortAlias.ExtractText());
        }
        if (emotes.Count == 0)
            this.logger.Error($"Failed to build Emotes list");
    }

    public bool IsEmote(string command) => emotes.Contains(command);
    public bool IsDestructiveCommand(string command) => DestructiveCommands.Contains(command);
    public bool IsMotionable(string command) => emotes.Contains(command) && !MotionlessEmotes.Contains(command);

    public void AddAllEmotesTo(List<string> list)
    {
        foreach (var emote in emotes.Order())
            if (!list.Contains(emote))
                list.Add(emote);
    }

    public void AddAllDestructiveCommandsTo(List<string> list)
    {
        foreach (var command in DestructiveCommands.Order())
            if (!list.Contains(command))
                list.Add(command);
    }

    private void AddCommand(string? command)
    {
        if (!string.IsNullOrEmpty(command))
        {
            logger.Debug($"Registering emote {command}");
            emotes.Add(command);
        }
    }
}
