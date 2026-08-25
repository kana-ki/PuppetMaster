using Dalamud.Game.Text;
using Dalamud.Plugin;

namespace PuppetMaster;

internal class ConfigurationProvider
{
    private const uint CHANNEL_COUNT = 23;

    public Configuration Configuration { get; }

    public ConfigurationProvider(IDalamudPluginInterface pluginInterface)
    {
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(pluginInterface);
        SeedDefaults();
        Configuration.Save();
    }

    public void Save() => Configuration.Save();

    private void SeedDefaults()
    {
        if (Configuration.EnabledChannels.Count != CHANNEL_COUNT)
        {
            Configuration.EnabledChannels =
            [
                new() { ChatType = (int)XivChatType.CrossLinkShell1, Name = "CWLS1" },
                new() { ChatType = (int)XivChatType.CrossLinkShell2, Name = "CWLS2" },
                new() { ChatType = (int)XivChatType.CrossLinkShell3, Name = "CWLS3" },
                new() { ChatType = (int)XivChatType.CrossLinkShell4, Name = "CWLS4" },
                new() { ChatType = (int)XivChatType.CrossLinkShell5, Name = "CWLS5" },
                new() { ChatType = (int)XivChatType.CrossLinkShell6, Name = "CWLS6" },
                new() { ChatType = (int)XivChatType.CrossLinkShell7, Name = "CWLS7" },
                new() { ChatType = (int)XivChatType.CrossLinkShell8, Name = "CWLS8" },
                new() { ChatType = (int)XivChatType.Ls1, Name = "LS1" },
                new() { ChatType = (int)XivChatType.Ls2, Name = "LS2" },
                new() { ChatType = (int)XivChatType.Ls3, Name = "LS3" },
                new() { ChatType = (int)XivChatType.Ls4, Name = "LS4" },
                new() { ChatType = (int)XivChatType.Ls5, Name = "LS5" },
                new() { ChatType = (int)XivChatType.Ls6, Name = "LS6" },
                new() { ChatType = (int)XivChatType.Ls7, Name = "LS7" },
                new() { ChatType = (int)XivChatType.Ls8, Name = "LS8" },
                new() { ChatType = (int)XivChatType.TellIncoming, Name = "Tell" },
                new() { ChatType = (int)XivChatType.Say, Name = "Say" },
                new() { ChatType = (int)XivChatType.Party, Name = "Party" },
                new() { ChatType = (int)XivChatType.Yell, Name = "Yell" },
                new() { ChatType = (int)XivChatType.Shout, Name = "Shout" },
                new() { ChatType = (int)XivChatType.FreeCompany, Name = "Free Company" },
                new() { ChatType = (int)XivChatType.Alliance, Name = "Alliance" }
            ];
        }

        if (Configuration.Reactions.Count == 0)
            Configuration.Reactions.Add(new Reaction() { Name = "Reaction" });

        if (Configuration.CustomChannels.Count == 0)
            Configuration.CustomChannels.Add(new ChannelSetting() { Name = "SystemMessage", ChatType = 57 });

        Configuration.DebugLogTypes = false;
    }
}
