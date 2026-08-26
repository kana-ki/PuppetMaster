using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace PuppetMaster.UI;

internal class AllowedPlayersPanel(ReactionService reactions, WorldRegistry worlds)
{
    private const float PanelWidth = 320f;
    private static readonly string[] FilterModes = ["Allow everyone", "Allow only:"];

    private string nameInput = string.Empty;
    private string worldInput = string.Empty;
    private string worldFilter = string.Empty;

    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Allowed players", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        ImGui.SetNextItemWidth(250);
        var mode = (int)reaction.SenderFilterMode;
        if (ImGui.Combo("##SenderFilterMode", ref mode, FilterModes, FilterModes.Length))
        {
            reaction.SenderFilterMode = (SenderFilterMode)mode;
            reactions.Configuration.Save();
        }

        if (reaction.SenderFilterMode != SenderFilterMode.AllowOnly)
            return;

        ImGui.BeginChild("##AllowedSenders", new Vector2(PanelWidth, 120), true);

        var removeAt = -1;
        for (var i = 0; i < reaction.AllowedSenders.Count; i++)
        {
            var player = reaction.AllowedSenders[i];
            if (ImGuiExtensions.RemovableRow(i, $"{player.Name} @ {player.World}"))
                removeAt = i;
        }

        ImGui.EndChild();

        if (removeAt >= 0)
        {
            reaction.AllowedSenders.RemoveAt(removeAt);
            reactions.Configuration.Save();
        }

        ImGui.SetNextItemWidth(150);
        ImGui.InputTextWithHint("##SenderName", "Full name", ref nameInput, 60);

        ImGui.SameLine();
        DrawWorldCombo();

        ImGui.SameLine();
        if (ImGuiComponents.IconButton("##AddPlayer", FontAwesomeIcon.Plus))
            AddSender(reaction);
    }

    private void DrawWorldCombo()
    {
        ImGui.SetNextItemWidth(130);
        if (!ImGui.BeginCombo("##SenderWorld", string.IsNullOrEmpty(worldInput) ? "World" : worldInput))
            return;

        ImGui.SetNextItemWidth(-1);
        ImGui.InputTextWithHint("##WorldFilter", "Filter...", ref worldFilter, 40);

        foreach (var world in worlds.Worlds)
        {
            if (worldFilter.Length > 0 && !world.Contains(worldFilter, StringComparison.OrdinalIgnoreCase))
                continue;

            if (ImGui.Selectable(world, world == worldInput))
                worldInput = world;
        }

        ImGui.EndCombo();
    }

    private void AddSender(Reaction reaction)
    {
        var name = nameInput.Trim();
        if (name.Length == 0 || worldInput.Length == 0) return;

        if (!reaction.AllowedSenders.Exists(p => p.Matches(name, worldInput)))
            reaction.AllowedSenders.Add(new PlayerId { Name = name, World = worldInput });

        reactions.Configuration.Save();
        nameInput = string.Empty;
        worldInput = string.Empty;
        worldFilter = string.Empty;
    }
}
