using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LuckierBlackCat.Patching;

namespace LuckierBlackCat.Patches
{
    [HarmonyPatch(typeof(ThingGen), "TryLickChest")]
    public static class ThingGenTryLickChestPatch
    {
        public static void Validate()
        {
            var target = RequireTargetMethod();
            Transform(PatchProcessor.GetCurrentInstructions(target));
        }

        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            return Transform(instructions);
        }

        private static IEnumerable<CodeInstruction> Transform(
            IEnumerable<CodeInstruction> instructions)
        {
            return InstructionTransforms.ReplaceDistanceThreshold(
                instructions,
                AccessTools.Method(typeof(Card), "Dist", new Type[] { typeof(Card) }),
                5,
                1024,
                "ThingGen.TryLickChest");
        }

        private static MethodInfo RequireTargetMethod()
        {
            return AccessTools.Method(typeof(ThingGen), "TryLickChest", new Type[] { typeof(Thing) })
                ?? throw new MissingMethodException("Missing target: ThingGen.TryLickChest(Thing).");
        }
    }

    [HarmonyPatch(typeof(Card), "SpawnLoot")]
    public static class SpawnLootPatch
    {
        public static void Validate()
        {
            var target = RequireTargetMethod();
            Transform(PatchProcessor.GetCurrentInstructions(target));
        }

        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            return Transform(instructions);
        }

        private static IEnumerable<CodeInstruction> Transform(
            IEnumerable<CodeInstruction> instructions)
        {
            return InstructionTransforms.ReplaceDistanceThreshold(
                instructions,
                AccessTools.Method(typeof(Card), "Dist", new Type[] { typeof(Point) }),
                3,
                1024,
                "Card.SpawnLoot");
        }

        private static MethodInfo RequireTargetMethod()
        {
            return AccessTools.Method(typeof(Card), "SpawnLoot", new Type[] { typeof(Card) })
                ?? throw new MissingMethodException("Missing target: Card.SpawnLoot(Card).");
        }
    }
}
