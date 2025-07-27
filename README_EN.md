# LuckierBlackCat - The Luckier Black Cat

[![Version](https://img.shields.io/badge/Version-0.4.1.0-blue.svg)](https://github.com/travellerse/LuckierBlackCat/releases)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE.txt)
[![Game](https://img.shields.io/badge/Game-Elin-orange.svg)](https://store.steampowered.com/app/2135150/Elin/)

> **Language Versions**: [中文](README.md) | [English](README_EN.md) | [日本語](README_JP.md)

A BepInEx mod developed for the Elin game that significantly enhances the black cat's item licking functionality, making your equipment even luckier!

## 📖 Features

### 🎯 Core Functions

- **Unlimited Distance Item Licking** - Removes distance restrictions for black cat item licking, can be triggered from anywhere on the map
- **Auto-lick Picked Items** - Automatically triggers black cat item licking when the player picks up equipment
- **Batch Licking During Prayer** - Automatically licks all eligible equipment in the player's inventory during prayer
- **Enhanced Licking Effects** - Strengthens the black cat's licking effects based on the quantity of [Black Cat's Saliva]

### ⚠️ Usage Requirements

- **By default requires a party member with [Blessing of Ehekatl]**
- **Can be disabled by setting `RequireLickAbility = false` in configuration**
- Only works on equipment and ranged weapons
- Does not work on cursed items
- Only works on items with rarity higher than common
- Does not repeatedly affect items that have already been licked

## 🔧 Installation

### Automatic Installation

1. Subscribe on [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3366709105)

### Manual Installation

1. Download the [latest version](https://github.com/travellerse/LuckierBlackCat/releases) and extract
2. Place the `mod folder` into the `Elin\Package\` directory
3. Launch the game and enable the mod

## ⚙️ Configuration

Configuration file location: `Elin\BepInEx\config\LuckierBlackCat.cfg`

### Configurable Options

```ini
[Settings]

## Enable unlimited distance item licking function
# Type: Boolean
# Default: true
EnableLickWithoutDist = true

## Enable auto-lick when picking up equipment
# Type: Boolean  
# Default: true
EnableLickWhenPick = true

## Enable auto-lick during prayer
# Type: Boolean
# Default: true
EnableLickWhenPray = true

## Enable licking effect enhancement based on black cat saliva quantity
# Type: Boolean
# Default: true
EnableLickEnchant = true

## Enhancement effect multiplier
# Type: Integer
# Default: 1
EnchantTimes = 1

## Whether lick ability is required for mod functionality
# Type: Boolean
# Default: true
# Description: If set to false, the mod will work even without black cats with lick ability
RequireLickAbility = true
```

## 🔗 Project Links

- **GitHub Repository**: [https://github.com/travellerse/LuckierBlackCat](https://github.com/travellerse/LuckierBlackCat)
- **Issue Tracker**: [https://github.com/travellerse/LuckierBlackCat/issues](https://github.com/travellerse/LuckierBlackCat/issues)
- **Elin Official Page**: [https://store.steampowered.com/app/2135150/Elin/](https://store.steampowered.com/app/2135150/Elin/)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details

## ⚠️ Third-Party Copyright Notice

All third-party library files contained in the `ref/` folder of this project are copyrighted by their respective owners. These files are used for development purposes only and are not covered by this project's license.

### Open Source Library License Information

- **0Harmony.dll**: [MIT License](https://github.com/pardeike/Harmony/blob/master/LICENSE) - Copyright © Andreas Pardeike
- **BepInEx.Core.dll**: [LGPL-2.1 License](https://github.com/BepInEx/BepInEx/blob/master/LICENSE) - Copyright © BepInEx Contributors  
- **BepInEx.Unity.dll**: [LGPL-2.1 License](https://github.com/BepInEx/BepInEx/blob/master/LICENSE) - Copyright © BepInEx Contributors
- **UnityEngine.CoreModule.dll**: [Unity Reference-Only License](https://unity.com/legal/licenses/unity-reference-only-license) - Copyright © Unity Technologies
- **UnityEngine.dll**: [Unity Reference-Only License](https://unity.com/legal/licenses/unity-reference-only-license) - Copyright © Unity Technologies

### Proprietary Software

- **Elin.dll**: Proprietary Software - Copyright © Lafrontier/Noa
- **Plugins.BaseCore.dll**: Proprietary Software - Copyright © Lafrontier/Noa

When using this mod, please ensure you have legitimate authorization for the corresponding games and dependency libraries. Please comply with the license terms of each open source library.

## 💖 Support the Author

If this mod has been helpful to you, please consider giving the project a ⭐!
