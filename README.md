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
* [SealAttack](docs/SealAttack.md): profile for interactions with the Hull 
  Seals’ IRC server. (**VERY early alpha stage.**)
* [SpanshAttack](docs/SpanshAttack.md): profile to plot and follow trips along 
  the neutron highway using [spansh](https://spansh.co.uk/plotter).
* [StreamAttack](docs/StreamAttack.md): profile for writing various things to 
  files that can then be read by streaming software like OBS.

## Requirements ##

* [VoiceAttack](https://voiceattack.com): absolutely required (duh).
* [bindED](https://forum.voiceattack.com/smf/index.php?topic=564.0): required 
  for EliteDangerous and SpanshAttack; makes anything involving hitting E:D key 
  binds portable.
* [EDDI](https://github.com/EDCD/EDDI) installed as a VoiceAttack plugin: 
  required for EliteDangerous, SpanshAttack and Streamattack, optional for 
  RatAttack and SealAttack. If you are already running EDDI and want to keep the 
  default speech responder active, you will need to re-enable it in your profile 
  _after_ running the `<profile>.startup` command.
* [ED-NeutronRouter](https://github.com/sc-pulgan/ED-NeutronRouter): required 
  for SpanshAttack. **Make sure to [grab the pre-release 
  1.02](https://github.com/sc-pulgan/ED-NeutronRouter/releases/tag/1.02)** since 
  1.01 has a bug with a hardcoded 50 ly jump range.
* [elite-scripts](https://github.com/alterNERDtive/elite-scripts): required for 
  EliteDangerous, SpanshAttack and StreamAttack, recommended for RatAttack and 
  SealAttack (included).

Additionally, you need to have keyboard binds setup at least as secondary 
bindings in Elite’s controls options. VA _cannot_ “push” joystick buttons for 
you, it can only do keyboard inputs. Hence its only way to interact with Elite 
is through keyboard emulation, even if you otherwise play the game with 
a controller or HOTAS. Or racing wheel. Or Rock Band set. Or bananas.

## Installing ##

Install the plugins listed in [Requirements](#Requirements).

Download the profile package (`alterNERDtive-voiceattack-profiles.vax`) from the 
[release page](https://github.com/alterNERDtive/VoiceAttack-profiles/releases/latest)
and import it as a profile into VoiceAttack. This will install all 3 profiles, 
the referenced sound files and the Python scripts.

Last but not least, if you want to use my setup as-is, you need to go into the 
profile options for the freshly imported `EliteDangerous` profile and import 
`RatAttack` and `SpanshAttack`.

You can also download the profiles individually from the `profiles/` folder on 
github.

### Updating ###

If you use the profiles unchanged or just import them and override commands from 
your main profile, updating should work just like installing: import the profile 
package and tell VoiceAttack to overwrite commands when prompted.

### Major Version Changes ###

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

## Settings ##

All profiles will now load sane defaults if you haven’t changed anything. You no 
longer need to fiddle with the `startup` commands of each profile, instead you 
can use voice commands to change settings! See the `docs/` and the 
`_configuration` commands section of each profile.

One caveat applies: settings are stored in the _profile where you run the 
configuration commands_. If you change your active, main profile around a lot 
you’ll have to set everything up for each of them separately. I suggest instead 
having a single “main” profiles and including everything else.

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
included profiles’ startup commands. Main advantage is that you can just upgrade 
the included profiles to newer versions without losing your own stuff.

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
