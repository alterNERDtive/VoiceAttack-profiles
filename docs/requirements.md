# Requirements

## VoiceAttack

Obviously you will need to install [VoiceAttack](https://voiceattack.com). There
is a free trial version available, but that one is limited to a single profile
and a few commands. This is 5 profiles and … a lot of commands. You will need the
full version, available for $10 (official site) or €11.99 (Steam, IIRC $14.99
for our US-based friends).

I recommend buying on the site. Why? Because on Steam, Valve gets a 30% cut.
Unlike many other developers Gary (the developer of VoiceAttack) remedies that
by having a price on Steam that ends up paying $10 to him. So basically, you are
paying Valve out of your own pocket. Many other developers do not do that, and
by buying from them directly instead of on Steam you are literally giving them
extra money. Please do keep that in mind in the future!

You also will generally need to opt into the beta version. I am usually at the
forefront of bug reports and feature requests, and I do rely on the
fixes/additions in beta versions quite often.

## EDDI

[EDDI](https://github.com/EDCD/EDDI) is a companion application for Elite:
Dangerous, providing responses to events that occur in-game using data from the
game as well as various third-party tools. In this case, you will need to run it
as a VoiceAttack plugin.

EDDI also regularly publishes beta versions. Unless a profiles release
explicitly states it you will _not_ have to run EDDI beta.

## bindED

[bindED](https://alterNERDtive.github.io/bindED) reads your Elite Dangerous
binding files and makes them available to VoiceAttack as variables. That way
commands can be portable and you do not have to manually go through them and
change any actions that you happen to not have the standard binds for.

This plugin is _included_ in the release package.

## Elite Scripts

I have written a [collection of Python
scripts](https://github.com/alterNERDtive/elite-scripts) to interface with
various 3ʳᵈ party services like EDSM or Spansh. Those are called by the profiles
for various tasks, like checking a system’s body count.

In the future they will be replaced by VoiceAttack plugin code.

The scripts are _included_ in the release package.

## ED-NeutronRouter

(required for SpanshAttack)

[ED-NeutronRouter](https://github.com/sc-pulgan/ED-NeutronRouter) interfaces
with [Spansh’s neutron plotter](https://spansh.uk/plotter) and makes the result
available to VoiceAttack.

This will also eventually be replaced by my own plugins.
