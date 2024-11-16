using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LuckierBlackCat
{
    [BepInPlugin("com.travellerse.plugins.LuckierBlackCat", "Luckier Black Cat", "0.1.0.0")]
    [BepInProcess("Elin.exe")]
    public class LuckierBlackCat : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("Luckier Black Cat loaded!");
            Harmony harmony = new Harmony("com.travellerse.plugins.LuckierBlackCat");
            harmony.PatchAll();
            Logger.LogInfo("Luckier Black Cat patched!");
        }
    }

    [HarmonyPatch(typeof(ThingGen), "TryLickChest")]
    public static class ThingGenTryLickChestPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == System.Reflection.Emit.OpCodes.Callvirt && codes[i].operand.ToString().Contains("Dist"))
                {
                    if (codes[i + 1].opcode == System.Reflection.Emit.OpCodes.Ldc_I4_3)
                    {
                        codes[i + 1].opcode = System.Reflection.Emit.OpCodes.Ldc_I4;
                        codes[i + 1].operand = 1024;
                        break;
                    }
                }
            }
            return codes;
        }
    }

    [HarmonyPatch(typeof(Card), "SpawnLoot")]
    public static class SpawnLootPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == System.Reflection.Emit.OpCodes.Callvirt && codes[i].operand.ToString().Contains("Dist"))
                {
                    if (codes[i + 1].opcode == System.Reflection.Emit.OpCodes.Ldc_I4_3)
                    {
                        codes[i + 1].opcode = System.Reflection.Emit.OpCodes.Ldc_I4;
                        codes[i + 1].operand = 1024;
                        break;
                    }
                }
            }
            return codes;
        }
    }

    //Thing Chara::Pick(Thing,System.Boolean,System.Boolean)
    [HarmonyPatch(typeof(Chara), "Pick", new System.Type[] { typeof(Thing), typeof(bool), typeof(bool) })]
    public static class CharaPickPatch
    {
        private static bool Prefix(Chara __instance, ref Thing t, ref bool msg, ref bool tryStack)
        {
            if (!__instance.IsPC) return true;
            LuckierBlackCat.Logger.LogInfo("Chara" + __instance.Name + "::Pick" + t.Name + msg + tryStack);
            if (!t.IsEquipmentOrRanged)
            {
                return true;
            }
            if (t.IsCursed || t.rarity <= Rarity.Normal)
            {
                return true;
            }
            if (t.GetInt(107, null) > 0)
            {
                return true;
            }
            foreach (Chara chara in EClass._map.charas)
            {
                if (chara.HasElement(1412, 1))
                {
                    if (msg)
                    {
                        chara.Say("lick", chara, t, null, null);
                        t.PlaySound("offering", 1f, true);
                    }
                    t.TryLickEnchant(chara, false);
                    break;
                }
            }
            return true;
        }
    }
}
