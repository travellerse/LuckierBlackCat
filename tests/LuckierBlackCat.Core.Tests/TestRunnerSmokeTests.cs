using Xunit;

namespace LuckierBlackCat.Core.Tests;

public sealed class TestRunnerSmokeTests
{
    [Fact]
    public void TestRunnerLoadsProjectAssembly()
    {
        Assert.Equal(
            "LuckierBlackCat.Core.Tests",
            typeof(TestRunnerSmokeTests).Assembly.GetName().Name);
    }
}
