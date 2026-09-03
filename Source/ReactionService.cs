using Dalamud.Utility;
using System;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;

namespace PuppetMaster;

internal class ReactionService
{
    private readonly ConfigurationProvider _configurationProvider;
    private readonly CommandParser _parser;
    private readonly IPluginLog _log;

    public Configuration Configuration => _configurationProvider.Configuration;

    public ReactionService(ConfigurationProvider configurationProvider, IPluginLog log, CommandParser parser)
    {
        this._configurationProvider = configurationProvider;
        this._parser = parser;
        this._log = log;
        InitializeRegex();
    }

    public Reaction NewReaction() => _configurationProvider.NewReaction();

    public void SetEnabledAll(bool enabled = true)
    {
        for (var i = 0; i < Configuration.Reactions.Count; i++)
            Configuration.Reactions[i].Enabled = enabled;
        Configuration.Save();
    }

    public void SetEnabled(string name, bool enabled = true, StringComparison sc = StringComparison.Ordinal)
    {
        foreach (var reaction in Configuration.Reactions)
            if (reaction.Name.Equals(name, sc))
                reaction.Enabled = enabled;
        Configuration.Save();
    }

    public void ExportReactionToClipboard(Reaction? reaction)
    {
        var json = JsonSerializer.Serialize(reaction);
        var jsonBytes = Encoding.UTF8.GetBytes(json);
        var base64 = Convert.ToBase64String(jsonBytes);
        ImGui.SetClipboardText(base64);
    }

    public Reaction? ImportFromClipboard()
    {
        var base64 = ImGui.GetClipboardText();
        var base64Bytes = Convert.FromBase64String(base64);
        var json = Encoding.UTF8.GetString(base64Bytes);
        var reaction =  JsonSerializer.Deserialize<Reaction>(json);
        reaction?.Enabled = false;
        return reaction;
    }

    public string GetDefaultRegex(Reaction reaction) =>
        !reaction.TriggerPhrase.IsNullOrWhitespace()
             ? @"(?i)\b(" + reaction.TriggerPhrase + @")(?=\s)"
             : @"";

    // Relying on initalization is not reliable, and
    // runtime variables in configuration is just pikachu-surprise-face 
    // todo: remove the need for this
    private void InitializeRegex()
    {
        foreach (var reaction in Configuration.Reactions)
            InitializeRegex(reaction);
    }

    public void InitializeRegex(Reaction reaction, bool reload = false)
    {
        try {
            if (reaction.UseRegex && (reload || reaction.CustomRx == null))
                reaction.CustomRx = new Regex(reaction.CustomPhrase);
            reaction.Rx = new Regex(GetDefaultRegex(reaction));
        }
        catch (Exception) { }
    }

    // todo: remove later, redundant middleman smell
    public TextCommand GetTestInputCommand(Reaction reaction) =>
        _parser.Parse(reaction, reaction.TestInput) ?? new();
}
