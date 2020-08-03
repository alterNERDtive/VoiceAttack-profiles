# 3.0 (2020-08-03)

A bug fix release turned major version update. That’s something new.

The interesting changes are auto-updates and re-worked logging. Plus a bunch of 
fixes, as usual.

**Note**: Since 2.0.1 you are probably seeing a warning about the profiles 
having been created with a newer VoiceAttack version. I am currently alpha 
testing a VoiceAttack build containing a bug fix. You should be able to safely 
ignore that message :)

**BREAKING CHANGE IN ALL PROFILES**:

The profiles now contain flags that will make them overwrite the current version 
if you import them, streamlining the update process and making it work like 
I expected it to work in the first place.

In order for this to work properly in the future, you need to do the following 
steps _once_ when updating to this version:

1. Keep track of which of your profiles import any of my profiles and which 
   ones.
1. Delete all of my profiles from VoiceAttack (`EliteDangerous`, `RatAttack`, 
   `SpanshAttack`, `StreamAttack`). Technically also `SealAttack`, but since 
   I am not currently including this you might want to keep it around if you are 
   actually using it.
1. Import the updated profiles, either manually or in the way described under 
   “Updating” in the README.
1. Fix all profiles that include the updated profiles. You are going to have to 
   re-add the includes (“Edit Profile” → “Options” → “Include commands from
   other profiles”).
1. If you haven’t yet created your own custom profile and imported mine there, 
   a) do that now for the future and b) you’ll have to re-configure any settings 
   you have changed either through voice commands or fiddling with commands 
   manually (don’t do the latter, please).

You will only have to do this once. In the future you will also be able to make 
use of the new, improved and basically automated update process described in the 
README. Yay!

## EliteDangerous 7.0

### Added

* Parameterized `EliteDangerous.openSystemOnEdsm` command
* `EliteDangerous.enableAutoUpdateCheck` setting. Invoke `[enable;disable] auto 
  update check` to change.

### Changed

* Is now set up to overwrite an old version on import. See “Updating” 
  instructions in the README.
* All logging to the VoiceAttack event log is now properly encapsuled. See the 
  docs for instructions on how to enable logging.

### Fixed

