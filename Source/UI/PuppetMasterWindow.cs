using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace PuppetMaster.UI;

internal class PuppetMasterWindow : Window, IDisposable
{
    public const string Name = "PuppetMaster";

    private readonly SelectReactionTab selectTab;
    private readonly EditReactionTab editTab;

    public PuppetMasterWindow(ReactionService reactions, EmoteRegistry emotes) : base(Name)
    {
        selectTab = new SelectReactionTab(reactions);
        editTab = new EditReactionTab(reactions, emotes);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void PreloadTestResult() => editTab.PreloadTestResult();

    public override void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(480, 640), ImGuiCond.FirstUseEver);

        ImGui.BeginTabBar("PuppetMaster Config Tabs");

        if (ImGui.BeginTabItem("Reactions"))
        {
            selectTab.Draw();
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Edit Reactions"))
        {
            editTab.Draw();
            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();
    }
}
