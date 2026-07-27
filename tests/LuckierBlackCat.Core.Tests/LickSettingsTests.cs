using Xunit;

namespace LuckierBlackCat.Core.Tests;

public sealed class LickSettingsTests
{
    [Theory]
    [InlineData(-5, 0)]
    [InlineData(0, 0)]
    [InlineData(3, 3)]
    [InlineData(20, 20)]
    public void ConstructorNormalizesOnlyNegativeMultiplier(int input, int expected)
    {
        var settings = new LickSettings(true, input);

        Assert.True(settings.RequireLickAbility);
        Assert.Equal(expected, settings.EnchantMultiplier);
    }
}
