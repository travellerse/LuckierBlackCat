# ADR-003: Minimize and Validate Harmony Patches

## Status

Accepted

## Date

2026-07-14

## Context

The original implementation patched global enchantment generation and matched distance checks using method-name strings. Missing or ambiguous anchors could silently leave a feature ineffective or modify the wrong instruction.

## Decision

- Patch only methods required for black-cat workflows.
- Do not patch global `Thing.GetEnchant` or `Thing.AddEnchant` behavior.
- Match exact `MethodInfo`, threshold, and branch direction.
- Require exactly one transpiler anchor.
- Preserve existing instruction labels and exception blocks.
- Preflight every feature before applying it.
- Mark known contract failures as `Incompatible` and skip only that feature.
- Roll back every patch owned by this plugin when an unexpected apply error occurs.

## Alternatives Considered

### Replace Entire Elin Methods

- Rejected because it duplicates proprietary game logic and maximizes update drift.

### Best-Effort Transpilers

- Rejected because silent partial behavior is harder to diagnose than an explicit disabled feature.

## Consequences

- Startup logs provide `Applied`, `Disabled`, or `Incompatible` per feature.
- Game updates may disable a feature until its contract is reviewed.
- Independent compatible features can continue to run.
