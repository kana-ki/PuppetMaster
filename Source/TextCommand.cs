using System;

namespace PuppetMaster;

public struct TextCommand
{
    public string Main = string.Empty;
    public string Args = string.Empty;

    public TextCommand() { }

    public TextCommand(string command)
    {
        if (command == string.Empty) return;

        command = command.Trim();
        if (command.StartsWith('/'))
        {
            command = command.Replace('[', '<').Replace(']', '>');
            var space = command.IndexOf(' ');
            Main = (space == -1 ? command : command[..space]).ToLower();
            Args = (space == -1 ? string.Empty : command[(space + 1)..]);
        }
        else
        {
            Main = command;
        }
    }

    public override readonly string ToString()
    {
        return (Main + " " + Args).Trim();
    }
}
