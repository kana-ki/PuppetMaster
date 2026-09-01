using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using ECommons.Automation;

namespace PuppetMaster;

public record CommandHistoryEntry(DateTime Timestamp, PlayerId Sender, TextCommand Command);

internal class CommandManager(IPluginLog logger)
{
    private readonly List<CommandHistoryEntry> _history = [];

    public IReadOnlyList<CommandHistoryEntry> History => _history;

    public void Execute(PlayerId sender, TextCommand command)
    {
        logger.Information($"Executing command '{command}' from {sender.Name}@{sender.World}");
        _history.Add(new CommandHistoryEntry(DateTime.Now, sender, command));
        Chat.SendMessage($"{command}");
    }
}
