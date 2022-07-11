# RatAttack

This profile facilitates [Fuel Ratting](https://www.fuelrats.com). It aims to 
eliminate as much of the required manual task and attention switching as 
possible via automation and voice commands.

If you don’t know what the Fuel Rats are, come hang out and ask :)

## Going On/Off Duty

When you are on duty, RatAttack will automatically announce cases coming in 
through IRC. When off duty, it won’t.

* `[enable;disable] rat duty`: puts you on/off duty.
* `open [fuel rats;] dispatch board`: opens the web dispatch board.

## Case Handling

[If you have your IRC client setup
properly](../configuration/RatAttack/#getting-case-data-from-irc), VoiceAttack
will hold a list with all rat cases that have come in while you had it running.
It will save the case number, CMDR name, system, O₂ status and platform. There
are several commands you can run on this list, giving it a case number:

### Getting Case Information

* `rat case number [0..30] details`: Will give you all stored info on a case.
* `[current;] rat case details`: Will give you all stored info on the currently 
  open case.
* `distance to current rat case`: Will give you the distance from your current 
  location to the currently opened rat case.
* `distance to rat case number [0..30]`: Will give you the distance from your 
  current system to a case’s system.
* `latest rat case details`: Will give you the case data for the latest incoming
  case.
* `nearest commander to rat case number [0..30]`: Will give you the nearest of
  your CMDRs with their distance to a case’s system. [Requires some
  setup](../configuration/RatAttack/#announcing-your-nearest-cmdr).
* `nearest commander to [the;] rat case`: Will give you the nearest of your
  CMDRs with their distance to the current case’s system. [Requires some
  setup](../configuration/RatAttack/#announcing-your-nearest-cmdr).

### Opening a Case

* `open rat case number [0..30]`: Opens rat case with the given number. If there 
  is no case data for that case (e.g. because you don’t have your IRC client set 
  up properly), it will still open it, just not have any data on it.
* `open [latest;] rat case`: Opens the latest rat case that has come in through
  IRC. Will only work if you actually have [your IRC client setup to send case
  announcements to
  VoiceAttack](../configuration/RatAttack/#getting-case-data-from-irc).

Opening a case will automatically copy the client’s system to the clipboard for
easy route plotting. This can be disabled.

### Making Calls ###

There are a bunch of calls you can make for a case, the most common are modelled 
through VoiceAttack commands. The descriptive commands (e.g. “system confirmed”) 
will be shortened to the usual IRC short hands (e.g. “sysconf”). If you need 
something more unusual you can either still manually type it into your IRC 
client or use the “General IRC Integration”, see below.

* `call [1..20] jumps [and login;and takeoff;left;]`: Calls jumps for the 
  currently open case. You can optionally include that you will still have to 
  login to the game or have to take off from your current 
  station/port/outpost/planet.
* `call jumps [left;]`: Calls jumps for the currently open case based on a
  neutron trip (requires SpanshAttack) or a plotted ingame route.
* `call friend [positive;negative] [in pg;in private group;in solo;in main menu;sysconf;system confirmed;]`:
  Friend request confirmations, with all the 
  things you might want to / should call with it.
* `call [beacon;fuel;instance;pos;position;prep;sys;system;wing] [positive;negative]`: 
  All the stuff you usually need for ratting after you have received the friend request.
* `call wing pending`: Calls “wr pending” for when it takes 30s again to 
  actually show up.
* `call client in [exclusion zone;main menu;open;open sysconf;pg;private group;solo;super cruise]`:
  Callouts for all the various things a client could get themselves into.
* `call [client destroyed;client offline;sysconf;system confirmed]`: This is the 
  command you don’t want to use. Include sysconf in your “friend+” or “in open” 
  calls, and make sure you will never have to call “client destroyed”, would 
  you?

By default, VoiceAttack will ask for confirmation before sending calls to the
`#fuelrats` channel.

### Closing a Case

* `[close;clear] rat case`: Closes the currently open rat case.

## General IRC Interaction

Using EDDI to read the game’s journal, you can send messages to IRC from Elite’s 
ingame chat.

**Be aware that the chat message will still appear in the ingame chat channel 
you send it to!**

I recommend using local chat and limiting the use to instances that will 
probably not have other players in it (e.g. instanced in normal space with the 
client or in SC in some remote system out in the black on a long range rescue).

* \#fuelrats: Use `.fr <message>` to have VoiceAttack send
  `#<caseNumber> <message>` to \#fuelrats – or yell at you when you
  are not on a case.
* \#ratchat: Use `.rc <message>` to have VoiceAttack send `<message>` to 
  \#ratchat.

Make sure [that your IRC client is setup
properly](../configuration/RatAttack/#sending-text-to-fuelrats-irc).
