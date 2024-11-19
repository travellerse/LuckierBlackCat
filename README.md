# LuckierBlackCat

## Description
- 去除黑猫舔物品的距离限制
- 玩家拾取物品时自动舔物品
- 祈祷时自动舔玩家背包里全部物品
- 根据【黑猫的口水】数量强化黑猫的舔物品效果

- 所有功能都需要队伍里有黑猫。
------------------------------------
- Remove the distance limit for black cats to lick items
- Automatically lick items when picking them up
- Automatically lick items which is in the PC's inventory while praying
- Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva]

- All features require a black cat in the party.
------------------------------------
- 黒猫が物を舐める距離の制限を撤廃します
- プレイヤーがアイテムを拾うと自動的に舐めます
- お祈りをすると自動的にプレイヤーのリュックの中の全てを舐めます
- 「黒猫のよだれ」の数に応じて黒猫のペロペロ効果を強化します。

- すべての機能はチーム内に黒猫が必要です。

## Config
配置文件位于`Elin\BepInEx\config\LuckierBlackCat.cfg`，可以单独配置每个功能的开关。

The configuration file is located at `Elin\BepInEx\config\LuckierBlackCat.cfg`, you can configure the switch of each function separately.

設定ファイルは`Elin\BepInEx\config\LuckierBlackCat.cfg`にあり、各機能のスイッチを個別に設定できます。

```ini
[Settings]

## Enable lick without distance limit.
# Setting type: Boolean
# Default value: true
EnableLickWithoutDist = true

## Enable lick when pick up equipment.
# Setting type: Boolean
# Default value: true
EnableLickWhenPick = true

## Enable lick when pray.
# Setting type: Boolean
# Default value: true
EnableLickWhenPray = true

## Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].
# Setting type: Boolean
# Default value: true
EnableLickEnchant = true
```

