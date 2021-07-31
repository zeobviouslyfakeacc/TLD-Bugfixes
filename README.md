# TLD-Bugfixes

This mod fixes some old bugs and known exploits in *The Long Dark*.

### Bugs fixed:

- Map fixes:
  - Fixed a bug where caches would never appear on the map, even after having discovered them
  - Fixed a bug where the map could only show a certain amount of icons that many of the newer maps exceeded,
    which would lead to some icons suddenly not showing up anymore
- Fixed [z-fighting](https://en.wikipedia.org/wiki/Z-fighting) between dropped flat items like books
- Fixed a too small interior zone in the Hunting Lodge
  - There were places in that building where the temperature was several degrees lower than elsewhere for no apparent reason
- ~~When taking medicine without treating an affliction (e.g. when drinking medicinal tea for its calories),
    the player used to monologue about the treatment not working~~ (Fixed in TLD v1.41!)
- Fixed a bug where the game would call the Steam API thousands of times per second to check whether a Steam Controller is connected
  - Previously, this bug also caused a large amount of memory allocation, but this has already been fixed
- Fixed a bug where the game could get noticeably darker at night after opening and closing the options menu
- ~~Fixed a bug that would move cupboard doors, shelves, and similar objects around after breaking down some object~~ (Fixed in TLD v1.47!)
- ~~Fixed the position of the red "stamina reduction bar"~~ (Fixed in TLD v1.47!)
- ~~Fixed a bug where torches, flares, and lanterns would not increase the "feels like" temperature~~ (Fixed in TLD v1.47!)

### Exploits fixed:

- Rotate dropped items to the direction the player is facing
  - This prevents using dropped items as a free compass and makes it easier to use dropped sticks as direction markers
- Fixed the exploit where cancelling clearing an ice fishing hole would partially clear the hole without degrading the used tool

## Installation

1. If you haven't done so already, install MelonLoader by downloading and running [MelonLoader.Installer.exe](https://github.com/HerpDerpinstine/MelonLoader/releases/latest/download/MelonLoader.Installer.exe)
2. Download the latest version of `TLD-Bugfixes.dll` from the [releases page](https://github.com/ds5678/TLD-Bugfixes/releases)
3. Move `TLD-Bugfixes.dll` into the Mods folder in your TLD install directory