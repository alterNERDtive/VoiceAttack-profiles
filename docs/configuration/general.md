# General Configuration

Additionally, you need to have keyboard binds setup at least as secondary 
bindings in Elite’s controls options. VA _cannot_ “push” joystick buttons for 
you, it can only do keyboard inputs. Hence its only way to interact with Elite 
is through keyboard emulation, even if you otherwise play the game with 
a controller or HOTAS. Or racing wheel. Or Rock Band set. Or bananas.


### Settings ###

All profiles will load sane defaults if you haven’t changed anything. You no 
longer need to fiddle with the `startup` commands of each profile, instead you 
can use voice commands to change settings! See the `docs/` and the 
`_configuration` commands section of each profile.

Basically all the settings are available using the `customize settings` prefix, 
then saying `[enable;disable] <setting>` for on/off switches and `set <setting>` 
for text variables.

One caveat applies: settings will only be saved in the profile you have 
selected, but be preserved if you switch around.

### Making changes ###

If you want to edit a command or add your own, _do not edit the profiles 
directly_. Instead create commands in your custom profile, and copy commands you 
want to change over to that before editing them. This will make sure no changes 
are lost if you update the profiles.

Because of limitations of VoiceAttack itself, only the first matching command 
found will be executed, _including EDDI events_. That means that if you create 
commands to handle EDDI events, you are going to have to check the imported 
profiles if they rely on these event handlers as well, and call them manually if 
they do.

E.g.  if you want to create a custom `((EDDI Message sent))` handler in your 
profile, you will have to make it excute the `EliteDangerous.EDDI Message sent` 
and `RatAttack.EDDI Message sent` commands.  Otherwise stuff _will_ break.

If you have no idea what the previous two paragraphs were about, you can most 
likely just ignore them.