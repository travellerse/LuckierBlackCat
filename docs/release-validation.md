# Release Validation

This checklist is a release gate. Do not publish version 2.0.0 until every item is complete.

## Automated Gates

- [x] `dotnet restore LuckierBlackCat.sln`
- [x] `dotnet build LuckierBlackCat.sln --configuration Release --no-restore`
- [x] `dotnet test LuckierBlackCat.sln --configuration Release --no-build`
- [x] Core line and branch coverage are at least 90%.
- [x] Reference DLL hash, method contracts, and IL anchors pass.
- [x] Transpiler 0/1/2 match cases pass.
- [x] Transpiler metadata preservation cases pass.
- [x] Package contains the three plugin-owned DLLs and six release resources.
- [x] CI and Release workflow YAML passes `yamllint`.
- [x] `git diff --check` passes.
- [x] `decompiled/` is ignored and untracked.

## Elin Runtime Gates

Startup validation used the installed Elin build `24059635` on CachyOS with BepInEx
`6.0.0-pre.1`. The installed `Elin.dll` matched the contract fixture SHA-256
`118334af7a7ae6ce6798946865904594817734553c6f3728ea9fbea2eac9579a`.
LuckierBlackCat was loaded from a local package with only the built-in packages active.
Each startup used the original default configuration except for the one option under
test, and the configuration was restored after the matrix.

- [x] Start Elin with only LuckierBlackCat enabled; verify all enabled features report `Applied`.
- [x] Disable each feature configuration option separately; verify it reports `Disabled` while the other features report `Applied`.
- [ ] Open a generated chest with the Lucky Cat character farther than the vanilla range.
- [ ] Spawn eligible loot with the Lucky Cat character farther than the vanilla range.
- [ ] Pick up eligible equipment into free inventory space; verify exactly one lick.
- [ ] Pick up eligible equipment into a stack; verify the returned stack is processed once.
- [ ] Attempt an eligible pickup with a full backpack; verify no lick.
- [ ] Trigger a pickup transformation; verify the returned item controls eligibility.
- [ ] Have an NPC pick up eligible equipment; verify no lick.
- [ ] Perform active prayer; verify all eligible player items are processed once.
- [ ] Trigger passive prayer; verify the same behavior.
- [ ] Set `RequireLickAbility = false`; verify licking works without a character
  with the Ehekatl's Blessing trait.
- [ ] Change `EnchantTimes` between launches; verify the runtime value changes enhancement.
- [ ] Test with another enchantment mod; verify unrelated enchantment generation is unchanged.
- [ ] Quit or disable the mod; verify Harmony patches owned by the plugin are removed.

The startup logs are retained outside the repository under the runtime-validation
temporary directory. Gameplay, normal Steam coexistence, and graceful unload checks
remain open and must not be inferred from the isolated startup results.
