using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace PuppetMaster.UI;

internal class PuppetMasterWindow : Window, IDisposable
{
    public const string Name = "PuppetMaster";

    private readonly ReactionService reactions;
    private readonly ReactionListPanel listPanel;
    private readonly ReactionEditor editor;

    public PuppetMasterWindow(ReactionService reactions, EmoteRegistry emotes, WorldRegistry worlds) : base(Name)
    {
        this.reactions = reactions;
        editor = new ReactionEditor(reactions, emotes, worlds);
        listPanel = new ReactionListPanel(reactions, Select);

        Size = new Vector2(720, 520);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void PreloadTestResult() => editor.Reload();

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
        listPanel.Draw();
        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("###ReactionEditor", new Vector2(0, 0), true);
        editor.Draw();
        ImGui.EndChild();
    }

    private void Select(int index)
    {
        reactions.Configuration.CurrentReactionEdit = index;
        reactions.Configuration.Save();

        if (reactions.IsValidReactionIndex(index))
            reactions.InitializeRegex(index);

        editor.Reload();
    }
}
