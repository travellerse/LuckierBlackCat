using System;

namespace LuckierBlackCat.Patching;

public sealed class PatchAnchorException : InvalidOperationException
{
    public PatchAnchorException(string targetName, int expectedMatches, int actualMatches)
        : base(
            $"Patch anchor mismatch for {targetName}: "
            + $"expected {expectedMatches} match, found {actualMatches}.")
    {
        TargetName = targetName;
        ExpectedMatches = expectedMatches;
        ActualMatches = actualMatches;
    }

    public string TargetName { get; }

    public int ExpectedMatches { get; }

    public int ActualMatches { get; }
}
