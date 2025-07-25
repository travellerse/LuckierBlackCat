using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LuckierBlackCat.Patches
{
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
                if (codes[i].opcode == OpCodes.Callvirt &&
                    codes[i].operand.ToString().Contains("Dist"))
                {
                    // 检查是否是大于等于比较指令（距离检查）
                    if (codes[i + 2].opcode == OpCodes.Bge || codes[i + 2].opcode == OpCodes.Bge_S)
                    {
                        // 将比较的常数值修改为 1024，有效移除距离限制
                        codes[i + 1].opcode = OpCodes.Ldc_I4;
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
                if (codes[i].opcode == OpCodes.Callvirt &&
                    codes[i].operand.ToString().Contains("Dist"))
                {
                    // 检查是否是大于等于比较指令（距离检查）
                    if (codes[i + 2].opcode == OpCodes.Bge || codes[i + 2].opcode == OpCodes.Bge_S)
                    {
                        // 将比较的常数值修改为 1024，有效移除距离限制
                        codes[i + 1].opcode = OpCodes.Ldc_I4;
                        codes[i + 1].operand = 1024;
                        break;
                    }
                }
            }
            return codes;
        }
    }
}
