# EliteDangerous #

This is my personal VoiceAttack profile for Elite: Dangerous. It started out 
ages ago as a modification of [MalicVR’s public 
profile](https://forums.frontier.co.uk/threads/malics-voice-attack-profile-for-vr.351050/), 
then looked less and less and less like that and I added and cleaned up a lot of 
things while removing the stuff I didn’t use anyway. By now it would have 
probably been simpler to start from scratch.

Some of it has grown to a state that it might be useful to others in its own 
package, so I’ve separated the neutron jumping and Seals stuff into their own 
profiles.

The rest is a random conglomerate of all things VA and E:D; from various voice 
commands to lots of EDDI event handlers.

Speaking of EDDI; it has become in integral part of my Elite experience, 
especially the plethora of information it extracts from the game’s journal and 
presents to you via lots and lots of status variables and by firing various 
events that can then be handled through VA commands. It’s great. Check it out. 
(You might want to make it talk a lot less in it’s personality options, or 
disable the speech responder entirely like I have.)

## Requirements ##

In addition to the bindED and EDDI VoiceAttack plugins, this profiles needs my 
[Python elite-scripts](https://github.com/alterNERDtive/elite-scripts) to do 
everything properly. The release page here includes a compiled version for 
Windows that does not need Python installed. If you use the profile package from 
the release page, they will be installed automatically.

## Settings ##

Because Elite’s keyboard handling is … weird you’ll have to set the key to use 
for pasting text into Elite:Dangerous. If you are not using a “standard” 
QWERT[YZ] layout, you will have to change it back to the key that is physically 
in the place where `v` would be on QWERTY.

For other settings, see the [Configuration Variables](#Configuration-Variables) 
section.

## Including the Profile In Another Profile ##

This is meant to be a standalone profile, including the others in this repo (and 
a couple more). It was never designed to be included into your existing profile. 
Nevertheless, it _should_ work properly if you follow some guide lines:

* Run the startup command. You will need to have a startup command in your 
  profile (= one that is run on profile loading) and call 
  `EliteDangerous.startup` by name from that one.
* Make sure all EDDI events that EliteDangerous needs are correctly handled. For 
  all events used in EliteDangerous that you already have handlers for in your 
  profile, you’ll have to include a call to `EliteDangerous.<event name>`.  E.g.  
  for “EDDI Jumped”, call `EliteDangerous.EDDI Jumped` by name from your `((EDDI 
  Jumped))` command.

## Usage ##

### Chat Commands ###

There’s a bunch of commands in here to send certain things to chat. Unless 
stated otherwise, they will only work with the comms panel active, and you 
should be in the edit window ready to send. They will _not_ hit Enter on their 
own.

* `clear [chat;text]`: Clears the chat window. Use from outside the comms panel.
* `[local;squad;system;wing] chat`: Puts you into the chosen chat channel.
* `paste text`: Pastes your clipboard into Elite. Works outside the comms panel 
  too, e.g. on the galaxy map inside the search field.
* `salute; oh seven`: Will put “o7” into the chat.

### Engineering/Materials ###

* `how many [<g5 manufactured materials list>] do i have`: Tells you how many of 
  the given g5 manufactured material you currently have on board. I’ve 
  restricted it to just those to not spam speech recognition with too many 
  phrases, and because those are the ones I usually want to know while jumping 
  around the bubble and having an eye on any HGE that might be around.
* `open e d engineer`: Opens the ED Engineer tool.
* `what [mats;materials] do i need?`: Runs the EDDI responder that tells you 
  which materials are below wanted threshold. Needs setting those first. Gets 
  very spammy if you do it for all of them; personally I only set them for g5 
  manufactured, so I can quickly check if it’s worth looking for HGE in a system 
  I’m in.

### Events ###

The main point of this profile is to react to Elite’s journal events (powered by 
EDDI). Each of the events listed here will trigger a `((EDDI <event>))` command 
in VoiceAttack which in turn triggers `EliteDangerous.EDDI <event>` and all 
included profiles’ similar commands, e.g. `SpanshAttack.EDDI <event>`.

The actual `((EDDI <event>))` command will only be executed once by VoiceAttack, 
the first one it can find. Keeping it separate from the actual code to be run 
makes it easier to handle (multiple) included profiles.

If the command for an event will send key presses to Elite, it focuses the 
client window first to make sure they get registered properly. If it is not in 
focus, the first key press might be swallowed otherwise.

For other commands, the profile just assumes that you are actively doing 
something in game and it is already focused :)

#### Body Mapped ####

Announces an estimate for high-value bodies’ payouts and the remaining mapping 
candidates in the system as given by EDDI.

#### Body scanned ####

Announces any interesting body traits found when scanning:

* terraformable
* Earth-like World, Ammonia World or Water World
* landable and >5 g
* semimajor axis <1.5 ls (only really interesting for planets, but there’s no 
  proper way to separate them from moons, sadly)
* radius <300 km

Feel free to suggest more!

#### Carrier Cooldown ####

Tells you when your carrier is able to do its next jump. Technically only works 
if you were docked at it when it performed the jump, but I’m doing some 
behind-the-scenes magic if you weren’t :)

Might be slightly off (too early) in the latter case.

#### Carrier Jumped ####

Announces system and body your carrier has just jumped to.

#### Carrier Jump Engaged ####

This event fires when your carrier jumps but you are _not_ docked at it. It 
provides way less information than the `Carrier Jumped` event, but hey, I don’t 
use most of it anyway.

Basically just calls `Carrier Jumped` (and makes sure that a `Carrier cooldown` 
event is triggered manually at approximately the right time).

#### Carrier Jump Request ####

Announces the system and body your carrier has just been scheduled to jump to. 
Use this to double check ingame information; I’ve had my carrier accept a body 
as jump target, but then end up around the star. This _might_ give you a heads 
up on that.

#### Carrier Pads Locked ####

Announces your carriers lockdown procedures. This _might_ only work when you are 
docked (which would make it pretty useless). Feel free to open in issue if 
that’s the case.

#### Commander Loading ####

Auto-sets my nick in the FuelRats IRC. Probably largely irrelevant to you.

#### Discovery Scan ####

Announces the number of bodies (and non-body signals) found in the system. Also 
compares the number of bodies to the amount reported by EDSM (requires Python 
scripts).

Last but not least tells you about planets worth scanning if you are on the R2R.

#### Docked ####

Automatically gets your ship into the hangar and opens station services.

#### Docking Denied ####

Tells you the reason for docking denial.

#### Entered Normal Space ####

Throttles to 0 upon dropping from SC, if `EliteDangerous.hyperSpaceDethrottle` is set.

#### Fighter Launched ####

Orders your ship to hold position so it doesn’t chase after you immediately.

#### Jet Cone Boost ####

Sets your ship to full throttle immediately after you have supercharged.

#### Jumped ####

* zeroes throttle
* gets the system’s body count from EDSM (requires Python scripts)
* gets stations with outdated data (older than 1 year) from Spansh’s API
* if you haven’t visited the systems, starts a discovery scan (see the discovery 
  scan command)

#### Liftoff ####

Retracts landing gear for you. Seriously, is there any occasion in which you 
_don’t_ immediately want to retract it after takeoff?

#### Low Fuel ####

Warns you when you reach 25% fuel.

#### Material Threshold ####

Warns you when a monitored material falls below it’s minimum stock level. You 
will have to set a minimum desired amount in EDDI’s material monitor options 
first for all materials you wish to be monitored.

#### Message Sent ####

Checks any message you send for a chat prefix and sends it to the proper chat 
window. Probably largely useless to you without modification.

* `.nc`: Actually doesn’t send anything, but runs the 
  `RatAttack.announceNearestCMDR` command with the system given in the rest of 
  the message.
* `.dc`: Sends the message to the Discord window.
* `.tc`: Sends the message to my twitch channel window (IRC #alternerdtive).

There are similar event commands in RatAttack and SealAttack handling other chat 
windows.

#### Ship FSD ####

This event actually is several different events in one. Currently the following 
are handled:

* charging: Warns you if your target system’s main star is not scoopable, 
  including an extra warning at low fuel levels. (__Note__: This only works if 
  the target system is in EDSM. So it’s kind of useless for its intended use 
  (exploration) and probably going to be removed at some point.)
* cooldown complete: Announces FSD cooldown if you are currently in normal 
  space.

#### Ship interdicted ####

Tells you when you have been interdicted by a player. Is also supposed to target 
the interdictor automatically, but randomly sometimes just doesn’t work.  Yay!

#### Ship targeted ####

This currently doesn’t do anything. I was fiddling around with automatically 
targeting a certain module on ship targeting, but it was more hassle than I had 
thought.

#### Shutdown ####

Changes my nick back to default in FuelRats IRC. Probably largely useless to 
you. If you are using FuelRats IRC you need to change/deactivate this (see 
[below](#Configuration-Variables)) or you will start impersonating me by 
accident :)

#### SRV Launched ####

Toggles SRV lights off after launching. Might not work if you drop particularly 
far after deployment because it works off a timer. Conversely might take 
a second to turn your lights off on a short drop and/or in high gravity.

#### System Scan Complete ####

Lists you all bodies EDDI considers worth mapping in the current system.

#### Undocked ####

Retracts landing gear for you. Seriously, is there any occasion in which you 
_don’t_ immediately want to retract it after takeoff?

#### VA initialized ####

Fires when the EDDI VoiceAttack plugin is loaded. Makes sure that EDDI is set to 
quite mode even if the profile was loaded before plugin initialization had 
completed.

### Misc ###

The commands in here do random more or less useful things.

* `bind keys;reset key binds`: Reloads your key binds through the bindED plugin. 
  You should do that after changing anything in the controls options.
* `copy current system`: Copies the current system name into the clipboard.
* `distance [from;to] […]`: Tells you the distance from your current position to 
  the other thing you mentioned and is supported in the command. (requires 
  Python scripts)
* `do a barrow roll`: WHOOOOOOO!
* `fix window dimensions`: When you start the game in VR, it forces into 
  windowed mode with weird resolution. This changes it back. Hover the “PLAY” 
  entry in the main menu, then run this. Will need adjustment for different 
  graphics cards/drivers and the resolution you want.
* `neutron [jump;trip] time`: Shorter version of the same thing in SpanshAttack.
* `neutron jumps left`: Shorter version of the same thing in SpanshAttack.
* `open copied system on EDSM`: Opens the system in your clipboard on EDSM in 
  your default browser.
  * `open coriolis`: Opens Coriolis in your default browser.
* `open [current;] system on EDSM`: Opens your current system on EDSM in your 
  default browser.
* `open EDDI options; configure EDDI`: Opens the EDDI configuration window.
* `open e d d b [station;system;faction;] [search;]`: Opens EDDB in your default 
  browser.
* `open e d s m`: Opens EDSM in your default browser.
* `open inara`: Opens Inara in your default browser.
* `open materials finder`: Opens EDTutorials’ materials finder in your default 
  browser.
* `open miner’s tool`: Opens https://edtools.ddns.net/miner in your default 
  browser.
* `reload bindings`: Reloads your bindings for bindED.
* `shut up EDDI`: Immediately stops any ongoing (and queued) EDDI speech.
* `[start;stop] [EDISON;navigation]`: Hits `CTRL+ALT+E` which just so happens to 
  be the start/stop hotkey I have set in E.D.I.S.O.N.
* `[what’s;what is] left to [map;be mapped]`: Tells you which bodies EDDI thinks 
  are worth mapping in the system that you haven’t mapped yet.

### Navigation ###

There are so many navigation-focused commands now, they deserve there own 
category. Basically anything that helps you plot anywhere. A lot of those are 
powered by awesome EDDI so I don’t have to do the work myself!

* `plot course;[target;] next [waypoint;way point]`: Plots a course to the 
  system set in `~~system` or the one in your clipboard. The former way is 
  usually used by other commands to not interfere with your clipboard.
* `target nearest [encoded;manufactured;raw] material trader`: Targets the 
  nearest respective material trader.
* `target nearest [guardian;human] tech broker`: Targets the nearest respective 
  tech broker.
* `target nearest [interstellar factor;mission system;scoopable star]`: Well, 
  you know the drill by now.
* `target [<system>]`: Targets the given system on the galaxy map. There’s 
  a bunch in there, the list is easily extensible. Drop me a note if you want 
  something included.

### Ship Controls ###

Basically anything that is related to directly doing something with your ship.

* `[abort;cancel;stop] jumping`: Stops a currently charging FSD jump.
* `[buggy;exploration] power`: Sets your PD to 0/4/2 or 2/4/0 respectively. 
  Works in SRV too.
* `[close;deploy;extend;open;retract;] […] [up;down;]`: Overly complicated 
  command to handle everything related to Cargo Scoop, Hard Points, Landing 
  Gear. You get the gist, I guess. Works in SRV too.
* `[disco;discovery scan]`: Executes a discovery scan. To work properly, you’ll 
  have to set the Discovery Scanner to your first fire group, secondary fire.
* `[dis;]engage silent running`: Turns silent running on and off.
* `[head;spot;] lights [on;off]`: Turns your lights on and off. Works in SRV 
  too, kinda; turning lights off there relies on the state updating fast enough, 
  which sometimes leads to weird results.
* `[jump;engage;get me out;punch it chewie] [and scan;] [when ready;]`: Retracts 
  everything that might be protruding from your ship, then jumps to the next 
  system. If the FSD isn’t charging within 1s, it gets you into SC instead (e.g. 
  if your target is obstructed). If given “and scan” runs a discovery scan. If 
  given “when ready” waits for mass lock to clear, your FSD to cool down and you 
  to leave scoop range before jumping.
* `night vision [on;off]`: Toggles your night vision on/off. Works in SRV too.
* `power to [engines;shields;systems;weapons`: Sets 4 pips to the thing you told 
  it, 1 to the others.
* `rapid fire lights`: Flashes your lights 5 times in a row.
* `retract [all;everything]`: Retracts, well, everything.
* `[start;stop] [firing;mining]`: Starts/stops holding down primary fire. Mostly 
  useful when mining. When triggered with “mining”, also deploys the cargo 
  scoop.
* `[super;] cruise [when ready;]`: Retracts everything, then jumps to SC. If 
  given “when ready” will wait for mass lock to clear and your FSD to cool down 
  first.

### SRV controls ###

Things relevant to your SRV, but not your ship.

* `[recall;dismiss] ship`: Recalls or dismisses ship. Currently does the same 
  thing regardless of the state of your ship. I wish it would be possible to 
  restrict it to doing one thing each, but that’s currently not possible sadly.
* `[toggle;enable;disable] drive assist`: Handles all your drive assist needs!

### Targeting ###

Well … targeting stuff, I guess. Not really sure why I made that it’s own 
category, but oh well :)

* `target next system`: Selects the next system on your route.
* `target wing man [1;2;3]`: Targets your wing men.
* `target’s target`: Targets your target’s target.
* `wing man [1;2;3] target`: Targets your wing men’s target.
* `wing man nav lock`: Toggles wing man nav lock on the selected wing member.

### UI Commands ###

Everything handling stuff that’s not related to controlling your ship, but 
manipulating some UI element(s).

* `controls options`: Opens the controls options menu.
* `docking request;request dock[ing;]`: Sends a docking request.
* `[enter;leave] F S S`: Opens/closes FSS.
* `galaxy map`: Opens the galaxy map.
* `[main;game] menu`: Opens the ESC menu.
* `[relog;reset] to [open;solo]`: Relogs to Open or Solo mode, respectively.
* `restart from Desktop`: Quits the game and restarts from an open launcher by 
  clicking the play button.
* `set […] filter`: Sets a nav panel filter setting. See the command or just try 
  different things for what is possible. You need to clear filters and hover 
  over the filter button, then run this.
* `system map`: Opens the system map.
* `take [high res;] screenshot`: Takes a (high res) screenshot.
* `toggle orbit lines`: Toggles the visibility of orbit lines.
* `[toggle;show;hide] interface`: Toggles the cockpit interface (CTRL+ALT+G). 
  Probably needs to be adjusted if you are not playing with Neo2 keyboard layout 
  :)

### Configuration Variables ###

There are a bunch of configuration variables. You should not overwrite those 
manually, instead use the provided commands in the `_configuration` section!

Basically all the settings are available using the `customize settings` prefix, 
then saying `[enable;disable] <setting>` for on/off switches and `set <setting>` 
for text variables.

* `EDDI.quietMode` (boolean): whether or not to set EDDI to quite mode. Default: 
  true.
* `Elite.pasteKey` (string): the key used for pasting into Elite. On QWERTY this 
  is `v`. Default: `p`.
* `EliteDangerous.announceMappingCandidates` (boolean): whether to announce mapping candidates 
  when they are scanned. Default: true.
* `EliteDangerous.announcemeR2RMappingCandidates` (boolean): whether to announce 
  planets worth mapping when jumping into a known system.  This is useful for 
  doing some R2R on the side.  Default: false.
* `EliteDangerous.autoChangeFuelratsNick` (boolean): whether to change the FuelRats IRC 
  nickname automatically when changing commanders. Probably largely irrelevant 
  to you. Default: false.
* `EliteDangerous.enableCarrierAnnouncements` (boolean): whether or not to 
  process fleet carrier events. Default: true.
* `EliteDangerous.flightAssistOff` (boolean): whether to automatically toggle 
  FlightAssist off on liftoff. Default: true.
* `EliteDangerous.hyperspaceDethrottle` (boolean): same thing as the SC assist setting; if on, 
  will throttle to 0 automatically after jumping. Default: true.
* `EliteDangerous.jumpTargetFile` (string): the file the distance to the currently set jump 
  target will be written to.
* `EliteDangerous.targetSubsystem` (string): the default target subsystem. Unused. Default: 
  “drive”.
* `python.ScriptPath` (string): the path you have placed the compiled python 
  scripts in. Default: “{VA_DIR}\Sounds\scripts” (the “\Sounds\scripts” folder 
  in your VoiceAttack installation directory).

#### Delays / Pauses ####

Delays needed e.g. between key presses in UI navigation can vary wildly 
depending on your PC’s specs and configuration. Therefore they should be 
configurable, shouldn’t they?

So far those actually are:

* `EliteDangerous.delays.quitToDesktop`: Delay between quitting to desktop and hitting the 
  play button in the launcher.
