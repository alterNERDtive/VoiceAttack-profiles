# StreamAttack #

This profile uses the [EDDI](https://github.com/EDCD/EDDI) plugin to write 
a bunch of information about your commander, your current location and your ship 
to files that can be accessed e.g. by your streaming software to be displayed on 
stream.

Default folder is `%appdata%\StreamAttack\`.

## Requirements ##

In addition to VoiceAttack, you will need the following plugins to use this 
profile:

* [EDDI](https://github.com/EDCD/EDDI) installed as a VoiceAttack plugin

### EDDI speech responder ###

For the convenience of people that have not been using EDDI in the past, 
StreamAttack will deactivate the speech responder automatically to not clutter 
them with unwanted TTS.

If you are already an EDDI user and want to keep the default speech responder 
functionality, you will have to disable the `EDDI.quietMode` setting by running 
the `customize settings disable eddi quiet mode` command.

## Settings ##

See the [Configuration Variables](#Configuration-Variables) section.

## Including the Profile ##

When including the profile, be sure to

* Run the startup command. You will need to have a startup command in your 
  profile (= one that is run on profile loading) and call `StreamAttack.startup` 
  from that one.
* Make sure all EDDI events that StreamAttack needs are correctly handled. For 
  all events used in StreamAttack that you already have handlers for in your 
  profile, you’ll have to include a call to `StreamAttack.<event name>`. E.g. 
  for “EDDI Jumped”, call `StreamAttack.EDDI Jumped` by name from your `((EDDI 
  Jumped))` command.

## Commands ##

* `clear jump target`: clears the current jump target.
* `set jump target`: sets the jump target to the currently targeted system. 
  Distance will be written to the configured file.

* `[copy;open] ship build`: copies the current ship build (coriolis) or opens it 
  in your default browser.
* `open StreamAttack folder`: opens the configured folder in Explorer.

## Files the Profile Provides ##

### Elite ###

#### Commander ####

* `Elite\cmdr\name`: the current commander’s name.

#### Jump Target ####

* `Elite\jumpTarget\distance`: distance to current jump target in light years.
* `Elite\jumpTarget\full`: pretty-printed `<distance> ly to <name>`.
* `Elite\jumpTarget\name`: the current jump target’s system name.

#### Location ####

* `Elite\location\full`: depending on your status, either the station you are 
  currently docked at (+ system), the body you are currently near, or the system 
  you are currently in.
* `Elite\location\system`: the system you are currently in.

#### Ship ####

* `Elite\ship\build`: your current ship’s loadout (link to coriolis).
* `Elite\ship\full`: `“<name>” | <model> | <build>`.
* `Elite\ship\model`: your current ship’s model.
* `Elite\ship\name`: your current ship’s name.

## Logging ##

The profile supports logging a bunch of stuff to the VoiceAttack event log. By 
default, logging is concise and constrained to basically error messages.

If you need more logging (usually for debugging purposes), say `enable logging`. 
If you want to enable verbose logging _by default_, call the 
`Logging.enableLogging` command from your custom profile’s `startup` command.

## Exposed Variables ##

The following Variables are _global_ and thus readable (and writeable! Please 
don’t unless it’s a config variable …) from other profiles:

### Configuration Variables ###

There are a bunch of configuration variables. You should not overwrite those 
manually, instead use the provided commands in the `_configuration` section!

Basically all the settings are available using the `customize settings` prefix, 
then saying `[enable;disable] <setting>` for on/off switches and `set <setting>` 
for text variables.

* `EDDI.quietMode` (boolean): whether or not to set EDDI to quite mode. Default: 
  true.
* `EDDI.useEddiForVoice` (boolean): whether to use EDDI over VA’s builtin `say` 
  command. Default: false.
* `StreamAttack.outputDir` (string): the directory StreamAttack will save its 
  information to. Default: `%appdata%\StreamAttack\`.
* `python.ScriptPath` (string): the path you have placed the compiled Python 
  scripts in.  Default: “{VA_DIR}\Sounds\scripts” (the “\Sounds\scripts” folder 
  in your VoiceAttack installation directory).

### Other Variables ###

These variables can be used to get information about the current neutron route. 
Please do not set them manually and / or from outside the StreamAttack profile.

* `StreamAttack.Elite.jumpTarget` (string): the current jump target.
