using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dalamud.Game.Chat;
using ECommons.Automation;

namespace PuppetMaster;

internal class ChatHandler(ReactionService reactions, EmoteRegistry emotes, IChatGui chatGui)
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

        for (var index = 0; index < configuration.Reactions.Count; index++)
        {
            if (configuration.Reactions[index].Enabled)
                DoCommand(index, message.LogKind, message.Message.ToString());
        }
    }

    private async Task RunMacroAsync(string[] lines, int index)
    {
        reactions.Semaphore.WaitOne();
        var reaction = reactions.Configuration.Reactions[index];
        reactions.Semaphore.Release();

        foreach (var line in lines)
        {
            var textCommand = new TextCommand(line);
            if (!string.IsNullOrEmpty(textCommand.Main))
            {
                // Process emote
                var isEmote = emotes.IsEmote(textCommand.Main);
                if (isEmote)
                {
                    if ((textCommand.Main == "/sit" || textCommand.Main == "/groundsit" || textCommand.Main == "/lounge") && !reaction.AllowSit)
                        textCommand.Main = "/no";
                    if (reaction.MotionOnly)
                        textCommand.Args = "motion";
                }

                if (!reaction.CommandBlacklist.Contains(textCommand.Main))
                {
                    // Execute command
                    if (reaction.AllowAllCommands || isEmote || reaction.CommandWhitelist.Contains(textCommand.Main))
                    {
                        if (textCommand.Main == "/wait" && float.TryParse(textCommand.Args, out var seconds))
                            await Task.Delay((int)(Math.Clamp(seconds, 0.0, 60.0) * 1000.0));
                        else
                            Chat.SendMessage($"{textCommand}");
                    }
                }
#if DEBUG
                else
                {
                    chatGui.Print($"{textCommand.Main} in CommandBlacklist");
                    return;
                }
#endif
            }
        }
    }

    private void DoCommand(int index, XivChatType type, string message)
    {
        var configuration = reactions.Configuration;

        // Check if part of enabled channels
        if (!configuration.Reactions[index].EnabledChannels.Contains((int)type)) return;

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
