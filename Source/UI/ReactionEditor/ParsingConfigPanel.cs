using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class ParsingConfigPanel(ReactionService reactions)
{
    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Parsing", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        var replaceBrackets = reaction.ReplaceBrackets;
        if (ImGui.Checkbox("Replace [ ] characters with < >", ref replaceBrackets))
        {
            reaction.ReplaceBrackets = replaceBrackets;
            reactions.Configuration.Save();
        }
        
        var motionOnly = reaction.MotionOnly;
        if (ImGui.Checkbox("Append \"motion\" to emote commands", ref motionOnly))
        {
            reaction.MotionOnly = motionOnly;
            reactions.Configuration.Save();
        }
    }
}
