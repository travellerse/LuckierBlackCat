using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace LuckierBlackCat
{
    /// <summary>
    /// Luckier Black Cat 插件主类
    /// 这是一个用于游戏 Elin 的 BepInEx 插件，主要功能是增强黑猫的舔舐效果
    /// </summary>
    [BepInPlugin("com.travellerse.plugins.LuckierBlackCat", "Luckier Black Cat", "0.4.1.0")]
    [BepInProcess("Elin.exe")]
    public class LuckierBlackCat : BaseUnityPlugin
    {
        /// <summary>
        /// 日志记录器，用于输出调试信息
        /// </summary>
        public static new ManualLogSource Logger;

        /// <summary>
        /// 配置文件，存储用户设置
        /// </summary>
        ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "LuckierBlackCat.cfg"), true);

        // 配置项：各种功能的开关
        /// <summary>
        /// 配置项：启用无距离限制的舔舐功能
        /// </summary>
        private ConfigEntry<bool> _enableLickWithoutDist;

        /// <summary>
        /// 配置项：启用拾取装备时的舔舐功能
        /// </summary>
        private ConfigEntry<bool> _enableLickWhenPick;

        /// <summary>
        /// 配置项：启用祈祷时的舔舐功能
        /// </summary>
        private ConfigEntry<bool> _enableLickWhenPray;

        /// <summary>
        /// 配置项：启用增强版舔舐附魔功能
        /// </summary>
        private ConfigEntry<bool> _enableLickEnchant;

        /// <summary>
        /// 配置项：附魔增强倍数
        /// </summary>
        public static ConfigEntry<int> enchantTimes;

        /// <summary>
        /// 插件初始化方法，在插件加载时调用
        /// 设置配置项并根据配置启用相应的 Harmony 补丁
        /// </summary>
        public void Awake()
        {
            // 初始化日志记录器
            Logger = base.Logger;
            Logger.LogInfo("Luckier Black Cat loaded!");

            // 创建 Harmony 实例，用于运行时代码修改
            Harmony harmony = new Harmony("com.travellerse.plugins.LuckierBlackCat");

            // 绑定配置项，设置默认值和描述
            _enableLickWithoutDist = configFile.Bind("Settings", "EnableLickWithoutDist", true,
                "Enable lick without distance limit.");
            _enableLickWhenPick = configFile.Bind("Settings", "EnableLickWhenPick", true,
                "Enable lick when pick up equipment.");
            _enableLickWhenPray = configFile.Bind("Settings", "EnableLickWhenPray", true, "Enable lick when pray.");
            _enableLickEnchant = configFile.Bind("Settings", "EnableLickEnchant", true,
                "Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].");
            enchantTimes = configFile.Bind("Settings", "EnchantTimes", 1, "The times of enhance");

            // 根据配置启用相应功能的补丁
            if (_enableLickWithoutDist.Value)
            {
                // 应用无距离限制舔舐的补丁
                harmony.PatchAll(typeof(ThingGenTryLickChestPatch));
                harmony.PatchAll(typeof(SpawnLootPatch));
                LuckierBlackCat.Logger.LogInfo("Enable lick without distance limit.");
            }

            if (_enableLickWhenPick.Value)
            {
                // 应用拾取装备时舔舐的补丁
                harmony.PatchAll(typeof(CharaPickPatch));
                LuckierBlackCat.Logger.LogInfo("Enable lick when pick up equipment.");
            }

            if (_enableLickWhenPray.Value)
            {
                // 应用祈祷时舔舐的补丁
                harmony.PatchAll(typeof(ActPrayTryPrayPatch));
                LuckierBlackCat.Logger.LogInfo("Enable lick when pray.");
            }

            if (_enableLickEnchant.Value)
            {
                // 应用增强舔舐附魔效果的补丁
                harmony.PatchAll(typeof(ThingTryLickEnchantPatch));
                harmony.PatchAll(typeof(ThingGetEnchantPatch));
                harmony.PatchAll(typeof(ThingAddEnchantPatch));
                LuckierBlackCat.Logger.LogInfo(
                    "Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].");
            }

            Logger.LogInfo("Luckier Black Cat patched!");
        }
    }

    /// <summary>
    /// ThingGen.TryLickChest 方法的补丁类
    /// 用于移除舔舐宝箱时的距离限制，将距离检查修改为一个较大的值（1024）
    /// </summary>
    [HarmonyPatch(typeof(ThingGen), "TryLickChest")]
    public static class ThingGenTryLickChestPatch
    {
        /// <summary>
        /// 使用 Transpiler 修改原方法的 IL 代码
        /// 查找距离比较的指令并将距离值修改为 1024，从而绕过距离限制
        /// </summary>
        /// <param name="instructions">原方法的 IL 指令序列</param>
        /// <returns>修改后的 IL 指令序列</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // 遍历所有指令，查找距离检查相关的代码
            for (int i = 0; i + 2 < codes.Count; i++)
            {
                // 查找调用 Dist 相关方法的指令
                if (codes[i].opcode == System.Reflection.Emit.OpCodes.Callvirt &&
                    codes[i].operand.ToString().Contains("Dist"))
                {
                    //LuckierBlackCat.Logger.LogInfo("ThingGen::TryLickChest " + "codes:" + codes[i + 2].operand.ToString());

                    // 检查是否是大于等于比较指令（距离检查）
                    if (codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge || codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge_S)
                    {
                        //LuckierBlackCat.Logger.LogInfo("ThingGen" + "::TryLickChest");

                        // 将比较的常数值修改为 1024，有效移除距离限制
                        codes[i + 1].opcode = System.Reflection.Emit.OpCodes.Ldc_I4;
                        codes[i + 1].operand = 1024;
                        break;
                    }
                }
            }
            return codes;
        }
    }

    /// <summary>
    /// Card.SpawnLoot 方法的补丁类
    /// 功能与 ThingGenTryLickChestPatch 类似，用于移除生成战利品时的距离限制
    /// </summary>
    [HarmonyPatch(typeof(Card), "SpawnLoot")]
    public static class SpawnLootPatch
    {
        /// <summary>
        /// 使用 Transpiler 修改原方法的 IL 代码
        /// 查找距离比较的指令并将距离值修改为 1024，从而绕过距离限制
        /// </summary>
        /// <param name="instructions">原方法的 IL 指令序列</param>
        /// <returns>修改后的 IL 指令序列</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // 遍历所有指令，查找距离检查相关的代码
            for (int i = 0; i + 2 < codes.Count; i++)
            {
                // 查找调用 Dist 相关方法的指令
                if (codes[i].opcode == System.Reflection.Emit.OpCodes.Callvirt &&
                    codes[i].operand.ToString().Contains("Dist"))
                {
                    //LuckierBlackCat.Logger.LogInfo("Card::SpawnLoot " + "codes:" + codes[i + 2].operand.ToString());

                    // 检查是否是大于等于比较指令（距离检查）
                    if (codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge || codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge_S)
                    {
                        //LuckierBlackCat.Logger.LogInfo("Card" + "::SpawnLoot");

                        // 将比较的常数值修改为 1024，有效移除距离限制
                        codes[i + 1].opcode = System.Reflection.Emit.OpCodes.Ldc_I4;
                        codes[i + 1].operand = 1024;
                        break;
                    }
                }
            }
            return codes;
        }
    }

    /// <summary>
    /// Thing.TryLickEnchant 方法的补丁类
    /// 增强黑猫舔舐附魔效果，根据玩家拥有的"黑猫唾液"数量来增加附魔等级
    /// 原始调用：this.AddEnchant(base.LV)
    /// 修改为：this.AddEnchant(base.LV + EClass.player.CountKeyItem("well_enhance") * enchantTimes)
    /// </summary>
    //System.Void Thing::TryLickEnchant(Chara,System.Boolean,Chara,BodySlot)
    //this.AddEnchant(base.LV);  -->  this.AddEnchant(base.LV + EClass.player.CountKeyItem("well_enhance"));
    [HarmonyPatch(typeof(Thing), "TryLickEnchant",
        new System.Type[] { typeof(Chara), typeof(bool), typeof(Chara), typeof(BodySlot) })]
    public static class ThingTryLickEnchantPatch
    {
        /// <summary>
        /// 使用 Transpiler 修改原方法的 IL 代码
        /// 在调用 AddEnchant 方法前，将基础等级与黑猫唾液增强效果相加
        /// </summary>
        /// <param name="instructions">原方法的 IL 指令序列</param>
        /// <returns>修改后的 IL 指令序列</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions, null).MatchForward(false, new CodeMatch[]
            {
                // 匹配获取基础等级和调用 AddEnchant 的指令
                new CodeMatch(new OpCode?(OpCodes.Call), AccessTools.Method(typeof(Card), "get_LV"), null),
                new CodeMatch(new OpCode?(OpCodes.Call), AccessTools.Method(typeof(Thing), "AddEnchant"), null),
            }).Advance(1).InsertAndAdvance(new CodeInstruction[]
            {
                // 插入获取玩家实例的指令
                //call	class Player EClass::get_player()
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EClass), "get_player")),
                
                // 插入"well_enhance"字符串（黑猫唾液的内部ID）
                //ldstr	"well_enhance"
                new CodeInstruction(OpCodes.Ldstr, "well_enhance"),
                
                // 调用 CountKeyItem 方法获取黑猫唾液数量
                //callvirt	instance int32 Player::CountKeyItem(string)
                new CodeInstruction(OpCodes.Callvirt,
                    AccessTools.Method(typeof(Player), "CountKeyItem", new System.Type[] { typeof(string) })),
                
                // 加载增强倍数
                //ldc.i4
                new CodeInstruction(OpCodes.Ldc_I4, LuckierBlackCat.enchantTimes.Value),
                
                // 相乘：黑猫唾液数量 * 增强倍数
                //mul
                new CodeInstruction(OpCodes.Mul),
                
                // 相加：基础等级 + (黑猫唾液数量 * 增强倍数)
                //add
                new CodeInstruction(OpCodes.Add),
            }).InstructionEnumeration();
        }
    }

    /// <summary>
    /// Thing.GetEnchant 方法的补丁类
    /// 在获取附魔时对附魔列表进行随机洗牌，增加随机性和多样性
    /// </summary>
    //System.Tuple`2<SourceElement/Row,System.Int32> Thing::GetEnchant(System.Int32,System.Func`2<SourceElement/Row,System.Boolean>,System.Boolean)
    //public static Tuple<SourceElement.Row, int> GetEnchant(int lv, Func<SourceElement.Row, bool> func, bool neg)
    [HarmonyPatch(typeof(Thing), "GetEnchant",
        new System.Type[] { typeof(int), typeof(System.Func<SourceElement.Row, bool>), typeof(bool) })]
    public static class ThingGetEnchantPatch
    {
        /// <summary>
        /// 使用 Transpiler 修改原方法的 IL 代码
        /// 在遍历附魔列表前对其进行洗牌操作
        /// </summary>
        /// <param name="instructions">原方法的 IL 指令序列</param>
        /// <returns>修改后的 IL 指令序列</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions, null).MatchForward(false, new CodeMatch[]
            {
                // 匹配加载本地变量和获取枚举器的指令
                //ldloc.0
                new CodeMatch(new OpCode?(OpCodes.Ldloc_0), null, null),
                //callvirt    instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib] System.Collections.Generic.List`1<class SourceElement/Row>::GetEnumerator()
                new CodeMatch(new OpCode?(OpCodes.Callvirt),
                    AccessTools.Method(typeof(List<SourceElement.Row>), "GetEnumerator"), null),
            }).Advance(1).InsertAndAdvance(new CodeInstruction[]
            {
                // 插入洗牌方法调用
                //call    class [mscorlib] System.Collections.Generic.IList`1<!!0> [Plugins.BaseCore] ClassExtension::Shuffle<class [Elin] SourceElement/Row>(class [mscorlib] System.Collections.Generic.IList`1<!!0>)
                //list.Shuffle<SourceElement.Row>()
                new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(ThingGetEnchantPatch), "Shuffle",
                        new System.Type[] { typeof(IList<SourceElement.Row>) })),
            }).InstructionEnumeration();
        }

        /// <summary>
        /// Fisher-Yates 洗牌算法实现
        /// 对附魔源元素列表进行随机洗牌，确保每次获取附魔时都有不同的顺序
        /// </summary>
        /// <param name="list">需要洗牌的附魔源元素列表</param>
        /// <returns>洗牌后的新列表</returns>
        //Shuffle
        private static List<SourceElement.Row> Shuffle(IList<SourceElement.Row> list)
        {
            // 创建输入列表的副本
            List<SourceElement.Row> list2 = new List<SourceElement.Row>(list);

            // Fisher-Yates 洗牌算法
            for (int i = 0; i < list2.Count; i++)
            {
                // 从当前位置到列表末尾随机选择一个索引
                int index = UnityEngine.Random.Range(i, list2.Count);

                // 交换当前位置和随机位置的元素
                SourceElement.Row value = list2[i];
                list2[i] = list2[index];
                list2[index] = value;
            }
            return list2;
        }
    }

    /// <summary>
    /// Thing.AddEnchant 方法的补丁类
    /// 用于记录附魔添加过程的调试信息
    /// </summary>
    //Element Thing::AddEnchant(System.Int32)
    [HarmonyPatch(typeof(Thing), "AddEnchant", new System.Type[] { typeof(int) })]
    public static class ThingAddEnchantPatch
    {
        /// <summary>
        /// 在 AddEnchant 方法执行后调用，记录附魔信息
        /// </summary>
        /// <param name="__instance">调用方法的 Thing 实例</param>
        /// <param name="__result">返回的 Element 结果</param>
        /// <param name="lv">附魔等级参数</param>
        private static void Postfix(Thing __instance, ref Element __result, int lv)
        {
            // 记录附魔添加的详细信息，包括物品名称、等级和稀有度
            LuckierBlackCat.Logger.LogInfo("Thing" + "::AddEnchant name:" + __instance.Name + " LV:" + lv + " rarity:" +
                                           __instance.rarity);

            // 以下是被注释的调试代码，用于分析所有可用的附魔元素
            //Func<SourceElement.Row, bool> func = (SourceElement.Row r) => r.IsEncAppliable(__instance.category);
            //foreach (SourceElement.Row row in EClass.sources.elements.rows)
            //{
            //    if ((!row.tag.Contains("flag")) && func(row))
            //        LuckierBlackCat.Logger.LogInfo("Element " + row.name + " chance:" + row.chance + " LV:" + row.LV + " mtp:" + row.mtp);
            //}
        }
    }


    /// <summary>
    /// Chara.Pick 方法的补丁类
    /// 当玩家角色拾取装备时，自动触发黑猫的舔舐附魔效果
    /// </summary>
    //Thing Chara::Pick(Thing,System.Boolean,System.Boolean)
    [HarmonyPatch(typeof(Chara), "Pick", new System.Type[] { typeof(Thing), typeof(bool), typeof(bool) })]
    public static class CharaPickPatch
    {
        /// <summary>
        /// 在 Pick 方法执行前调用，检查并触发舔舐附魔
        /// </summary>
        /// <param name="__instance">调用方法的角色实例</param>
        /// <param name="t">要拾取的物品</param>
        /// <param name="msg">是否显示消息</param>
        /// <param name="tryStack">是否尝试堆叠</param>
        /// <returns>是否继续执行原方法</returns>
        private static bool Prefix(Chara __instance, ref Thing t, ref bool msg, ref bool tryStack)
        {
            // 只处理玩家角色的拾取行为
            if (!__instance.IsPC) return true;

            // 记录拾取行为的详细信息
            LuckierBlackCat.Logger.LogInfo("Chara" + __instance.Name + "::Pick" + t.Name + msg + tryStack);

            // 检查是否为装备或远程武器
            if (!t.IsEquipmentOrRanged)
            {
                return true;
            }

            // 跳过被诅咒的物品或普通品质及以下的物品
            if (t.IsCursed || t.rarity <= Rarity.Normal)
            {
                return true;
            }

            // 检查物品是否已经被附魔过（GetInt(107) 可能是附魔次数或状态）
            if (t.GetInt(107, null) > 0)
            {
                return true;
            }

            // 遍历地图上的所有角色，寻找有舔舐能力的黑猫
            foreach (Chara chara in EClass._map.charas)
            {
                // 检查角色是否拥有舔舐能力（元素ID 1412）
                if (chara.HasElement(1412, 1))
                {
                    // 如果需要显示消息
                    if (msg)
                    {
                        // 让黑猫说出舔舐的话并播放音效
                        chara.Say("lick", chara, t, null, null);
                        t.PlaySound("offering", 1f, true);
                    }

                    // 对物品进行舔舐附魔
                    t.TryLickEnchant(chara, false);
                    break; // 只需要一只黑猫进行舔舐
                }
            }

            return true; // 继续执行原方法
        }
    }

    /// <summary>
    /// ActPray.TryPray 方法的补丁类
    /// 当角色进行祈祷时，自动让黑猫对其身上符合条件的装备进行舔舐附魔
    /// </summary>
    //System.Boolean ActPray::TryPray(Chara,System.Boolean)
    //public static bool TryPray(Chara c, bool passive = false)
    [HarmonyPatch(typeof(ActPray), "TryPray", new System.Type[] { typeof(Chara), typeof(bool) })]
    public static class ActPrayTryPrayPatch
    {
        /// <summary>
        /// 在 TryPray 方法执行后调用，为祈祷角色的装备进行舔舐附魔
        /// </summary>
        /// <param name="c">进行祈祷的角色</param>
        /// <param name="passive">是否为被动祈祷</param>
        private static void Postfix(Chara c, bool passive)
        {
            // 记录祈祷行为
            LuckierBlackCat.Logger.LogInfo("ActPray::TryPray" + c.Name + passive);

            // 遍历地图上的所有角色，寻找有舔舐能力的黑猫
            foreach (Chara chara in EClass._map.charas)
            {
                // 检查角色是否拥有舔舐能力（元素ID 1412）
                if (chara.HasElement(1412, 1))
                {
                    // 遍历祈祷角色身上的所有物品
                    foreach (Thing thing in c.things)
                    {
                        // 检查物品是否符合舔舐条件：
                        // 1. 是装备或远程武器
                        // 2. 没有被诅咒
                        // 3. 稀有度高于普通
                        // 4. 没有被附魔过（GetInt(107) <= 0）
                        if (thing.IsEquipmentOrRanged && !thing.IsCursed && thing.rarity > Rarity.Normal &&
                            thing.GetInt(107, null) <= 0)
                        {
                            // 让黑猫说出舔舐的话并播放音效
                            chara.Say("lick", chara, thing, null, null);
                            thing.PlaySound("offering", 1f, true);

                            // 对物品进行舔舐附魔
                            thing.TryLickEnchant(chara, false);

                            // 记录附魔详细信息用于调试
                            LuckierBlackCat.Logger.LogInfo("[Enchant]" + c.Name + passive + " " + thing.Name +
                                                           thing.IsEquipmentOrRanged + !thing.IsCursed + " " +
                                                           thing.rarity + " " + thing.GetInt(107, null));
                        }
                    }
                }
            }
        }
    }
}
