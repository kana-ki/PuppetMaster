using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class AllowedChannelsPanel(ReactionService reactions)
{
    public void Draw(Reaction reaction)
    {
        ImGui.Spacing();
        if (!ImGui.CollapsingHeader("Allowed channels", ImGuiTreeNodeFlags.DefaultOpen))
            return;

        for (var channelIndex = 16; channelIndex < 23; ++channelIndex)
            DrawChannelCheckbox(reaction, channelIndex);

        for (var channelIndex = 0; channelIndex < 8; ++channelIndex)
            DrawChannelCheckbox(reaction, channelIndex);

        for (var channelIndex = 8; channelIndex < 16; ++channelIndex)
            DrawChannelCheckbox(reaction, channelIndex);
    }

    private void DrawChannelCheckbox(Reaction reaction, int channelIndex)
    {
        if (channelIndex % 4 != 0) ImGui.SameLine();

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

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"ID:{channel.ChatType}");
            ImGui.EndTooltip();
        }
    }
}
