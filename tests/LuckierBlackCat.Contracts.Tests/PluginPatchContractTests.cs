using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xunit;

namespace LuckierBlackCat.Contracts.Tests;

public sealed class PluginPatchContractTests
{
    [Theory]
    [InlineData("LuckierBlackCat.Patches.CharaPickPatch", "Prefix")]
    [InlineData("LuckierBlackCat.Patches.CharaPickPatch", "Postfix")]
    [InlineData("LuckierBlackCat.Patches.ActPrayTryPrayPatch", "Postfix")]
    [InlineData("LuckierBlackCat.Patches.ThingGenTryLickChestPatch", "Transpiler")]
    [InlineData("LuckierBlackCat.Patches.SpawnLootPatch", "Transpiler")]
    [InlineData("LuckierBlackCat.Patches.ThingTryLickEnchantPatch", "Transpiler")]
    public void PluginContainsExpectedPatchMethod(string typeName, string methodName)
    {
        using var assembly = LoadPlugin();
        var type = assembly.MainModule.GetType(typeName);

        Assert.NotNull(type);
        Assert.Single(type!.Methods, method => method.Name == methodName);
    }

    [Fact]
    public void AnnotatedPatchClassesDoNotDeclareDynamicTargetSelectors()
    {
        using var assembly = LoadPlugin();
        var reservedSelectors = new HashSet<string>(StringComparer.Ordinal)
        {
            "TargetMethod",
            "TargetMethods",
        };

        var conflictingMethods = assembly.MainModule.Types
            .Where(type => type.CustomAttributes.Any(attribute =>
                attribute.AttributeType.FullName == "HarmonyLib.HarmonyPatch"))
            .SelectMany(type => type.Methods
                .Where(method => reservedSelectors.Contains(method.Name))
                .Select(method => type.FullName + "." + method.Name))
            .ToArray();

        Assert.Empty(conflictingMethods);
    }

    [Fact]
    public void PluginLogsCompleteExceptionOnLoadFailure()
    {
        using var assembly = LoadPlugin();
        var pluginType = assembly.MainModule.GetType("LuckierBlackCat.LuckierBlackCat");
        var awake = pluginType.Methods.Single(method => method.Name == "Awake");

        Assert.Contains(awake.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.Name == "ToString"
            && method.Parameters.Count == 0);
        Assert.DoesNotContain(awake.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "System.Exception"
            && method.Name is "get_Message" or "get_StackTrace");
    }

    [Fact]
    public void PluginGuidRemainsStable()
    {
        using var assembly = LoadPlugin();
        var pluginType = assembly.MainModule.GetType("LuckierBlackCat.LuckierBlackCat");
        var field = pluginType.Fields.Single(candidate => candidate.Name == "PLUGIN_GUID");

        Assert.Equal("com.travellerse.plugins.LuckierBlackCat", field.Constant);
    }

    [Fact]
    public void PluginVersionMatchesReleaseVersion()
    {
        using var assembly = LoadPlugin();
        var pluginType = assembly.MainModule.GetType("LuckierBlackCat.LuckierBlackCat");
        var field = pluginType.Fields.Single(candidate => candidate.Name == "PLUGIN_VERSION");

        Assert.Equal("2.0.0.0", field.Constant);
        Assert.Equal(new Version(2, 0, 0, 0), assembly.Name.Version);
    }

    [Fact]
    public void AbilityRequirementUsesEhekatlsBlessingTerminology()
    {
        using var assembly = LoadPlugin();
        var configType = assembly.MainModule.GetType("LuckierBlackCat.Core.ConfigManager");
        var initialize = configType.Methods.Single(method => method.Name == "Initialize");

        Assert.Contains(initialize.Body.Instructions, instruction =>
            instruction.Operand is string value
            && value.Contains("Ehekatl's Blessing trait", StringComparison.Ordinal));
    }

    [Fact]
    public void EnchantmentRollCountHasIndependentConfiguration()
    {
        using var assembly = LoadPlugin();
        var configType = assembly.MainModule.GetType("LuckierBlackCat.Core.ConfigManager");
        var initialize = configType.Methods.Single(method => method.Name == "Initialize");

        Assert.Contains(configType.Properties, property => property.Name == "EnchantCount");
        Assert.Contains(initialize.Body.Instructions, instruction =>
            instruction.Operand is string value
            && value == "EnchantCount");
    }

