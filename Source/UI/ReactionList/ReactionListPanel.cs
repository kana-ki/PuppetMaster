using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class ReactionListPanel
{
    private readonly ReactionService reactions;
    private readonly Action<int> onSelect;
    private readonly ReactionListFooter footer;

    public ReactionListPanel(ReactionService reactions, Action<int> onSelect)
    {
        this.reactions = reactions;
        this.onSelect = onSelect;
        footer = new ReactionListFooter(reactions, onSelect);
    }

    public void Draw()
    {
        var frameHeight = ImGui.GetFrameHeight();
        var listHeight = ImGui.GetContentRegionAvail().Y - frameHeight - ImGui.GetStyle().ItemSpacing.Y;

        ImGui.BeginChild("###ReactionItems", new Vector2(0, listHeight));
        DrawItems();
        ImGui.EndChild();

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.BeginChild("###ReactionListFooter", new Vector2(-1, frameHeight));
        footer.Draw();
        ImGui.EndChild();
        ImGui.PopStyleVar(2);
    }

    private void DrawItems()
    {
        for (var i = 0; i < reactions.Configuration.Reactions.Count; i++)
        {
            var reaction = reactions.Configuration.Reactions[i];
            var selected = reactions.Configuration.CurrentReactionEdit == i;
            var name = string.IsNullOrEmpty(reaction.Name) ? "(unnamed)" : reaction.Name;

            if (!reaction.Enabled) ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled));
            if (ImGui.Selectable($"{name}###Reaction{i}", selected))
                onSelect(i);
            if (!reaction.Enabled) ImGui.PopStyleColor();
        }
    }
}
