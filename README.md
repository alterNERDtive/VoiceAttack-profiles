= Elite Dangerous VoiceAttack Profiles =

These are various profiles for [VoiceAttack](https://voiceattack.com) (VA) I use 
to enhance my Elite experience. They give me important info, facilitate 
day-to-day gaming and do some special things for [Fuel 
Rats](https://fuelrats.com) and [Hull Seals](https://hullseals.space) work.

Each of the profiles is documented in `/docs/`.

== Available Profiles ==

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

== Requirements ==

* [VoiceAttack](https://voiceattack.com): absolutely required (duh).
* [bindED](FIXXME): required for all profiles; makes anything involving hitting 
  E:D key binds portable.
* [EDDI](FIXXME) installed as a VoiceAttack plugin: required for my personal 
  profile and for SpanshAttack, optional for RatAttack and SealAttack.
* [FIXXME that neutron plugin thing](FIXXME): required for SpanshAttack.

Additionally, you need to have keyboard binds setup at least as secondary 
bindings in Elite’s controls options. VA _cannot_ “push” joystick buttons for 
you, it can only do keyboard inputs. Hence its only way to interact with Elite 
is through keyboard emulation, even if you otherwise play the game with 
a controller or HOTAS. Or racing wheel. Or Rock Band set. Or bananas.

== Settings ==

Each profile has its respective `startup` command that should be launched upon 
loading the profile. If you include the profile in your own (see below) you have 
to manually call them for each included profile when yours is loaded.

== Using a Profile ==

Import the profile into VA, check the startup command for any settings you might 
want to adjust, activate it, done.

Oh, and you probably might want to check the corresponding README first.

<! FIXXME: does importing it include the startup settings? Check! !>

== Including a Profile ==

If you are already using a custom profile (or want to use mine), you can include 
others by FIXXME.

VoiceAttack does not execute configured startup commands for included profiles.  
Hence, you’ll have your own profile have one that in turn runs the included 
profiles’ startup commands. While you are doing that, you might as well set all 
settings here, centrally. Main advantage is that you can just upgrade the 
included profiles to newer versions without losing your settings.

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
