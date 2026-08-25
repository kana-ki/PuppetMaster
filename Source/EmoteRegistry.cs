using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

using System.Collections.Generic;

namespace PuppetMaster;

internal class EmoteRegistry
{
    private readonly IPluginLog logger;
    private readonly HashSet<string> emotes = [];

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
            logger.Debug($"Registering emote {emote}");
            AddCommand(emote.TextCommand.ValueNullable?.Command.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.ShortCommand.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.Alias.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.ShortAlias.ExtractText());
        }
        if (emotes.Count == 0)
            this.logger.Error($"Failed to build Emotes list");
    }

    public bool IsEmote(string command) => emotes.Contains(command);

    private void AddCommand(string? command)
    {
        if (!string.IsNullOrEmpty(command)) emotes.Add(command);
    }
}
