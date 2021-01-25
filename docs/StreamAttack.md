# StreamAttack

This profile uses the [EDDI](https://github.com/EDCD/EDDI) plugin to write 
a bunch of information about your commander, your current location and your ship 
to files that can be accessed e.g. by your streaming software to be displayed on 
stream.

Default folder is `%appdata%\StreamAttack\`.

## Commands

* `clear jump target`: clears the current jump target.
* `set jump target`: sets the jump target to the currently targeted system. 
  Distance will be written to the configured file.

* `[copy;open] ship build`: copies the current ship build (coriolis) or opens it 
  in your default browser.
* `open StreamAttack folder`: opens the configured folder in Explorer.

## Output Files

### Elite

#### Commander

* `Elite\cmdr\name`: the current commander’s name.

#### Jump Target

* `Elite\jumpTarget\distance`: distance to current jump target in light years.
* `Elite\jumpTarget\full`: pretty-printed `<distance> ly to <name>`.
* `Elite\jumpTarget\name`: the current jump target’s system name.

#### Location

* `Elite\location\full`: depending on your status, either the station you are 
  currently docked at (+ system), the body you are currently near, or the system 
  you are currently in.
* `Elite\location\system`: the system you are currently in.

#### Ship

* `Elite\ship\build`: your current ship’s loadout (link to coriolis).
* `Elite\ship\full`: `“<name>” | <model> | <build>`.
* `Elite\ship\model`: your current ship’s model.
* `Elite\ship\name`: your current ship’s name.