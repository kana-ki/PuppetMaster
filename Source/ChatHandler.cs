using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dalamud.Game.Chat;
using ECommons.Automation;
using ECommons.GameHelpers;

namespace PuppetMaster;

internal class ChatHandler(ReactionService reactions, EmoteRegistry emotes, IChatGui chatGui, IObjectTable objectTable)
{
    private static readonly Regex NewLine = new("\r\n|\r|\n");

    public void OnChatMessage(IHandleableChatMessage message)
    {
        var configuration = reactions.Configuration;

        if (configuration.DebugLogTypes && message.LogKind != XivChatType.Debug)
        {
            var prefix = int.TryParse(message.LogKind.ToString(), out var number)
                             ? "[" + number + "]"
                             : "[" + ((int)message.LogKind) + "][" + message.LogKind + "]";
            prefix += (message.Sender.ToString().IsNullOrEmpty() ? "" : "<" + message.Sender + "> ");
            chatGui.Print(prefix + " " + message.Message);
        }

        if (message.IsHandled) return;

        var sender = ResolveSender(message.Sender);

        for (var index = 0; index < configuration.Reactions.Count; index++)
        {
            if (configuration.Reactions[index].Enabled)
                DoCommand(index, message.LogKind, message.Message.ToString(), sender);
        }
    }

    private PlayerId ResolveSender(SeString sender)
    {
        var payload = sender.Payloads.OfType<PlayerPayload>().FirstOrDefault();
        if (payload is not null)
        {
            var world = payload.World.RowId != 0 ? payload.World.ValueNullable?.Name.ExtractText() : null;
            return new PlayerId
            {
                Name = payload.PlayerName ?? string.Empty,
                World = string.IsNullOrEmpty(world) ? Player.CurrentWorldName : world,
            };
        }

        // No player payload means it's their own message apparently
        return new PlayerId { Name = Player.Name, World = Player.HomeWorldName };
    }

    private static bool IsSenderAllowed(Reaction reaction, PlayerId sender) => reaction.SenderFilterMode switch
    {
        SenderFilterMode.AllowEveryone => true,
        SenderFilterMode.AllowOnly => reaction.AllowedSenders.Any(p => p.Matches(sender.Name, sender.World)),
        _ => false,
    };

    private async Task RunMacroAsync(string[] lines, int index)
    {
        var reaction = reactions.Configuration.Reactions[index];

        foreach (var line in lines)
        {
            var textCommand = new TextCommand(line);
            if (string.IsNullOrEmpty(textCommand.Main)) continue;

            if (reaction.MotionOnly && emotes.IsEmote(textCommand.Main))
                textCommand.Args = "motion";

            if (!IsCommandAllowed(reaction, textCommand.Main)) continue;

            if (textCommand.Main == "/wait" && float.TryParse(textCommand.Args, out var seconds))
                await Task.Delay((int)(Math.Clamp(seconds, 0.0, 60.0) * 1000.0));
            else
                Chat.SendMessage($"{textCommand}");
        }
    }

    private static bool IsCommandAllowed(Reaction reaction, string command) => reaction.FilterMode switch
    {
        CommandFilterMode.AllowAll => true,
        CommandFilterMode.AllowOnly => reaction.CommandWhitelist.Contains(command),
        CommandFilterMode.AllowAllExcept => !reaction.CommandBlacklist.Contains(command),
        _ => false,
    };

    private void DoCommand(int index, XivChatType type, string message, PlayerId sender)
    {
        var configuration = reactions.Configuration;

        // Check if part of enabled channels
        if (!configuration.Reactions[index].EnabledChannels.Contains((int)type)) return;

        // Check if sender is allowed to command
        if (!IsSenderAllowed(configuration.Reactions[index], sender)) return;

        var usingRegex = (configuration.Reactions[index].UseRegex && configuration.Reactions[index].CustomRx != null);

        // Guard against whitespace regex
        if ((usingRegex && configuration.Reactions[index].CustomRx!.ToString().IsNullOrWhitespace()) ||
            (!usingRegex && configuration.Reactions[index].Rx!.ToString().IsNullOrWhitespace()))
        {
#if DEBUG
            chatGui.PrintError($"[PuppetMaster][ERR] Empty RegEx [{message}]");
#endif
            return;
        }

        // Find command in message
        var matches = usingRegex ?
                          configuration.Reactions[index].CustomRx!.Matches(message) :
                          configuration.Reactions[index].Rx!.Matches(message);
        if (matches.Count == 0) return;
        var command = string.Empty;
        try
        {
            command = usingRegex ?
                          configuration.Reactions[index].CustomRx!.Replace(matches[0].Value, configuration.Reactions[index].ReplaceMatch) :
                          configuration.Reactions[index].Rx!.Replace(matches[0].Value, reactions.GetDefaultReplaceMatch());
        } catch (Exception) { }


        var lines = NewLine.Split(command.ToString());
        _ = RunMacroAsync(lines, index);
    }
}
