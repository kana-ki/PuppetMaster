using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;

namespace PuppetMaster.UI;

internal class ReactionListPanel
{
    private readonly ReactionService _reactionService;
    private readonly Action<Reaction> _onReactionSelected;
    private readonly ReactionListFooter _footer;
    private Reaction? _selectedReaction;

    public ReactionListPanel(ReactionService reactionService, Action<Reaction?> onReactionSelected)
    {
        _reactionService = reactionService;
        _onReactionSelected = onReactionSelected;
        _footer = new (reactionService, OnSelectFromFooter);
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
        _footer.Draw();
        ImGui.EndChild();
        ImGui.PopStyleVar(2);
    }

    private void OnSelectFromFooter(Reaction? reaction)
    {
        this._selectedReaction = reaction;
        this._onReactionSelected(reaction);
    }

    private void DrawItems()
    {
        var i = 0;
        foreach (var reaction in this._reactionService.Configuration.Reactions)
        {
            var selected = reaction == _selectedReaction;
            var name = string.IsNullOrEmpty(reaction.Name) ? "(unnamed)" : reaction.Name;

            if (!reaction.Enabled) ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGuiCol.TextDisabled));
            if (ImGui.Selectable($"{name}###Reaction{++i}", selected))
            {
                this._selectedReaction = reaction;
                this._footer.Load(reaction);
                this._onReactionSelected(reaction);
            }
            if (!reaction.Enabled) ImGui.PopStyleColor();
        }
    }
}
