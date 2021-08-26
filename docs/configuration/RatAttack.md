# RatAttack

## Getting Case Data From IRC

You can setup your IRC client to pass incoming RATSIGNALS to VoiceAttack by
calling the `RatAttack-cli.exe` helper tool with the RATSIGNAL as first argument
and an optional boolean as second argument that triggers a TTS case
announcement. You can find it under your VoiceAttack “Apps” folder,
`\alterNERDtive\RatAttack-cli.exe`.

**Note**: If you are running VoiceAttack as admin, you need to run your IRC
client as admin, too! Otherwise they cannot communicate. In general you want to
run both with normal privileges.

This has two purposes:

1. Announcing a new incoming case (if passing `true` as second argument).
1. Storing case data and making it available to VoiceAttack, e.g. for copying 
   the client’s system into the clipboard.

For my AdiIRC, it looks like this (obviously change the path, please):

```adiirc
on *:TEXT:*RATSIGNAL*(??_SIGNAL):#fuelrats:{
  if ( $away ) {
    /run -h "X:\path\to\VoiceAttack\Apps\alterNERDtive\RatAttack-cli.exe" " $+ $replace($1-,","") $+ " false
  }
  else {
    /run -h "X:\path\to\VoiceAttack\Apps\alterNERDtive\RatAttack-cli.exe" " $+ $replace($1-,","") $+ " true
  }
}
```

If I am away it will just add the new case to the list. If I am not away, it
will announce it using TTS.

The “replace” part handles the fact that announcements now put the system in
quotes. They have to be escaped as double quotes (`""`) to create a correct
command invocation.

You get the gist; if not and you don’t know how to do the same thing for your
IRC client, either switch to AdiIRC or bribe me to make an example for yours.

mIRC is a straight forward copy & paste job from Adi to mIRC (“Tools” → “Script
Editor” → “Remote”). Make sure you do not strip colour codes from incoming
messages!

Note for Hexchat users: Hexchat doesn’t seem to have a simple way of doing this.
If you figure it out, I’ll gladly add instructions here.

Keep in mind that if you are not on duty (see below) you will _not_ get case
announcements.

## Announcing Your Nearest CMDR

In case you have more than one CMDR registered as a Fuel Rat you can have
VoiceAttack announce the nearest one to a case and the distance. You will have
to go through a couple steps to set that up:

1. Have all CMDRs on EDSM.
1. Have all profiles on EDSM set to _public_ including your flight log (which
   includes the current location).
1. Set the CMDR names you want to use (“customize setting set fuel rat
   commanders”).
1. Enable the nearest CMDR announcements (“customize setting enable nearest
   commander to fuel rat case”).

You can use this for a single CMDR, too. A less convoluted setup for announcing
the distance to your location in that case is on the list™ but does not have an
ETA yet.

Currently there is no way to specify a platform for each CMDR separately.

## Sending Text to FuelRats IRC

The profile will attempt to send calls to “\#fuelrats”, and you can send
messages from ingame chat to “\#fuelrats” and “\#ratchat”.

That will send text to windows with “\#fuelrats” and “\#ratchat” in 
their title, respectively. If your IRC client does not do that, you will have to 
change the “target” window of the `RatAttack.sendToFuelrats` and 
`RatAttack.sendToRatchat` commands to reflect the actual window titles on your 
system. I will look into making this more elegant to change in the future.

## Settings

Toggles:

* `auto close fuel rat case`: Automatically close a rat case when sending
  “fuel+” via voice command or ingame chat. Default: false.
* `fuel rat call confirmation`: Only make calls in #fuelrats after vocal
  confirmation to prevent mistakes. Default: true.
* `fuel rat duty`: On duty, receiving case announcements via TTS. Default: true.
* `nearest commander to fuel rat case`: Announce the nearest commander to
  incoming rat cases. Default: false.
* `platform for fuel rat case`: Announce the platform for incoming rat cases.
  Default: false.
* `system information for fuel rat case`: System information provided by Mecha.
  Default: true.

Other Settings:

* `fuel rat commanders`: All your CMDRs that are ready to take rat cases. Use
  ‘;’ as separator, e.g. “Bud Spencer;Terrence Hill”. Default: "".
* `fuel rat platforms`: The platform(s) you want to get case announcements for
  (PC, Xbox, Playstation). Use ‘;’ as separator, e.g. “PC;Xbox”. Default: "PC".
