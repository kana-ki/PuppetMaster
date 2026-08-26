using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace PuppetMaster.UI;

internal partial class ConfigWindow(ReactionService reactions, EmoteRegistry emotes) : Window(Name), IDisposable
{
    public const string Name = "Puppet Master settings";

    private TextCommand textCommand = new();
    private int currentReactionIndex = reactions.Configuration.CurrentReactionEdit;
    private string commandInput = string.Empty;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void PreloadTestResult()
    {
        textCommand = reactions.GetTestInputCommand(reactions.Configuration.CurrentReactionEdit);
    }

    public override void Draw()
    {
        ImGui.SetNextWindowSize(new Vector2(480, 640), ImGuiCond.FirstUseEver);

        ImGui.BeginTabBar("PuppetMaster Config Tabs");

        if (ImGui.BeginTabItem("Reactions"))
        {
            DrawReactionsTab();
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Edit Reactions"))
        {
            DrawEditReactionTab();
            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();
    }
}
