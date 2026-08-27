using System.Text;
using System.Text.RegularExpressions;
using Dalamud.Plugin.Services;
using Dalamud.Utility;

namespace PuppetMaster;

public class CommandParser(IPluginLog logger)
{
    public TextCommand? Parse(Reaction reaction, string input)
    {
        // Continue to replace [] with <> for compatability with commands people
        // are used to or have saved in macros/silkstring etc.
        if (reaction.ReplaceBrackets)  
            input = input.Replace('[', '<').Replace(']', '>');
        
        var usingRegex = reaction is { UseRegex: true, CustomRx: not null };
        if (usingRegex)
            return Parse(reaction.CustomRx, input);
        return Parse(reaction.Rx, input);
    }

    public TextCommand? Parse(Regex? regex, string input)
    {
        logger.Debug($"Parsing input {input} with Regex {regex}");

        if (regex is null)
            return null;
        if (regex.ToString().IsNullOrWhitespace())
            return null;

        var match = regex.Match(input);
        if (!match.Success)
            return null;
        
        var command = input.Substring(match.Index + match.Length).Trim();
        var commandStringBuilder = new StringBuilder("/");

        var escapeNext = false;
        var breakAtBracket = false;
        foreach (var @char in command)
        {
            logger.Debug($"Evaluating character: {@char}");
            if (escapeNext)
            {
                logger.Debug($"Character escaped, unconditionally appending");
                commandStringBuilder.Append(@char);
                escapeNext = false;
                continue;
            }
            
            if (@char == '\\')
            {
                logger.Debug($"Character is escape char, will skip next, moving to next");
                escapeNext = true;
                continue;
            }

            if (commandStringBuilder.Length == 1 && @char == '(')
            {
                breakAtBracket = true;
                continue;
            }
            
            if (breakAtBracket && @char == ')')
            {
                logger.Debug($"Character is ) and breakAtBracket is true, completing parse");
                break;
            }

            if (!breakAtBracket && @char == ' ')
            {
                logger.Debug($"Found white space and breakAtBracket is false, completing parse");
                break;
            }
            
            commandStringBuilder.Append(@char);
        }

        if (commandStringBuilder.Length == 1)
            return null;
        
        var parsedCommand = commandStringBuilder.ToString();
        logger.Debug($"Completed parse is {parsedCommand}");
        var textCommand = new TextCommand(parsedCommand);
        logger.Debug($"Result is {textCommand}");
        return textCommand;
    }
}
