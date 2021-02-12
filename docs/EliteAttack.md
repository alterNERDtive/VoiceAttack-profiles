# EliteAttack

This is my personal VoiceAttack profile for Elite: Dangerous. It started out 
ages ago as a modification of [MalicVR’s public 
profile](https://forums.frontier.co.uk/threads/malics-voice-attack-profile-for-vr.351050/), 
then looked less and less and less like that and I added and cleaned up a lot of 
things while removing the stuff I didn’t use anyway. By now it would have 
probably been simpler to start from scratch.

Funnily enough it has grown to rely way more on events provided by
[EDDI](https://github.com/EDCD/EDDI) than actual voice commands.

## Chat Commands

These commands will only work with the comms panel active, and you should be in
the edit window ready to send. They will _not_ hit Enter on their own.

* `clear [chat;text]`: Clears the chat window. Use from outside the comms panel.
* `[local;squad;system;wing] chat`: Puts you into the chosen chat channel.
* `salute; oh seven`: Will put “o7” into the chat.

## Engineering/Materials

* `how many [<g5 manufactured materials list>] do i have`: Tells you how many of 
  the given g5 manufactured material you currently have on board. I’ve 
  restricted it to just those to not spam speech recognition with too many 
  phrases, and because those are the ones I usually want to know while jumping 
  around the bubble and having an eye on any HGE that might be around.
* `[launch;open] e d engineer`: Opens the ED Engineer tool.
* `what [mats;materials] do i need?`: Runs the EDDI responder that tells you 
  which materials are below wanted threshold. Needs setting those first. Gets 
  very spammy if you do it for all of them; personally I only set them for g5 
  manufactured, so I can quickly check if it’s worth looking for HGE in a system 
  I’m in.

## Navigation

There are so many navigation-focused commands now, they deserve there own 
category. Basically anything that helps you plot anywhere. A lot of those are 
powered by awesome EDDI so I don’t have to do the work myself!

* `distance [from;to] [Sol;home;the center;Beagle Point;Colonia]`: Gives you the
  current distance to the given POI.
* `[find;target] nearest [encoded;manufactured;raw] material trader`: Targets 
  the nearest respective material trader.
* `[find;target] nearest [fuel;scoopable] star`: Targets the nearest scoopable
  star.
* `[find;target] nearest [guardian;human] tech broker`: Targets the nearest
  respective tech broker.
* `[find;target] nearest [interstellar factor;mission system;scoopable star]`:
  Well, you know the drill by now.
* `[find;target] nearest mission system`: Targets the nearest system that has a
  mission target.
* `how many jumps left`: Announces the jumps left on a neutron trip (requires
  SpanshAttack) or a course plotted via the galaxy map.
* `plot course;[target;] next [waypoint;way point]`: Plots a course to the text
  in your clipboard.
* `target [bug killer;colonia;dav’s hope;explorer’s anchorage;jackson's lighthouse;jameson’s cobra;robigo;shinrarta dezhra;sagittarius a*;shinrarta;sothis]`:
  Targets the given system on the galaxy map.
* `[where’s;where is] my landing pad`: Will tell you the location of your
  assigned landing pad on a starport.

## Ship Controls

Basically anything that is related to directly doing something with your ship.

* `[abort;cancel;stop] jumping`: Stops a currently charging FSD jump.
* `[half;] power to [engines;shields;systems;weapons]`: Sets pips to 6/3/3
  (`half`) or 8/2/2 towards the given capacitor.
* `[balanced;half;] power to [engines;shields;systems;weapons] [and engines;and shields;and systems;and weapons;]`:
  Sets pips to 6/6/0 (balanced), 5/5/2 (half) or 8/4/0 towards the given capacitors.
* `[close;deploy;extend;open;retract;] [cargo scoop;hard points; landing gear] [up;down;]`:
  Overly complicated command to handle everything related to Cargo Scoop, Hard
  Points, Landing Gear. You get the gist, I guess. Works in SRV too.
* `[dis;]engage silent running`: Handles silent running.
* `[disco;discovery scan]`: Executes a discovery scan. Expects the Discovery
  Scanner in your first fire group, secondary fire. [You can change
  that](/configuration/EliteAttack/#settings).
* `[dis;]engage silent running`: Turns silent running on and off.
* `[head;spot;] lights [on;off]`: Turns your lights on and off. Works in SRV 
  too, kinda; turning lights off there relies on the state updating fast enough, 
  which sometimes leads to weird results.
* `[jump;engage;get me out] [and scan;] [when ready;]`: Retracts everything that
  might be protruding from your ship, then jumps to the next system. If the FSD
  isn’t charging within 1s, it gets you into SC instead (e.g. if your target is
  obstructed). If given “and scan” runs a discovery scan. If given “when ready”
  waits for mass lock to clear, your FSD to cool down and you to leave scoop
  range before jumping.
* `night vision [on;off]`: Toggles your night vision on/off. Works in SRV too.
* `rapid fire lights`: Flashes your lights 5 times in a row.
* `retract [all;everything]`: Retracts, well, everything.
* `[start;stop] [firing;mining]`: Starts/stops holding down primary fire. Mostly 
  useful when mining. When triggered with “mining”, also deploys the cargo 
  scoop.
* `[super;] cruise [when ready;]`: Retracts everything, then jumps to SC. If 
  given “when ready” will wait for mass lock to clear and your FSD to cool down 
  first.

## SRV controls

Things relevant to your SRV, but not your ship.

* `[recall;dismiss] ship`: Recalls or dismisses ship. Currently does the same 
  thing regardless of the state of your ship. I wish it would be possible to 
  restrict it to doing one thing each, but that’s currently not possible sadly.
* `[toggle;enable;disable] drive assist`: Handles all your drive assist needs!

## Targeting

Well … targeting stuff, I guess. Not really sure why I made that it’s own 
category, but oh well :)

* `target the [drive;drives;power plant;frame shift drive;f s d;shield
  generator]`:* 
  Targets the given submodule on your current target, or your next target if you
  don’t have one currently. Does not persist between targets.
* `clear sub [module;system] target`: Clears the current submodule target.
* `target next system`: Selects the next system on your route.
* `target wing man [1;2;3]`: Targets your wingmen.
* `target’s target`: Targets your target’s target (only works on wingmen).
* `wing man [1;2;3] target`: Targets your wingmen’s target.
* `wing man nav lock`: Toggles wing man nav lock on the selected wing member.

## UI Commands

Everything handling stuff that’s not related to controlling your ship, but 
manipulating some UI element(s).

* `[enter;leave] F S S`: Opens/closes FSS.
* `[main;game] menu`: Opens the ESC menu.
* `[relog;reset] to [open;solo]`: Relogs to Open or Solo mode, respectively.
* `controls options`: Opens the controls options menu.
* `docking request;request dock[ing;]`: Sends a docking request.
* `galaxy map`: Opens the galaxy map.
* `restart from Desktop`: Quits the game and restarts from an open launcher by 
  clicking the play button.
* `set [default;star;station;settlement;signal sources;anomaly;unknown;system] filter`: 
  Sets a nav panel filter setting. See the command or just try different things
  for what is possible. You need to clear filters and hover over the filter
  button, then run this.
* `system map`: Opens the system map.
* `take [high res;] screenshot`: Takes a (high res) screenshot.
* `toggle orbit lines`: Toggles the visibility of orbit lines.

## Miscellaneous

The commands in here do random more or less useful things.

* `[are there any;] outdated stations [in this system;]`: Runs an on-demand
  check for outdated stations in the current system.
* `[bodies;what’s;what is] left to [map;be mapped;scan]`: Tells you which bodies 
  EDDI thinks are worth mapping in the system that you haven’t mapped yet.
* `copy current system`: Copies the current system name into the clipboard.
* `open [current;] system on EDSM`: Opens your current system on EDSM in your 
  default browser.
* `open copied system on EDSM`: Opens the system in your clipboard on EDSM in 
  your default browser.
* `open coriolis`: Opens Coriolis in your default browser.
* `open e d d b [station;system;faction;] [search;]`: Opens EDDB in your default 
  browser.
* `open e d s m`: Opens EDSM in your default browser.
* `open inara`: Opens Inara in your default browser.
* `open materials finder`: Opens EDTutorials’ materials finder in your default 
  browser.
* `open miner’s tool`: Opens https://edtools.cc/miner in your default browser.
* `open spansh`: Opens https://spansh.uk in your default browser.

## Events

### Body Mapped

Announces an estimate for high-value bodies’ payouts and the remaining mapping
candidates in the system as given by EDDI.

### Body scanned

Announces any interesting body traits found when scanning:

* terraformable
* Earth-like World, Ammonia World or Water World
* landable and >5 g
* semimajor axis <1.5 ls (only really interesting for planets, but there’s no 
  proper way to separate them from moons, sadly)
* radius <300 km

Feel free to suggest more!

### Carrier Cooldown

Tells you when your carrier is able to do its next jump.

### Carrier Jump Engaged

This event fires when your carrier jumps but you are _not_ docked at it. It 
provides way less information than the `Carrier Jumped` event, but hey, I don’t 
use most of it anyway. Basically just calls `Carrier Jumped`.

### Carrier Jump Request

Announces the system and body your carrier has just been scheduled to jump to. 
Use this to double check ingame information; I’ve had my carrier accept a body 
as jump target, but then end up around the star. This _might_ give you a heads 
up on that.

Also starts a command queue to give you advance warnings on carrier lockdown at
-10, -5 and -2 minutes.

### Carrier Jumped

Announces system and body your carrier has just jumped to.

### Carrier Pads Locked

Announces your carrier’s lockdown procedures.

### Discovery Scan

Announces the number of bodies (and non-body signals) found in the system. Also 
compares the number of bodies to the amount reported by EDSM (requires Python 
scripts).

### Docked

Automatically refuels, repairs, optionally rearms, then gets your ship into the
hangar and opens station services.

### Docking Denied

Tells you the reason for docking denial.

### Entered Normal Space

Throttles to 0 upon dropping from SC, if the hyperspace dethrottle option is
enabled.

### Fighter Launched

Orders your ship to hold position so it doesn’t chase after you immediately.

### Jet Cone Boost

Sets your ship to full throttle immediately after you have supercharged.

### Jumped

* Zeroes throttle if the hyperspace dethrottle option is enabled.
* Gets the system’s body count from EDSM if that option is enabled.
* Gets stations with outdated data (by default: older than 1 year) from Spansh’s
  API. Again, if it is enabled.
* Starts a discovery scan if that is enabled.
* Last but not least tells you about planets worth scanning if you are on the
  R2R.

### Liftoff

Retracts landing gear for you. Seriously, is there any occasion in which you 
_don’t_ immediately want to retract it after takeoff?

### Low Fuel

Warns you when you reach 25% fuel. Also reports number of jumps you have left or 
the (rough) range you still have on the fumes left in your tank.

### Material Threshold

Warns you when a monitored material falls below it’s minimum stock level and
tells you when you reach your desired level or fill up.

You will have to set minimum and desired amounts in EDDI’s material monitor
options first for all materials you wish to be monitored.

### Next Jump

Gives you a jump count upon plotting a route using the galaxy map.

### Ship FSD

This event actually is several different events in one. Currently the following 
are handled:

* Charging: Warns you if you are jumping with less than 25% fuel.
* Cooldown complete: Announces FSD cooldown if you are currently in normal 
  space.

### Ship Interdicted

Tells you when you have been interdicted by a player. Is also supposed to target 
the interdictor automatically, but randomly sometimes just doesn’t work.  Yay!

### SRV Launched

Toggles SRV lights off after launching. Might not work if you drop particularly
far after deployment because it works off a timer. Conversely might take a
second to turn your lights off on a short drop and/or in high gravity.

### System Scan Complete

Lists you all bodies EDDI considers worth mapping in the current system.

### Undocked

Retracts landing gear for you. Seriously, is there any occasion in which you 
_don’t_ immediately want to retract it after takeoff?
