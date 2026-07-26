using HarmonyLib;
using LuckierBlackCat.Core;
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
}
