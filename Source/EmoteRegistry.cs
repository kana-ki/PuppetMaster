using Lumina.Excel.Sheets;
using System.Collections.Generic;

namespace PuppetMaster;

internal static class EmoteRegistry
{
    private static readonly HashSet<string> Emotes = [];

    public static void Initialize()
    {
        var emotes = Service.DataManager.GetExcelSheet<Emote>();
        if (emotes == null)
        {
            Service.ChatGui.PrintError($"[PuppetMaster][Error] Failed to read Emotes list");
            return;
        }

        foreach (var emoteCommand in emotes)
        {
            AddCommand(emoteCommand.TextCommand.ValueNullable?.Command.ExtractText());
            AddCommand(emoteCommand.TextCommand.ValueNullable?.ShortCommand.ExtractText());
            AddCommand(emoteCommand.TextCommand.ValueNullable?.Alias.ExtractText());
            AddCommand(emoteCommand.TextCommand.ValueNullable?.ShortAlias.ExtractText());
        }

        if (Emotes.Count == 0)
            Service.ChatGui.PrintError($"[PuppetMaster][Error] Failed to build Emotes list");
    }

    public static bool IsEmote(string command) => Emotes.Contains(command);

    private static void AddCommand(string? command)
    {
        if (!string.IsNullOrEmpty(command)) Emotes.Add(command);
    }
}
