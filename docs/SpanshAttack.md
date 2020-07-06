# SpanshAttack #

This profile uses the 
[ED-NeutronRouter](https://github.com/sc-pulgan/ED-NeutronRouter) plugin to plot 
neutron jumps using [spansh](https://spansh.co.uk/plotter). It fully does 
everything you need from within the game and VoiceAttack, you won’t have to 
visit the site at any point.

## Requirements ##

In addition to VoiceAttack, you will need the following plugins to use this 
profile:

* [bindED](https://forum.voiceattack.com/smf/index.php?topic=564.0)
* [EDDI](https://github.com/EDCD/EDDI) installed as a VoiceAttack plugin
* [ED-NeutronRouter](https://github.com/sc-pulgan/ED-NeutronRouter)

### EDDI speech responder ###

For the convenience of people that have not been using EDDI in the past, 
SpanshAttack will deactivate the speech responder automatically to not clutter 
them with unwanted TTS.

If you are already an EDDI user and want to keep the default speech responder 
functionality, you will have to disable the `EDDI.quietMode` setting by running 
the `customize settings disable eddi quiet mode` command.

## Settings ##

Because Elite’s keyboard handling is … weird you’ll have to set the key to use 
for pasting text into Elite:Dangerous. If you are not using a “standard” 
QWERT[YZ] layout, you will have to change it back to the key that is physically 
in the place where `v` would be on QWERTY.

For other settings, see the [Configuration Variables](#Configuration-Variables) 
section.

The last “setting” in the not-so-strict sense of the word is the 
`SpanshAttack.getShipRange` command. Any ship listed in there will automatically 
have its jump range used instead of VA prompting you for it. Since, again, VA 
will execute the first matching command found, you can create this command in 
your own profile when you are using SpanshAttack by including it. You can 
override a saved range for your ship by using the `plot neutron 
[course;route;trip] with custom range` command.

The ED-NeutronRouter plugin is technically supposed to read the current jump 
range from EDDI; sadly a) it’s 
[bugged](https://github.com/sc-pulgan/ED-NeutronRouter/issues/3) right now, and 
b) EDDI is storing the _maximum_ distance for your ship instead of the current 
/ full on fuel one.

## Including the Profile ##

When including the profile, be sure to

* Run the startup command. You will need to have a startup command in your 
  profile (= one that is run on profile loading) and call `SpanshAttack.startup` 
  from that one.
* Make sure all EDDI events that SpanshAttack needs are correctly handled. For 
  all events used in SpanshAttack that you already have handlers for in your 
  profile, you’ll have to include a call to `SpanshAttack.<event name>`. E.g.  
  for “EDDI Jumped”, call `SpanshAttack.EDDI Jumped` by name from your `((EDDI 
  Jumped))` command.
* (Optional) Have a `SpanshAttack.getShipRange` command in your profile to 
  overwrite the default one with your ship’s ranges. See the default command for 
  pointers.

## Usage ##

### Plotting a Route ###

1. _Target_ the system you want to be routed to (target, do not plot to it).
1. Either exit the galaxy map or make sure you are on its first tab (or 
   auto-plotting will break).
1. Trigger the `SpanshAttack.plotRoute` command either by voice (`plot neutron 
   [course;route;trip] [with custom range;]`) or calling it from another 
   command.
1. Enter your ship’s jump range if prompted.
1. Wait for the route to be calculated. The command will automatically open the 
   galaxy map and jump to the first waypoint on your route. If you run into 
   weird behaviour, it’s probably because your target system is not in EDDB.
1. Either target the first waypoint or plot to it.
1. Start jumping!

#### Plotting to a System Unknown to the Neutron Router ####

The router can only plot a route to a system that is in its data base (obviously 
can also only give you way points that are). If your target system is not, there 
are several levels of fallback handling to find a system that is.

1. Check `Next system` coordinates provided by EDDI. If the system is in EDSM, 
   but has for some reason not been sent over EDDN to other sites including 
   Spansh we can get coordinates here.
1. If the system is not in EDSM check EDTS. It can calculate approximate 
   coordinates for a given procedurally generated system name.
1. If that fails prompt the user for input.
1. Query Spansh’ API for the closest system to these coordinates.
1. Plot a route to the closest system.

Generally you should almost never be asked to input coordinates manually. If 
EDTS provides coordinates with an accuracy that is worse than ±100 ly per axis, 
you will be prompetd to make sure you are going roughly to the right 
coordinates. You will find the system that is used for plotting, its coordinates 
and the accuracy in VoiceAttack’s log window.

### Neutron Jumping ###

With standard settings, just supercharge off a neutron cone. You should 
automatically be taken to the galaxy map with the next waypoint selected.

In case you have disabled auto-plotting to the next waypoint, manually invoke 
the `SpanshAttack.targetNextNeutronWaypoint` command by voice (`[target;] next 
neutron [waypoint; way point]` or calling it from another command.

Additionally, you can use the `SpanshAttack.copyNextNeutronWaypoint` 
/ `[get;copy] next neutron [waypoint;way point]` command to copy the next 
neutron waypoint to the clipboard.

#### Manual Re-Plot ####

Trigger the `SpanshAttack.replotRoute` command either by voice (`replot neutron 
[course;route;trip]`) or calling it from another command. This will start 
a re-plot of the current route with the same target system and jump range.

### Refueling ###

Whenever you refuel off a scoopable star, the profile will automatically 
throttle back up to 100% speed. Unless you have disabled it in your 
configuration, you will also automatically target the next system on your route 
and jump to it once you leave fuel scoop range.

### Clearing a Route ###

When you reach your target system, the neutron route will automatically be 
cleared. If you want to prematurely end your trip, call the 
`SpanshAttack.clearRoute` / `clear neutron [course;route;trip]` command.

## Other Commands ##

### Announcing Jumps Left ###

You can have VoiceAttack tell you the amount of jumps left on the current route 
by invoking `SpanshAttack.announceJumpsLeft` or saying
`how many [neutron;] jumps [are;] left?`.

**Note**: Because it’s pretty much impossible to calculate a 100% accurate value 
for the total jumps left, it will just tell you the jump count _from the current 
neutron waypoint_.

### Announce elapsed time on the trip ###

SpanshAttack keeps track of your start time, even if you have the option to time 
your trip turned off. This way you can get the time you’ve been jumping with the 
`SpanshAttack.announceTripTime` or
`how long have i been [jumping;on this trip;on this neutron trip]?` commands.

### Reload bindings ###

If you change any relevant bindings (e.g. the galaxy map key), you should run 
the `reload bindings` command to make sure that SpanshAttack presses the right 
thing for you.

Eh, just do it every time you edit your controls without re-starting 
VoiceAttack, just to be sure.

### Helper Functions ###

The profile contains a lot of helper functions that get called by the 
aforementioned commands. Have a look around, maybe learn something about 
VoiceAttack :)

## Exposed Variables ##

The following Variables are _global_ and thus readable (and writeable! Please 
don’t unless it’s a config variable …) from other profiles:

### Configuration Variables ###

There are a bunch of configuration variables. You should not overwrite those 
manually, instead use the provided commands in the `_configuration` section!

* `EDDI.quietMode` (boolean): whether or not to set EDDI to quite mode. Default: 
  true.
* `EDDI.useEddiForVoice` (boolean): whether to use EDDI over VA’s builtin `say` 
  command. Default: false.
* `Elite.pasteKey` (string): the key used for pasting into Elite. On QWERTY this 
  is `v`. Default: `v`.
* `SpanshAttack.timeTrip` (boolean): whether to automatically tell you at the 
  end of a trip how long it took you to get there. Default: false.
* `SpanshAttack.announceWaypoints` (boolean): whether to announce each waypoint 
  of the neutron route. Default: true.
* `SpanshAttack.announceJumpsLeft` (string): `;`-separated list of remaining 
  jumps to announce when said amounts are reached. Right now only works if they 
  are _exactly_ reached when supercharging off a neutron. Note the extra `;` at 
  the beginning and end of the string. Default: `;1;3;5;10;15;20;30;50;75;100;`
* `SpanshAttack.autoJumpAfterScooping` (boolean): whether to automatically jump 
  after fuel scooping (and moving out of scoop range). Default: true.
* `SpanshAttack.autoPlot` (boolean): whether to automatically plot to the next 
  waypoint on supercharging. Default: true.
* `SpanshAttack.clearOnShutdown` (boolean): whether or not to automatically 
  clear an active neutron route on Elite client shutdown. Default: true.
* `SpanshAttack.defaultToLadenRange` (boolean): whether or not to default to 
  your ship’s laden range (as reported by EDDI) instead of asking for user 
  input. Sadly it’s with _current_ fuel, not full. Setting a ship’s jump range 
  in the `SpanshAttack.getShipRange` command will still overrule this. Default: 
  false.
* `SpanshAttack.copyWaypointToClipboard` (boolean): whether to copy the next 
  waypoint into the Windows clipboard for use in other programs. Default: false.
* `python.scriptPath` (string): the path you put the Python scripts in.  
  Default: “{VA_DIR}\Sounds\scripts”.

### Other Variables ###

These variables can be used to get information about the current neutron route. 
Please do not set them manually and / or from outside the SpanshAttack profile.

* `SpanshAttack.plotSystem` (string): the system actually plotted towards using 
  the neutron router (onley used/set if the target system is not in the data 
  base)
* `SpanshAttack.targetSystem` (string): the target system for the current 
  neutron route
* `SpanshAttack.nextNeutronWaypoint` (string): the next waypoint on the current 
  neutron route
* `SpanshAttack.neutronJumpMode` (boolean): neutron jump mode active/inactive
* `SpanshAttack.jumpRange` (decimal): the current ship’s jump range
