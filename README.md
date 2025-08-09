# Puppet Master

This plugin allows others to control you via in-game commands.
This is a fork of the original PuppetMaster from [DodingDaga](https://github.com/dodingdaga/DalamudPlugins) that will have my own take on command parsing.

## Install

Add the following repository to custom repo to your plugin sources
```
https://raw.githubusercontent.com/kana-ki/PuppetMaster/refs/heads/main/PuppetMaster.json
```

## Setup

- Use the `/puppetmaster` command
- In the new window, Reactions tab, click the "Add" button
- Give the new Reaction profile a name
- Tick the checkbox next to it to enable it
- Go to "Edit Reactions" tab
- Write into the "Trigger" textbox your chosen trigger word
- If you only want to allow "sit" commands, then leave this checked
- If you want to allow *all* commands from your trusted person(s) then check "Allow all text commands" (see the [safety note](#Safety) below)
- Below, check all the tickboxes for all the channels you want this trigger work to work in

This trigger word will be the word your dom will type into a channel to trigger a game command in your client.

## Safety

Only share your trigger word with those that you trust. The PuppetMaster plugin allows those who know it to use any command in the game on your behalf.

Doing this in public will be quite obvious that you're using plugins, furthermore someone with maliscious intent could cause damage with certain commands.

## How to use

The general pattern of usage is to type into the chat your keyword, followed by any game command inside brackets, such as `pet (say meow)`. In the following examples we'll assume the trigger keyword of "pet".

- `pet (sit)`
- `pet (gaction jump)`
- `pet (follow)`
- `pet (gs change 1)`

Sumarily, any command you could usually use yourself with a `/` in your chatbox, works here. Furthermore, if the command is only one word, you may omit the brackets, such as `pet sit`.

## Notes

For now, this fork of PuppetMaster will use the original configuration you already have if you've used PuppetMaster before. As soon as any divergence exists, configuration will become independent.
