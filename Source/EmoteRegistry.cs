using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

using System.Collections.Generic;

namespace PuppetMaster;

internal class EmoteRegistry
{
    private readonly IChatGui chatGui;
    private readonly HashSet<string> emotes = [];

    public EmoteRegistry(IDataManager dataManager, IChatGui chatGui)
    {
        this.chatGui = chatGui;
        Load(dataManager);
    }

    private void Load(IDataManager dataManager)
    {
        var sheet = dataManager.GetExcelSheet<Emote>();
        if (sheet == null)
        {
            chatGui.PrintError($"[PuppetMaster][Error] Failed to read Emotes list");
            return;
        }

        foreach (var emote in sheet)
        {
            AddCommand(emote.TextCommand.ValueNullable?.Command.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.ShortCommand.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.Alias.ExtractText());
            AddCommand(emote.TextCommand.ValueNullable?.ShortAlias.ExtractText());
        }

        if (emotes.Count == 0)
            chatGui.PrintError($"[PuppetMaster][Error] Failed to build Emotes list");
    }

    public bool IsEmote(string command) => emotes.Contains(command);

    private void AddCommand(string? command)
    {
        if (!string.IsNullOrEmpty(command)) emotes.Add(command);
    }
}
