using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Components;

namespace PuppetMaster.UI;

internal static class ImGuiExtensions
{
    private const float IconScale = 0.7f;
    private static readonly Vector4 LinkColor = new(0.26f, 0.59f, 0.98f, 1f);

    public static bool RemovableRow(int id, string text)
    {
        var buttonSize = ImGui.GetFontSize() * IconScale + ImGui.GetStyle().FramePadding.Y * 2;
        var textHeight = ImGui.GetTextLineHeight();
        var rowHeight = buttonSize > textHeight ? buttonSize : textHeight;
        var rowY = ImGui.GetCursorPosY();

        ImGui.SetCursorPosY(rowY + (rowHeight - textHeight) * 0.5f);
        ImGui.TextUnformatted(text);

        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - buttonSize);
        ImGui.SetCursorPosY(rowY + (rowHeight - buttonSize) * 0.5f);

        ImGui.SetWindowFontScale(IconScale);
        var clicked = ImGuiComponents.IconButton(id, FontAwesomeIcon.Times, new Vector2(20, 20));
        ImGui.SetWindowFontScale(1f);

        return clicked;
    }

    public static bool Link(string label)
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
}
