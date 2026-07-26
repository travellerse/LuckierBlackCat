using Xunit;

namespace LuckierBlackCat.Contracts.Tests;

public sealed class ElinMethodContractTests
{
    [Fact]
    public void ReferenceAssemblyMatchesReviewedBuild()
    {
        Assert.Equal(ReferenceAssembly.ExpectedSha256, ReferenceAssembly.GetElinSha256());
    }

    [Theory]
    [MemberData(nameof(PatchTargets))]
    public void PatchTargetMatchesExpectedContract(
        string declaringType,
        string name,
        string returnType,
        bool isStatic,
        string[] parameterTypes)
    {
        using var reference = ReferenceAssembly.LoadElin();

        var method = reference.RequireMethod(
            declaringType,
            name,
            returnType,
            isStatic,
            parameterTypes);

        Assert.True(method.HasBody);
    }

    public static TheoryData<string, string, string, bool, string[]> PatchTargets => new()
    {
        {
            "Chara",
            "Pick",
            "Thing",
            false,
            new[] { "Thing", "System.Boolean", "System.Boolean" }
        },
        {
            "ActPray",
            "TryPray",
            "System.Boolean",
            true,
            new[] { "Chara", "System.Boolean" }
        },
        {
            "Thing",
            "TryLickEnchant",
            "System.Void",
            false,
            new[] { "Chara", "System.Boolean", "Chara", "BodySlot" }
        },
        {
            "ThingGen",
            "TryLickChest",
            "System.Void",
            true,
            new[] { "Thing" }
        },
        {
            "Card",
            "SpawnLoot",
            "System.Void",
            false,
            new[] { "Card" }
        },
        {
            "Thing",
            "AddEnchant",
            "Element",
            false,
            new[] { "System.Int32" }
        },
        {
            "Thing",
            "GetEnchant",
            "System.Tuple`2<SourceElement/Row,System.Int32>",
            true,
            new[]
            {
                "System.Int64",
                "System.Func`2<SourceElement/Row,System.Boolean>",
                "System.Boolean",
            }
        },
        {
            "Card",
            "Dist",
            "System.Int32",
            false,
            new[] { "Card" }
        },
        {
            "Card",
            "Dist",
            "System.Int32",
            false,
            new[] { "Point" }
        },
    };
}
