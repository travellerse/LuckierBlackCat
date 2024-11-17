# LuckierBlackCat

## Description
- 去除黑猫舔物品的距离限制
- 玩家拾取物品时自动舔物品
- 祈祷时自动舔物品

- 所有功能都需要队伍里有黑猫。
------------------------------------
- Remove the distance limit for black cats to lick items
- Automatically lick items when picking them up
- Automatically lick items while praying

- All features require a black cat in the party.
------------------------------------
- 黒猫が物を舐める距離の制限を撤廃します
- プレイヤーがアイテムを拾うと自動的に舐めます
- 祈りの時に自動的に物を舐めます

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
```

## Source
[[GitHub] https://github.com/travellerse/LuckierBlackCat](https://github.com/travellerse/LuckierBlackCat)
