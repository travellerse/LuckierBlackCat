using System;
using System.Collections.Generic;
using HarmonyLib;
using LuckierBlackCat.Patching;

namespace LuckierBlackCat.Patches
{
    [HarmonyPatch(typeof(ThingGen), "TryLickChest")]
    public static class ThingGenTryLickChestPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            return InstructionTransforms.ReplaceDistanceThreshold(
                instructions,
                AccessTools.Method(typeof(Card), "Dist", new Type[] { typeof(Card) }),
                5,
                1024,
                "ThingGen.TryLickChest");
        }
    }

    [HarmonyPatch(typeof(Card), "SpawnLoot")]
    public static class SpawnLootPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            return InstructionTransforms.ReplaceDistanceThreshold(
                instructions,
                AccessTools.Method(typeof(Card), "Dist", new Type[] { typeof(Point) }),
                3,
                1024,
                "Card.SpawnLoot");
        }
    }
}
