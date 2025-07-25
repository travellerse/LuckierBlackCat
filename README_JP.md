# LuckierBlackCat - より幸運な黒猫

[![バージョン](https://img.shields.io/badge/バージョン-0.4.1.0-blue.svg)](https://github.com/travellerse/LuckierBlackCat/releases)
[![ライセンス](https://img.shields.io/badge/ライセンス-MIT-green.svg)](LICENSE.txt)
[![ゲーム](https://img.shields.io/badge/ゲーム-Elin-orange.svg)](https://store.steampowered.com/app/2135150/Elin/)

> **言語バージョン**: [中文](README.md) | [English](README_EN.md) | [日本語](README_JP.md)

Elinゲーム用に開発されたBepInExモッド。黒猫のアイテム舐め機能を大幅に強化し、装備をより幸運にします！

## 📖 機能紹介

### 🎯 コア機能

- **距離制限なしアイテム舐め** - 黒猫のアイテム舐めの距離制限を撤廃し、マップのどこからでも発動可能
- **拾得時自動舐め** - プレイヤーが装備を拾った際に自動で黒猫のアイテム舐めを発動
- **祈祷時一括舐め** - 祈祷時にプレイヤーのインベントリ内の条件を満たすすべての装備を自動で舐める
- **舐め効果強化** - 【黒猫のよだれ】の数量に基づいて黒猫の舐め効果を強化

### ⚠️ 使用条件

- **すべての機能にはパーティメンバーが【エヘカトルの祝福】を持っている必要があります**
- 装備と遠距離武器にのみ有効
- 呪われたアイテムには無効
- レア度が一般より高いアイテムにのみ有効
- すでに舐められたアイテムには重複して効果なし

## 🔧 インストール方法

### 自動インストール

1. [Steamワークショップ](https://steamcommunity.com/sharedfiles/filedetails/?id=3366709105)でサブスクライブ

### 手動インストール

1. [最新バージョン](https://github.com/travellerse/LuckierBlackCat/releases)をダウンロードして解凍
2. `modフォルダ`を`Elin\Package\`ディレクトリに配置
3. ゲームを起動してモッドを有効化

## ⚙️ 設定説明

設定ファイルの場所：`Elin\BepInEx\config\LuckierBlackCat.cfg`

### 設定可能オプション

```ini
[Settings]

## 距離制限なし舐め機能を有効化
# タイプ: Boolean
# デフォルト値: true
EnableLickWithoutDist = true

## 装備拾得時自動舐め機能を有効化
# タイプ: Boolean  
# デフォルト値: true
EnableLickWhenPick = true

## 祈祷時自動舐め機能を有効化
# タイプ: Boolean
# デフォルト値: true
EnableLickWhenPray = true

## 黒猫のよだれ数量による舐め効果強化を有効化
# タイプ: Boolean
# デフォルト値: true
EnableLickEnchant = true

## 強化効果倍率
# タイプ: Integer
# デフォルト値: 1
EnchantTimes = 1
```

## 🔗 プロジェクトリンク

- **GitHubリポジトリ**: [https://github.com/travellerse/LuckierBlackCat](https://github.com/travellerse/LuckierBlackCat)
- **問題報告**: [https://github.com/travellerse/LuckierBlackCat/issues](https://github.com/travellerse/LuckierBlackCat/issues)
- **Elin公式ページ**: [https://store.steampowered.com/app/2135150/Elin/](https://store.steampowered.com/app/2135150/Elin/)

## 📄 ライセンス

このプロジェクトはMITライセンスの下で提供されています - 詳細は[LICENSE.txt](LICENSE.txt)ファイルをご覧ください

## ⚠️ サードパーティ著作権表示

本プロジェクトの`ref/`フォルダに含まれるすべてのサードパーティライブラリファイルの著作権は、それぞれの所有者に帰属します。これらのファイルは開発目的でのみ使用され、本プロジェクトのライセンス範囲には含まれません。

### オープンソースライブラリのライセンス情報

- **0Harmony.dll**: [MITライセンス](https://github.com/pardeike/Harmony/blob/master/LICENSE) - Copyright © Andreas Pardeike
- **BepInEx.Core.dll**: [LGPL-2.1ライセンス](https://github.com/BepInEx/BepInEx/blob/master/LICENSE) - Copyright © BepInEx Contributors  
- **BepInEx.Unity.dll**: [LGPL-2.1ライセンス](https://github.com/BepInEx/BepInEx/blob/master/LICENSE) - Copyright © BepInEx Contributors
- **UnityEngine.CoreModule.dll**: [Unity Reference-Only License](https://unity.com/legal/licenses/unity-reference-only-license) - Copyright © Unity Technologies
- **UnityEngine.dll**: [Unity Reference-Only License](https://unity.com/legal/licenses/unity-reference-only-license) - Copyright © Unity Technologies

### プロプライエタリソフトウェア

- **Elin.dll**: プロプライエタリソフトウェア - Copyright © Lafrontier/Noa
- **Plugins.BaseCore.dll**: プロプライエタリソフトウェア - Copyright © Lafrontier/Noa

このモッドを使用する際は、対応するゲームと依存ライブラリの正当な認可を持っていることを確認してください。各オープンソースライブラリのライセンス条項に従ってください。

## 💖 作者サポート

このモッドがお役に立ちましたら、プロジェクトに⭐をお願いします！
