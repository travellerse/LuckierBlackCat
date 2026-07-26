using System.Collections.Generic;
using LuckierBlackCat.Core;

namespace LuckierBlackCat.Utils
{
    public static class BlackCatUtils
    {
        private const int EhekatlBlessingElementId = 1412;
        private const int EnchantStatusPropertyId = 107;

        public static ItemFacts ToItemFacts(Thing item)
        {
            return new ItemFacts(
                item.IsEquipment,
                item.IsCursed,
                (int)item.rarity,
                (int)Rarity.Normal,
                item.GetInt(EnchantStatusPropertyId, null));
        }

        public static bool IsItemEligibleForLicking(Thing item)
        {
            return LickRules.IsEligible(ToItemFacts(item));
        }

        public static bool TryLickItem(Thing item, bool showMessage = true)
        {
            if (!IsItemEligibleForLicking(item))
            {
                return false;
            }

            Chara licker = SelectLicker();
            if (licker == null)
            {
                return false;
            }

            int previousState = item.GetInt(EnchantStatusPropertyId, null);
            PerformLickAction(licker, item, showMessage);
            int currentState = item.GetInt(EnchantStatusPropertyId, null);
            bool changed = currentState != previousState;

            if (changed)
            {
                Logger.LogDebug("Licker " + licker.Name + " enchanted " + item.Name);
            }

            return changed;
        }

        public static int LickAllEligibleItems(Chara character, bool showMessage = true)
        {
            int lickCount = 0;
            foreach (Thing item in character.things.List(_ => true, true))
            {
                if (TryLickItem(item, showMessage))
                {
                    lickCount++;
                }
            }

            return lickCount;
        }

        private static Chara SelectLicker()
        {
            if (!ConfigManager.RequireLickAbility.Value)
            {
                return EClass.pc;
            }

            var blessings = new List<bool>(EClass._map.charas.Count);
            foreach (Chara character in EClass._map.charas)
            {
                blessings.Add(character.HasElement(EhekatlBlessingElementId, 1));
            }

            int selectedIndex = LickRules.SelectLicker(true, blessings);
            return selectedIndex >= 0 ? EClass._map.charas[selectedIndex] : null;
        }

        private static void PerformLickAction(Chara licker, Thing item, bool showMessage)
        {
            if (showMessage)
            {
                if (ConfigManager.RequireLickAbility.Value)
                {
                    licker.Say("lick", licker, item, null, null);
                }

                item.PlaySound("offering", 1f, true);
            }

            item.TryLickEnchant(licker, false);
        }
    }
}
