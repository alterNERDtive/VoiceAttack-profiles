# StreamAttack

This profile uses the [EDDI](https://github.com/EDCD/EDDI) plugin to write 
a bunch of information about your commander, your current location and your ship 
to files that can be accessed e.g. by your streaming software to be displayed on 
stream.

Default folder is `%appdata%\StreamAttack\`.

## Commands

* `clear jump target`: Clears the current jump target.
* `distance [to;from] jump target`: Tells you the current distance to the jump
  target.
* `set jump target`: Sets the jump target to the currently targeted system.
  Distance will be written to the configured file.

* `[copy;open] ship build`: Copies the current ship build (coriolis) or opens it 
  in your default browser.
* `open StreamAttack folder`: Opens the configured folder in Explorer.

## Output Files

### Elite

Please do note that information in the output files is only updated when a
journal event that contains the information is detected. E.g. the distance to
your jump target is not constantly calculated, but only updated after a jump.

#### Commander

* `Elite\cmdr\name`: The current commander’s name.

#### Jump Target

* `Elite\jumpTarget\distance`: Distance to current jump target in light years.
* `Elite\jumpTarget\full`: Pretty-printed `<distance> ly to <name>`.
* `Elite\jumpTarget\name`: The current jump target’s system name.

#### Location

* `Elite\location\full`: Depending on your status, either the station you are 
  currently docked at (+ system), the body you are currently near, or the system 
  you are currently in.
* `Elite\location\system`: The system you are currently in.

#### Ship

* `Elite\ship\build`: Your current ship’s loadout (link to coriolis).
* `Elite\ship\full`: `“<name>” | <model> | <build>`.
* `Elite\ship\model`: Your current ship’s model.
* `Elite\ship\name`: Your current ship’s name.
