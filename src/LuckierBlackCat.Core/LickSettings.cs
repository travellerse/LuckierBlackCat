namespace LuckierBlackCat.Core;

public sealed class LickSettings
{
    public LickSettings(bool requireLickAbility, int enchantMultiplier)
    {
        RequireLickAbility = requireLickAbility;
        EnchantMultiplier = enchantMultiplier < 0 ? 0 : enchantMultiplier;
    }

    public bool RequireLickAbility { get; }

    public int EnchantMultiplier { get; }
}
