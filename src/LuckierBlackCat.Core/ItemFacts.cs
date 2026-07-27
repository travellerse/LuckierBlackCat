namespace LuckierBlackCat.Core;

public sealed class ItemFacts
{
    public ItemFacts(bool isEquipment, bool isCursed, int rarity, int normalRarity, int lickState)
    {
        IsEquipment = isEquipment;
        IsCursed = isCursed;
        Rarity = rarity;
        NormalRarity = normalRarity;
        LickState = lickState;
    }

    public bool IsEquipment { get; }

    public bool IsCursed { get; }

    public int Rarity { get; }

    public int NormalRarity { get; }

    public int LickState { get; }
}
