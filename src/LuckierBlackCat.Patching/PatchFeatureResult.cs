namespace LuckierBlackCat.Patching;

public sealed class PatchFeatureResult
{
    public PatchFeatureResult(string name, PatchFeatureStatus status, string detail)
    {
        Name = name;
        Status = status;
        Detail = detail;
    }

    public string Name { get; }

    public PatchFeatureStatus Status { get; }

    public string Detail { get; }
}
