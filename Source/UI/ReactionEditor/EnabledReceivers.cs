using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace PuppetMaster.UI;

internal class EnabledReceivers(ReactionService reactions, WorldRegistry worlds)
{
    private const float PanelWidth = 320f;
    private static readonly string[] FilterModes = ["Allow all of my characters", "Allow only characters:"];

    private string nameInput = string.Empty;
    private string worldInput = string.Empty;
    private string worldFilter = string.Empty;

    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        var expanded = ImGui.CollapsingHeader("Which of your own characters should listen to commands", ImGuiTreeNodeFlags.DefaultOpen);
        if (!expanded)
            return;

        ImGui.SetNextItemWidth(250);
        var mode = (int)reaction.ReceiverFilterMode;
        if (ImGui.Combo("##ReceiverFilterMode", ref mode, FilterModes, FilterModes.Length))
        {
            reaction.ReceiverFilterMode = (ReceiverFilterMode)mode;
            reactions.Configuration.Save();
        }

        if (reaction.ReceiverFilterMode == ReceiverFilterMode.EnableAllCharacters)
            return;

        ImGui.BeginChild("##EnabledReceivers", new Vector2(PanelWidth, 120), true);

        var removeAt = -1;
        for (var i = 0; i < reaction.EnabledReceivers.Count; i++)
        {
            var player = reaction.EnabledReceivers[i];
            if (ImGuiExtensions.RemovableRow(i, $"{player.Name} @ {player.World}"))
                removeAt = i;
        }

        ImGui.EndChild();

        if (removeAt >= 0)
        {
            reaction.EnabledReceivers.RemoveAt(removeAt);
            reactions.Configuration.Save();
        }

        ImGui.SetNextItemWidth(150);
        ImGui.InputTextWithHint("##ReceiverName", "Full name", ref nameInput, 60);

        ImGui.SameLine();
        DrawWorldCombo();

        ImGui.SameLine();
        if (ImGuiComponents.IconButton("##AddReceiver", FontAwesomeIcon.Plus))
            AddSender(reaction);
    }

    private void DrawWorldCombo()
    {
        ImGui.SetNextItemWidth(130);
        if (!ImGui.BeginCombo("##ReceiverWorld", string.IsNullOrEmpty(worldInput) ? "World" : worldInput))
            return;

        ImGui.SetNextItemWidth(-1);
        ImGui.InputTextWithHint("##ReceiverWorldFilter", "Filter...", ref worldFilter, 40);

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

        if (!reaction.EnabledReceivers.Exists(p => p.Matches(name, worldInput)))
            reaction.EnabledReceivers.Add(new PlayerId { Name = name, World = worldInput });

        reactions.Configuration.Save();
        nameInput = string.Empty;
        worldInput = string.Empty;
        worldFilter = string.Empty;
    }
}
