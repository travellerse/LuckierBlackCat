# Spec: Multiple Enchantment Rolls per Lick

## Objective

Allow one successful Ehekatl's Blessing lick to roll multiple enchantments while preserving
Elin's rule that an item can be licked only once. Players configure the number of rolls
independently from the existing enchantment-level multiplier.

## Tech Stack

- C# targeting `netstandard2.0` for pure rules and patch transforms.
- C# targeting `net462` for the Elin and BepInEx plugin adapter.
- Harmony transpilers for the scoped `Thing.TryLickEnchant` modification.
- xUnit and Mono.Cecil for unit and compiled-plugin contract tests.
- XLSX language tables consumed by Mod Config GUI and Elin's runtime message system.

## Commands

- Restore: `dotnet restore LuckierBlackCat.sln`
- Build: `dotnet build LuckierBlackCat.sln --configuration Release --no-restore`
- Test: `dotnet test LuckierBlackCat.sln --configuration Release --no-build`
- Repository checks: `pre-commit run --all-files`
- Workflow lint: `yamllint .github/workflows/ci.yml .github/workflows/release.yml`

## Project Structure

- `src/LuckierBlackCat.Core` owns input normalization independent of Elin.
- `src/LuckierBlackCat.Patching` owns the exact IL call replacement.
- `src/LuckierBlackCat.Plugin` owns configuration and repeated `Thing.AddEnchant` calls.
- `tests` contains rule, transform, and compiled-plugin contract coverage.
- `LangConfig`, `LangMod`, and the three README files contain player-facing text.

## Code Style

Keep Elin objects in the Plugin layer and pass the game method into the tested Core rule. The
patch helper observes successful rolls for one player-facing summary, then returns the last
successful enchantment so the original method can preserve its existing lick-state behavior.

```csharp
Element lastEnchant = LickRules.RollEnchantments(
    rollCount,
    adjustedLevel,
    item.AddEnchant,
    RecordSuccessfulRoll);
```

## Testing Strategy

- Core tests define the supported enchant-count range of 1 through 10.
- Patching tests require exactly one `get_LV` and `AddEnchant` anchor, replace only that call,
  and preserve instruction metadata.
- Contract tests verify the compiled plugin exposes `EnchantCount` and routes the original
  enchantment call through the scoped helper and Elin's game-log API.
- The full solution test suite and repository checks must pass before publishing.

## Boundaries

- Always: keep `EnchantTimes` as the level multiplier and `EnchantCount` as the roll count.
- Always: preserve the original item eligibility and one-lick state checks.
- Always: clamp persisted or manually edited roll counts to 1 through 10.
- Always: permit duplicate enchantment rolls; Elin may stack their levels.
- Ask first: changing the default above 1 or changing the maximum above 10.
- Never: patch global `Thing.AddEnchant` or remove Elin's lick-state guard.
- Never: add custom save data merely to mark every rolled enchantment with the cat icon.

## Success Criteria

- `EnchantCount = 1` is behaviorally equivalent to the current single-roll implementation.
- `EnchantCount = N` invokes `Thing.AddEnchant` exactly N times during one successful lick.
- Every roll uses the existing adjusted level from `EnchantTimes` and black-cat saliva.
- The helper returns the last non-null enchantment; Elin stores that ID as the lick marker.
- Every successful lick writes one localized game-log summary containing the item, configured
  roll count, successful roll count, and number of distinct affected enchantments.
- A previously licked item remains ineligible for pickup and prayer licking.
- Mod Config GUI and all README variants describe both configuration values distinctly.

## Open Questions

None. The gameplay semantics and compatibility boundaries were approved before implementation.
