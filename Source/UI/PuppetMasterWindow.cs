using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace PuppetMaster.UI;

internal class PuppetMasterWindow : Window, IDisposable
{
    public const string Name = "PuppetMaster";

    private readonly ReactionService _reactionService;
    private readonly ReactionListPanel _listPanel;
    private readonly ReactionEditor _editorPanel;

    public PuppetMasterWindow(ReactionService reactionService, EmoteRegistry emotes, WorldRegistry worlds) : base(Name)
    {
        this._reactionService = reactionService;
        this._editorPanel = new ReactionEditor(reactionService, emotes, worlds);
        this._listPanel = new ReactionListPanel(reactionService, OnReactionSelected);

        Size = new Vector2(720, 520);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void PreDraw()
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(560, 320),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    public override void Draw()
    {
        var scale = ImGui.GetIO().FontGlobalScale;

        ImGui.BeginChild("###ReactionList", new Vector2(200 * scale, 0), true);
        _listPanel.Draw();
        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("###ReactionEditor", new Vector2(0, 0), true);
        _editorPanel.Draw();
        ImGui.EndChild();
    }

    private void OnReactionSelected(Reaction? reaction)
    {
        if (reaction is not null)
            _reactionService.InitializeRegex(reaction);
        _editorPanel.Load(reaction);
    }

    public void Dispose() =>
        GC.SuppressFinalize(this);
}
