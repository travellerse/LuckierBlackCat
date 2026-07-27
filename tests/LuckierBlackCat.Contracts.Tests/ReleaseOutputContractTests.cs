using System;
using System.IO;
using System.Linq;
using Xunit;

namespace LuckierBlackCat.Contracts.Tests;

public sealed class ReleaseOutputContractTests
{
    [Fact]
    public void ReleaseOutputContainsPluginOwnedDllsWithoutGameReferences()
    {
        var output = Path.Combine(FindRepositoryRoot(), "bin", "Release");
        var dllNames = Directory
            .GetFiles(output, "*.dll")
            .Select(Path.GetFileName)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Contains("LuckierBlackCat.Core.dll", dllNames);
        Assert.Contains("LuckierBlackCat.Patching.dll", dllNames);
        Assert.Contains("LuckierBlackCat.dll", dllNames);
        Assert.DoesNotContain("0Harmony.dll", dllNames);
        Assert.DoesNotContain("BepInEx.Core.dll", dllNames);
        Assert.DoesNotContain("BepInEx.Unity.dll", dllNames);
        Assert.DoesNotContain("Elin.dll", dllNames);
        Assert.DoesNotContain("Plugins.BaseCore.dll", dllNames);
        Assert.DoesNotContain("Plugins.Sound.dll", dllNames);
        Assert.DoesNotContain("UnityEngine.dll", dllNames);
        Assert.DoesNotContain("UnityEngine.CoreModule.dll", dllNames);
    }

    [Theory]
    [InlineData("package.xml")]
    [InlineData("preview.jpg")]
    [InlineData("LICENSE.txt")]
    [InlineData("README.md")]
    [InlineData("README_EN.md")]
    [InlineData("README_JP.md")]
    [InlineData("LangConfig/CN.xlsx")]
    [InlineData("LangConfig/EN.xlsx")]
    [InlineData("LangConfig/JP.xlsx")]
    [InlineData("LangMod/CN/LuckierBlackCat.xlsx")]
    [InlineData("LangMod/EN/LuckierBlackCat.xlsx")]
    [InlineData("LangMod/JP/LuckierBlackCat.xlsx")]
    public void ReleaseOutputContainsRequiredResource(string fileName)
    {
        var path = Path.Combine(FindRepositoryRoot(), "bin", "Release", fileName);

        Assert.True(File.Exists(path), $"Missing release resource: {fileName}");
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
