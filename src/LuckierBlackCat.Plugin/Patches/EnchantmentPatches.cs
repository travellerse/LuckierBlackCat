using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LuckierBlackCat.Core;
using LuckierBlackCat.Patching;

namespace LuckierBlackCat.Patches
{
    [HarmonyPatch(
        typeof(Thing),
        "TryLickEnchant",
        new Type[] { typeof(Chara), typeof(bool), typeof(Chara), typeof(BodySlot) })]
    public static class ThingTryLickEnchantPatch
    {
        public static void Validate()
        {
            var target = TargetMethod();
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
            return InstructionTransforms.InsertLevelAdjustment(
                instructions,
                AccessTools.PropertyGetter(typeof(Card), "LV"),
                AccessTools.Method(typeof(Thing), "AddEnchant", new Type[] { typeof(int) }),
                AccessTools.Method(typeof(ThingTryLickEnchantPatch), nameof(AdjustLevel)),
                "Thing.TryLickEnchant");
        }

        public static int AdjustLevel(int baseLevel)
        {
            int salivaCount = EClass.player.CountKeyItem("well_enhance");
            return LickRules.ComputeEnchantLevel(
                baseLevel,
                salivaCount,
                ConfigManager.EnchantTimes.Value);
        }

        private static MethodInfo TargetMethod()
        {
            return AccessTools.Method(
                    typeof(Thing),
                    "TryLickEnchant",
                    new Type[] { typeof(Chara), typeof(bool), typeof(Chara), typeof(BodySlot) })
                ?? throw new MissingMethodException(
                    "Missing target: Thing.TryLickEnchant(Chara, Boolean, Chara, BodySlot).");
        }
    }
}
