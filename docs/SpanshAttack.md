# SpanshAttack

This profile uses the 
[ED-NeutronRouter](https://github.com/sc-pulgan/ED-NeutronRouter) plugin to plot 
neutron jumps using [spansh](https://spansh.co.uk/plotter). It fully does 
everything you need from within the game and VoiceAttack, you won’t have to 
visit the site at any point.

## Plotting a Route

1. _Target_ the system you want to be routed to (target, do not plot to it).
1. Either exit the galaxy map or make sure you are on its first tab (or 
   auto-plotting will break).
1. Trigger the `SpanshAttack.plotRoute` command either by voice
   (`plot neutron [course;route;trip] [with custom range;]`) or by
   calling it from another command.
1. Enter your ship’s jump range if prompted.
1. Wait for the route to be calculated. The command will automatically open the 
   galaxy map and search it for the first waypoint on your route.
1. Either target the first waypoint or plot to it.
1. Start jumping!

### Plotting to a System Unknown to the Neutron Router

The router can only plot a route to a system that is in its database (obviously 
can also only give you way points that are). If your target system is not, there 
are several levels of fallback handling to find a system that is.

1. Check `Next system` coordinates provided by EDDI. If the system is in EDSM, 
   but has for some reason not been sent over EDDN to other sites including 
   Spansh, we can get coordinates here.
1. If the system is not in EDSM check EDTS. It can calculate approximate 
   coordinates for a given procedurally generated system name.
1. If that fails prompt the user for input.
1. Query Spansh’ API for the closest system to these coordinates.
1. Plot a route to the closest system.

Generally you should almost never be asked to input coordinates manually. If 
EDTS provides coordinates with an accuracy that is worse than ±100 ly per axis, 
you will be prompted to make sure you are going roughly to the right 
coordinates.  You will find the system that is used for plotting, its 
coordinates and the accuracy in VoiceAttack’s log window.

## Neutron Jumping

With standard settings, just supercharge off a neutron cone. You should 
automatically be taken to the galaxy map with the next waypoint selected.

In case you have disabled auto-plotting to the next waypoint, manually invoke 
the `SpanshAttack.targetNextNeutronWaypoint` command by voice
(`[target;] next neutron [waypoint; way point]`) or calling it from
another command.

Additionally, you can use the `SpanshAttack.copyNextNeutronWaypoint` 
/ `[get;copy] next neutron [waypoint;way point]` command to copy the next 
neutron waypoint to the clipboard.

### Skipping a waypoint

Sometimes, especially in very neutron-sparse areas of the galaxy, the plotter
will give you weird jumps. E.g. I recently got neutron → 37 ly → neutron → 440
ly.

In these cases you can use the `SpanshAttack.skipNeutronWaypoint` / `skip
[this;current] neutron waypoint` command to move on to the next one in the
list.

### Manual Re-Plot

Trigger the `SpanshAttack.replotRoute` command either by voice
(`replot neutron [course;route;trip]`) or calling it from another command.
This will start a re-plot of the current route with the same target system and
jump range.

## Refueling

Whenever you finish refueling off a scoopable star, the profile will
automatically throttle back up to 100% speed. Unless you have disabled it in
your configuration, you will also automatically target the next system on your
route and jump to it once you leave fuel scoop range.

## Clearing a Route

When you reach your target system the neutron route will automatically be
cleared. If you want to prematurely end your trip, call the
`SpanshAttack.clearRoute` / `clear neutron [course;route;trip]` command.

## Other Commands

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
