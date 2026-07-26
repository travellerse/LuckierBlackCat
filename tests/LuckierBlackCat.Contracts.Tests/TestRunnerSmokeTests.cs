using Xunit;

namespace LuckierBlackCat.Contracts.Tests;

public sealed class TestRunnerSmokeTests
{
    [Fact]
    public void TestRunnerLoadsProjectAssembly()
    {
        Assert.Equal(
            "LuckierBlackCat.Contracts.Tests",
            typeof(TestRunnerSmokeTests).Assembly.GetName().Name);
    }
}
