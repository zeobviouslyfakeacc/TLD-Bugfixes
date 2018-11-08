# TLD-Bugfixes

This mod fixes some old bugs and known exploits in *The Long Dark*.

### Bugs fixed:

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
- Fixed lots of item weights such that crafting no longer creates or destroys mass
  - Newspaper rolls now weigh 0.2 kg instead of 0.1 kg, as they can be broken down to 4 x 0.05 kg of tinder
  - Coffee and {Herbal | Reishi | Rosehip} Tea now weigh 0.25 kg instead of 0.1 kg, as they are crafted with 0.25 liters of water
    and restore the same amount of hydration as 0.25 liters of water
  - Deer hides now weigh 1 kg instead of 0.5 kg, as two of them are required to craft the 2 kg deerskin boots
    and because deer have more surface area than wolves, whose pelts weigh just 0.75 kg
  - Bear hides now weigh 2.5 kg instead of 1 kg, as two of them are required to craft the 5 kg bearskin coat
    and because their pelts are almost as huge as the moose hides, which weigh 5 kg each
  - Rabbitskin mitts now weigh 0.5 kg instead of 1 kg, as they are created using 4 rabbit pelts and 2 guts at 0.1 kg each
    and because 1 kg is way too much for a pair of gloves
  - Bandages now weigh 0.05 kg, as two of them can be created by breaking down a piece of cloth, which weighs 0.1 kg
  - Old man's beard wound dressings now weighs 0.03 kg instead of 0.1 kg, as they are crafted using 3 lichens at 0.01 kg each

### Exploits fixed:

- Randomized the rotation of dropped items, which could be used as a free compass
- Prevent the player from eating ruined food items
  - This was always the case before the cooking update in v1.30, but after v1.30 this seems to have been forgotten
  - (Though, of course, even before v1.30 there were exploits that still allowed you to eat ruined food...)
  - Cooking ruined raw food now also yields ruined cooked food
- Fixed an exploit where the user could pick up, equip, and use a bow that was still in progress of being crafted
  - This bug basically gave you free bows that you could craft in 0.5 hours and didn't waste any crafting materials
- Fixed the cooking skill exploit where players could quickly level up by cooking lots of tiny pieces of meat
- Fixed the strange food poisoning design where items at 74% condition had the same food poisoning chance as an item at 21% condition


## Installation

1. If you haven't done so already, install the [Mod Loader](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller)
2. Install the mod automatically by using [WulfMarius's Mod-Installer](https://github.com/WulfMarius/Mod-Installer/releases)

or

1. If you haven't done so already, install the [Mod Loader](https://github.com/zeobviouslyfakeacc/ModLoaderInstaller)
2. Download the latest version of `TLD-Bugfixes.dll` from the [releases page](https://github.com/zeobviouslyfakeacc/TLD-Bugfixes/releases)
3. Move `TLD-Bugfixes.dll` into the mods folder in your TLD install directory