    [Fact]
    public void EnchantmentPatchRoutesRollsThroughScopedHelper()
    {
        using var assembly = LoadPlugin();
        var patchType = assembly.MainModule.GetType(
            "LuckierBlackCat.Patches.ThingTryLickEnchantPatch");
        var transform = patchType.Methods.Single(method => method.Name == "Transform");
        var helper = patchType.Methods.Single(method => method.Name == "ApplyEnchantments");

        Assert.Contains(transform.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "LuckierBlackCat.Patching.InstructionTransforms"
            && method.Name == "ReplaceEnchantCall");
        Assert.Contains(helper.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "Thing"
            && method.Name == "AddEnchant");
        Assert.Contains(helper.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "LuckierBlackCat.Core.LickRules"
            && method.Name == "RollEnchantments");
    }

    [Fact]
    public void PluginExposesExistingConfigFileThroughBasePluginConfig()
    {
        using var assembly = LoadPlugin();
        var pluginType = assembly.MainModule.GetType("LuckierBlackCat.LuckierBlackCat");
        var initialize = pluginType.Methods.Single(method => method.Name == "InitializeConfig");

        Assert.Contains(initialize.Body.Instructions, instruction =>
            instruction.Operand is string value
            && value == "LuckierBlackCat.cfg");
        Assert.Contains(initialize.Body.Instructions, instruction =>
            instruction.Operand is string value
            && value == "<Config>k__BackingField");
        Assert.Contains(initialize.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "System.Reflection.FieldInfo"
            && method.Name == "SetValue");
    }

    [Fact]
    public void PrayerLickingEnumeratesAccessibleNestedInventory()
    {
        using var assembly = LoadPlugin();
        var utilityType = assembly.MainModule.GetType("LuckierBlackCat.Utils.BlackCatUtils");
        var lickAll = utilityType.Methods.Single(method => method.Name == "LickAllEligibleItems");
        var instructions = lickAll.Body.Instructions;
        var listCallIndex = instructions.IndexOf(instructions.Single(instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "ThingContainer"
            && method.Name == "List"
            && method.Parameters.Count == 2));

        Assert.Equal(OpCodes.Ldc_I4_1, instructions[listCallIndex - 1].OpCode);
    }

    [Fact]
    public void PrayerLickingCountsSuccessfulItemsWithLinq()
    {
        using var assembly = LoadPlugin();
        var utilityType = assembly.MainModule.GetType("LuckierBlackCat.Utils.BlackCatUtils");
        var lickAll = utilityType.Methods.Single(method => method.Name == "LickAllEligibleItems");

        Assert.Contains(lickAll.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "System.Linq.Enumerable"
            && method.Name == "Where");
        Assert.Contains(lickAll.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "System.Linq.Enumerable"
            && method.Name == "Count");
    }

    [Fact]
    public void SuccessfulItemLicksUseDebugLogging()
    {
        using var assembly = LoadPlugin();
        var utilityType = assembly.MainModule.GetType("LuckierBlackCat.Utils.BlackCatUtils");
        var tryLick = utilityType.Methods.Single(method => method.Name == "TryLickItem");

        Assert.Contains(tryLick.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "LuckierBlackCat.Utils.Logger"
            && method.Name == "LogDebug");
        Assert.DoesNotContain(tryLick.Body.Instructions, instruction =>
            instruction.Operand is MethodReference method
            && method.DeclaringType.FullName == "LuckierBlackCat.Utils.Logger"
            && method.Name == "LogInfo");
    }

    [Fact]
    public void PackageAndReadmeRecordReleaseCompatibility()
    {
        var root = FindRepositoryRoot();
        var package = File.ReadAllText(Path.Combine(root, "package.xml"));
        var readme = File.ReadAllText(Path.Combine(root, "README_EN.md"));

        Assert.Contains(
            "<version>2.0.0+elin.0.23.325.patch.2</version>",
            package);
        Assert.Contains(
            "Compatible with Elin 0.23.325 Patch 2 (Steam build 24059635)",
            readme);
    }

    private static AssemblyDefinition LoadPlugin()
    {
        var root = FindRepositoryRoot();
        var path = Path.Combine(root, "bin", "Release", "LuckierBlackCat.dll");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Build the Release configuration before running contract tests.",
                path);
        }

        return AssemblyDefinition.ReadAssembly(path);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "LuckierBlackCat.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root.");
    }
}
