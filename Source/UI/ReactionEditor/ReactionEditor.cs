using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class ReactionEditor
{
    private static readonly Vector4 EnabledColor = new(0.24f, 0.52f, 0.24f, 1f);
    private static readonly Vector4 DisabledColor = new(0.55f, 0.24f, 0.24f, 1f);

    private readonly ReactionService reactions;
    private readonly TriggerConfigPanel triggerPanel;
    private readonly AllowedCommandsPanel commandsPanel;
    private readonly AllowedPlayersPanel playersPanel;
    private readonly EnabledChannelsPanel channelsPanel;

    public ReactionEditor(ReactionService reactions, EmoteRegistry emotes, WorldRegistry worlds)
    {
        this.reactions = reactions;
        triggerPanel = new TriggerConfigPanel(reactions);
        commandsPanel = new AllowedCommandsPanel(reactions, emotes);
        playersPanel = new AllowedPlayersPanel(reactions, worlds);
        channelsPanel = new EnabledChannelsPanel(reactions);
    }

    public void Reload() => triggerPanel.Reload(reactions.Configuration.CurrentReactionEdit);

    public void Draw()
    {
        var index = reactions.Configuration.CurrentReactionEdit;
        if (!reactions.IsValidReactionIndex(index))
        {
            DrawEmptyState();
            return;
        }

        var reaction = reactions.Configuration.Reactions[index];

        DrawHeader(reaction);
        ImGui.Separator();

        triggerPanel.Draw(index);
        commandsPanel.Draw(reaction);
        playersPanel.Draw(reaction);
        channelsPanel.Draw(reaction);
    }

    private void DrawHeader(Reaction reaction)
    {
        var color = reaction.Enabled ? EnabledColor : DisabledColor;
        ImGui.PushStyleColor(ImGuiCol.Button, color);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
        if (ImGui.Button($"{(reaction.Enabled ? "Enabled" : "Disabled")}###EnableToggle", new Vector2(90, 0)))
        {
            reaction.Enabled = !reaction.Enabled;
            reactions.Configuration.Save();
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);
        var name = reaction.Name;
        if (ImGui.InputTextWithHint("###ReactionName", "Reaction name", ref name, 100))
        {
            reaction.Name = name;
            reactions.Configuration.Save();
        }
    }

    private static void DrawEmptyState()
    {
        const string placeholder = "Select or create a reaction";
        var size = ImGui.CalcTextSize(placeholder);
        var region = ImGui.GetContentRegionAvail();

        ImGui.SetCursorPos(new Vector2((region.X - size.X) / 2, (region.Y - size.Y) / 2));
        ImGui.TextDisabled(placeholder);
    }
}
