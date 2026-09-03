using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace PuppetMaster.UI;

internal class ReactionListFooter(ReactionService reactionService, Action<Reaction?> onSelect)
{
    private const int ButtonCount = 5;
    private Reaction? _selectedReaction;

    public void Draw()
    {
        var available = ImGui.GetContentRegionAvail();
        var buttonSize = new Vector2(MathF.Floor(available.X / ButtonCount), available.Y);
        var hasSelection = _selectedReaction is not null;
        var canDelete = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

        if (DrawIconButton(FontAwesomeIcon.Plus, buttonSize, "New reaction"))
            AddReaction();

        ImGui.SameLine();
        if (DrawIconButton(FontAwesomeIcon.Clone, buttonSize, "Duplicate reaction", disabled: !hasSelection))
            DuplicateReaction();
        
        ImGui.SameLine();
        if (DrawIconButton(FontAwesomeIcon.FileExport, buttonSize, "Export reaction to clipboard"))
            ExportToClipboard();
        
        ImGui.SameLine();
        if (DrawIconButton(FontAwesomeIcon.FileImport, buttonSize, "Import reaction from clipboard"))
            ImportFromClipboard();
        
        ImGui.SameLine();
        if (DrawIconButton(FontAwesomeIcon.Trash, buttonSize,
                           canDelete ? "Delete reaction" : "Hold Shift + Ctrl to delete",
                           disabled: !canDelete || !hasSelection))
            DeleteReaction();

        ImGui.PopStyleVar();
    }


    public void Load(Reaction reaction)
    {
        _selectedReaction = reaction;
    }

    private void AddReaction()
    {
        var newReaction = reactionService.NewReaction();
        reactionService.Configuration.Reactions.Add(reactionService.NewReaction());
        reactionService.Configuration.Save();
        onSelect(newReaction);
    }
    
    private void ExportToClipboard()
    {
        reactionService.ExportReactionToClipboard(_selectedReaction);
    }

    private void ImportFromClipboard()
    {
        var reaction = reactionService.ImportFromClipboard();
        if (reaction is null)
            return;
        reactionService.Configuration.Reactions.Add(reaction);
        reactionService.Configuration.Save();
    }

    private void DuplicateReaction()
    {
        if (this._selectedReaction is null)
            return; 
        
        var clone = this._selectedReaction.Clone();
        reactionService.Configuration.Reactions.Add(clone);
        reactionService.Configuration.Save();
        onSelect(clone);
    }

    private void DeleteReaction()
    {
        if (this._selectedReaction is null)
            return; 

        reactionService.Configuration.Reactions.Remove(this._selectedReaction);
        reactionService.Configuration.Save();
        onSelect(null);
    }

    private static bool DrawIconButton(FontAwesomeIcon icon, Vector2 size, string tooltip, bool disabled = false)
    {
        var framePadding = ImGui.GetStyle().FramePadding;
        var padX = Math.Max(0, (size.X - ImGui.GetFrameHeight()) / 2f + framePadding.X);

        if (disabled) ImGui.BeginDisabled();
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(padX, framePadding.Y));
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0f);
        var clicked = ImGuiComponents.IconButton((int)icon, icon);
        ImGui.PopStyleVar(2);
        if (disabled) ImGui.EndDisabled();

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(tooltip);
        return clicked;
    }
}
