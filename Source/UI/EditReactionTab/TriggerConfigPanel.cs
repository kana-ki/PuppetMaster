using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class TriggerConfigPanel(ReactionService reactions)
{
    private TextCommand testCommand = new();

    public void Reload(int index) => testCommand = reactions.GetTestInputCommand(index);

    public void Draw(int index)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Trigger", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        var reaction = reactions.Configuration.Reactions[index];
        
        DrawUseRegex(reaction, index);

        var labelWidth = ImGui.GetCursorPosX() + ImGui.CalcTextSize("Replacement").X + ImGui.GetStyle().ItemSpacing.X * 2;

        ImGui.PushItemWidth(350);

        ImGui.Text("Trigger");
        ImGui.SameLine(labelWidth);
        var trigger = reaction.UseRegex ? reaction.CustomPhrase : reaction.TriggerPhrase;
        if (ImGui.InputText("##Trigger", ref trigger, reactions.Configuration.MaxRegexLength))
        {
            if (!reaction.UseRegex)
                reaction.TriggerPhrase = trigger;
            else
                reaction.CustomPhrase = trigger;

            reactions.InitializeRegex(index, true);
            testCommand = reactions.GetTestInputCommand(index);
            reactions.Configuration.Save();
        }

        if (!reaction.UseRegex && ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Separate multiple trigger phrases with |\nExample: please do|simon says");
            ImGui.EndTooltip();
        }

        if (reaction.UseRegex)
        {
            var replaceMatch = reaction.ReplaceMatch;
            ImGui.Text("Replacement");
            ImGui.SameLine(labelWidth);
            if (ImGui.InputText("##Replacement", ref replaceMatch, 500))
            {
                reaction.ReplaceMatch = replaceMatch;
                testCommand = reactions.GetTestInputCommand(index);
                reactions.Configuration.Save();
            }
        }

        ImGui.Text("Test");
        ImGui.SameLine(labelWidth);
        var testInput = reaction.TestInput;
        if (ImGui.InputText("##TestInput", ref testInput, 500))
        {
            reaction.TestInput = testInput;
            testCommand = reactions.GetTestInputCommand(index);
            reactions.Configuration.Save();
        }

        ImGui.PopItemWidth();

        if (reaction.UseRegex)
            ImGui.Text($"Matched: {testCommand.Args}");

        ImGui.Text($"Result: {testCommand.Main}");
    }

    private void DrawUseRegex(Reaction reaction, int index)
    {
        var useRegex = reaction.UseRegex;
        if (ImGui.Checkbox("Use Regex", ref useRegex))
        {
            reaction.UseRegex = useRegex;
            reactions.InitializeRegex(index);
            testCommand = reactions.GetTestInputCommand(index);
            reactions.Configuration.Save();
        }

        if (!reaction.UseRegex)
            return;

        ImGui.SameLine();
        if (ImGui.Button("Reset"))
        {
            reaction.CustomPhrase = reactions.GetDefaultRegex(index);
            reaction.ReplaceMatch = reactions.GetDefaultReplaceMatch();
            reactions.InitializeRegex(index, true);
            testCommand = reactions.GetTestInputCommand(index);
            reactions.Configuration.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Initialize regex and replacement\nbased on current non-regex trigger phrase");
            ImGui.EndTooltip();
        }
    }
}
