using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Xunit;

namespace LuckierBlackCat.Patching.Tests;

public sealed class InstructionTransformsTests
{
    private static readonly MethodInfo DistanceMethod = RequireMethod(nameof(Fixtures.Distance));
    private static readonly MethodInfo LevelGetter = RequireMethod(nameof(Fixtures.GetLevel));
    private static readonly MethodInfo AddEnchantMethod = RequireMethod(nameof(Fixtures.AddEnchant));
    private static readonly MethodInfo AdjustLevelMethod = RequireMethod(nameof(Fixtures.AdjustLevel));

    [Fact]
    public void DistanceTransformRejectsMissingAnchor()
    {
        var error = Assert.Throws<PatchAnchorException>(() => TransformDistance(new List<CodeInstruction>()));

        Assert.Equal("ThingGen.TryLickChest", error.TargetName);
        Assert.Equal(0, error.ActualMatches);
    }

    [Fact]
    public void DistanceTransformReplacesOnlyUniqueThreshold()
    {
        var instructions = DistanceAnchor();

        var result = TransformDistance(instructions);

        Assert.Equal(OpCodes.Ldc_I4, result[1].opcode);
        Assert.Equal(1024, result[1].operand);
        Assert.Equal(OpCodes.Bge_S, result[2].opcode);
    }

    [Fact]
    public void DistanceTransformRejectsAmbiguousAnchor()
    {
        var instructions = DistanceAnchor();
        instructions.AddRange(DistanceAnchor());

        var error = Assert.Throws<PatchAnchorException>(() => TransformDistance(instructions));

        Assert.Equal(2, error.ActualMatches);
    }

    [Fact]
    public void DistanceTransformPreservesLabelsAndExceptionBlocks()
    {
        var instructions = DistanceAnchor();
        var generator = new DynamicMethod("labels", typeof(void), Type.EmptyTypes).GetILGenerator();
        var label = generator.DefineLabel();
        var block = new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock);
        instructions[1].labels.Add(label);
        instructions[1].blocks.Add(block);

        var result = TransformDistance(instructions);

        Assert.Contains(label, result[1].labels);
        Assert.Contains(block, result[1].blocks);
    }

    [Fact]
    public void LevelTransformRejectsMissingAnchor()
    {
        var error = Assert.Throws<PatchAnchorException>(() => TransformLevel(new List<CodeInstruction>()));

        Assert.Equal("Thing.TryLickEnchant", error.TargetName);
        Assert.Equal(0, error.ActualMatches);
    }

    [Fact]
    public void LevelTransformInsertsAdjustmentBeforeAddEnchant()
    {
        var result = TransformLevel(LevelAnchor());

        Assert.Equal(3, result.Count);
        Assert.Equal(LevelGetter, result[0].operand);
        Assert.Equal(AdjustLevelMethod, result[1].operand);
        Assert.Equal(AddEnchantMethod, result[2].operand);
    }

    [Fact]
    public void LevelTransformRejectsAmbiguousAnchor()
    {
        var instructions = LevelAnchor();
        instructions.AddRange(LevelAnchor());

        var error = Assert.Throws<PatchAnchorException>(() => TransformLevel(instructions));

        Assert.Equal(2, error.ActualMatches);
    }

    [Fact]
    public void LevelTransformPreservesExistingMetadata()
    {
        var instructions = LevelAnchor();
        var generator = new DynamicMethod("metadata", typeof(void), Type.EmptyTypes).GetILGenerator();
        var label = generator.DefineLabel();
        var block = new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock);
        instructions[1].labels.Add(label);
        instructions[1].blocks.Add(block);

        var result = TransformLevel(instructions);

        Assert.Empty(result[1].labels);
        Assert.Empty(result[1].blocks);
        Assert.Contains(label, result[2].labels);
        Assert.Contains(block, result[2].blocks);
    }

    private static IReadOnlyList<CodeInstruction> TransformDistance(
        IEnumerable<CodeInstruction> instructions)
    {
        return InstructionTransforms.ReplaceDistanceThreshold(
            instructions,
            DistanceMethod,
            5,
            1024,
            "ThingGen.TryLickChest");
    }

    private static IReadOnlyList<CodeInstruction> TransformLevel(
        IEnumerable<CodeInstruction> instructions)
    {
        return InstructionTransforms.InsertLevelAdjustment(
            instructions,
            LevelGetter,
            AddEnchantMethod,
            AdjustLevelMethod,
            "Thing.TryLickEnchant");
    }

    private static List<CodeInstruction> DistanceAnchor()
    {
        return new List<CodeInstruction>
        {
            new CodeInstruction(OpCodes.Callvirt, DistanceMethod),
            new CodeInstruction(OpCodes.Ldc_I4_5),
            new CodeInstruction(OpCodes.Bge_S),
        };
    }

    private static List<CodeInstruction> LevelAnchor()
    {
        return new List<CodeInstruction>
        {
            new CodeInstruction(OpCodes.Call, LevelGetter),
            new CodeInstruction(OpCodes.Call, AddEnchantMethod),
        };
    }

    private static MethodInfo RequireMethod(string name)
    {
        return typeof(Fixtures).GetMethod(name, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Missing fixture method: {name}");
    }

    public static class Fixtures
    {
        public static int Distance(object value) => value is null ? 0 : 1;

        public static int GetLevel() => 1;

        public static object AddEnchant(int level) => level;

        public static int AdjustLevel(int level) => level + 1;
    }
}
