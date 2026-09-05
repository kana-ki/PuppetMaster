using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace PuppetMaster.UI;

internal class ReactionEditor(ReactionService reactionService, CommandRegistry commands, WorldRegistry worlds)
{
    private static readonly Vector4 EnabledColor = new(0.24f, 0.52f, 0.24f, 1f);
    private static readonly Vector4 DisabledColor = new(0.55f, 0.24f, 0.24f, 1f);

    private readonly TriggerConfigPanel _triggerPanel = new(reactionService);
    private readonly ParsingConfigPanel _parsingPanel = new(reactionService);
    private readonly AllowedCommandsPanel _commandsPanel = new(reactionService, commands);
    private readonly AllowedPlayersPanel _playersPanel = new(reactionService, worlds);
    private readonly AllowedChannelsPanel _channelsPanel = new(reactionService);

    private Reaction? _reaction;

    public void Load(Reaction? reaction)
    {
        this._reaction = reaction;
        _triggerPanel.Reload(reaction);
    }

    public void Draw()
    {
        if (this._reaction is null)
        {
            DrawEmptyState();
            return;
        }

        DrawHeader(this._reaction);
        ImGui.Separator();

        _triggerPanel.Draw(this._reaction);
        _parsingPanel.Draw(this._reaction);
        _commandsPanel.Draw(this._reaction);
        _playersPanel.Draw(this._reaction);
        _channelsPanel.Draw(this._reaction);
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
            reactionService.Configuration.Save();
        }

        ImGui.PopStyleColor(3);

        ImGui.SameLine();
        ImGui.SetNextItemWidth(-1);
        var name = reaction.Name;
        if (ImGui.InputTextWithHint("###ReactionName", "Reaction name", ref name, 100))
        {
            reaction.Name = name;
            reactionService.Configuration.Save();
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
