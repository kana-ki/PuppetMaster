using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace PuppetMaster.UI;

internal class AllowedCommandsPanel(ReactionService reactions, EmoteRegistry emotes)
{
    private const float PanelWidth = 300f;
    private static readonly string[] FilterModes = ["Allow all", "Allow only:", "Allow all except:"];

    private string commandInput = string.Empty;

    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Allowed commands", ImGuiTreeNodeFlags.DefaultOpen))
            return;
        
        var motionOnly = reaction.MotionOnly;
        if (ImGui.Checkbox("Motion only", ref motionOnly))
        {
            reaction.MotionOnly = motionOnly;
            reactions.Configuration.Save();
        }

        ImGui.SetNextItemWidth(200);
        var mode = (int)reaction.FilterMode;
        if (ImGui.Combo("##CommandFilterMode", ref mode, FilterModes, FilterModes.Length))
        {
            reaction.FilterMode = (CommandFilterMode)mode;
            reactions.Configuration.Save();
        }

        if (reaction.FilterMode == CommandFilterMode.AllowOnly)
            DrawCommandList(reaction.CommandWhitelist);
        else if (reaction.FilterMode == CommandFilterMode.AllowAllExcept)
            DrawCommandList(reaction.CommandBlacklist);
    }

    private void DrawCommandList(List<string> list)
    {
        var leftX = ImGui.GetCursorPosX();

        if (ImGuiExtensions.Link("Add all emotes"))
        {
            emotes.AddAllTo(list);
            reactions.Configuration.Save();
        }

        ImGui.SameLine(leftX + PanelWidth - ImGui.CalcTextSize("Clear").X);
        if (ImGuiExtensions.Link("Clear"))
        {
            list.Clear();
            reactions.Configuration.Save();
        }

        ImGui.BeginChild("##CommandListRegion", new Vector2(PanelWidth, 150), true);

        var removeAt = -1;
        for (var i = 0; i < list.Count; i++)
        {
            if (ImGuiExtensions.RemovableRow(i, list[i]))
                removeAt = i;
        }

        ImGui.EndChild();

        if (removeAt >= 0)
        {
            list.RemoveAt(removeAt);
            reactions.Configuration.Save();
        }

        ImGui.SetNextItemWidth(PanelWidth - ImGui.GetFrameHeight() - ImGui.GetStyle().ItemSpacing.X);
        ImGui.InputText("##CommandInput", ref commandInput, 100);
        ImGui.SameLine();
        if (ImGuiComponents.IconButton("##AddCommand", FontAwesomeIcon.Plus))
            AddCommand(list, commandInput);
    }

    private void AddCommand(List<string> list, string input)
    {
        var command = NormalizeCommand(input);
        if (command.Length == 0) return;

        if (!list.Contains(command)) list.Add(command);
        reactions.Configuration.Save();

        commandInput = string.Empty;
    }

    private static string NormalizeCommand(string input)
    {
        input = input.Trim();
        if (input.Length == 0) return string.Empty;
        if (!input.StartsWith('/')) input = "/" + input;
        return input.ToLower();
    }
}
