using Xunit;

namespace LuckierBlackCat.Patching.Tests;

public sealed class TestRunnerSmokeTests
{
    [Fact]
    public void TestRunnerLoadsProjectAssembly()
    {
        Assert.Equal(
            "LuckierBlackCat.Patching.Tests",
            typeof(TestRunnerSmokeTests).Assembly.GetName().Name);
    }
}
