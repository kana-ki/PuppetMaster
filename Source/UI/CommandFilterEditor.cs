using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace PuppetMaster.UI;

internal partial class ConfigWindow
{
    private const float CommandPanelWidth = 300f;
    private static readonly string[] FilterModes = ["Allow all", "Allow only:", "Allow all except:"];
    private static readonly Vector4 LinkColor = new(0.26f, 0.59f, 0.98f, 1f);

    private void DrawCommandFilter(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Commands", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        ImGui.Indent(20);

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

        ImGui.Unindent(20);
    }

    private void DrawCommandList(List<string> list)
    {
        var leftX = ImGui.GetCursorPosX();

        if (DrawLink("Add all emotes"))
        {
            emotes.AddAllTo(list);
            reactions.Configuration.Save();
        }

        ImGui.SameLine(leftX + CommandPanelWidth - ImGui.CalcTextSize("Clear").X);
        if (DrawLink("Clear"))
        {
            list.Clear();
            reactions.Configuration.Save();
        }

        ImGui.BeginChild("##CommandListRegion", new Vector2(CommandPanelWidth, 150), true);

        var buttonWidth = ImGui.GetFrameHeight();
        var removeAt = -1;
        for (var i = 0; i < list.Count; i++)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(list[i]);
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - buttonWidth);
            if (ImGuiComponents.IconButton(i, FontAwesomeIcon.Times))
                removeAt = i;
        }

        ImGui.EndChild();

        if (removeAt >= 0)
        {
            list.RemoveAt(removeAt);
            reactions.Configuration.Save();
        }

        ImGui.SetNextItemWidth(CommandPanelWidth - buttonWidth - ImGui.GetStyle().ItemSpacing.X);
        ImGui.InputText("##CommandInput", ref commandInput, 100);
        ImGui.SameLine();
        if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
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

    private static bool DrawLink(string label)
    {
        ImGui.TextColored(LinkColor, label);
        var clicked = ImGui.IsItemClicked();

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();
            ImGui.GetWindowDrawList()
                 .AddLine(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y), ImGui.GetColorU32(LinkColor));
        }

        return clicked;
    }

    private static string NormalizeCommand(string input)
    {
        input = input.Trim();
        if (input.Length == 0) return string.Empty;
        if (!input.StartsWith('/')) input = "/" + input;
        return input.ToLower();
    }
}
