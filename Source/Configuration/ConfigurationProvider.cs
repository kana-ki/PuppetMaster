using Dalamud.Game.Text;
using Dalamud.Plugin;

namespace PuppetMaster;

internal class ConfigurationProvider
{
    private const uint CHANNEL_COUNT = 23;

    private readonly EmoteRegistry emotes;

    public Configuration Configuration { get; }

    public ConfigurationProvider(IDalamudPluginInterface pluginInterface, EmoteRegistry emotes)
    {
        this.emotes = emotes;
        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(pluginInterface);
        Migrate();
        SeedDefaults();
        Configuration.Save();
    }

    public void Save() => Configuration.Save();

    public Reaction NewReaction(string name = "Reaction")
    {
        var reaction = new Reaction { Name = name, FilterMode = CommandFilterMode.AllowOnly };
        emotes.AddAllTo(reaction.CommandWhitelist);
        return reaction;
    }

    private void Migrate()
    {
        if (Configuration.Version < 2)
        {
            foreach (var reaction in Configuration.Reactions)
                MigrateCommandFilter(reaction);
            Configuration.Version = 2;
        }
    }

    private void MigrateCommandFilter(Reaction reaction)
    {
        if (reaction.AllowAllCommands)
        {
            reaction.FilterMode = CommandFilterMode.AllowAll;
            return;
        }

        reaction.FilterMode = CommandFilterMode.AllowOnly;
        emotes.AddAllTo(reaction.CommandWhitelist);

        if (reaction.AllowSit)
        {
            if (!reaction.CommandWhitelist.Contains("/sit")) reaction.CommandWhitelist.Add("/sit");
            if (!reaction.CommandWhitelist.Contains("/groundsit")) reaction.CommandWhitelist.Add("/groundsit");
        }
        
        reaction.SenderFilterMode = SenderFilterMode.AllowOnly;
    }

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
            Configuration.Reactions.Add(NewReaction());

        Configuration.DebugLogTypes = false;
    }
}
