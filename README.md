# Puppet Master

This plugin allows others to control you via in-game commands.
This is a fork of the original PuppetMaster from [DodingDaga](https://github.com/dodingdaga/DalamudPlugins) that will have my own take on command parsing.

## Difference

This variant adds:
- Redesigned UI
- Ability to filter players
- Ability to filter commands
- Ability to set different permissions for different channels/players for the same trigger
- Adjustments to allow for commands that need ) or [] characters

## Install

Add the following repository url to your Custom Plugin Repositories list in the experimental tab of Dalamud Setting
```
https://raw.githubusercontent.com/kana-ki/PuppetMaster/refs/heads/main/PuppetMaster.json
```

Find "Puppet Master" in the Plugin Installer, and hit Install.

## Setup

- Use the `/puppet` command
- In the new window, at the bottom, click the "+" button
- At the top, give the new Reaction profile a name
- Write into the "Trigger" textbox your chosen trigger word
- Below, in the "Allowed commands" section, set this to what you'd like to allow others to do on your behalf (see #safety)
- Below, in the "Allowed players" section, set this to who you'd like to allow to control you (see #safety)
- Below, in the "Allowed channels" section, check all the tickboxes for all the channels you want this trigger work to work in (see #safety)
- At the top, click to change "Disabled" to "Enabled"
  
This trigger word will now be the word your dom can type into a channel to trigger a game command in your client.

## How to command

The general pattern of usage is to type into the chat your keyword, followed by any game command inside brackets, such as `pet (say meow)`. In the following examples we'll assume the trigger keyword of "pet".

- `pet (sit)`
- `pet (gaction jump)`
- `pet (follow)`
- `pet (gs change 1)`

Summarily, any command you could usually use yourself with a `/` in your chatbox, works here. 
Furthermore, if the command is only one word, you may omit the brackets, such as `pet sit`.

## Safety

Only use PuppetMaster with those whom you trust. The PuppetMaster plugin allows those who know your trigger word and are allowed in configurion to to use any allowed command in the game on your behalf. Doing this in public will be quite obvious that you're using plugins, furthermore someone with malicious intent could cause damage with certain commands.

### Setting allowed players
PuppetMaster allows you to constrain who can use your trigger word and control you via it.

In the Reaction profile for the trigger, in the "Allowed players" section, while the drop down here is set to "Allow everyone", anyone with the trigger word can use it to control you and perform commands on your behalf (assuming other command/channel constraints don't apply). 

Whilst the drop down is set to "Allow only:", only Players in the list below it will be able to use the trigger word in this reaction profile. You may add players to the list box beneath by typing in the full name of the player, selecting their home world in the drop down list, and then clicking the "+" button. 

### Setting allowed commands

PuppetMaster allows you to constrain what commands can be used by players that use your trigger word. 

In the Reaction profile for the trigger, in the "Allowed commands" section, while the drop down here is set to "Allow all", anyone with the trigger word can use it to perform any command on your behalf (assuming other player/channel constraints don't apply). This includes /say, /shout and other chat commands, and /glamourer, /penumbra and other plugin commands. 

Whilst the drop down is set to "Allow only: ", only command in the list below it will be allowed with the trigger in this reaction profile. You may add an allowed command to the list box beneath by typing in the command in the textbox below it, and clicking the "+" button. You may also add all emotes to the list quickly via the blue "Add all emotes" link above the list box; this is equivalent to having "Allow "Sit" or "groundsit" request" checked and "Allow all text commands" unchecked in the original PuppetMaster. 

Whilst the drop down is set to "Allow all except: ", all command will be allowed with the trigger in this reaction profile unless they are in the list below. You may add an disallowed command to the list box beneath by typing in the command in the textbox below it, and clicking the "+" button. You may also add all recommended disallowed commands to this list quickly via the blue "Add recommended exceptions" link above the list box; this includes all chat channels, UI commands, friends list and black list commands, and hud commands. 
