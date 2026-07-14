using System;
using System.Collections.Generic;

namespace LuckierBlackCat.Core;

public static class LickRules
{
    public static bool IsEligible(ItemFacts item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        return item.IsEquipment
            && !item.IsCursed
            && item.Rarity > item.NormalRarity
            && item.LickState <= 0;
    }

    public static int SelectLicker(bool requireLickAbility, IReadOnlyList<bool> hasLickAbility)
    {
        if (hasLickAbility is null)
        {
            throw new ArgumentNullException(nameof(hasLickAbility));
        }

        if (!requireLickAbility)
        {
            return -1;
        }

        for (var index = 0; index < hasLickAbility.Count; index++)
        {
            if (hasLickAbility[index])
            {
                return index;
            }
        }

        return -2;
    }

    public static bool ShouldLickAfterPickup(
        bool pickerIsPlayer,
        bool wasOwnedByPlayer,
        bool resultOwnedByPlayer,
        ItemFacts item)
    {
        return pickerIsPlayer
            && !wasOwnedByPlayer
            && resultOwnedByPlayer
            && IsEligible(item);
    }

    public static bool ShouldProcessPrayer(bool characterIsPlayer, bool prayerSucceeded)
    {
        return characterIsPlayer && prayerSucceeded;
    }

    public static int CountEligible(IReadOnlyList<ItemFacts> items)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var count = 0;
        for (var index = 0; index < items.Count; index++)
        {
            if (IsEligible(items[index]))
            {
                count++;
            }
        }

        return count;
    }

    public static int ComputeEnchantLevel(int baseLevel, int salivaCount, int multiplier)
    {
        var safeSalivaCount = salivaCount < 0 ? 0 : salivaCount;
        var safeMultiplier = multiplier < 0 ? 0 : multiplier;
        var level = (long)baseLevel + ((long)safeSalivaCount * safeMultiplier);

        if (level > int.MaxValue)
        {
            return int.MaxValue;
        }

        if (level < int.MinValue)
        {
            return int.MinValue;
        }

        return (int)level;
    }
}
