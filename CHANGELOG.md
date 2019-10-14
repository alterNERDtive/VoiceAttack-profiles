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
* trip time is now also wrote to the VoiceAttack log

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
