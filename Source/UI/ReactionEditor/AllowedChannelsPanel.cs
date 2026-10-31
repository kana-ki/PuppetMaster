using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class AllowedChannelsPanel(ReactionService reactions)
{
    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        var expanded = ImGui.CollapsingHeader("What channels should you listen to commands in", ImGuiTreeNodeFlags.DefaultOpen);
        if (!expanded)
            return;
        
        if (!ImGui.BeginTable("##AllowedChannels", 4, ImGuiTableFlags.SizingStretchSame))
            return;

        for (var channelIndex = 16; channelIndex < 23; ++channelIndex)
            DrawChannelCheckbox(reaction, channelIndex);

        for (var channelIndex = 0; channelIndex < 8; ++channelIndex)
            DrawChannelCheckbox(reaction, channelIndex);

        for (var channelIndex = 8; channelIndex < 16; ++channelIndex)
            DrawChannelCheckbox(reaction, channelIndex);
        
        ImGui.EndTable();
    }

    private void DrawChannelCheckbox(Reaction reaction, int channelIndex)
    {
        if (channelIndex % 4 == 0) ImGui.TableNextRow();
        ImGui.TableNextColumn();

        var channel = reactions.Configuration.EnabledChannels[channelIndex];
        var enabled = reaction.EnabledChannels.Contains(channel.ChatType);

        if (ImGui.Checkbox($"{channel.Name}##DefaultChannelCheckBox{channelIndex}{channel.ChatType}", ref enabled))
        {
            if (enabled)
                reaction.EnabledChannels.Add(channel.ChatType);
            else
                reaction.EnabledChannels.Remove(channel.ChatType);

            reactions.Configuration.Save();
        }
    }
}
