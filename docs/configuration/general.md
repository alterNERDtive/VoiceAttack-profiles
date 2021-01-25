# General Configuration

## Settings

All profiles will load sane defaults if you haven’t changed anything. The
configuration is stored in a bunch of VoiceAttack variables stored in your
custom profile. You could even have different custom profiles with their own
distinct settings.

You change the configuration via voice commands:

* For booleans (toggles): `customize setting [enable;disable] <trigger phrase>`
* For everything else: `customize setting set <trigger phrase>`

You can find a list of trigger phrases here, or you can say
`customize setting list [options;settings]` for a list of voice triggers and
what they do. If you don’t remember your settings or just want a nice list,
`customize setting report [options;settings]` will print that to the
VoiceAttack log. If you want to reset everything back to default state, say
`customize setting clear all my settings`.

The “customize setting” preset is kind of a leftover from times long gone and
does not quite fit anymore. Might change in a future version.

Basically all the settings are available using the `customize settings` prefix, 
then saying `[enable;disable] <setting>` for on/off switches and `set <setting>` 
for text variables.

One caveat applies: settings will only be saved in the profile you have 
selected, but be preserved if you switch around.

### General Settings for All Profiles

Toggles:

* `eddi quiet mode`: Make EDDI shut up. Disables all built-in speech responders.

Other settings:

* `elite paste key`: The key used to paste in conjunction with CTRL. The
  physical key in your layout that would be 'V' on QWERTY.
* `log level`: The level of detail for logging to the VoiceAttack log. Valid
  levels are "ERROR", "WARN", "NOTICE", "INFO" and "DEBUG". Default: "NOTICE".

## Adding Commands

If you want to edit a command or add your own, _do not edit the profiles 
directly_. Instead create commands in your custom profile, and copy commands you 
want to change over to that before editing them. This will make sure no changes 
are lost if you update the profiles.

### EDDI Events

Because of limitations of VoiceAttack itself, only the first matching command
found will be executed, _including EDDI events_. That means that if you create
commands to handle EDDI events, you are going to have to make sure that the
appropriate handlers in my profiles are called, too. That is done by invoking
the `eddi.Event` plugin context of the `alterNERDtive-base` plugin. Otherwise
stuff _will_ break.

![[EDDI-event.png]]