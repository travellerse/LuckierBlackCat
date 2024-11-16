using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

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
    public class ThingGenTryLickChestPatch
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
    public class SpawnLootPatch
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
}
