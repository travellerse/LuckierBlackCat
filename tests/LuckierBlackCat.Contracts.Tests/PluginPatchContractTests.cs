using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
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
    public void PackageVersionRecordsGameAndModCompatibility()
    {
        var root = FindRepositoryRoot();
        var package = File.ReadAllText(Path.Combine(root, "package.xml"));

        Assert.Contains("<version>0.23.325-2.0.0</version>", package);
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
