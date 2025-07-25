using HarmonyLib;
using LuckierBlackCat.Core;
using LuckierBlackCat.Utils;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LuckierBlackCat.Patches
{
    /// <summary>
    /// Thing.TryLickEnchant 方法的补丁类
    /// 增强黑猫舔舐附魔效果，根据玩家拥有的"黑猫唾液"数量来增加附魔等级
    /// 原始调用：this.AddEnchant(base.LV)
    /// 修改为：this.AddEnchant(base.LV + EClass.player.CountKeyItem("well_enhance") * enchantTimes)
    /// </summary>
    [HarmonyPatch(typeof(Thing), "TryLickEnchant",
        new Type[] { typeof(Chara), typeof(bool), typeof(Chara), typeof(BodySlot) })]
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
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EClass), "get_player")),
                
                // 插入"well_enhance"字符串（黑猫唾液的内部ID）
                new CodeInstruction(OpCodes.Ldstr, "well_enhance"),
                
                // 调用 CountKeyItem 方法获取黑猫唾液数量
                new CodeInstruction(OpCodes.Callvirt,
                    AccessTools.Method(typeof(Player), "CountKeyItem", new Type[] { typeof(string) })),
                
                // 加载增强倍数
                new CodeInstruction(OpCodes.Ldc_I4, ConfigManager.EnchantTimes.Value),
                
                // 相乘：黑猫唾液数量 * 增强倍数
                new CodeInstruction(OpCodes.Mul),
                
                // 相加：基础等级 + (黑猫唾液数量 * 增强倍数)
                new CodeInstruction(OpCodes.Add),
            }).InstructionEnumeration();
        }
    }

    /// <summary>
    /// Thing.GetEnchant 方法的补丁类
    /// 在获取附魔时对附魔列表进行随机洗牌，增加随机性和多样性
    /// </summary>
    [HarmonyPatch(typeof(Thing), "GetEnchant",
        new Type[] { typeof(int), typeof(Func<SourceElement.Row, bool>), typeof(bool) })]
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
                new CodeMatch(new OpCode?(OpCodes.Ldloc_0), null, null),
                new CodeMatch(new OpCode?(OpCodes.Callvirt),
                    AccessTools.Method(typeof(List<SourceElement.Row>), "GetEnumerator"), null),
            }).Advance(1).InsertAndAdvance(new CodeInstruction[]
            {
                // 插入洗牌方法调用
                new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(CollectionUtils), "Shuffle",
                        new Type[] { typeof(System.Collections.Generic.IList<SourceElement.Row>) })),
            }).InstructionEnumeration();
        }
    }

    /// <summary>
    /// Thing.AddEnchant 方法的补丁类
    /// 用于记录附魔添加过程的调试信息
    /// </summary>
    [HarmonyPatch(typeof(Thing), "AddEnchant", new Type[] { typeof(int) })]
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
            Logger.LogInfo("Thing::AddEnchant - Name: " + __instance.Name + ", LV: " + lv + ", Rarity: " + __instance.rarity);

            // 以下是被注释的调试代码，用于分析所有可用的附魔元素
            /*
            Func<SourceElement.Row, bool> func = (SourceElement.Row r) => r.IsEncAppliable(__instance.category);
            foreach (SourceElement.Row row in EClass.sources.elements.rows)
            {
                if ((!row.tag.Contains("flag")) && func(row))
                    Logger.LogInfo("Element " + row.name + " - Chance: " + row.chance + ", LV: " + row.LV + ", MTP: " + row.mtp);
            }
            */
        }
    }
}
