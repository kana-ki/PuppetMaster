using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class SelectReactionTab(ReactionService reactions)
{
    public void Draw()
    {
        if (ImGui.Button($"Add##ReactionAddButton"))
        {
            reactions.Configuration.Reactions.Add(reactions.NewReaction());
            reactions.Configuration.Save();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        for (var index = 0; index < reactions.Configuration.Reactions.Count; index++)
        {
            DrawReaction(index);
        }
    }

    private void DrawReaction(int index)
    {
        var reaction = reactions.Configuration.Reactions[index];

        var enabled = reaction.Enabled;
        if (ImGui.Checkbox($"##{reaction.Name}##ReactionCheckBox{index}", ref enabled))
        {
            reaction.Enabled = enabled;
            reactions.Configuration.Save();
        }

        ImGui.SameLine();
        ImGui.Spacing();
        ImGui.SameLine();

        ImGui.PushItemWidth(150);
        var reactionName = reaction.Name;
        if (ImGui.InputText($"##ReactionLabel##{index}", ref reactionName, 100))
        {
            reaction.Name = reactionName;
            reactions.Configuration.Save();
        }

        ImGui.PopItemWidth();

        ImGui.SameLine();
        if (ImGui.Button($"Delete##ReactionDelete##{index}"))
        {
            reactions.Configuration.Reactions.RemoveAt(index);
            reactions.Configuration.Save();
        }
    }
}
