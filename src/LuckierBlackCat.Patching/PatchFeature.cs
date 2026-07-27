using System;

namespace LuckierBlackCat.Patching;

public sealed class PatchFeature
{
    public PatchFeature(
        string name,
        bool enabled,
        Action validate,
        Action apply)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Enabled = enabled;
        Validate = validate ?? throw new ArgumentNullException(nameof(validate));
        Apply = apply ?? throw new ArgumentNullException(nameof(apply));
    }

    public string Name { get; }

    public bool Enabled { get; }

    internal Action Validate { get; }

    internal Action Apply { get; }
}
