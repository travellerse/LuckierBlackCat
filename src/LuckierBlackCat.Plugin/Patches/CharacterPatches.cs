using System;
using HarmonyLib;
using LuckierBlackCat.Core;
using LuckierBlackCat.Utils;

namespace LuckierBlackCat.Patches
{
    [HarmonyPatch(typeof(Chara), "Pick", new Type[] { typeof(Thing), typeof(bool), typeof(bool) })]
    public static class CharaPickPatch
    {
        private static void Prefix(Chara __instance, Thing t, out bool __state)
        {
            __state = t.GetRootCard() == __instance;
        }

        private static void Postfix(
            Chara __instance,
            Thing __result,
            bool msg,
            bool __state)
        {
            if (__result == null)
            {
                return;
            }

            bool resultOwnedByPlayer = __result.GetRootCard() == __instance;
            if (!LickRules.ShouldLickAfterPickup(
                    __instance.IsPC,
                    __state,
                    resultOwnedByPlayer,
                    BlackCatUtils.ToItemFacts(__result)))
            {
                return;
            }

            BlackCatUtils.TryLickItem(__result, msg);
        }
    }

    [HarmonyPatch(typeof(ActPray), "TryPray", new Type[] { typeof(Chara), typeof(bool) })]
    public static class ActPrayTryPrayPatch
    {
        private static void Postfix(Chara c, bool passive, bool __result)
        {
            if (!LickRules.ShouldProcessPrayer(c.IsPC, __result, passive))
            {
                return;
            }

            int lickCount = BlackCatUtils.LickAllEligibleItems(c, true);
            if (lickCount > 0)
            {
                Logger.LogInfo("Prayer triggered licking for " + lickCount + " items on " + c.Name);
            }
        }
    }
}
