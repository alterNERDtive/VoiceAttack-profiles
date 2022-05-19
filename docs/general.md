# General Commands

## Configuration

The base profile provides voice commands for changing the profiles settings.
See [the configuration section](../configuration/general#settings).

## Chat

* `paste text`: Pastes the contents of your current clipboard. Note that this
  command is supposed to be used for pasting _into Elite_ and hence uses the
  configured paste key. If you’re using a non-standard layout that means that
  you can _not_ use this command to paste text into other applications.

## Updating

* `check for profiles update`: Checks Github for a new version, and alerts you
  if there is one.
* `download profiles update`: Opens the latest release on Github and the
  VoiceAttack import folder where you can drop it.
* `open profiles [docs;documentation;help] [file;site;]`: Opens this
  documentation, either on the web or the PDF file supplied with the installed
  release.
* `open profiles change log`: Opens the CHANGELOG on Github.
* `open voiceattack [apps;import;sounds] [folder;directory]`: Opens the
  respective VoiceAttack-related folder.

## Miscellaneous

* `generate missing key binds report`: Generates a report of missing key binds
  and places it on your Desktop. Note that this currently uses bindED’s built-in
  report which will output _any_ bind that does not have a keyboard key set,
  including axis binds and binds that are not actually used by the profiles.
* `open documentation`: Opens the documentation in your default browser.
* `open EDDI options;configure EDDI`: Displays EDDI’s configuration window.
* `open elite bindings folder`: Opens Elite’s bindings folder
  (`%localappdata%\Frontier Developments\Elite Dangerous\Options\Bindings`)
* `reload elite key binds`: Forces a reload of your Elite binds. Should not be
  necessary.
* `shut up EDDI`: Immediately interrupts any current and pending speech on
  EDDI’s end.
