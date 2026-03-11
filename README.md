# Very Very Valet Archipelago Client Mod

## Install Guide
1. Download [BepinEx](https://github.com/BepInEx/BepInEx/releases) 5.4 for x64 architecture
2. Extract all files into your `Very Very Valet/windows` directory
3. Launch the game
4. Close the game
5. Go to BepinEx/config and open BepinEx.cfg in a text editor
6. Change HideManagerGameObject to true
7. Download the [latest release of this mod](https://github.com/xMcacutt-Archipelago/VeryVeryValet-APClient/releases)
8. Extract the Parkipelago folder into the BepinEx/plugins folder in your game folder
9. Launch the game
10. Pick up cars
11. Park them
13. Cry

## Implementation Details

### ★ The Star ★

46 stars are added to the pool. This is two per level so in a solo run you are expected to achieve at least two stars on every level.
Each level has three stars which send checks. 
If you are comfortable achieving three stars, you can turn off `Limit Stars` in your yaml options to allow stars and other games' progression items to be placed on the third star locations.

### Level Unlocks

Each level will be locked behind a star count.
There are two level unlocking modes. If you want to force yourself to play the levels, you can turn on `Require Level Completions` in your yaml options.
If you would rather just be able to play any level as long as you have received enough stars then leave this option off.

### Expert Mode

The expert mode of the levels can be forced using the `Require Red Stars` yaml option.

The apworld can be found [here](https://github.com/xMcacutt-Archipelago/Archipelago-VeryVeryValet/releases/latest)
