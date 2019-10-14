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
commands and some buttons that are now being handled by VA to lots of EDDI event 
handlers.

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
Windows that does not need Python installed. Just download the zip file from the 
release page and extract into your VoiceAttack directory.

## Settings ##

Because Elite’s keyboard handling is … weird you’ll have to set the key to use 
for pasting text into Elite:Dangerous. If you are using a “standard” QWERT[YZ] 
layout, you will have to change it back to `v`. It’s defaulting to `p` because 
that’s the key that is in `v`’s place on my keyboard layout, Neo2.

For other settings, see the [Configuration Variables](#Configuration-Variables) 
section.

## Including the Profile In Another Profile ##

This is meant to be a standalone profile, including the others in this repo (and 
a couple more). It was never designed to be included into your existing profile. 
Nevertheless, it _should_ work properly if you follow some guide lines:

* Run the startup command. You will need to have a startup command in your 
  profile (= one that is run on profile loading) and call 
  `EliteDangerous.startup` by name from that one.
* Set configuration options. In the same startup command of yours, overwrite all 
  configuration variables you want changed; _after_ the `EliteDangerous.startup` 
  call. See [below](#Configuration-Variables).
* Make sure all EDDI events that EliteDangerous needs are correctly handled. For 
  all events used in EliteDangerous that you already have handlers for in your 
  profile, you’ll have to include a call to `EliteDangerous.<event name>`.  E.g. 
  for “EDDI Jumped”, call `EliteDangerous.EDDI Jumped` by name from your `((EDDI 
  Jumped))` command.
* Initialise the [bindED](https://forum.voiceattack.com/SMF?topic=564.0) plugin 
  correctly to read your Elite keybinds.

## Usage ##

### Chat Commands ###

There’s a bunch of commands in here to send certain things to chat. Unless 
stated otherwise, they will only work with the comms panel active, and you 
should be in the edit window ready to send. They will _not_ hit Enter on their 
own.

* `announce my services`: Gives a rundown of my refuel&repair services. 
  I usually post that in system chat coming into the waypoint systems of an 
  expedition.
* `clear [chat;text]`: Clears the chat window. Use from outside the comms panel.
* `[local;squad;system;wing] chat`: Puts you into the chosen chat channel.
* `paste text`: Pastes your clipboard into Elite. Works outside the comms panel 
  too, e.g. on the galaxy map inside the search field.
* `salute; oh seven`: Will put “o7” into the chat.
* `send fuel rats p.s.a.`: Sends a fuel rats PSA. Duh.
* `send high gravity p.s.a.`: Warns about (a) high gravity planet(s) being in 
  the system. I use that on expedition waypoints, too – if it applies.

### Events ###

The main point of this profile is to react to Elite’s journal events (powered by 
EDDI). Each of the events listed here will trigger a `((EDDI <event>))` command 
in VoiceAttack which in turn triggers `EliteDangerous.EDDI <event>` and all 
included profiles’ similar commands, e.g. `SpanshAttack.EDDI <event>`.

The actual `((EDDI <event>))` command will only be executed once by VoiceAttack, 
the first one it can find. Keeping it separate from the actual code to be run 
makes it easier to handle (multiple) included profiles.

#### Body Mapped ####

Announces remaining mapping candidates as given by EDDI.

#### Body scanned ####

Announces any interesting body traits found when scanning:

* scan data worth >300,000 cr (this translates to roughly 1.5 million cr 
  including bonuses)
* landable and >5 g
* semimajor axis <1.5 ls (only really interesting for planets, but there’s no 
  proper way to separate them from moons, sadly)
* radius <300 km

Feel free to suggest more!

#### Commander Continued ####

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

#### Fighter Launched ####

Orders your ship to hold position so it doesn’t chase after you immediately.

#### Jet Cone Boost ####

Sets your ship to full throttle immediately after you have supercharged.

#### Jumped ####

* zeroes throttle
* gets the system’s body count from EDSM (requires Python scripts)
* if you hvaen’t visited the systems, starts a discovery scan (see the discovery 
  scan command)

#### Liftoff ####

Retracts landing gear for you. Seriously, is there any occasion in which you 
_don’t_ immediately want to retract it after takeoff?

#### Low Fuel ####

Warns you when you reach 25% fuel.

#### Message Sent ####

Checks any message you send for a chat prefix and sends it to the proper chat 
window. Probably largely useless to you without modification.

* `.dt`: Actually doesn’t send anything, but runs the 
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
  including an extra warning at low fuel levels. (__Note__: This is currently 
  kind of bugged, not sure if it’s EDDI’s or Elite’s fault.)
* cooldown complete: Announces FSD cooldown if you are currently in normal 
  space.

#### Ship interdiction ####

Tells you when you are interdicted by a player. Is also supposed to target the 
interdictor automatically, but that randomly sometimes just doesn’t work. Yay!

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

### HOTAS Buttons ####

These commands are all bound to keys on my throttle to make them do different 
things on tapping and on holding.

* `docking request key`: Well, that one just sends a docking request using the 
  proper command. No secondary function.
* `FSS key`: Well … that one too just opens/closes FSS. And sets throttle to 0% 
  so you can enter it.
* `jump combo key`: Jumps on tap, executes the `jump` command on hold.
* `plot combo key`: Targets next system on route on tap, plots to the thing in 
  your clipboard via the galaxy map on hold.
* `sc combo key`: Goes into SC on tap, executes the `cruise` command on hold.

Main benefit of using the jump/cruise commands instead of the buttons would be 
automatically retracting everything.

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
* `open [current;] system on EDSM`: Opens your current system on EDSM in your 
  default browser.
* `open EDDI options; configure EDDI`: Opens the EDDI configuration window.
* `open [the;] miner’s tool`: Opens https://edtools.ddns.net/miner in your 
  default browser.
* `[start;stop] [EDISON;navigation]`: Hits `CTRL+ALT+E` which just so happens to 
  be the start/stop hotkey I have set in E.D.I.S.O.N.
* `[what’s;what is] left to [map;be mapped]`: Tells you which bodies EDDI thinks 
  are worth mapping in the system that you haven’t mapped yet.

### Ship Controls ###

Basically anything that is related to directly doing something with your ship.

* `[abort;cancel;stop] jumping`: Stops a currently charging FSD jump.
* `[buggy;exploration] power`: Sets your PD to 0/4/2 or 2/4/0 respectively. 
  Works in SRV too.
* `check next [star;hop;jump;system]`: Will quickly engage and disengage your 
  FSD to show you the star class of the currently targeted system in the info 
  window (top right).
* `[close;deploy;extend;open;retract;] […] [up;down;]`: Overly complicated 
  command to handle everything related to Cargo Scoop, Hard Points, Landing 
  Gear. You get the gist, I guess. Works in SRV too.
* `[disco;discovery scan]`: Executes a discovery scan. To work properly, you’ll 
  have to set the Discovery Scanner to your first fire group, secondary fire.
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
* `[super;] cruise [when ready;]`: Retracts everything, then jupms to SC. If 
  given “when ready” will wait for mass lock to clear and your FSD to cool down 
  first.

### SRV controls ###

Things revelant to your SRV, but not your ship.

* `[recall;dismiss] ship`; Recalls or dismisses ship. Currently does the same 
  thing regardless of the state of your ship. I wish it would be possible to 
  restrict it to doing one thing each, but that’s currently not possible sadly.
* `[toggle;enable;disable] drive assist`: Handles all your drive assist needs!

### Targeting ###

Well … targeting stuff, I guess. Not really sure why I made that it’s own 
category, but oh well :)

* `target <list of subsystems>`: NYI. Way too fiddly and buggy to be reliable.
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
* `[enter;leave] F S S`: Opnes/closes FSS.
* `galaxy map`: Opens the galaxy map.
* `[main;game] menu`: Opens the ESC menu.
* `plot course;[target;] next [waypoint;way point]`: Plots a course to the 
  system set in `~~system` or the one in your clipboard. The former way is 
  usually used by other commands to not interfere with your clipboard.
* `[relog;reset] to [open;solo]`: Relogs to Open or Solo mode, respectively.
* `set […] filter`: Sets a nav panel filter setting. See the command or just try 
  different things for what is possible. You need to clear filters and hover 
  over the filter button, then run this.
* `system map`: Opens the system map.
* `take [high res;] screenshot`: Takes a (high res) screenshot.
* `target […]`: Targets the given system on the galaxy map. There’s a bunch in 
  there, the list is easily extensible. Drop me a note if you want something 
  included.
* `toggle orbit lines`: Toggles the visibility of orbit lines.
* `[toggle;show;hide] interface`: Toggles the cockpit interface (CTRL+ALT+G). 
  Probably needs to be adjusted if you are not playing with Neo2 keyboard layout 
  :)

### Configuration Variables ###

These are set in `EliteDangerous.startup` and can be overriden from your profile 
if you have included EliteDangerous.

* `Elite.pasteKey` (string): the key used for pasting into Elite. On QWERTY this 
  is `v`. Default: `p`.
* `>announceMappingCandidates` (boolean): whether to announce mapping candidates 
  when they are scanned. Default: true.
* `>autoChangeFuelratsNick` (boolean): whether to change the FuelRats IRC 
  nickname automatically when changing commanders. Probably largely irrelevant 
  to you. Default: true.
* `>enableR2Rannouncements` (boolean): whether to announce planets worth mapping 
  when jumping into a known system. This is useful for doing some R2R on the 
  side. Default: false.
* `>targetSubsystem` (string): the default target subsystem. Unused. Default: 
  “drive”.
* `python.ScriptPath` (string): the path you have placed the compiled python 
  scripts in. Default: “{VA_DIR}\scripts” (the “scripts” folder in your 
  VoiceAttack installation directory).
* `>bindingsFile` (string): the bindings file bindED should use for your key 
  binds. Default: mine. You should really change this setting.
