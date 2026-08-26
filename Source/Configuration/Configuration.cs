using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace PuppetMaster;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = ConfigVersion.CURRENT;

    public List<ChannelSetting> EnabledChannels { get; set; } = [];
    public List<Reaction> Reactions { get; set; } = [];
    public int CurrentReactionEdit = -1;
    public bool DebugLogTypes { get; set; } = false;
    public int MaxRegexLength { get; set; } = 1000;

    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        this.pluginInterface!.SavePluginConfig(this);
    }
}
