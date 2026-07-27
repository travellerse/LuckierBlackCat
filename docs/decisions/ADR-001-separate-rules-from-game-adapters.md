# ADR-001: Separate Rules from Game Adapters

## Status

Accepted

## Date

2026-07-14

## Context

The original plugin mixed item eligibility, licker selection, configuration, map access, audiovisual effects, and Harmony patches in static classes. Tests could not exercise the rules without loading Elin and Unity state.

## Decision

Split the implementation into three modules:

- `LuckierBlackCat.Core` targets `netstandard2.0` and owns pure decisions.
- `LuckierBlackCat.Patching` targets `netstandard2.0` and owns Harmony instruction transforms and patch orchestration rules.
- `LuckierBlackCat.Plugin` targets `net462` and adapts BepInEx, Elin, Unity, and Harmony runtime objects.

Dependencies flow from Plugin to Core and Patching. Core does not reference game or mod framework assemblies.

## Alternatives Considered

### Mock Elin Objects Directly

- Rejected because most relevant state is static or tied to Unity objects.
- Tests would verify mocks rather than stable game-independent decisions.

### Keep a Single Plugin Assembly

- Rejected because cross-platform rule tests would continue to load proprietary game references.
- Runtime packaging is slightly simpler, but change locality and testability remain poor.

## Consequences

- Release packages contain three plugin-owned DLLs.
- Core rules run on CachyOS without Elin installed.
- Game object conversion remains localized in the Plugin module.
- New rule changes require Core tests before adapter changes.
