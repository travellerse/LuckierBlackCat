using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace LuckierBlackCat.Patching;

public static class InstructionTransforms
{
    public static IReadOnlyList<CodeInstruction> ReplaceDistanceThreshold(
        IEnumerable<CodeInstruction> instructions,
        MethodInfo distanceMethod,
        int expectedThreshold,
        int replacementThreshold,
        string targetName)
    {
        var codes = Copy(instructions);
        var matches = new List<int>();

        for (var index = 0; index + 2 < codes.Count; index++)
        {
            if (Calls(codes[index], distanceMethod)
                && LoadsInteger(codes[index + 1], expectedThreshold)
                && IsGreaterThanOrEqualBranch(codes[index + 2]))
            {
                matches.Add(index);
            }
        }

        RequireUniqueMatch(targetName, matches.Count);

        var threshold = codes[matches[0] + 1];
        threshold.opcode = OpCodes.Ldc_I4;
        threshold.operand = replacementThreshold;
        return codes;
    }

    public static IReadOnlyList<CodeInstruction> InsertLevelAdjustment(
        IEnumerable<CodeInstruction> instructions,
        MethodInfo levelGetter,
        MethodInfo addEnchantMethod,
        MethodInfo adjustLevelMethod,
        string targetName)
    {
        var codes = Copy(instructions);
        var matches = new List<int>();

        for (var index = 0; index + 1 < codes.Count; index++)
        {
            if (Calls(codes[index], levelGetter)
                && Calls(codes[index + 1], addEnchantMethod))
            {
                matches.Add(index);
            }
        }

        RequireUniqueMatch(targetName, matches.Count);
        codes.Insert(matches[0] + 1, new CodeInstruction(OpCodes.Call, adjustLevelMethod));
        return codes;
    }

    private static List<CodeInstruction> Copy(IEnumerable<CodeInstruction> instructions)
    {
        if (instructions is null)
        {
            throw new ArgumentNullException(nameof(instructions));
        }

        return instructions.Select(instruction => new CodeInstruction(instruction)).ToList();
    }

    private static bool Calls(CodeInstruction instruction, MethodInfo expectedMethod)
    {
        return (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt)
            && Equals(instruction.operand, expectedMethod);
    }

    private static bool LoadsInteger(CodeInstruction instruction, int expected)
    {
        int? value;
        if (instruction.opcode == OpCodes.Ldc_I4_M1)
        {
            value = -1;
        }
        else if (instruction.opcode == OpCodes.Ldc_I4_0) value = 0;
        else if (instruction.opcode == OpCodes.Ldc_I4_1) value = 1;
        else if (instruction.opcode == OpCodes.Ldc_I4_2) value = 2;
        else if (instruction.opcode == OpCodes.Ldc_I4_3) value = 3;
        else if (instruction.opcode == OpCodes.Ldc_I4_4) value = 4;
        else if (instruction.opcode == OpCodes.Ldc_I4_5) value = 5;
        else if (instruction.opcode == OpCodes.Ldc_I4_6) value = 6;
        else if (instruction.opcode == OpCodes.Ldc_I4_7) value = 7;
        else if (instruction.opcode == OpCodes.Ldc_I4_8) value = 8;
        else if (instruction.opcode == OpCodes.Ldc_I4_S) value = (sbyte)instruction.operand;
        else if (instruction.opcode == OpCodes.Ldc_I4) value = (int)instruction.operand;
        else value = null;

        return value == expected;
    }

    private static bool IsGreaterThanOrEqualBranch(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Bge || instruction.opcode == OpCodes.Bge_S;
    }

    private static void RequireUniqueMatch(string targetName, int actualMatches)
    {
        if (actualMatches != 1)
        {
            throw new PatchAnchorException(targetName, 1, actualMatches);
        }
    }
}
