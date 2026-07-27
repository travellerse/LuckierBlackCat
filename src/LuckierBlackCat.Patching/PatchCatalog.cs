using System;
using System.Collections.Generic;

namespace LuckierBlackCat.Patching;

public static class PatchCatalog
{
    public static IReadOnlyList<PatchFeatureResult> Apply(
        IEnumerable<PatchFeature> features,
        Action rollback)
    {
        if (features is null)
        {
            throw new ArgumentNullException(nameof(features));
        }

        if (rollback is null)
        {
            throw new ArgumentNullException(nameof(rollback));
        }

        var results = new List<PatchFeatureResult>();
        try
        {
            foreach (var feature in features)
            {
                if (!feature.Enabled)
                {
                    results.Add(new PatchFeatureResult(
                        feature.Name,
                        PatchFeatureStatus.Disabled,
                        "Disabled by configuration."));
                    continue;
                }

                try
                {
                    feature.Validate();
                }
                catch (PatchAnchorException error)
                {
                    results.Add(new PatchFeatureResult(
                        feature.Name,
                        PatchFeatureStatus.Incompatible,
                        error.Message));
                    continue;
                }
                catch (MissingMethodException error)
                {
                    results.Add(new PatchFeatureResult(
                        feature.Name,
                        PatchFeatureStatus.Incompatible,
                        error.Message));
                    continue;
                }

                feature.Apply();
                results.Add(new PatchFeatureResult(
                    feature.Name,
                    PatchFeatureStatus.Applied,
                    "Patch applied."));
            }

            return results;
        }
        catch
        {
            rollback();
            throw;
        }
    }
}
