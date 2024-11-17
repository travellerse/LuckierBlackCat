using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

namespace LuckierBlackCat
{
    [BepInPlugin("com.travellerse.plugins.LuckierBlackCat", "Luckier Black Cat", "0.2.0.0")]
    [BepInProcess("Elin.exe")]
    public class LuckierBlackCat : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "LuckierBlackCat.cfg"), true);

        private ConfigEntry<bool> _enableLickWithoutDist;
        private ConfigEntry<bool> _enableLickWhenPick;
        private ConfigEntry<bool> _enableLickWhenPray;
        private ConfigEntry<bool> _enableLickEnchant;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("Luckier Black Cat loaded!");
            Harmony harmony = new Harmony("com.travellerse.plugins.LuckierBlackCat");
            _enableLickWithoutDist = configFile.Bind("Settings", "EnableLickWithoutDist", true, "Enable lick without distance limit.");
            _enableLickWhenPick = configFile.Bind("Settings", "EnableLickWhenPick", true, "Enable lick when pick up equipment.");
            _enableLickWhenPray = configFile.Bind("Settings", "EnableLickWhenPray", true, "Enable lick when pray.");
            _enableLickEnchant = configFile.Bind("Settings", "EnableLickEnchant", true, "Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].");

            if (_enableLickWithoutDist.Value)
            {
                harmony.PatchAll(typeof(ThingGenTryLickChestPatch));
                harmony.PatchAll(typeof(SpawnLootPatch));
                LuckierBlackCat.Logger.LogInfo("Enable lick without distance limit.");
            }
            if (_enableLickWhenPick.Value)
            {
                harmony.PatchAll(typeof(CharaPickPatch));
                LuckierBlackCat.Logger.LogInfo("Enable lick when pick up equipment.");
            }
            if (_enableLickWhenPray.Value)
            {
                harmony.PatchAll(typeof(ActPrayTryPrayPatch));
                LuckierBlackCat.Logger.LogInfo("Enable lick when pray.");
            }
            if (_enableLickEnchant.Value)
            {
                harmony.PatchAll(typeof(ThingTryLickEnchantPatch));
                harmony.PatchAll(typeof(ThingAddEnchantPatch));
                LuckierBlackCat.Logger.LogInfo("Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].");
            }

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

    //System.Void Thing::TryLickEnchant(Chara,System.Boolean,Chara,BodySlot)
    //this.AddEnchant(base.LV);  -->  this.AddEnchant(base.LV + EClass.player.CountKeyItem("well_enhance"));
    [HarmonyPatch(typeof(Thing), "TryLickEnchant", new System.Type[] { typeof(Chara), typeof(bool), typeof(Chara), typeof(BodySlot) })]
    public static class ThingTryLickEnchantPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions, null).MatchForward(false, new CodeMatch[]
                {
                    new CodeMatch(new OpCode?(OpCodes.Call), AccessTools.Method(typeof(Card), "get_LV"), null),
                    new CodeMatch(new OpCode?(OpCodes.Call), AccessTools.Method(typeof(Thing), "AddEnchant"), null),
                    }).Advance(1).InsertAndAdvance(new CodeInstruction[]
                {
                    //call	class Player EClass::get_player()
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EClass), "get_player")),
                    //ldstr	"well_enhance"
                    new CodeInstruction(OpCodes.Ldstr, "well_enhance"),
                    //callvirt	instance int32 Player::CountKeyItem(string)
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Player), "CountKeyItem", new System.Type[] { typeof(string) })),
                    //add
                    new CodeInstruction(OpCodes.Add),
                }).InstructionEnumeration();
        }
    }

    //Element Thing::AddEnchant(System.Int32)
    [HarmonyPatch(typeof(Thing), "AddEnchant", new System.Type[] { typeof(int) })]
    public static class ThingAddEnchantPatch
    {
        private static void Postfix(Thing __instance, ref Element __result, int lv)
        {
            LuckierBlackCat.Logger.LogInfo("Thing" + __instance.Name + "::AddEnchant LV:" + lv);
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

    //System.Boolean ActPray::TryPray(Chara,System.Boolean)
    //public static bool TryPray(Chara c, bool passive = false)
    [HarmonyPatch(typeof(ActPray), "TryPray", new System.Type[] { typeof(Chara), typeof(bool) })]
    public static class ActPrayTryPrayPatch
    {
        private static void Postfix(Chara c, bool passive)
        {
            LuckierBlackCat.Logger.LogInfo("ActPray::TryPray" + c.Name + passive);
            foreach (Chara chara in EClass._map.charas)
            {
                if (chara.HasElement(1412, 1))
                {
                    foreach (Thing thing in c.things)
                    {
                        if (thing.IsEquipmentOrRanged && !thing.IsCursed && thing.rarity > Rarity.Normal && thing.GetInt(107, null) <= 0)
                        {
                            chara.Say("lick", chara, thing, null, null);
                            thing.PlaySound("offering", 1f, true);
                            thing.TryLickEnchant(chara, false);
                            LuckierBlackCat.Logger.LogInfo("[Enchant]" + c.Name + passive + " " + thing.Name + thing.IsEquipmentOrRanged + !thing.IsCursed + " " + thing.rarity + " " + thing.GetInt(107, null));
                        }
                    }
                }
            }
        }
    }
}
