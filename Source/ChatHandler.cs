using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System.Linq;
using Dalamud.Game.Chat;
using ECommons.Automation;
using ECommons.GameHelpers;

namespace PuppetMaster;

internal class ChatHandler(ReactionService reactionService, EmoteRegistry emotes, CommandParser parser)
{
    public void OnChatMessage(IHandleableChatMessage message)
    {
        var configuration = reactionService.Configuration;
        if (message.IsHandled) return;

        var sender = ResolveSender(message.Sender);
        foreach (var reaction in configuration.Reactions)
        {
            var command = CheckAndPrepareCommand(reaction, message.LogKind, message.Message.ToString(), sender);
            if (command is not null)
            {
                Chat.SendMessage($"{command}");
                break;
            }
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

    private static bool IsCommandAllowed(Reaction reaction, string command) => reaction.FilterMode switch
    {
        CommandFilterMode.AllowAll => true,
        CommandFilterMode.AllowOnly => reaction.CommandWhitelist.Contains(command),
        CommandFilterMode.AllowAllExcept => !reaction.CommandBlacklist.Contains(command),
        _ => false,
    };

    private TextCommand? CheckAndPrepareCommand(Reaction reaction, XivChatType type, string message, PlayerId sender)
    {
        if (!reaction.Enabled) 
            return null;
        
        if (!reaction.EnabledChannels.Contains((int)type))
            return null;

        if (!IsSenderAllowed(reaction, sender)) 
            return null;

        var textCommand = parser.Parse(reaction, message);
        if (textCommand == null)
            return null;

        if (!IsCommandAllowed(reaction, textCommand.Main))
            return null;
        
        if (reaction.MotionOnly && emotes.IsMotionable(textCommand.Main))
            textCommand.Args = "motion";
        
        return textCommand;
    }

   
}
