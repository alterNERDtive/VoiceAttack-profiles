# SpanshAttack #

This profile uses the FIXXME plugin to plot neutron jumps using 
[spansh](https://spansh.co.uk/plotter). It fully does everything you need from 
within the game and VoiceAttack, you won’t have to visit the site at any point.

## Settings ##

Currently the only setting in the strict sense is the key use for pasting text 
into Elite:Dangerous. If you are using a “standard” QWERT[YZ] layout, you don’t 
have to do anything; if you are using something different, you have to set it to 
the symbol that’s on the physical button that has `V` on QWERT[YZ]. E.g. for 
AZERTY, set it to FIXXME.

The other “setting” in the not-so-strict sense of the word is the 
`SpanshAttack.getShipRange` command. Any ship listed in there will automatically 
have its jump range used instead of VA prompting you for it. Since, again, VA 
will execute the first matching command found, you can create this command in 
your own profile when you are using SpanshAttack by including it.

The FIXXME plugin is technically supposed to read the current jump range from 
EDDI; sadly a) it’s [bugged](FIXXME) right now, and EDDI is storing the 
_maximum_ distance for your ship instead of the current / full on fuel one.

## Usage ##

### Plotting a Route ###

1. _Target_ the system you want to be routed to (target, do not plot to it).
1. Either exit the galaxy map or make sure you are on its first tab (or plotting 
   will break).
1. Trigger the `SpanshAttack.plotRoute` command either by voice (`plot neutron 
   [course;route;trip]`) or calling it from another command
1. (if ship not listed in `SpanshAttack.getShipRange`) Enter your ship’s jump 
   range when prompted.
1. Wait for the route to be calculated. The command will automatically open the 
   galaxy map and jump to the first waypoint on your route.
1. Either target the first waypoint or plot to it.
1. Start jumping!

### Neutron Jumping ###

With standard setting, just supercharge off a neutron cone, you should 
automatically be taken to the galaxy map with the next waypoint selected.

In case you have disabled auto-plotting to the next waypoint, manually invoke 
the `SpanshAttack.targetNextNeutronWaypoint` command by voice (`[target;] next 
neutron [waypoint; way point]` or calling it from another command.

Additionally, you can use the `SpanshAttack.getNextNeutronWaypoint` 
/ `[get;copy] next neutron [waypoint;way point]` command to copy the next 
neutron waypoint to the clipboard.

#### Manual Re-Plot ####

Trigger the `SpanshAttack.replotRoute` command either by voice (`replot neutron 
[course;route;trip]`) or calling it from another command. This will start 
a re-plot of the current route with the same target system and jump range.

### Refueling ###

Whenever you refuel off a scoopable star, the profile will automatically 
throttle back up to 100% speed.

### Clearing a Route ###

When you reach your target system, the neutron route will automatically be 
cleared. If you want to prematurely end your trip, call the 
`SpanshAttack.clearRoute` / `clear neutron [course;route;trip]` command.

## Other Commands ##

The profile contains a lot of helper functions that get called by the 
aforementionde commands. Have a look around, maybe learn something about 
VoiceAttack :)

## Exposed Variables ##

The following Variables are _global_ and thus readable (and writeable! please 
don’t unless it’s a config variable …) from other profiles:

### Configuration Variables ###

These are set in `SpanshAttack.startup` and can be overriden from your profile 
if you have imported SpanshAttack.

* Elite.pasteKey (string): the key used for pasting into Elite. On QWERTY this 
  is `v`. Default: `v`.
* SpanshAttack.announceWaypoints (boolean): whether to announce each waypoint of 
  the neutron route. Default: true.
* SpanshAttack.announceJumpsLeft (string): `;`-separated list of remaining jumps 
  to announce when said amounts are reached. Right now only works if they are 
  _exactly_ reached when supercharging off a neutron. Default: 
  “1;3;5;10;15;20;30;50;75;100”
* SpanshAttack.autoPlot (boolean): whether to automatically plot to the next 
  waypoint on supercharging. Default: true.
* SpanshAttack.copyWaypointToClipboard (boolean): whether to copy the next 
  waypoint into the Windows clipboard for use in other programs. Default: false.
* SpanshAttack.useEddiForVoice (boolean): whether to use EDDI over VA’s builtin 
  `say` command. Default: false.

### Other Variables ###

These variables can be used to get information about the current neutron route.  
Please do not set them manually and / or from outside the SpanshAttack profile.

* SpanshAttack.targetSystem (string): the target system for the current neutron 
  route
* SpanshAttack.nextNeutronWaypoint (string): the next waypoint on the current 
  neutron route
* SpanshAttack.neutronJumpMode (boolean): neutron jump mode active/inactive
* SpanshAttack.jumpRange (decimal): the current ship’s jump range
