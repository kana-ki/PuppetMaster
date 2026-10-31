using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class ParsingConfigPanel(ReactionService reactions)
{
    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Behaviour options", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        var replaceBrackets = reaction.ReplaceBrackets;
        if (ImGui.Checkbox("Replace [ ] characters with < >", ref replaceBrackets))
        {
            reaction.ReplaceBrackets = replaceBrackets;
            reactions.Configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"Enable this if you want [] characters to act as they did in the original PuppetMaster. Default off. ");
            ImGui.EndTooltip();
        }
        
        var motionOnly = reaction.MotionOnly;
        if (ImGui.Checkbox("Display log message for emote commands", ref motionOnly))
        {
            reaction.MotionOnly = motionOnly;
            reactions.Configuration.Save();
        }
    }
}
