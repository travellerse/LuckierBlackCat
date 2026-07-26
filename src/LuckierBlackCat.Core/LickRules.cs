using System;
using System.Collections.Generic;

namespace LuckierBlackCat.Core;

public static class LickRules
{
    public const int MinEnchantCount = 1;
    public const int MaxEnchantCount = 10;

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

    public static bool ShouldProcessPrayer(
        bool characterIsPlayer,
        bool prayerSucceeded,
        bool passive)
    {
        _ = passive;
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

    public static int NormalizeEnchantCount(int count)
    {
        if (count < MinEnchantCount)
        {
            return MinEnchantCount;
        }

        return count > MaxEnchantCount ? MaxEnchantCount : count;
    }

    public static T? RollEnchantments<T>(
        int count,
        int level,
        Func<int, T?> addEnchant)
        where T : class
    {
        if (addEnchant is null)
        {
            throw new ArgumentNullException(nameof(addEnchant));
        }

        T? lastEnchant = null;
        int normalizedCount = NormalizeEnchantCount(count);
        for (int index = 0; index < normalizedCount; index++)
        {
            T? enchant = addEnchant(level);
            if (enchant is not null)
            {
                lastEnchant = enchant;
            }
        }

        return lastEnchant;
    }
}
