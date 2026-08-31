using Dalamud.Utility;
using System;
using System.Text.RegularExpressions;

namespace PuppetMaster;

internal class ReactionService
{
    private readonly ConfigurationProvider _configurationProvider;
    private readonly CommandParser _parser;

    public Configuration Configuration => _configurationProvider.Configuration;

    public ReactionService(ConfigurationProvider configurationProvider, CommandParser parser)
    {
        this._configurationProvider = configurationProvider;
        this._parser = parser;
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
        if (reaction.UseRegex && (reload || reaction.CustomRx == null))
            try
            {
                reaction.CustomRx = new Regex(reaction.CustomPhrase);
            }
            catch (Exception) { }
        else if (reload || reaction.Rx == null)
            try
            {
                reaction.Rx = new Regex(GetDefaultRegex(reaction));
            }
            catch (Exception) { }
    }

    // todo: remove later, redundant middleman smell
    public TextCommand GetTestInputCommand(Reaction reaction) =>
        _parser.Parse(reaction, reaction.TestInput) ?? new();
}
