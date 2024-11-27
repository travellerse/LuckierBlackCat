using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace LuckierBlackCat
{
    [BepInPlugin("com.travellerse.plugins.LuckierBlackCat", "Luckier Black Cat", "0.4.1.0")]
    [BepInProcess("Elin.exe")]
    public class LuckierBlackCat : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "LuckierBlackCat.cfg"), true);

        private ConfigEntry<bool> _enableLickWithoutDist;
        private ConfigEntry<bool> _enableLickWhenPick;
        private ConfigEntry<bool> _enableLickWhenPray;
        private ConfigEntry<bool> _enableLickEnchant;
        public static ConfigEntry<int> enchantTimes;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("Luckier Black Cat loaded!");
            Harmony harmony = new Harmony("com.travellerse.plugins.LuckierBlackCat");
            _enableLickWithoutDist = configFile.Bind("Settings", "EnableLickWithoutDist", true,
                "Enable lick without distance limit.");
            _enableLickWhenPick = configFile.Bind("Settings", "EnableLickWhenPick", true,
                "Enable lick when pick up equipment.");
            _enableLickWhenPray = configFile.Bind("Settings", "EnableLickWhenPray", true, "Enable lick when pray.");
            _enableLickEnchant = configFile.Bind("Settings", "EnableLickEnchant", true,
                "Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].");
            enchantTimes = configFile.Bind("Settings", "EnchantTimes", 1, "The times of enhance");

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
                harmony.PatchAll(typeof(ThingGetEnchantPatch));
                harmony.PatchAll(typeof(ThingAddEnchantPatch));
                LuckierBlackCat.Logger.LogInfo(
                    "Enhanced the black cat's licking effect based on the amount of [Black Cat's Saliva].");
            }

            Logger.LogInfo("Luckier Black Cat patched!");
        }
    }

    [HarmonyPatch(typeof(ThingGen), "TryLickChest")]
    public static class ThingGenTryLickChestPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i + 2 < codes.Count; i++)
            {
                if (codes[i].opcode == System.Reflection.Emit.OpCodes.Callvirt &&
                    codes[i].operand.ToString().Contains("Dist"))
                {
                    //LuckierBlackCat.Logger.LogInfo("ThingGen::TryLickChest " + "codes:" + codes[i + 2].operand.ToString());
                    if (codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge || codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge_S)
                    {
                        //LuckierBlackCat.Logger.LogInfo("ThingGen" + "::TryLickChest");
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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i + 2 < codes.Count; i++)
            {
                if (codes[i].opcode == System.Reflection.Emit.OpCodes.Callvirt &&
                    codes[i].operand.ToString().Contains("Dist"))
                {
                    //LuckierBlackCat.Logger.LogInfo("Card::SpawnLoot " + "codes:" + codes[i + 2].operand.ToString());
                    if (codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge || codes[i + 2].opcode == System.Reflection.Emit.OpCodes.Bge_S)
                    {
                        //LuckierBlackCat.Logger.LogInfo("Card" + "::SpawnLoot");
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
    [HarmonyPatch(typeof(Thing), "TryLickEnchant",
        new System.Type[] { typeof(Chara), typeof(bool), typeof(Chara), typeof(BodySlot) })]
    public static class ThingTryLickEnchantPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
                new CodeInstruction(OpCodes.Callvirt,
                    AccessTools.Method(typeof(Player), "CountKeyItem", new System.Type[] { typeof(string) })),
                //ldc.i4
                new CodeInstruction(OpCodes.Ldc_I4, LuckierBlackCat.enchantTimes.Value),
                //mul
                new CodeInstruction(OpCodes.Mul),
                //add
                new CodeInstruction(OpCodes.Add),
            }).InstructionEnumeration();
        }
    }

    //System.Tuple`2<SourceElement/Row,System.Int32> Thing::GetEnchant(System.Int32,System.Func`2<SourceElement/Row,System.Boolean>,System.Boolean)
    //public static Tuple<SourceElement.Row, int> GetEnchant(int lv, Func<SourceElement.Row, bool> func, bool neg)
    [HarmonyPatch(typeof(Thing), "GetEnchant",
        new System.Type[] { typeof(int), typeof(System.Func<SourceElement.Row, bool>), typeof(bool) })]
    public static class ThingGetEnchantPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions, null).MatchForward(false, new CodeMatch[]
            {
                //ldloc.0
                new CodeMatch(new OpCode?(OpCodes.Ldloc_0), null, null),
                //callvirt    instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib] System.Collections.Generic.List`1<class SourceElement/Row>::GetEnumerator()
                new CodeMatch(new OpCode?(OpCodes.Callvirt),
                    AccessTools.Method(typeof(List<SourceElement.Row>), "GetEnumerator"), null),
            }).Advance(1).InsertAndAdvance(new CodeInstruction[]
            {
                //call    class [mscorlib] System.Collections.Generic.IList`1<!!0> [Plugins.BaseCore] ClassExtension::Shuffle<class [Elin] SourceElement/Row>(class [mscorlib] System.Collections.Generic.IList`1<!!0>)
                //list.Shuffle<SourceElement.Row>()
                new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(ThingGetEnchantPatch), "Shuffle",
                        new System.Type[] { typeof(IList<SourceElement.Row>) })),
            }).InstructionEnumeration();
        }

        //Shuffle
        private static List<SourceElement.Row> Shuffle(IList<SourceElement.Row> list)
        {
            List<SourceElement.Row> list2 = new List<SourceElement.Row>(list);
            for (int i = 0; i < list2.Count; i++)
            {
                int index = UnityEngine.Random.Range(i, list2.Count);
                SourceElement.Row value = list2[i];
                list2[i] = list2[index];
                list2[index] = value;
            }
            return list2;
        }
    }

    //Element Thing::AddEnchant(System.Int32)
    [HarmonyPatch(typeof(Thing), "AddEnchant", new System.Type[] { typeof(int) })]
    public static class ThingAddEnchantPatch
    {
        private static void Postfix(Thing __instance, ref Element __result, int lv)
        {
            LuckierBlackCat.Logger.LogInfo("Thing" + "::AddEnchant name:" + __instance.Name + " LV:" + lv + " rarity:" +
                                           __instance.rarity);

            //Func<SourceElement.Row, bool> func = (SourceElement.Row r) => r.IsEncAppliable(__instance.category);
            //foreach (SourceElement.Row row in EClass.sources.elements.rows)
            //{
            //    if ((!row.tag.Contains("flag")) && func(row))
            //        LuckierBlackCat.Logger.LogInfo("Element " + row.name + " chance:" + row.chance + " LV:" + row.LV + " mtp:" + row.mtp);
            //}
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
                        if (thing.IsEquipmentOrRanged && !thing.IsCursed && thing.rarity > Rarity.Normal &&
                            thing.GetInt(107, null) <= 0)
                        {
                            chara.Say("lick", chara, thing, null, null);
                            thing.PlaySound("offering", 1f, true);
                            thing.TryLickEnchant(chara, false);
                            LuckierBlackCat.Logger.LogInfo("[Enchant]" + c.Name + passive + " " + thing.Name +
                                                           thing.IsEquipmentOrRanged + !thing.IsCursed + " " +
                                                           thing.rarity + " " + thing.GetInt(107, null));
                        }
                    }
                }
            }
        }
    }
}
