using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class TriggerConfigPanel(ReactionService reactions)
{
    private TextCommand testCommand = new();

    public void Reload(Reaction? reaction) =>
        testCommand = reaction is not null ? reactions.GetTestInputCommand(reaction) : new();

    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Trigger", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        DrawUseRegex(reaction);

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

            reactions.InitializeRegex(reaction, true);
            testCommand = reactions.GetTestInputCommand(reaction);
            reactions.Configuration.Save();
        }

        if (!reaction.UseRegex && ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Separate multiple trigger phrases with |\nExample: please do|simon says");
            ImGui.EndTooltip();
        }

        ImGui.Text("Test");
        ImGui.SameLine(labelWidth);
        var testInput = reaction.TestInput;
        if (ImGui.InputText("##TestInput", ref testInput, 500))
        {
            reaction.TestInput = testInput;
            testCommand = reactions.GetTestInputCommand(reaction);
            reactions.Configuration.Save();
        }

        ImGui.PopItemWidth();
        ImGui.Text($"Result: {testCommand}");
    }

    private void DrawUseRegex(Reaction reaction)
    {
        var useRegex = reaction.UseRegex;
        if (ImGui.Checkbox("Use Regex", ref useRegex))
        {
            reaction.UseRegex = useRegex;
            reactions.InitializeRegex(reaction);
            testCommand = reactions.GetTestInputCommand(reaction);
            reactions.Configuration.Save();
        }

        if (!reaction.UseRegex)
            return;

        ImGui.SameLine();
        if (ImGui.Button("Reset"))
        {
            reaction.CustomPhrase = reactions.GetDefaultRegex(reaction);
            reactions.InitializeRegex(reaction, true);
            testCommand = reactions.GetTestInputCommand(reaction);
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
