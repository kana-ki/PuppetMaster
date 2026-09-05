using System;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Linq;

namespace PuppetMaster;

internal class EmoteRegistry
{
    private readonly IPluginLog logger;
    private HashSet<string> emotes = [];
    private static readonly string[] MotionlessEmotes = ["/cpose"];

    public EmoteRegistry(IDataManager dataManager, IPluginLog logger)
    {
        this.logger = logger;
        Load(dataManager);
    }

    private void Load(IDataManager dataManager)
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
    public bool IsMotionable(string command) => emotes.Contains(command) && !MotionlessEmotes.Contains(command);

    public void AddAllTo(List<string> list)
    {
        foreach (var emote in emotes.Order())
            if (!list.Contains(emote))
                list.Add(emote);
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
