using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;
using ECommons;
using Microsoft.Extensions.DependencyInjection;

namespace PuppetMaster;

public class Plugin : IDalamudPlugin
{
    public static string Name => "PuppetMaster";
    private const string CommandName = "/puppetmaster";
    private const string CommandName2 = "/puppet";
    private const string CommandHelp = @"Open settings dialog
/{0} on|off - enable or disable all reactions
/{0 on|off <ReactionName> - enable or disable reactions by name";

    private readonly ServiceProvider provider;
    private readonly WindowSystem windowSystem = new("PuppetMaster");

    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ICommandManager commandManager;
    private readonly IChatGui chatGui;

    private readonly ReactionService reactions;
    private readonly ChatHandler chatHandler;
    private readonly UI.PuppetMasterWindow configWindow;

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        provider = new ServiceCollection()
           // Dalamud services
           .AddSingleton(this.pluginInterface = pluginInterface)
           .AddSingleton(this.commandManager = pluginInterface.GetRequiredService<ICommandManager>())
           .AddSingleton(this.chatGui = pluginInterface.GetRequiredService<IChatGui>())
           .AddSingleton(pluginInterface.GetRequiredService<IDataManager>())
           .AddSingleton(pluginInterface.GetRequiredService<IPluginLog>())
           .AddSingleton(pluginInterface.GetRequiredService<IObjectTable>())
           // App services
           .AddSingleton<ConfigurationProvider>()
           .AddSingleton<ReactionService>()
           .AddSingleton<EmoteRegistry>()
           .AddSingleton<WorldRegistry>()
           .AddSingleton<ChatHandler>()
           .AddSingleton<CommandParser>()
           .AddSingleton<UI.PuppetMasterWindow>()
           .BuildServiceProvider();

        reactions = provider.GetRequiredService<ReactionService>();
        chatHandler = provider.GetRequiredService<ChatHandler>();
        configWindow = provider.GetRequiredService<UI.PuppetMasterWindow>();

        windowSystem.AddWindow(configWindow);

        commandManager.AddHandler(CommandName, new CommandInfo(OnCommand) { HelpMessage = CommandHelp });
        commandManager.AddHandler(CommandName2, new CommandInfo(OnCommand) { HelpMessage = CommandHelp });

        chatGui.ChatMessage += chatHandler.OnChatMessage;
        this.pluginInterface.UiBuilder.Draw += windowSystem.Draw;
        this.pluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        this.pluginInterface.UiBuilder.OpenMainUi += DrawConfigUI;

        ECommonsMain.Init(pluginInterface, this, ECommons.Module.DalamudReflector);
    }

    public void Dispose()
    {
        windowSystem.RemoveAllWindows();
        chatGui.ChatMessage -= chatHandler.OnChatMessage;
        commandManager.RemoveHandler(CommandName);
        commandManager.RemoveHandler(CommandName2);
        pluginInterface.UiBuilder.Draw -= windowSystem.Draw;
        pluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
        pluginInterface.UiBuilder.OpenMainUi -= DrawConfigUI;
        GC.SuppressFinalize(this);

        ECommonsMain.Dispose();
        provider.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            DrawConfigUI();
            return;
        }
        var subCommand = new TextCommand($"/{args}");
        void enableReactions(bool enable)
        {
            if (string.IsNullOrEmpty(subCommand.Args))
                reactions.SetEnabledAll(enable);
            else
                reactions.SetEnabled(subCommand.Args, enable);
        }

        if (subCommand.Main.Equals("/on"))
            enableReactions(true);
        else if (subCommand.Main.Equals("/off"))
            enableReactions(false);
    }

    private void DrawConfigUI()
    {
        configWindow.IsOpen = true;
    }
}
