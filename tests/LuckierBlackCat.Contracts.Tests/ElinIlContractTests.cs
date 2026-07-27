using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xunit;

namespace LuckierBlackCat.Contracts.Tests;

public sealed class ElinIlContractTests
{
    [Fact]
    public void LickEnhancementHasOneLevelToAddEnchantAnchor()
    {
        using var reference = ReferenceAssembly.LoadElin();
        var method = reference.RequireMethod(
            "Thing",
            "TryLickEnchant",
            "System.Void",
            false,
            "Chara",
            "System.Boolean",
            "Chara",
            "BodySlot");

        var anchors = FindSequences(
            method,
            instruction => Calls(instruction, "Card", "get_LV", "System.Int32"),
            instruction => Calls(instruction, "Thing", "AddEnchant", "Element", "System.Int32"));

        Assert.Single(anchors);
    }

    [Fact]
    public void ChestDistanceHasOneCardDistanceThresholdAnchor()
    {
        using var reference = ReferenceAssembly.LoadElin();
        var method = reference.RequireMethod(
            "ThingGen",
            "TryLickChest",
            "System.Void",
            true,
            "Thing");

        var anchors = FindSequences(
            method,
            instruction => Calls(instruction, "Card", "Dist", "System.Int32", "Card"),
            instruction => LoadsInteger(instruction, 5),
            instruction => IsBranch(instruction, Code.Bge, Code.Bge_S));

        Assert.Single(anchors);
    }

    [Fact]
    public void SpawnLootDistanceHasOnePointDistanceThresholdAnchor()
    {
        using var reference = ReferenceAssembly.LoadElin();
        var method = reference.RequireMethod(
            "Card",
            "SpawnLoot",
            "System.Void",
            false,
            "Card");

        var anchors = FindSequences(
            method,
            instruction => Calls(instruction, "Card", "Dist", "System.Int32", "Point"),
            instruction => LoadsInteger(instruction, 3),
            instruction => IsBranch(instruction, Code.Bge, Code.Bge_S));

        Assert.Single(anchors);
    }

    private static IReadOnlyList<Instruction> FindSequences(
        MethodDefinition method,
        params Func<Instruction, bool>[] predicates)
    {
        var instructions = method.Body.Instructions;
        return instructions
            .Take(instructions.Count - predicates.Length + 1)
            .Where((instruction, index) =>
                predicates
                    .Select((predicate, offset) => predicate(instructions[index + offset]))
                    .All(matches => matches))
            .ToArray();
    }

    private static bool Calls(
        Instruction instruction,
        string declaringType,
        string name,
        string returnType,
        params string[] parameterTypes)
    {
        if (instruction.OpCode.Code is not (Code.Call or Code.Callvirt)
            || instruction.Operand is not MethodReference method)
        {
            return false;
        }

        return method.DeclaringType.FullName == declaringType
            && method.Name == name
            && method.ReturnType.FullName == returnType
            && method.Parameters
                .Select(parameter => parameter.ParameterType.FullName)
                .SequenceEqual(parameterTypes);
    }

    private static bool LoadsInteger(Instruction instruction, int expected)
    {
        var value = instruction.OpCode.Code switch
        {
            Code.Ldc_I4_M1 => -1,
            Code.Ldc_I4_0 => 0,
            Code.Ldc_I4_1 => 1,
            Code.Ldc_I4_2 => 2,
            Code.Ldc_I4_3 => 3,
            Code.Ldc_I4_4 => 4,
            Code.Ldc_I4_5 => 5,
            Code.Ldc_I4_6 => 6,
            Code.Ldc_I4_7 => 7,
            Code.Ldc_I4_8 => 8,
            Code.Ldc_I4_S => (sbyte)instruction.Operand,
            Code.Ldc_I4 => (int)instruction.Operand,
            _ => (int?)null,
        };

        return value == expected;
    }

    private static bool IsBranch(Instruction instruction, params Code[] expected)
    {
        return expected.Contains(instruction.OpCode.Code);
    }
}
