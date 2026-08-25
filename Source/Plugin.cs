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
/puppetmaster on|off - enable or disable all reactions
/puppetmaster on|off <ReactionName> - enable or disable reactions by name";

    private readonly ServiceProvider provider;
    private readonly WindowSystem windowSystem = new("PuppetMaster");

    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ICommandManager commandManager;
    private readonly IChatGui chatGui;

    private readonly ReactionService reactions;
    private readonly ChatHandler chatHandler;
    private readonly ConfigWindow configWindow;

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        var dalamud = pluginInterface.Create<DalamudServices>()
                      ?? throw new InvalidOperationException("[PuppetMaster] Failed to inject Dalamud services");

        provider = new ServiceCollection()
                   // Dalamud services
                   .AddSingleton(dalamud.PluginInterface)
                   .AddSingleton(dalamud.CommandManager)
                   .AddSingleton(dalamud.ChatGui)
                   .AddSingleton(dalamud.DataManager)
                   // App services
                   .AddSingleton<ReactionService>()
                   .AddSingleton<EmoteRegistry>()
                   .AddSingleton<ChatHandler>()
                   .AddSingleton<ConfigWindow>()
                   .BuildServiceProvider();

        reactions = provider.GetRequiredService<ReactionService>();
        chatHandler = provider.GetRequiredService<ChatHandler>();
        configWindow = provider.GetRequiredService<ConfigWindow>();
        
        windowSystem.AddWindow(configWindow);

        dalamud.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) { HelpMessage = CommandHelp });
        dalamud.CommandManager.AddHandler(CommandName2, new CommandInfo(OnCommand) { HelpMessage = CommandHelp });
        
        dalamud.ChatGui.ChatMessage += chatHandler.OnChatMessage;
        dalamud.PluginInterface.UiBuilder.Draw += DrawUI;
        dalamud.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        dalamud.PluginInterface.UiBuilder.OpenMainUi += DrawConfigUI;

        ECommonsMain.Init(pluginInterface, this, ECommons.Module.All);
    }

    public void Dispose()
    {
        windowSystem.RemoveAllWindows();
        chatGui.ChatMessage -= chatHandler.OnChatMessage;
        commandManager.RemoveHandler(CommandName);
        pluginInterface.UiBuilder.Draw -= DrawUI;
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

    private void DrawUI() =>
        windowSystem.Draw();

    private void DrawConfigUI()
    {
        configWindow.IsOpen = true;
        configWindow.PreloadTestResult();
    }
}
