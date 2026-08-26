using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal static class ImGuiExtensions
{
    private static readonly Vector4 LinkColor = new(0.26f, 0.59f, 0.98f, 1f);

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
