using System;
using System.Collections.Generic;
using Xunit;

namespace LuckierBlackCat.Patching.Tests;

public sealed class PatchCatalogTests
{
    [Fact]
    public void ApplyRejectsNullArguments()
    {
        Assert.Throws<ArgumentNullException>(() => PatchCatalog.Apply(null!, () => { }));
        Assert.Throws<ArgumentNullException>(() => PatchCatalog.Apply(Array.Empty<PatchFeature>(), null!));
    }

    [Fact]
    public void FeatureRejectsNullArguments()
    {
        Assert.Throws<ArgumentNullException>(() => new PatchFeature(null!, true, () => { }, () => { }));
        Assert.Throws<ArgumentNullException>(() => new PatchFeature("a", true, null!, () => { }));
        Assert.Throws<ArgumentNullException>(() => new PatchFeature("a", true, () => { }, null!));
    }

    [Fact]
    public void DisabledFeatureIsNotValidatedOrApplied()
    {
        var validated = false;
        var applied = false;
        var feature = new PatchFeature(
            "pickup",
            false,
            () => validated = true,
            () => applied = true);

        var result = Assert.Single(PatchCatalog.Apply(new[] { feature }, () => { }));

        Assert.Equal(PatchFeatureStatus.Disabled, result.Status);
        Assert.False(validated);
        Assert.False(applied);
    }

    [Fact]
    public void CompatibleFeatureIsApplied()
    {
        var applied = false;
        var feature = new PatchFeature("pickup", true, () => { }, () => applied = true);

        var result = Assert.Single(PatchCatalog.Apply(new[] { feature }, () => { }));

        Assert.Equal("pickup", result.Name);
        Assert.Equal(PatchFeatureStatus.Applied, result.Status);
        Assert.Equal("Patch applied.", result.Detail);
        Assert.True(applied);
    }

    [Theory]
    [MemberData(nameof(KnownCompatibilityErrors))]
    public void KnownCompatibilityFailureSkipsOnlyAffectedFeature(Exception compatibilityError)
    {
        var secondApplied = false;
        var features = new[]
        {
            new PatchFeature("distance", true, () => throw compatibilityError, () => { }),
            new PatchFeature("pickup", true, () => { }, () => secondApplied = true),
        };

        var results = PatchCatalog.Apply(features, () => { });

        Assert.Equal(PatchFeatureStatus.Incompatible, results[0].Status);
        Assert.Equal(compatibilityError.Message, results[0].Detail);
        Assert.Equal(PatchFeatureStatus.Applied, results[1].Status);
        Assert.True(secondApplied);
    }

    [Fact]
    public void UnexpectedValidationFailureRollsBackAndRethrows()
    {
        var rollbackCount = 0;
        var error = new InvalidOperationException("unexpected");
        var feature = new PatchFeature("distance", true, () => throw error, () => { });

        var actual = Assert.Throws<InvalidOperationException>(() =>
            PatchCatalog.Apply(new[] { feature }, () => rollbackCount++));

        Assert.Same(error, actual);
        Assert.Equal(1, rollbackCount);
    }

    [Fact]
    public void UnexpectedApplyFailureRollsBackAndRethrows()
    {
        var rollbackCount = 0;
        var feature = new PatchFeature(
            "distance",
            true,
            () => { },
            () => throw new InvalidOperationException("patch failed"));

        Assert.Throws<InvalidOperationException>(() =>
            PatchCatalog.Apply(new[] { feature }, () => rollbackCount++));

        Assert.Equal(1, rollbackCount);
    }

    public static TheoryData<Exception> KnownCompatibilityErrors => new()
    {
        new PatchAnchorException("Thing.TryLickEnchant", 1, 0),
        new MissingMethodException("Missing target method."),
    };
}
