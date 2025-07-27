# LuckierBlackCat - 更幸运的黑猫

[![版本](https://img.shields.io/badge/版本-0.4.1.0-blue.svg)](https://github.com/travellerse/LuckierBlackCat/releases)
[![许可证](https://img.shields.io/badge/许可证-MIT-green.svg)](LICENSE.txt)
[![游戏](https://img.shields.io/badge/游戏-Elin-orange.svg)](https://store.steampowered.com/app/2135150/Elin/)

> **语言版本**: [中文](README.md) | [English](README_EN.md) | [日本語](README_JP.md)

一个为Elin游戏开发的BepInEx模组，大幅增强黑猫的舔物品功能，让你的装备变得更加幸运！

## 📖 功能介绍

### 🎯 核心功能

- **无距离限制舔物品** - 去除黑猫舔物品的距离限制，在地图任何位置都能触发
- **自动舔拾取物品** - 玩家拾取装备时自动触发黑猫舔物品
- **祈祷时批量舔物品** - 祈祷时自动舔玩家背包中的所有符合条件的装备
- **强化舔物品效果** - 根据【黑猫的口水】数量强化黑猫的舔物品效果

### ⚠️ 使用条件

- **默认情况下需要队伍里有队友拥有【艾赫卡托尔的祝福】**
- **可通过配置`RequireLickAbility = false`来关闭舔舐能力要求**
- 只对装备和远程武器生效
- 不对被诅咒的物品生效
- 只对稀有度高于普通的物品生效
- 不对已经被舔过的物品重复生效

## 🔧 安装方法

### 自动安装

1. 订阅 [Steam创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3366709105)

### 手动安装

1. 下载 [最新版本](https://github.com/travellerse/LuckierBlackCat/releases) 并解压
2. 将 `mod文件夹` 放入 `Elin\Package\` 目录
3. 进入游戏，启用模组

## ⚙️ 配置说明

配置文件位置：`Elin\BepInEx\config\LuckierBlackCat.cfg`

### 可配置选项

```ini
[Settings]

## 启用无距离限制舔物品功能
# 类型: Boolean
# 默认值: true
EnableLickWithoutDist = true

## 启用拾取装备时自动舔物品功能
# 类型: Boolean  
# 默认值: true
EnableLickWhenPick = true

## 启用祈祷时自动舔物品功能
# 类型: Boolean
# 默认值: true
EnableLickWhenPray = true

## 启用基于黑猫口水数量的舔物品效果强化
# 类型: Boolean
# 默认值: true
EnableLickEnchant = true

## 强化效果倍数
# 类型: Integer
# 默认值: 1
EnchantTimes = 1

## 是否需要舔舐能力才能发挥作用
# 类型: Boolean
# 默认值: true  
# 说明: 如果设置为false，即使没有拥有舔舐能力的黑猫也可以使用mod功能
RequireLickAbility = true
```

## 🔗 项目链接

- **GitHub仓库**: [https://github.com/travellerse/LuckierBlackCat](https://github.com/travellerse/LuckierBlackCat)
- **问题反馈**: [https://github.com/travellerse/LuckierBlackCat/issues](https://github.com/travellerse/LuckierBlackCat/issues)
- **Elin官方页面**: [https://store.steampowered.com/app/2135150/Elin/](https://store.steampowered.com/app/2135150/Elin/)

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE.txt](LICENSE.txt) 文件了解详情

## ⚠️ 第三方版权声明

本项目 `ref/` 文件夹中包含的所有第三方库文件的版权归其各自的所有者所有。这些文件仅用于开发目的，不属于本项目的许可证范围。

### 开源库许可证信息

- **0Harmony.dll**: [MIT License](https://github.com/pardeike/Harmony/blob/master/LICENSE) - Copyright © Andreas Pardeike
- **BepInEx.Core.dll**: [LGPL-2.1 License](https://github.com/BepInEx/BepInEx/blob/master/LICENSE) - Copyright © BepInEx Contributors  
- **BepInEx.Unity.dll**: [LGPL-2.1 License](https://github.com/BepInEx/BepInEx/blob/master/LICENSE) - Copyright © BepInEx Contributors
- **UnityEngine.CoreModule.dll**: [Unity Reference-Only License](https://unity.com/legal/licenses/unity-reference-only-license) - Copyright © Unity Technologies
- **UnityEngine.dll**: [Unity Reference-Only License](https://unity.com/legal/licenses/unity-reference-only-license) - Copyright © Unity Technologies

### 专有软件

- **Elin.dll**: 专有软件 - Copyright © Lafrontier/Noa
- **Plugins.BaseCore.dll**: 专有软件 - Copyright © Lafrontier/Noa

使用本模组时，请确保您拥有相应游戏和依赖库的合法授权。请遵守各开源库的许可证条款。

## 💖 支持作者

如果这个模组对您有帮助，请考虑给项目一个⭐！
