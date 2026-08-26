using System.Collections.Generic;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class EditReactionTab
{
    private readonly ReactionService reactions;
    private readonly TriggerConfigPanel triggerPanel;
    private readonly AllowedCommandsPanel commandsPanel;
    private readonly EnabledChannelsPanel channelsPanel;

    public EditReactionTab(ReactionService reactions, EmoteRegistry emotes)
    {
        this.reactions = reactions;
        triggerPanel = new TriggerConfigPanel(reactions);
        commandsPanel = new AllowedCommandsPanel(reactions, emotes);
        channelsPanel = new EnabledChannelsPanel(reactions);
    }

    public void PreloadTestResult() => triggerPanel.Reload(reactions.Configuration.CurrentReactionEdit);

    public void Draw()
    {
        DrawReactionSelector();

        var index = reactions.Configuration.CurrentReactionEdit;
        if (!reactions.IsValidReactionIndex(index))
            return;

        var reaction = reactions.Configuration.Reactions[index];

        triggerPanel.Draw(index);
        commandsPanel.Draw(reaction);
        channelsPanel.Draw(reaction);
    }

    private void DrawReactionSelector()
    {
        var reactionNames = new List<string>();
        foreach (var reaction in reactions.Configuration.Reactions)
            reactionNames.Add(reaction.Name);

        ImGui.SetNextItemWidth(450);
        if (ImGui.Combo("##ReactEditSelector", ref reactions.Configuration.CurrentReactionEdit,
                        [.. reactionNames], reactionNames.Count))
        {
            reactions.Configuration.Save();
            reactions.InitializeRegex(reactions.Configuration.CurrentReactionEdit);
            triggerPanel.Reload(reactions.Configuration.CurrentReactionEdit);
        }

        ImGui.Spacing();
        ImGui.Spacing();
    }
}
