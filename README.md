# TLD-Bugfixes

This mod fixes some old bugs and known exploits in *The Long Dark*.

### Bugs fixed:

- Fixed the broken weapon inaccuracy and improved the sway animation
  - Arrows now fly to where their tip points at
  - The rifle sights now stay aligned and can actually be used to aim
- Fixed the bug where the 2 head clothing slots in the clothing UI were swapped
  - For all other clothing regions, the slot closer to the character is the "inner" clothing slot
- Custom mode fixes:
  - Fixed the bug where the "Wildlife Respawn Frequency" setting had the exact opposite effect of what the user intended
    - Setting the respawn frequency to low would result in higher respawn frequencies, and vice versa
  - Fixed a bug where the starting weather, wolf spawn distance, and wildlife detection range weren't saved properly
- Map fixes:
  - Fixed a bug where caches would never appear on the map, even after having discovered them
  - Fixed a bug where the map could only show a certain amount of icons that many of the newer maps exceeded,
    which would lead to some icons suddenly not showing up anymore
- Fixed a bug where you'd always get torches with the same condition from a fire if you didn't move between the "take torch" actions
- Fixed that ruined snow shelters couldn't be destroyed
- Fixed a bug where people could get stuck crafting forever until they died
- Fixed [z-fighting](https://en.wikipedia.org/wiki/Z-fighting) between dropped flat items like books
- Fixed a bug where the crafting menu could display the wrong crafting time
- Fixed a too small interior zone in the Hunting Lodge
  - There were places in that building where the temperature was several degrees lower than elsewhere for no apparent reason
- Fixed some problems when the "Survival Monologue" was turned off in a custom mode game
  - Unlike eating, drinking was completely silent
  - ~~When taking medicine without treating an affliction (e.g. when drinking medicinal tea for its calories),
    the player used to monologue about the treatment not working~~ (Fixed in TLD v1.41!)
- Fixed a bug where the game would call the Steam API thousands of times per second to check whether a Steam Controller is connected
  - Previously, this bug also caused a large amount of memory allocation, but this has already been fixed
- Fixed a bug where the game would get noticeably darker at night after opening and closing the options menu
- Fixed a bug where the crafting recipe scroll bar had an incorrect number of steps
- ~~Fixed a bug that would move cupboard doors, shelves, and similar objects around after breaking down some object~~ (Fixed in TLD v1.47!)
- ~~Fixed the position of the red "stamina reduction bar"~~ (Fixed in TLD v1.47!)
- ~~Fixed a bug where torches, flares, and lanterns would not increase the "feels like" temperature~~ (Fixed in TLD v1.47!)

### Exploits fixed:

- Rotate dropped items to the direction the player is facing
  - This prevents using dropped items as a free compass and makes it easier to use dropped sticks as direction markers
- Prevent the player from eating ruined food items
  - This was always the case before the cooking update in v1.30, but after v1.30 this seems to have been forgotten
  - (Though, of course, even before v1.30 there were exploits that still allowed you to eat ruined food...)
  - Cooking ruined raw food now also yields ruined cooked food
- Fixed an exploit where the user could pick up, equip, and use a bow that was still in progress of being crafted
  - This bug basically gave you free bows that you could craft in 0.5 hours and didn't waste any crafting materials
- Fixed the cooking skill exploit where players could quickly level up by cooking lots of tiny pieces of meat
- Fixed the strange food poisoning design where items at 74% condition had the same food poisoning chance as an item at 21% condition
- Fixed the exploit where cancelling clearing an ice fishing hole would partially clear the hole without degrading the used tool

## Installation

1. If you haven't done so already, install the [Mod Loader](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller)
2. Install the mod automatically by using [WulfMarius's Mod-Installer](https://github.com/WulfMarius/Mod-Installer/releases)

or

1. If you haven't done so already, install the [Mod Loader](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller)
2. Download the latest version of `TLD-Bugfixes.dll` from the [releases page](https://github.com/zeobviouslyfakeacc/TLD-Bugfixes/releases)
3. Move `TLD-Bugfixes.dll` into the mods folder in your TLD install directory
