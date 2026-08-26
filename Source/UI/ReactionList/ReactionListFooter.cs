using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace PuppetMaster.UI;

internal class ReactionListFooter(ReactionService reactions, Action<int> onSelect)
{
    private const int ButtonCount = 3;

    public void Draw()
    {
        var available = ImGui.GetContentRegionAvail();
        var buttonSize = new Vector2(MathF.Floor(available.X / ButtonCount), available.Y);
        var hasSelection = reactions.IsValidReactionIndex(reactions.Configuration.CurrentReactionEdit);
        var canDelete = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

        if (DrawIconButton(FontAwesomeIcon.Plus, buttonSize, "New reaction"))
            AddReaction();

        ImGui.SameLine();
        if (DrawIconButton(FontAwesomeIcon.Clone, buttonSize, "Duplicate reaction", disabled: !hasSelection))
            DuplicateReaction();

        ImGui.SameLine();
        if (DrawIconButton(FontAwesomeIcon.Trash, buttonSize,
                           canDelete ? "Delete reaction" : "Hold Shift + Ctrl to delete",
                           disabled: !canDelete || !hasSelection))
            DeleteReaction();

        ImGui.PopStyleVar();
    }

    private void AddReaction()
    {
        reactions.Configuration.Reactions.Add(reactions.NewReaction());
        reactions.Configuration.Save();
        onSelect(reactions.Configuration.Reactions.Count - 1);
    }

    private void DuplicateReaction()
    {
        var clone = reactions.Configuration.Reactions[reactions.Configuration.CurrentReactionEdit].Clone();
        reactions.Configuration.Reactions.Add(clone);
        reactions.Configuration.Save();
        onSelect(reactions.Configuration.Reactions.Count - 1);
    }

    private void DeleteReaction()
    {
        var index = reactions.Configuration.CurrentReactionEdit;
        reactions.Configuration.Reactions.RemoveAt(index);
        reactions.Configuration.Save();
        onSelect(Math.Min(index, reactions.Configuration.Reactions.Count - 1));
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
