using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace PuppetMaster.UI;

internal class CommandHistoryWindow : Window, IDisposable
{
    public const string Name = "PuppetMaster History";

    private readonly CommandManager _commandManager;

    public CommandHistoryWindow(CommandManager commandManager) : base(Name)
    {
        _commandManager = commandManager;
        Size = new Vector2(640, 400);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public override void PreDraw()
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(420, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };
    }

    public override void Draw()
    {
        var history = _commandManager.History;
        if (history.Count == 0)
        {
            ImGui.TextDisabled("No commands have been parsed yet.");
            return;
        }

        const ImGuiTableFlags flags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg |
                                      ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable;
        if (ImGui.BeginTable("###CommandHistory", 3, flags))
        {
            ImGui.TableSetupColumn("When", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("Sender", ImGuiTableColumnFlags.WidthFixed, 180);
            ImGui.TableSetupColumn("Command", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            // Show the most recent commands first.
            for (var i = history.Count - 1; i >= 0; i--)
            {
                var entry = history[i];
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.TextUnformatted(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                ImGui.TableNextColumn();
                ImGui.TextUnformatted($"{entry.Sender.Name}@{entry.Sender.World}");

                ImGui.TableNextColumn();
                ImGui.TextUnformatted(entry.Command.ToString());
            }

            ImGui.EndTable();
        }
    }

    public void Dispose() =>
        GC.SuppressFinalize(this);
}
