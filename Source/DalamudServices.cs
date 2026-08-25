using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace PuppetMaster;

internal class DalamudServices
{
    [PluginService] internal IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal IDataManager DataManager { get; private set; } = null!;
}
