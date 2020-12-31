# Elite Dangerous VoiceAttack Profiles #

These are various profiles for [VoiceAttack](https://voiceattack.com) (VA) I use 
to enhance my Elite experience. They give me important info, facilitate 
day-to-day gaming and do some special things for [Fuel 
Rats](https://fuelrats.com) and [Hull Seals](https://hullseals.space) work.

Each of the profiles is documented in `docs/`.

## Available Profiles ##

* [EliteDangerous](docs/EliteDangerous.md): My main Elite VA profile. Here be 
  dragons; things may be heavily tailored towards how _I_ play the game and may 
  not apply to how you play it. Included for reference and as a baseline or 
  inspiration to create your own stuff.
* [RatAttack](docs/RatAttack.md): profile for interactions with the Fuel Rats’ 
  IRC server.
* [SpanshAttack](docs/SpanshAttack.md): profile to plot and follow trips along 
  the neutron highway using [spansh](https://spansh.co.uk/plotter).
* [StreamAttack](docs/StreamAttack.md): profile for writing various things to 
  files that can then be read by streaming software like OBS.

## Requirements ##

* [VoiceAttack](https://voiceattack.com): absolutely required (duh).
* [bindED](https://forum.voiceattack.com/smf/index.php?topic=564.0): required 
  for EliteDangerous and SpanshAttack; makes anything involving hitting E:D key 
  binds portable. You can also use [my updated fork of 
  that](https://github.com/alterNERDtive/bindED); read the README over there for 
  more information.
* [EDDI](https://github.com/EDCD/EDDI) installed as a VoiceAttack plugin: 
  required for EliteDangerous, SpanshAttack and Streamattack, optional for 
  RatAttack.
* [ED-NeutronRouter](https://github.com/sc-pulgan/ED-NeutronRouter): required 
  for SpanshAttack. **Make sure to [grab the pre-release 
  1.02](https://github.com/sc-pulgan/ED-NeutronRouter/releases/tag/1.02)** since 
  1.01 has a bug with a hardcoded 50 ly jump range.
* [elite-scripts](https://github.com/alterNERDtive/elite-scripts): required for 
  EliteDangerous, SpanshAttack and StreamAttack, recommended for RatAttack 
  (included).

Additionally, you need to have keyboard binds setup at least as secondary 
bindings in Elite’s controls options. VA _cannot_ “push” joystick buttons for 
you, it can only do keyboard inputs. Hence its only way to interact with Elite 
is through keyboard emulation, even if you otherwise play the game with 
a controller or HOTAS. Or racing wheel. Or Rock Band set. Or bananas.

## Installing ##

1. Install [VoiceAttack](https://voiceattack.com).
1. Install the plugins listed in [Requirements](#Requirements).
1. Download the profile package (`alterNERDtive-voiceattack-profiles.vax`) from 
   the [release 
   page](https://github.com/alterNERDtive/VoiceAttack-profiles/releases/latest) 
   and import it as a profile into VoiceAttack. This will install all included 
   profiles, the referenced sound files and the Python scripts.

(You can also download the profiles individually from the `profiles/` folder on 
github.)

## Getting Started ##

You will want to create your own profile and then import the downloaded ones 
into your custom profile. This way you can easily add commands (to your custom 
profile), change commands (by copying them into your custom profile and editing 
them) and change settings (by overriding them in your custom profile). Note that 
most settings can be changed with voice commands. If you find any that cannot 
but you feel should, please file an issue or report it on Discord.

### Creating a Custom Profile ###

Hit the second button next to the profile dropdown menu and choose “Create new 
Profile”. Give it a name and add some commands if you want to.

You can also just keep using a profile you have already created.

### Importing profiles ###

First off, create a startup command. You can name it anything you want, but 
I recommend calling it “startup” or similar, and to deactivate the “when i say” 
checkbox in the command options to make sure you don’t accidentally run it via 
voice. We will need this command later.

While editing the profile, hit the “Options” button. On the section labeled 
“Include commands from other profiles”, hit the “…” button and add all profiles 
(`EliteDangerous`, `RatAttack`, `SpanshAttack`, `StreamAttack`).  All commands 
defined in these profiles will be available to you. Make sure that 
“EliteDangerous” is on top of the list.

Now switch to the “Profile Exec” tab. Tick the “Execute a command each time this 
profile is loaded” checkbox, and select the “startup” command you have created 
earlier.

Edit your startup command. Add a new action using “Other” → “VoiceAttack Action” 
→ “Execute Another Command”. Choose “Execute by name (Advanced)” and enter 
“EliteDangerous.startup”.

### Settings ###

All profiles will load sane defaults if you haven’t changed anything. You no 
longer need to fiddle with the `startup` commands of each profile, instead you 
can use voice commands to change settings! See the `docs/` and the 
`_configuration` commands section of each profile.

Basically all the settings are available using the `customize settings` prefix, 
then saying `[enable;disable] <setting>` for on/off switches and `set <setting>` 
for text variables.

One caveat applies: settings will only be saved in the profile you have 
selected, but be preserved if you switch around.

### Making changes ###

If you want to edit a command or add your own, _do not edit the profiles 
directly_. Instead create commands in your custom profile, and copy commands you 
want to change over to that before editing them. This will make sure no changes 
are lost if you update the profiles.

Because of limitations of VoiceAttack itself, only the first matching command 
found will be executed, _including EDDI events_. That means that if you create 
commands to handle EDDI events, you are going to have to check the imported 
profiles if they rely on these event handlers as well, and call them manually if 
they do.

E.g.  if you want to create a custom `((EDDI Message sent))` handler in your 
profile, you will have to make it excute the `EliteDangerous.EDDI Message sent` 
and `RatAttack.EDDI Message sent` commands.  Otherwise stuff _will_ break.

If you have no idea what the previous two paragraphs were about, you can most 
likely just ignore them.

## Updating ##

### The Best™ Way ###

Say “check for profiles update”. If it finds one, say “download profiles 
update”. Restart VoiceAttack.

There will also be an update check every time `EliteDangerous.startup` is run.

**Note**: This is temporarily disabled because version 4.0 will introduce
breaking changes that need manual upgrade steps.

### The Manual Way ###

If you don’t like automation or do not use all provided profiles, you’ll have to 
update semi-manually.

Say “open VoiceAttack import folder”. Download the current release profile 
package, drop it in there. Restart VoiceAttack.

### Note about Admin Privileges ###

The update process will run VoiceAttack with admin privileges. If you do not 
usually run VoiceAttack with admin privileges, do _not_ have it start 
VoiceAttack for you after importing the profiles update. It will keep running 
with elevated privileges. Instead restart it manually. If you are not sure if 
you are running with elevated privileges check the “VoiceAttack Information” 
dialog in VoiceAttack General settings → “System Info >”. It will include a line 
about running with admin prvileges if it does.

### Major Version Changes ###

If a profile’s major version number changes (e.g. SpanshAttack 1.x.x to 2.0.0) 
there _will_ be changes to the profile that include one or any amount of the 
following changes:

* command names / command invocation have changed
* configuration variable name or format have changed
* features removed
* _major_ features added

**If you see a major version number change in the release notes, please pay 
attention to said notes to know what you might have to change to get it to 
work!**

## Need Help / Want to Contribute? ##

If you run into any errors, please try running the profile in question on its 
own / get a fresh version. If that doesn’t fix the error, look at the 
[devel](https://github.com/alterNERDtive/VoiceAttack-profiles/tree/devel) branch 
and see if it’s fixed there already.

If you have no idea what I was saying in that last parargraph and / or the 
things mentioned there don’t fix your problem, please [file an 
issue](https://github.com/alterNERDtive/VoiceAttack-profiles/issues). Thanks! :)

You can also [say “Hi” on Discord](https://discord.gg/kXtXm54) if that is your 
thing.
