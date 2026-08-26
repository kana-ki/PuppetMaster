using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal partial class ConfigWindow
{
    private void DrawEditReactionTab()
    {
        DrawReactionSelector();

        if (!reactions.IsValidReactionIndex(reactions.Configuration.CurrentReactionEdit))
            return;

        var reaction = reactions.Configuration.Reactions[currentReactionIndex];

        DrawTriggerEditor(reaction);
        DrawToggles(reaction);
        DrawCommandFilter(reaction);
        DrawChannels(reaction);
    }

    private void DrawReactionSelector()
    {
        var reactionNames = new List<string>();
        foreach (var reaction in reactions.Configuration.Reactions)
            reactionNames.Add(reaction.Name);

        ImGui.SetNextItemWidth(450);
        if (ImGui.Combo("##ReactEditSelector", ref currentReactionIndex, [.. reactionNames], reactionNames.Count))
        {
            reactions.Configuration.CurrentReactionEdit = currentReactionIndex;
            reactions.Configuration.Save();
            reactions.InitializeRegex(currentReactionIndex);
            textCommand = reactions.GetTestInputCommand(currentReactionIndex);
        }

        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Separator();
    }

    private void DrawTriggerEditor(Reaction reaction)
    {
        ImGui.PushItemWidth(350);
        ImGui.Indent(40);
        ImGui.Text("Trigger");
        ImGui.SameLine();

        var trigger = reaction.UseRegex ? reaction.CustomPhrase : reaction.TriggerPhrase;
        if (ImGui.InputText("##Trigger", ref trigger, reactions.Configuration.MaxRegexLength))
        {
            if (!reaction.UseRegex)
                reaction.TriggerPhrase = trigger;
            else
                reaction.CustomPhrase = trigger;

            reactions.InitializeRegex(currentReactionIndex, true);
            textCommand = reactions.GetTestInputCommand(currentReactionIndex);
            reactions.Configuration.Save();
        }

        if (!reaction.UseRegex && ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Separate multiple trigger phrases with |\nExample: please do|simon says");
            ImGui.EndTooltip();
        }

        ImGui.Unindent(35);

        if (reaction.UseRegex)
        {
            var replaceMatch = reaction.ReplaceMatch;
            ImGui.Text("Replacement");
            ImGui.SameLine();
            if (ImGui.InputTextMultiline("##Replacement", ref replaceMatch, 500, new Vector2(350, 80)))
            {
                reaction.ReplaceMatch = replaceMatch;
                textCommand = reactions.GetTestInputCommand(currentReactionIndex);
                reactions.Configuration.Save();
            }
        }

        ImGui.Indent(50);
        ImGui.Text("Test");
        ImGui.SameLine();

        var testInput = reaction.TestInput;
        if (ImGui.InputText("##TestInput", ref testInput, 500))
        {
            reaction.TestInput = testInput;
            textCommand = reactions.GetTestInputCommand(currentReactionIndex);
            reactions.Configuration.Save();
        }

        ImGui.Unindent(45);

        if (reaction.UseRegex)
            ImGui.Text($"Matched: {textCommand.Args}");

        ImGui.Text($"Result: {textCommand.Main}");

        ImGui.PopItemWidth();
        ImGui.Unindent(10);
        ImGui.Spacing();
        ImGui.Spacing();
        ImGui.Separator();
    }

    private void DrawToggles(Reaction reaction)
    {
        var useRegex = reaction.UseRegex;
        if (ImGui.Checkbox("Use Regex", ref useRegex))
        {
            reaction.UseRegex = useRegex;
            reactions.InitializeRegex(currentReactionIndex);
            textCommand = reactions.GetTestInputCommand(currentReactionIndex);
            reactions.Configuration.Save();
        }

        if (reaction.UseRegex)
        {
            ImGui.SameLine();
            if (ImGui.Button("Reset"))
            {
                reaction.CustomPhrase = reactions.GetDefaultRegex(currentReactionIndex);
                reaction.ReplaceMatch = reactions.GetDefaultReplaceMatch();
                reactions.InitializeRegex(currentReactionIndex, true);
                textCommand = reactions.GetTestInputCommand(currentReactionIndex);
                reactions.Configuration.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextUnformatted("Initialize regex and replacement\nbased on current non-regex trigger phrase");
                ImGui.EndTooltip();
            }
        }

        var motionOnly = reaction.MotionOnly;
        if (ImGui.Checkbox("Motion only", ref motionOnly))
        {
            reaction.MotionOnly = motionOnly;
            reactions.Configuration.Save();
        }
    }

    private void DrawChannels(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Enabled Channels", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        ImGui.Indent(20);

        ImGui.Separator();
        for (var channelIndex = 16; channelIndex < 23; ++channelIndex)
            DrawChannelCheckbox(currentReactionIndex, channelIndex);

        ImGui.Separator();
        for (var channelIndex = 0; channelIndex < 8; ++channelIndex)
            DrawChannelCheckbox(currentReactionIndex, channelIndex);

        ImGui.Separator();
        for (var channelIndex = 8; channelIndex < 16; ++channelIndex)
            DrawChannelCheckbox(currentReactionIndex, channelIndex);

        ImGui.Unindent(20);
    }

    private void DrawChannelCheckbox(int reactionIndex, int channelIndex)
    {
        if (channelIndex % 4 != 0) ImGui.SameLine();

        var channel = reactions.Configuration.EnabledChannels[channelIndex];
        var reaction = reactions.Configuration.Reactions[reactionIndex];
        var enabled = reaction.EnabledChannels.Contains(channel.ChatType);

        if (ImGui.Checkbox($"{channel.Name}##DefaultChannelCheckBox{channelIndex}{channel.ChatType}", ref enabled))
        {
            if (enabled)
                reaction.EnabledChannels.Add(channel.ChatType);
            else
                reaction.EnabledChannels.Remove(channel.ChatType);

            reactions.Configuration.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"ID:{channel.ChatType}");
            ImGui.EndTooltip();
        }
    }
}