* `Route details event` now only firing when it should, not on automated 
  triggers like handing in missions (#46)
* `Route details event` now with correct escaping of Cottle syntax (no more “P 
  \<system\>” TTS)
* TTS now handles systems containing `'` correctly. (#49)

### Removed

* `((EDDI Commander continued))` since that event is only used in StreamAttack 
  anyway, so doesn’t need multiple handlers loaded. Shouldn’t really change 
  anything.
* `EliteDangerous.openCurrentSystemOnEdsm`, 
  `EliteDangerous.openCopiedSystemOnEdsm`

## RatAttack 5.0

### Changed

* Is now set up to overwrite an old version on import. See “Updating” 
  instructions in the README.
* All logging to the VoiceAttack event log is now properly encapsuled. See the 
  docs for instructions on how to enable logging.

### Fixed

* `RatAttack.enableRatDuty` has TTS again.
* TTS now handles systems containing `'` correctly. (#49)

## SealAttack

**Has been removed for now.**

You can still your current version around if you want to. It’s based on a very 
old version of RatAttack and the Seals IRC is still coming “when it’s done”, so 
currently I don’t see a good reason to keep it.

When the Seals IRC becomes a thing, I’ll reintroduce this profile, based on the 
then-current version of RatAttack. That’s less work than trying to get current 
SealAttack up to date

## SpanshAttack 6.0

### Changed

* Is now set up to overwrite an old version on import. See “Updating” 
  instructions in the README.
* All logging to the VoiceAttack event log is now properly encapsuled. See the 
  docs for instructions on how to enable logging.

### Fixed

* TTS now handles systems containing `'` correctly. (#49)

## StreamAttack 1.0

### Changed

* Is now set up to overwrite an old version on import. See “Updating” 
  instructions in the README.
* All logging to the VoiceAttack event log is now properly encapsuled. See the 
  docs for instructions on how to enable logging.

### Fixed

* TTS now handles systems containing `'` correctly. (#49)

-----

# 2.0.1 (2020-07-21)

## EliteDangerous 6.0.1

### Fixed

* `Carrier jump request` event now has TTS again.
* (Hopefully) fixed timing on the carrier pre-lockdown warnings.
* `Carrier jump engaged` TTS is now working properly again.
* `how many <thing> do i have` working properly now? again? W/e, works.

## RatAttack 4.0.1

### Fixed

* `distance to …` commands will now report about invalid system names instead of 
  silently failing.

-----

# 2.0 (2020-07-18)

This is a HUGE one! :D

You now no longer have to fiddle with configuration options in the various 
`startup` commands. Instead you can set everything with voice commands! See the 
`docs/` and the new `_configuration` command sections of the individual profiles 
for details.

Settings will only be saved in the profile you have selected when changing them, 
but be preserved if you switch around.

This also means that importing the profiles and updating them has become easier 
on you, the user. The README has been updated with the new and improved 
installation, update and customization process.

**Important**: I’ve made a lot of changes that _might_ break stuff if you just 
import into/over your existing profiles. Please _delete_ (or rename) the ones 
you currently have and import everything from scratch. Make sure to keep track 
of configuration/commands you have changed so you can get a similar state back 
after upgrading.

**Importanter**: The profiles are now implementing features introduced in 
VoiceAttack 1.8.6. *You’ll __have__ to upgrade to __VoiceAttack 1.8.6__ to use 
them*.

* Updated to `elite-scripts` 0.6.

## EliteDangerous 6.0

### Added

* Configuration via voice commands!
* `Low Fuel` event will now tell you how many jumps you have left, or an 
  estimate for your remaining range if it’s less than a single full range jump.
* The threshold for stations to be considered outdated is now a configuration 
  option.
* Announcement of missing bodies on EDSM and outdated station data is now 
  a configuration option.
* `reload bindings` now has TTS feedback.
* Old station search now also writes results to the log window.
* New `open spansh` command.
* Carrier lockdown announcements at 10, 5 and 2 minutes left.

### Changed

* Configuration variables have been renamed to fit the global scheme of 
  `Profile.variable`. If you have overwritten any, you’ll have to re-change 
  them. You’ll have to do that anyway though because of voice command 
  configuration :)
* The Elite client is now targeted by process name instead of window title. 
  Should technically be faster, but probably won’t make much of a difference.
* System/body names are now passed to EDDI using the `{P("<string>")}` literal 
  which should improve pronunciation in most cases.
* No longer targeting the Elite client profile-wide, instead on command level. 
  Should be more accurate and alleviate potential issues with focus switching, 
  and should be more robust when importing the profile.
* Renamed `EDDI Events` category to `EDDI events` for consistency. Shouldn’t 
  bother you at all unless you have fiddled with the commands in there.
* Restricted a bunch of commands to only execute if they are not already 
  running.
* Focusing the Elite client for event commands that need to send keys should be 
  more sturdy now.
* Bumped required detection confidence for the `FSS` and `cruise` commands to 85 
  since they were misfiring a lot. If you have any problems please file an 
  issue.

### Removed

* `fix window dimensions` command. Too specific to my setup.

## RatAttack 4.0

### Added

* Configuration via voice commands!
* `RatAttack.setRatDuty` command. Accepts a boolean parameter. 
  `RatAttack.[enable;disable]RatDuty` still exist for calling from the command 
  line.
* Will now inform you if an incoming rat case is within the permit locked 
  starter area.

### Changed

* **Breaking Change**: Passing RATSIGNALs to VoiceAttack is no longer done 
  through the windows clipboard, and instead through a file. See `docs/` for 
  details. You have to change your IRC client’s configuration.
* System/body names are now passed to EDDI using the `{P("<string>")}` literal 
  which should improve pronunciation in most cases.
* Converted all command category names to lower case. Shouldn’t bother you at 
  all unless you have fiddled with the commands.

## SealAttack 0.2

### Added

* Configuration via voice commands! As this profile is … a work in progress, not 
  everything is implemented yet.

## SpanshAttack 5.0

### Added

* Configuration via voice commands!
* `reload bindings` now has TTS feedback.
* Renamed `EDDI Events` category to `EDDI events` for consistency. Shouldn’t 
  bother you at all unless you have fiddled with the commands in there.
* Restricted a bunch of commands to only execute if they are not already 
  running.

### Changed

* The Elite client is now targeted by process name instead of window title. 
  Should technically be faster, but probably won’t make much of a difference.
* System/body names are now passed to EDDI using the `{P("<string>")}` literal 
  which should improve pronunciation in most cases.
* No longer targeting the Elite client profile-wide, instead on command level. 
  Should be more accurate and alleviate potential issues with focus switching, 
  and should be more robust when importing the profile.

## StreamAttack 0.3

### Added

* Configuration via voice commands! Not a lot to configure here though.

### Changed

* Restricted `distance to jump target` command to a single running instance.

-----

# 1.5.1 (2020-07-05)

This is just a minor bug fix release.

## EliteDangerous 5.2.1

### Fixed

* `EDDI Carrier cooldown` will no longer call your carrier “Not set”.

-----

# 1.5 (2020-07-05)

Changed Changelog format. Should be even clearer now at a glance!

See [KeepAChangelog](https://keepachangelog.com/en/1.0.0/).

## EliteDangerous 5.2

### Added

* Support for EDDI’s new `Carrier` events!
* `>enableCarrierAnnouncements` configuration variable. Default: true.

### Removed

* `check next [star;hop;jump;system]` command. Only works if the target system 
  is in EDSM which makes the entire thing kind of pointless.
* A bunch of personal chat macros.
* `target <subsystem>` command. NYI, haven’t found a way to make it work 
  properly.

## RatAttack 3.1.1

### Fixed

* `distance to rat case […]` commands now actually work.

### Changed

* Now setting EDDI to quiet mode even if the profile was loaded before plugin 
  initialization had been completed.

## SealAttack 0.1.1

### Changed

* Now setting EDDI to quiet mode even if the profile was loaded before plugin 
  initialization had been completed.

## SpanshAttack 4.0

### Added

* If the target system is not in the router’s data base but has already been 
  uploaded to EDSM, it will now pull target coordinates from EDDI instead of 
  making the user input them manually. This at least works for systems that are 
  in EDSM, but haven’t been sent through EDDN.
* If EDSM doesn’t know about the system either, it will now try getting 
  coordinates through EDTS. EDTS calculates them based on the procedurally 
  generated system name. Only if this fails will you have to input coordinates 
  manually now!
* Re-plotting a route is now aware of the actual target ≠ plot target conundrum. 
  It will remember the plot target and re-use it instead of starting the process 
  of finding a plottable system again.

### Changed

* Now setting EDDI to quiet mode even if the profile was loaded before plugin 
  initialization had been completed.
* `SpanshAttack.getNextNeutronWaypoint` reworked. The old one is now available 
  as `SpanshAttack.copyNextNeutronWaypoint`.
* If the target system is not in the database, it will now acknowledge that the 
  system you plot to is not the actual target system and target the latter 
  instead on the last way point.
* Trip time will no longer be reset when re-plotting, nor when plotting a new 
  route while a route is currently active. If you want to reset, make sure to 
  `clear` the current route first.

## StreamAttack 0.2.1

### Added

* Now updates your location on `Carrier jumped` events. Those only fire when you 
  are on board a carrier that is jumping, and it clearly changes your location.

### Changed

* Now setting EDDI to quiet mode even if the profile was loaded before plugin 
  initialization had been completed.

-----

# v1.4 (2020-06-19)

This is basically just a bug fix release, but I happened to have one new feature 
ready, so it’s in, too!

I managed to have leftover debug output in the version of `elite-scripts` 
published with v1.3. You might have noticed after jumping into a system that had 
a station with data in EDDN >1 year old. Apologies.

* New, fixed version of `elite-scripts` included.

## EliteDangerous v5.1

* Added `shut up EDDI` command. Immediately makes EDDI stop talking. Might come 
  in handy at some point.
* Added a bunch of `target nearest <thing>` commands powered by EDDI’s `route()` 
  function. See the docs for details.
* Added new “Navigation” section. This includes the aforementioned new commands 
  and a bunch of old ones that fit there better than into “Misc”.

# v1.3 (2020-06-14)

## EliteDangerous v5.0

You don’t need to find out your bindings file anymore! It will now automatically 
be read from the file the game reads it from, too.

* Removed the HOTAS buttons section. They were pretty personal to me and not 
  very useful to other people anyway. They are in their own profile now without 
  polluting the public one.
* Fixed nav panel filter commands for the addition of fleet carriers.
* Fixed the `EDDI Docked` event for the new interface.
* There are a couple engineering/materials related things now. See the docs for 
  more info.
* Added `EDDI Entered normal space` event. Will now automatically throttle you 
  to 0 upon exiting SC if `>hyperSpaceDethrottle` is set.
* Replaced `EDDI Commander continued` with `EDDI commander loading` so it only 
  runs when the game starts, not on relogging. If you don’t know what that 
  means, it probably won’t affect you :)
* Added `restart from Desktop` command. Useful for farming HGEs. You might want 
  to set `>delays.quitToDesktop` according to your needs. Restarting the game 
  also only works on 1080p currently. Might change in the future.
* Updated miner’s tool URL.
* Added `open Inara` and `open materials finder` commands.

## RatAttack v3.1

* Added `RatAttack.[enable;disable]RatDuty` commands that can be invoked 
  externally more easily.

## SpanshAttack v3.1

Now also reads the correct bindings file (automatically). Didn’t read any 
bindings before even though it needs them …

## StreamAttack v0.2

* Now updates your location when dropping from SC.
* Fixed some copypasta errors in the documentation.

# v1.2 (2020-03-09)

## EliteDangerous v4.0

The big new feature is being able to pull data from Spansh’s database in 
addition to EDSM. That allows checking for stations that haven’t had their data 
updated in ages. Probably not that interesting to you, but I like keeping things 
up to date in EDDN.

* EDDI events will now focus Elite before sending keyboard inputs. That should 
  help with inputs being swallowed. For non-event commands I’m just going to 
  assume that Elite is already focused :)
* Changed logic for “worthwhile” bodies while scanning a system. It will now 
  alert you to terraformable bodies, Earth-like, Water and Ammonia Worlds. It 
  will no longer check against a scan value.
* Mapping a body will now announce the expected payout.
* Added `>hyperspaceDethrottle` setting. Basically does the same thing as the SC 
  Assist thing, and has been doing it forever. Now defaults to false.
* Added a call to `StreamAttack.startup` to `EliteDangerous.startup`. Duh. And 
  calls to the EDDI event handlers.
* Fixed the docking request command if you currently have a target (and 
  subsequently a forth “target” tab in your left hand panel).
* Upon jumping into a (populated) system, will now check Spansh for stations in 
  the system with outdated data (>1 yo) and announce those.

## RatAttack v3.0

* Added `open [rat;] dispatch board` command. Opens the web dispatch board in 
  your default browser.
* Added proper handling for multiple ratsignals hitting at once. That’s mainly 
  an IRC client config thing,
  [see the docs](docs/RatAttack.md#getting-case-data-from-irc).
* Renamed `RatAttack.getInfoFromRatsignal` to 
  `RatAttack.announceCaseFromRatsignal`. Removed the “open case?” voice input 
  prompt.
* Added new `RatAttack.getInfoFromRatsignal` command that will only add 
  a ratsignal’s data to the internal case list, but not announce the case.

## SpanshAttack v3.0

This now also pulls data from Spansh’s database. In this case to make sure your 
target system is actually in there for plotting. If not, it will prompt you for 
coordinates and use the nearest one that is.

This means that you will now need to install my `elite-scripts` to run 
SpanshAttack. If you are using the profile package, you are doing that already 
and don’t have to do anything.

* Added `SpanshAttack.defaultToLadenRange` config option. If enabled, will pull 
  current range from EDDI before asking you to manually input a range for your 
  ship. Sadly that will be range with full cargo but _current_ fuel levels. So 
  make sure your tank is full. Will default to false for this very reason.
* Will now check if you actually have a current and target system set and abort 
  plotting a route if you e.g. aren’t logged into Elite or EDDI is throwing 
  a fit.
* As said above, SpanshAttack will now check for the target system being in the 
  router’s database or ask you to input target coordinates instead.

## StreamAttack v0.1

This profile will write some state data to files that can then be read e.g. by 
OBS for streaming shenanigans. Yes, other tools do that as well, but this one is 
mine ;)

[See the docs](docs/StreamAttack.md).

# v1.1 (2020-01-30)

The compiled Python scripts are now distributed as two single .exe files. You 
can (but don’t have to) delete the subfolders in `Python.scriptDir`.

## RatAttack v2.0

* `[current;] rat case details` changed: Now always has to include “rat” in the 
  command to pave the way for SealAttack arriving soon™. **This breaks 
  compatibilty with the old `[current;] case details` command.**

features:

* added option to auto-close on fuel+: Defaults to off. Enabled in the 
  EliteDangerous profile.
* added option to call jumps “left”
* added distance commands `distance to current rat case`/`distance to rat case 
  number [0..19]`: Both require a current version of the elite-scripts.
* added `call client offline` command

## SealAttack v0.1

VERY early alpha. Basically the only thing that works and is “finished” already 
is pasting ingame chat into Seals IRC. use `.sb <msg>` for \#Seal-Bob and `.rr
<msg>` for \#Repair-Requests.

Use at your own risk.

## SpanshAttack v2.2

features:

* added plot command “with custom range”: Instead of using the saved jump range 
  for your current ship, you can now plot a route with a custom range; e.g. if 
  you have temporarily changed modules around or have more/less cargo than usual 
  for this trip.

fixes:

* added info about EDDB to docs: Just a little tidbit about making sure the 
  target system is actually in the DB.

## EliteDangerous v3.1

features:

* added jumpTarget stuff: You can now set/clear a jump target. This will start 
  writing the distance to said target to `>jumpTargetFile` for inclusion e.g. in 
  OBS.  If SpanshAttack currently has a target system, jump target will default 
  to that. A manually set target will take precedence.
* added `distance [from;to] jump target` command
* added some quick links: Coriolis, EDSM main page, EDDB (incl. 
  station/system/faction search page). `open coriolis`, `open e d s m`, `open 
  e d d b [station;system;faction;] [search;]`.

fixes:

* refactored for better variable scoping
* changed `EDDI Body scanned` constraint: Now checks for being in SC instead of 
  just in FSS; will now include auto-scans of bodies, while still excluding nav 
  beacon scan data.
* logic around getting bodycount more sturdy: Now actually notices when the 
  script errors out for some reason. Probably still won’t notice when it’s flat 
  out missing, but hey, that’s PEBKAC for not using the profile package.
* fixed race condition in `EDDI ship fsd`: Weirdly that only started being an 
  issue in VA 1.8.
* updated for new controls setup: the HOTAS buttons have changed because my 
  setup has changed. Probably not really relevant for you.

# v1.0 (2019-12-04)

### single-file profile package

You can now install a single “profile package” for all the packages and related 
stuff (excluding required plugins)! That should make the entire setup process 
way less of a hassle. Future releases will just be a single file to import into 
VoiceAttack. See the README for more info.

Note that for this the default Python script path has changed! I recommend that 
you delete the now obsolote `scripts` folder in your VoiceAttack directory, or 
wherever you have installed the scripts instead. If you haven’t been using them, 
just ignore this paragraph.

### new profile format

VoiceAttack 1.8 has the compressed profile as default export format. The 
profiles will now come in this since I would forget to change the export format 
every time I export them anyway. This shouldn’t lead to any issues but please do 
open a bug if it does.

## RatAttack v1.0

* fixed first case announcement after starting VoiceAttack
* on opening a case, VoiceAttack will now first copy the target system to the 
  clipboard and then announce case details
* now only executes the “nearest commander” Python script if the incoming case 
  is actually for a platform you want to have cases announced for
* added `call wing pending` command

## SpanshAttack v2.1

* will now set neutron mode and target system _before_ getting ship range. This 
  avoids race conditions with targeting a system, executing the plot command, 
  then changing the targeted system.

## EliteDangerous v3.0

* added `[dis;]engage silent running` command

## elite-scripts 0.1.1

* fixed bug with wonky system names (including e.g. `+` or `[]`)

# v0.5 (2019-11-09)

## RatAttack v0.1

RatAttack is now a thing! :D

## SpanshAttack v2.0

* SpanshAttack will now disable EDDI’s speech responder by default. To get it 
  back you will have to manually re-enable it after including 
  `SpanshAttack.startup`. See docs for more info.
* Fixed the target announcement when plotting. Will now properly use the plot 
  target instead of the system that you have targeted at the moment of the 
  announcement.
* Requirements are now listed in the documentation.
* Fixed auto jump on scooping. Now only queues a jump _once_ not once per 
  “refuelled” event (fires every 5s)

## EliteDangerous v2.0.2

* Scanning a body will now tell you if it is in a fast orbit (less than 6h).
* Fixed the `sentToX` commands to do a CTRL+A before pasting text. This will 
  prevent garbled things to be send when you have already typed some text into 
  the edit box.
* Now automatically toggles FlightAssist off on takeoff/undock. Set 
  `>FlightAssistOff` to false in `EliteDangerous.startup` to disable.
* Added `[start;stop] [firing;mining]`. This command will start/stop hold down 
  the primary fire button for you; in case of mining it will also deploy your 
  cargo scoop.
* Fixed the discovery scan event to only tell you about differences with EDSM 
  when EDSM knows _fewer_ bodies (there are some issues with duplicate entries 
  in EDSM, e.g. in Dromi)

# v0.4.1 (2019-10-14)

This is a bug fix release (as the version number indicates). Mainly for changes 
in EDDI 3.5.0-b1. If you haven’t updated to the beta, DO NOT update to this 
version yet!

There has also been a minor update to `elite-scripts` – this is not relevant 
yet, so you do not have to download the new archive.

## SpanshAttack v1.3.1

* fixed jump on scooping to no longer conflict with an already charging jump

## EliteDangerous v2.0.1

* fixed lights off command for SRV, will no longer have a loop on race condition
* fixed discovery scan event for new EDDI variable naming
* fixed srv lauched event to properly turn off the lights

# v0.4 (2019-10-12)

## SpanshAttack v1.3

* plotting a route now aborts if no jump range was given
* added option to clear an active neutron route on closing the game client 
  (default: on)
* added option to auto jump after fuel scooping (default: on)
* fixed `SpanshAttack.announceTripTime` saying “1 hours” and “1 minutes” (#13)

## EliteDangerous v2.0

* added documentation! (#11)
* fixed `EliteDangerous.jumpToSuperCruise` to no longer drop you from SC when 
  called while in SC
* fixed `relog to [open;solo]` command for new menu structure
* added `distance [from;to] [home;the center;Beagle Point;Colonia]` command 
  (requires python scripts, see docs)
* now compares current system bodies to the bodies found on EDSM and gives 
  feedback if there’s a discrepancy (and you should FSS the entire system to 
  update EDSM) (requires python scripts, see doc)
* added compatibility for SpanshAttack’s clear on shutdown

# v0.3 (2019-09-22)

## SpanshAttack v1.2

* improved trip time announcements
* trip time is now also written to the VoiceAttack log

## EliteDangerous v1.0

Should be usable by other people now without too much hassle. If you run into 
problems, please hit me up on [Discord](https://discord.gg/mD6dAb) or file an 
issue here.

# v0.2 (2019-08-28)

## SpanshAttack v1.1

* added a command to announce (approximate) jumps left, see docs (#1)
* added command to announce the time spent on a trip and configuration option to 
  automatically tell it after completing it (#3)
* fixed a race condition with waypoint pasting into the galaxy map (#2)
* fixed the list of jumps left to announce automatically not working correctly 
  in all cases (#4)

## EliteDangerous v… don’t even ask

Added EliteDangerous profile in its current state. It’s in no way sanitised or 
advised for general use, but feel free to dig around and draw inspiration from 
it. Soon™ it will be published in a more usable state, but I wanted to get it 
out there for reference.

# v0.1 (2019-08-06)

## SpanshAttack v1.0

* initial release
* expect bugs
* please help
