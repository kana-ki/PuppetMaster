using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;

namespace PuppetMaster;

internal class ConfigWindow : Window, IDisposable
{
    public const string Name = "Puppet Master settings";

    private readonly ReactionService reactions;

    private TextCommand TextCommand = new();
    private int CurrentReactionIndex;

    public ConfigWindow(ReactionService reactions) : base(Name)
    {
        this.reactions = reactions;
        CurrentReactionIndex = reactions.Configuration.CurrentReactionEdit;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void PreloadTestResult()
    {
        TextCommand = reactions.GetTestInputCommand(reactions.Configuration.CurrentReactionEdit);
    }

    private void DrawReaction(int index)
    {
        var enabled = reactions.Configuration.Reactions[index].Enabled;
        if (ImGui.Checkbox($"##{reactions.Configuration.Reactions[index].Name}##ReactionCheckBox{index}", ref enabled))
        {
            reactions.Semaphore.WaitOne();
            reactions.Configuration.Reactions[index].Enabled = enabled;
            reactions.Configuration.Save();
            reactions.Semaphore.Release();
        }

        ImGui.SameLine();
        ImGui.Spacing();
        ImGui.SameLine();

        ImGui.PushItemWidth(150);
        var reactionName = reactions.Configuration.Reactions[index].Name;
        if (ImGui.InputText($"##CustomChannelLabel##{index}", ref reactionName, 100))
        {
            reactions.Semaphore.WaitOne();
            reactions.Configuration.Reactions[index].Name = reactionName;
            reactions.Configuration.Save();
            reactions.Semaphore.Release();
        }

        ImGui.PopItemWidth();

        /*
        // Can't figure out how to set focus on a tab
        if (ImGui.Button($"Edit##ReactionEdit##{index}"))
        {
            reactions.Configuration.CurrentReactionEdit = index;
            reactions.Configuration.Save();
        }
        */

        ImGui.SameLine();
        if (ImGui.Button($"Delete##ReactionDelete##{index}"))
        {
            reactions.Semaphore.WaitOne();
            reactions.Configuration.Reactions.RemoveAt(index);
            reactions.Configuration.Save();
            reactions.Semaphore.Release();
        }
    }

    private void DrawChannelCheckbox(int reactionIndex, int channelIndex)
    {
        if (channelIndex % 4 != 0) ImGui.SameLine();

        var chatType = reactions.Configuration.EnabledChannels[channelIndex].ChatType;
        var enabled = reactions.Configuration.Reactions[reactionIndex].EnabledChannels.Contains(chatType);

        if (ImGui.Checkbox(
                $"{reactions.Configuration.EnabledChannels[channelIndex].Name}##DefaultChannelCheckBox{channelIndex}{chatType}",
                ref enabled))
        {
            reactions.Semaphore.WaitOne();
            if (enabled)
            {
                reactions.Configuration.Reactions[reactionIndex].EnabledChannels.Add(chatType);
            }
            else
            {
                reactions.Configuration.Reactions[reactionIndex].EnabledChannels.Remove(chatType);
            }

            reactions.Configuration.Save();
            reactions.Semaphore.Release();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"ID:{reactions.Configuration.EnabledChannels[channelIndex].ChatType}");
            ImGui.EndTooltip();
        }
    }

