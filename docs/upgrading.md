# Upgrading

To upgrade to the latest version, follow these simple steps:

1. Say “download profiles update”.
1. Put the `alterNERDtive-voiceattack-profiles.vax` file from Github into the
   “Import” folder.
1. Restart VoiceAttack.

In general, migration from old versions will be handled automatically. If there
is something you have to do manually you will find the necessary steps below.

## 3.x.x to 4.0

If you have been using a custom profile as outlined in [[Installing#Create a 
Custom Profile]] your settings will be migrated to the new variables
automatically.

If you have not and you have used my “EliteDangerous” profile as the main
profile … well, you will unfortunately have to take note of your settings, and
recreate them after creating a custom profile and including everything.

Please do not fiddle with the configuration variables from your startup command
(or any other, really). It _should_ not break anything, but it might. And it is
entirely unnecessary since configuration will be saved to and loaded from the
profile anyway.

### bindED

If you have done anything non-standard with bindED before, it might break. The
profiles now include my fork of bindED which has a lot more features, but
does no longer support a bunch of plugin invocations that have become obsolete.

Please [file an issue with
bindED](https://github.com/alterNERDtive/bindED/issues/new/choose) on Github if
your use case does not work anymore.

### EliteDangerous

The “EliteDangerous” profile is no longer the main profile. Instead you will
_have_ to create a custom profile, and the new “base” profile that the others
require to be included in your custom profile is “alterNERDtive-base”. That way
you can use e.g. RatAttack without having to also use the general Elite profile.

To be consistent with the other profiles it has been renamed to “EliteAttack”.
If you are upgrading from an older version the name will not change for you in
the profiles list. I recommend renaming your “EliteDangerous” profile to
“EliteAttack” to prevent confusion in the future, but it is not strictly
necessary to do so.

### RatAttack

Handing a RATSIGNAL from IRC to VoiceAttack via text file is now deprecated and
the feature will be removed in a future version.

Instead you should use the new `RatAttack-cli.exe` helper tool that uses IPC to
talk to the VoiceAttack plugin. For that you have to change the way your IRC
client handles incoming case announcements. Instead of writing the announcement
to the text file and calling VoiceAttack to run a command, it will have to call
the helper tool with a) the announcement and b) an optional true/false switch to
determine if the case should be announced via TTS or just added to the case
list.

For my AdiIRC, it looks like this (obviously change the path, please):

```adiirc
on *:TEXT:RATSIGNAL - CMDR*(??_SIGNAL):#fuelrats:{
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
command line invocation.