## Source
[[GitHub] https://github.com/travellerse/LuckierBlackCat](https://github.com/travellerse/LuckierBlackCat)

## Enhance the black cat's licking effect explanation
```plaintext
Element Nutrition chance:0 LV:1 mtp:1
Element Strength chance:1000 LV:1 mtp:2
Element Endurance chance:1000 LV:1 mtp:2
Element Dexterity chance:1000 LV:1 mtp:2
Element Perception chance:1000 LV:1 mtp:2
Element Learning chance:1000 LV:1 mtp:2
Element Will chance:1000 LV:1 mtp:2
Element Magic chance:1000 LV:1 mtp:2
Element Charisma chance:1000 LV:1 mtp:2
Element Critical chance:200 LV:20 mtp:1
Element Penetration chance:0 LV:1 mtp:1
Element Martial Art chance:200 LV:1 mtp:1
Element Long Sword chance:200 LV:1 mtp:1
Element Axe chance:200 LV:1 mtp:1
Element Staff chance:200 LV:1 mtp:1
Element Bow chance:200 LV:1 mtp:1
Element Gun chance:200 LV:1 mtp:1
Element Polearm chance:200 LV:1 mtp:1
Element Short Sword chance:200 LV:1 mtp:1
Element Throwing chance:200 LV:1 mtp:1
Element Crossbow chance:200 LV:1 mtp:1
Element Scythe chance:200 LV:1 mtp:1
Element Mace chance:200 LV:1 mtp:1
Element Light Armor chance:200 LV:1 mtp:1
Element Heavy Armor chance:200 LV:1 mtp:1
Element Shield chance:200 LV:1 mtp:1
Element Two Handed chance:200 LV:1 mtp:1
Element Dual Wield chance:200 LV:1 mtp:1
Element Tactics chance:200 LV:1 mtp:1
Element Marksman chance:200 LV:1 mtp:1
Element Eye of Mind chance:200 LV:1 mtp:1
Element Strategy chance:200 LV:1 mtp:1
Element Evasion chance:200 LV:1 mtp:1
Element Greater Evasion chance:200 LV:1 mtp:1
Element Stealth chance:200 LV:1 mtp:1
Element Weightlifting chance:200 LV:1 mtp:1
Element Spot Hidden chance:200 LV:1 mtp:1
Element Mining chance:20 LV:1 mtp:1
Element Lumberjacking chance:20 LV:1 mtp:1
Element Riding chance:200 LV:1 mtp:1
Element Symbiosis chance:200 LV:1 mtp:1
Element Digging chance:20 LV:1 mtp:1
Element Travel chance:200 LV:1 mtp:1
Element Music chance:20 LV:1 mtp:1
Element Fishing chance:20 LV:1 mtp:1
Element Gathering chance:20 LV:1 mtp:1
Element Carpentry chance:20 LV:1 mtp:1
Element Blacksmith chance:20 LV:1 mtp:1
Element Alchemy chance:20 LV:1 mtp:1
Element Sculpture chance:20 LV:1 mtp:1
Element Jewelry chance:20 LV:1 mtp:1
Element Weaving chance:20 LV:1 mtp:1
Element Crafting chance:20 LV:1 mtp:1
Element Lockpicking chance:200 LV:1 mtp:1
Element Pickpocket chance:200 LV:1 mtp:1
Element Literacy chance:200 LV:1 mtp:1
Element Farming chance:20 LV:1 mtp:1
Element Cooking chance:20 LV:1 mtp:1
Element Appraising chance:200 LV:1 mtp:1
Element Anatomy chance:200 LV:1 mtp:1
Element Negotiation chance:20 LV:1 mtp:1
Element Investing chance:20 LV:1 mtp:1
Element Disarm Trap chance:200 LV:1 mtp:1
Element Regeneration chance:200 LV:1 mtp:1
Element Meditation chance:200 LV:1 mtp:1
Element Control Magic chance:200 LV:1 mtp:1
Element Magic Capacity chance:200 LV:1 mtp:1
Element Casting chance:200 LV:1 mtp:1
Element Magic Device chance:200 LV:1 mtp:1
Element Faith chance:50 LV:1 mtp:1
Element Memorization chance:200 LV:1 mtp:1
Element Accelerate Ether chance:0 LV:1 mtp:1
Element Enhance Spell chance:200 LV:30 mtp:1
Element Dragon Bane chance:200 LV:30 mtp:1
Element Undead Bane chance:400 LV:1 mtp:1
Element Fairy Bane chance:200 LV:20 mtp:1
Element Animal Bane chance:800 LV:1 mtp:1
Element Man Bane chance:600 LV:1 mtp:1
Element Machine Bane chance:500 LV:20 mtp:1
Element God Bane chance:50 LV:50 mtp:1
Element Fsh Bane chance:50 LV:20 mtp:1
Element All Bane chance:2 LV:50 mtp:1
Element Mana Optimization chance:100 LV:25 mtp:1
Element Eco chance:0 LV:1 mtp:1
Element Living chance:0 LV:1 mtp:1
Element Absorb Life chance:0 LV:1 mtp:1
Element Absorb Mana chance:0 LV:1 mtp:1
Element Absorb Stamina chance:0 LV:1 mtp:1
Element Hardware Upgrade chance:0 LV:1 mtp:1
Element Melee Distance chance:0 LV:1 mtp:1
Element Fire Conversion chance:0 LV:30 mtp:1
Element Cold Conversion chance:0 LV:30 mtp:1
Element Lightning Conversion chance:0 LV:30 mtp:1
Element Impact Conversion chance:0 LV:30 mtp:1
Element Fire chance:150 LV:15 mtp:1
Element Cold chance:150 LV:15 mtp:1
Element Lightning chance:150 LV:15 mtp:1
Element Darkness chance:100 LV:30 mtp:1
Element Mind chance:100 LV:30 mtp:1
Element Poison chance:100 LV:30 mtp:1
Element Nether chance:100 LV:30 mtp:1
Element Sound chance:100 LV:30 mtp:1
Element Nerve chance:150 LV:15 mtp:1
Element Holy chance:50 LV:50 mtp:1
Element Chaos chance:50 LV:50 mtp:1
Element Magic chance:50 LV:50 mtp:1
Element Ether chance:50 LV:50 mtp:1
Element Acid chance:50 LV:50 mtp:1
Element Cut chance:50 LV:50 mtp:1
Element Fire Resistance chance:200 LV:1 mtp:5
Element Cold Resistance chance:200 LV:1 mtp:5
Element Lightning Resistance chance:200 LV:1 mtp:5
Element Darkness Resistance chance:150 LV:10 mtp:5
Element Mind Resistance chance:150 LV:10 mtp:5
Element Poison Resistance chance:150 LV:10 mtp:5
Element Nether Resistance chance:100 LV:20 mtp:5
Element Sound Resistance chance:100 LV:20 mtp:5
Element Nerve Resistance chance:150 LV:10 mtp:5
Element Chaos Resistance chance:100 LV:30 mtp:5
Element Holy Resistance chance:0 LV:50 mtp:5
Element Magic Resistance chance:0 LV:50 mtp:5
Element Ether Resistance chance:0 LV:50 mtp:5
```