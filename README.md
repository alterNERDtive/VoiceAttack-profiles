# Elite Dangerous VoiceAttack Profiles #

These are various profiles for [VoiceAttack](https://voiceattack.com) (VA) I use 
to enhance my Elite experience. They give me important info, facilitate 
day-to-day gaming and do some special things for [Fuel 
Rats](https://fuelrats.com) and [Hull Seals](https://hullseals.space) work.

Each of the profiles is documented in `/docs/`.

## Available Profiles ##

* [EliteDangerous](docs/EliteDangerous.md): My main Elite VA profile. Here be 
  dragons; things may be heavily tailored towards how _I_ play the game and may 
  not apply to how you play it. Included for reference and as a baseline or 
  inspiration to create your own stuff.
* [SpanshAttack](docs/SpanshAttack.md): profile to plot and follow trips along 
  the neutron highway using [spansh](https://spansh.co.uk/plotter).
* [RatAttack](docs/RatAttack.md): profile for interactions with the Fuel Rats’ 
  IRC server.
* [SealAttack](docs/SealAttack.md): profile for interactions with the Hull 
  Seals’ IRC server.

## Requirements ##

* [VoiceAttack](https://voiceattack.com): absolutely required (duh).
* [bindED](https://forum.voiceattack.com/smf/index.php?topic=564.0): required 
  for all profiles; makes anything involving hitting E:D key binds portable.
* [EDDI](https://github.com/EDCD/EDDI) installed as a VoiceAttack plugin: 
  required for my personal profile and for SpanshAttack, optional for RatAttack 
  and SealAttack.
* [ED-NeutronRouter](https://github.com/sc-pulgan/ED-NeutronRouter): required 
  for SpanshAttack.
* [elite-scripts](https://github.com/alterNERDtive/elite-scripts): required for 
  EliteDangerous, recommended for RatAttack and SealAttack. The release page 
  here includes a compiled version for Windows that does not need Python 
  installed. Just download the zip file from the release page and extract into 
  your VoiceAttack directory.

Additionally, you need to have keyboard binds setup at least as secondary 
bindings in Elite’s controls options. VA _cannot_ “push” joystick buttons for 
you, it can only do keyboard inputs. Hence its only way to interact with Elite 
is through keyboard emulation, even if you otherwise play the game with 
a controller or HOTAS. Or racing wheel. Or Rock Band set. Or bananas.

## Settings ##

Each profile has its respective `startup` command that should be launched upon 
loading the profile. If you include the profile in your own (see below) you have 
to manually call them for each included profile when yours is loaded.

## Using a Profile ##

Import the profile into VA, check the startup command for any settings you might 
want to adjust, activate it, done.

Oh, and you probably might want to check the corresponding README first.

## Including a Profile ##

If you are already using a custom profile (or want to use mine), you can include 
others by going to the profile options and adding them to the “Include commands 
form other profiles:” option.

VoiceAttack does not execute configured startup commands for included profiles. 
Hence, you’ll have to have your own profile have one that in turn runs the 
included profiles’ startup commands. While you are doing that, you might as well 
set all settings here, centrally. Main advantage is that you can just upgrade 
the included profiles to newer versions without losing your settings.

Because of limitations of VoiceAttack itself, only the first matching command 
found will be executed, _including EDDI events_. That means you have to check 
your profile against the imported ones for events they both handle. E.g. if you 
already have a `((EDDI Message sent))` handler in your profile, you have to run 
`RatAttack.EDDI Message sent` and `SealAttack.EDDI Message sent` from within it.

You also have to do that if you include multiple profiles using the same events 
(e.g. RatAttack + SealAttack), even if you don’t have the same event in the 
including profile! If you want to make sure, manually create all EDDI Event 
handlers used in imported profiles and have them call the corresponding 
commands. See the Elite Dangerous profile for reference.

## Major Version Changes ##

If a profile’s major version number changes (e.g. SpanshAttack 1.x.x to 2.0.0) 
there _will_ be changes to the profile that do one or any amount of the 
following:

* command names / command invocation have changed
* configuration variable name or format have changed
* features removed
* _major_ features added

**If you see a major version number change in the release notes, please pay 
attention to said notes to know what you might have to change to get it to 
work!**

If any of the steps are unclear, please see below.

## Need Help / Want to Contribute? ##

If you run into any errors, please try running the profile in question on its 
own / get a fresh version. If that doesn’t fix the error, look at the 
[devel](https://github.com/alterNERDtive/VoiceAttack-profiles/tree/devel) branch 
and see if it’s fixed there already.

If you have no idea what I was saying in that last parargraph and / or the 
things mentioned there don’t fix your problem, please [file an 
issue](https://github.com/alterNERDtive/VoiceAttack-profiles/issues). Thanks! :)

You can also [say “Hi” on Discord](https://discord.gg/mD6dAb) if that is your 
thing.
