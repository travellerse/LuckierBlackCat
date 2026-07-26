using System;
using Xunit;

namespace LuckierBlackCat.Core.Tests;

public sealed class LickRulesTests
{
    [Theory]
    [InlineData(true, false, 2, 1, 0, true)]
    [InlineData(false, false, 2, 1, 0, false)]
    [InlineData(true, true, 2, 1, 0, false)]
    [InlineData(true, false, 1, 1, 0, false)]
    [InlineData(true, false, 0, 1, 0, false)]
    [InlineData(true, false, 2, 1, 1, false)]
    public void EligibilityMatchesVanillaRules(
        bool equipment,
        bool cursed,
        int rarity,
        int normalRarity,
        int lickState,
        bool expected)
    {
        var item = Item(equipment, cursed, rarity, normalRarity, lickState);

        Assert.Equal(expected, LickRules.IsEligible(item));
    }

    [Fact]
    public void EligibilityRejectsNullFacts()
    {
        Assert.Throws<ArgumentNullException>(() => LickRules.IsEligible(null!));
    }

    [Theory]
    [InlineData(false, new[] { false, false }, -1)]
    [InlineData(true, new[] { false, true, true }, 1)]
    [InlineData(true, new[] { false, false }, -2)]
    public void LickerSelectionReturnsVirtualFirstOrMissingResult(
        bool required,
        bool[] abilities,
        int expected)
    {
        Assert.Equal(expected, LickRules.SelectLicker(required, abilities));
    }

    [Fact]
    public void LickerSelectionRejectsNullList()
    {
        Assert.Throws<ArgumentNullException>(() => LickRules.SelectLicker(true, null!));
    }

    [Theory]
    [InlineData(true, false, true, true)]
    [InlineData(false, false, true, false)]
    [InlineData(true, true, true, false)]
    [InlineData(true, false, false, false)]
    public void PickupRequiresPlayerOwnershipTransition(
        bool isPlayer,
        bool wasOwned,
        bool resultOwned,
        bool expected)
    {
        Assert.Equal(
            expected,
            LickRules.ShouldLickAfterPickup(isPlayer, wasOwned, resultOwned, Item()));
    }

    [Fact]
    public void PickupStillRequiresEligibleResult()
    {
        Assert.False(LickRules.ShouldLickAfterPickup(true, false, true, Item(cursed: true)));
    }

    [Theory]
    [InlineData(true, true, false, true)]
    [InlineData(true, true, true, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    public void PrayerRequiresPlayerAndSuccessfulOriginalCall(
        bool isPlayer,
        bool succeeded,
        bool passive,
        bool expected)
    {
        Assert.Equal(expected, LickRules.ShouldProcessPrayer(isPlayer, succeeded, passive));
    }

    [Fact]
    public void FullBackpackDoesNotTriggerPickupLick()
    {
        Assert.False(LickRules.ShouldLickAfterPickup(true, false, false, Item()));
    }

    [Fact]
    public void ExistingInventoryItemDoesNotTriggerPickupLick()
    {
        Assert.False(LickRules.ShouldLickAfterPickup(true, true, true, Item()));
    }

    [Fact]
    public void StackResultTriggersPickupLickWhenOwnershipChanges()
    {
        Assert.True(LickRules.ShouldLickAfterPickup(true, false, true, Item()));
    }

    [Fact]
    public void ConvertedResultUsesResultEligibility()
    {
        Assert.False(LickRules.ShouldLickAfterPickup(true, false, true, Item(equipment: false)));
    }

    [Fact]
    public void NpcPickupDoesNotTriggerPickupLick()
    {
        Assert.False(LickRules.ShouldLickAfterPickup(false, false, true, Item()));
    }

    [Fact]
    public void EligibleCountUsesSharedEligibilityRule()
    {
        var items = new[] { Item(), Item(cursed: true), Item(), Item(lickState: 107) };

        Assert.Equal(2, LickRules.CountEligible(items));
    }

    [Fact]
    public void EligibleCountRejectsNullList()
    {
        Assert.Throws<ArgumentNullException>(() => LickRules.CountEligible(null!));
    }

    [Theory]
    [InlineData(10, 0, 1, 10)]
    [InlineData(10, 5, 0, 10)]
    [InlineData(10, 5, -1, 10)]
    [InlineData(10, -5, 3, 10)]
    [InlineData(10, 5, 3, 25)]
    public void EnchantLevelUsesNormalizedInputs(
        int baseLevel,
        int saliva,
        int multiplier,
        int expected)
    {
        Assert.Equal(expected, LickRules.ComputeEnchantLevel(baseLevel, saliva, multiplier));
    }

    [Fact]
    public void EnchantLevelSaturatesPositiveOverflow()
    {
        Assert.Equal(int.MaxValue, LickRules.ComputeEnchantLevel(int.MaxValue, int.MaxValue, 2));
    }

    [Fact]
    public void EnchantLevelPreservesNegativeBaseLevelWithoutEnhancement()
    {
        Assert.Equal(int.MinValue, LickRules.ComputeEnchantLevel(int.MinValue, 0, 0));
    }

    private static ItemFacts Item(
        bool equipment = true,
        bool cursed = false,
        int rarity = 2,
        int normalRarity = 1,
        int lickState = 0)
    {
        return new ItemFacts(equipment, cursed, rarity, normalRarity, lickState);
    }
}