    private void DrawCustomChannelCheckbox(int reactionIndex, int channelIndex)
    {
        if (channelIndex % 4 != 0) ImGui.SameLine();

        var chatType = reactions.Configuration.CustomChannels[channelIndex].ChatType;
        var enabled = reactions.Configuration.Reactions[reactionIndex].EnabledChannels.Contains(chatType);

        if (ImGui.Checkbox(
                $"{reactions.Configuration.CustomChannels[channelIndex].Name}##CustomChannelCheckBox{channelIndex}{chatType}",
                ref enabled))
        {
            reactions.Semaphore.WaitOne();
            if (enabled)
            {
                reactions.Configuration.Reactions[reactionIndex].EnabledChannels.Add(chatType);
            }
            else
            {
                reactions.Configuration.Reactions[reactionIndex].EnabledChannels.Remove(chatType);
            }

            reactions.Configuration.Save();
            reactions.Semaphore.Release();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"ID:{reactions.Configuration.CustomChannels[channelIndex].ChatType}");
            ImGui.EndTooltip();
        }
    }

    private void DrawCustomChannels(int index)
    {
        ImGui.PushItemWidth(100);
        var channelID = (int)reactions.Configuration.CustomChannels[index].ChatType;
        if (ImGui.InputInt($"##CustomChannelID##{index}", ref channelID))
        {
            reactions.Configuration.CustomChannels[index].ChatType = channelID;
            reactions.Configuration.Save();
        }

        ImGui.PopItemWidth();

        ImGui.SameLine();
        ImGui.Spacing();
        ImGui.SameLine();

        ImGui.PushItemWidth(150);
        var channelName = reactions.Configuration.CustomChannels[index].Name;
        if (ImGui.InputText($"##CustomChannelLabel##{index}", ref channelName, 100))
        {
            reactions.Configuration.CustomChannels[index].Name = channelName;
            reactions.Configuration.Save();
        }

        ImGui.PopItemWidth();

        ImGui.SameLine();
        ImGui.Spacing();
        ImGui.SameLine();

        if (ImGui.Button($"Delete##CustomChannelDelete#{index}"))
        {
            reactions.Semaphore.WaitOne();
            for (var i = 0; i < reactions.Configuration.Reactions.Count; i++)
            {
                reactions.Configuration.Reactions[i].EnabledChannels.Remove(channelID);
            }

            reactions.Configuration.CustomChannels.RemoveAt(index);
            reactions.Configuration.Save();
            reactions.Semaphore.Release();
        }
    }

    public override void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(480, 640), ImGuiCond.FirstUseEver);

        ImGui.BeginTabBar("PuppetMaster Config Tabs");

        if (ImGui.BeginTabItem("Reactions"))
        {
            if (ImGui.Button($"Add##ReactionAddButton"))
            {
                reactions.Semaphore.WaitOne();
                reactions.Configuration.Reactions.Add(new Reaction() { Name = "Reaction" });
                reactions.Configuration.Save();
                reactions.Semaphore.Release();
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            for (var index = 0; index < reactions.Configuration.Reactions.Count; index++)
            {
                DrawReaction(index);
            }

            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Edit Reactions"))
        {
            var reactionNames = new List<string> { };
            foreach (var reaction in reactions.Configuration.Reactions)
                reactionNames.Add(reaction.Name);

            ImGui.SetNextItemWidth(450);
            if (ImGui.Combo("##ReactEditSelector", ref CurrentReactionIndex, [.. reactionNames], reactionNames.Count))
            {
                reactions.Configuration.CurrentReactionEdit = CurrentReactionIndex;
                reactions.Configuration.Save();
                reactions.InitializeRegex(CurrentReactionIndex);
                TextCommand = reactions.GetTestInputCommand(CurrentReactionIndex);
            }

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Separator();

            if (reactions.IsValidReactionIndex(reactions.Configuration.CurrentReactionEdit))
            {
                ImGui.PushItemWidth(350);
                ImGui.Indent(40);
                ImGui.Text("Trigger");
                ImGui.SameLine();

                var trigger = reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex
                                  ? reactions.Configuration.Reactions[CurrentReactionIndex].CustomPhrase
                                  : reactions.Configuration.Reactions[CurrentReactionIndex].TriggerPhrase;
                if (ImGui.InputText("##Trigger", ref trigger, reactions.Configuration.MaxRegexLength))
                {
                    reactions.Semaphore.WaitOne();
                    if (!reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex)
                        reactions.Configuration.Reactions[CurrentReactionIndex].TriggerPhrase = trigger;
                    else
                        reactions.Configuration.Reactions[CurrentReactionIndex].CustomPhrase = trigger;

                    reactions.InitializeRegex(CurrentReactionIndex, true);
                    TextCommand = reactions.GetTestInputCommand(CurrentReactionIndex);
                    reactions.Configuration.Save();
                    reactions.Semaphore.Release();
                }

                if (!reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex)
                {
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(
                            "Separate multiple trigger phrases with |\nExample: please do|simon says");
                        ImGui.EndTooltip();
                    }
                }

                ImGui.Unindent(35);

                var replaceMatch = reactions.Configuration.Reactions[CurrentReactionIndex].ReplaceMatch;
                if (reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex)
                {
                    ImGui.Text("Replacement");
                    ImGui.SameLine();
                    if (ImGui.InputTextMultiline("##Replacement", ref replaceMatch, 500, new Vector2(350, 80)))
                    {
                        reactions.Semaphore.WaitOne();
                        reactions.Configuration.Reactions[CurrentReactionIndex].ReplaceMatch = replaceMatch;
                        reactions.Configuration.Save();
                        TextCommand = reactions.GetTestInputCommand(CurrentReactionIndex);
                        reactions.Semaphore.Release();
                    }
                }

                ImGui.Indent(50);
                ImGui.Text("Test");
                ImGui.SameLine();

                var testInput = reactions.Configuration.Reactions[CurrentReactionIndex].TestInput;
                if (ImGui.InputText("##TestInput", ref testInput, 500))
                {
                    reactions.Semaphore.WaitOne();
                    reactions.Configuration.Reactions[CurrentReactionIndex].TestInput = testInput;
                    reactions.Configuration.Save();
                    TextCommand = reactions.GetTestInputCommand(CurrentReactionIndex);
                    reactions.Semaphore.Release();
                }

                ImGui.Unindent(45);

                if (reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex)
                {
                    ImGui.Text($"Matched: {TextCommand.Args}");
                }

                ImGui.Text($"Result: {TextCommand.Main}");

                ImGui.PopItemWidth();
                ImGui.Spacing();
                ImGui.Spacing();

                ImGui.Separator(); //----------------------------------------------

                var useRegex = reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex;
                if (ImGui.Checkbox("Use Regex", ref useRegex))
                {
                    reactions.Semaphore.WaitOne();
                    reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex = useRegex;
                    reactions.Configuration.Save();
                    reactions.InitializeRegex(CurrentReactionIndex);
                    TextCommand = reactions.GetTestInputCommand(CurrentReactionIndex);
                    reactions.Semaphore.Release();
                }

                if (reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex)
                {
                    ImGui.SameLine();
                    if (ImGui.Button("Reset"))
                    {
                        reactions.Semaphore.WaitOne();
                        reactions.Configuration.Reactions[CurrentReactionIndex].CustomPhrase =
                            replaceMatch = reactions.GetDefaultRegex(CurrentReactionIndex);
                        reactions.Configuration.Reactions[CurrentReactionIndex].ReplaceMatch =
                            trigger = reactions.GetDefaultReplaceMatch();
                        reactions.InitializeRegex(CurrentReactionIndex, true);
                        TextCommand = reactions.GetTestInputCommand(CurrentReactionIndex);
                        reactions.Configuration.Save();
                        reactions.Semaphore.Release();
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.TextUnformatted(
                            "Initialize regex and replacement\nbased on current non-regex trigger phrase");
                        ImGui.EndTooltip();
                    }
                }

                var allowAllCommands = reactions.Configuration.Reactions[CurrentReactionIndex].AllowAllCommands;
                if (ImGui.Checkbox("Allow all text commands", ref allowAllCommands))
                {
                    reactions.Semaphore.WaitOne();
                    reactions.Configuration.Reactions[CurrentReactionIndex].AllowAllCommands = allowAllCommands;
                    reactions.Configuration.Save();
                    TextCommand = reactions.GetTestInputCommand(CurrentReactionIndex);
                    reactions.Semaphore.Release();
                }

                if (!reactions.Configuration.Reactions[CurrentReactionIndex].UseRegex)
                {
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("If command has subcommands, enclose sequence in parentheses.");
                        ImGui.Text("For placeholders, replace angle brackets with square brackets.");
                        var found = reactions.Configuration.Reactions[CurrentReactionIndex].TriggerPhrase.IndexOf('|');
                        var firstTriggerPhrase = found == -1
                                                     ? reactions.Configuration.Reactions[CurrentReactionIndex]
                                                                .TriggerPhrase
                                                     : reactions.Configuration.Reactions[CurrentReactionIndex]
                                                                .TriggerPhrase[..found];
                        ImGui.Text("Example: " + firstTriggerPhrase + " (ac \"Vercure\" [t])");
                        ImGui.EndTooltip();
                    }
                }

                var allowSit = reactions.Configuration.Reactions[CurrentReactionIndex].AllowSit;
                if (ImGui.Checkbox("Allow \"sit\" or \"groundsit\" requests", ref allowSit))
                {
                    reactions.Configuration.Reactions[CurrentReactionIndex].AllowSit = allowSit;
                    reactions.Configuration.Save();
                }

                var motionOnly = reactions.Configuration.Reactions[CurrentReactionIndex].MotionOnly;
                if (ImGui.Checkbox("Motion only", ref motionOnly))
                {
                    reactions.Configuration.Reactions[CurrentReactionIndex].MotionOnly = motionOnly;
                    reactions.Configuration.Save();
                }

                ImGui.Spacing();
                ImGui.Text("Enabled Channels");
                ImGui.Indent(20);

                if (reactions.Configuration.CustomChannels.Count > 0)
                {
                    ImGui.Separator(); //----------------------------------------------
                    for (var channelIndex = 0;
                         channelIndex < reactions.Configuration.CustomChannels.Count;
                         ++channelIndex)
                        DrawCustomChannelCheckbox(CurrentReactionIndex, channelIndex);
                }

                ImGui.Separator(); //----------------------------------------------

                for (var channelIndex = 16; channelIndex < 23; ++channelIndex)
                {
                    DrawChannelCheckbox(CurrentReactionIndex, channelIndex);
                }

                ImGui.Separator(); //----------------------------------------------

                for (var channelIndex = 0; channelIndex < 8; ++channelIndex)
                {
                    DrawChannelCheckbox(CurrentReactionIndex, channelIndex);
                }

                ImGui.Separator(); //----------------------------------------------

                for (var channelIndex = 8; channelIndex < 16; ++channelIndex)
                {
                    DrawChannelCheckbox(CurrentReactionIndex, channelIndex);
                }
            }

            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Custom Channels"))
        {
            ImGui.SetNextItemWidth(400);

            var debugLogTypes = reactions.Configuration.DebugLogTypes;
            if (ImGui.Checkbox("Debug log types", ref debugLogTypes))
            {
                reactions.Configuration.DebugLogTypes = debugLogTypes;
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Enabling this print all game messages in the log windows.");
                ImGui.Text(
                    "Logs will be prefixed with log type ID (and optionally the type name and sender, if they exist)");
                ImGui.EndTooltip();
            }

            ImGui.SameLine();

            if (ImGui.Button("Add##CustomChannelAdd"))
            {
                reactions.Configuration.CustomChannels.Add(new ChannelSetting()
                                                               { ChatType = (int)0, Name = "Custom", Enabled = false });
                reactions.Configuration.Save();
            }

            ImGui.Spacing();
            ImGui.Spacing();

            if (reactions.Configuration.CustomChannels.Count > 0)
            {
                for (var index = 0; index < reactions.Configuration.CustomChannels.Count; ++index)
                {
                    DrawCustomChannels(index);
                }
            }

            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();
    }
}
