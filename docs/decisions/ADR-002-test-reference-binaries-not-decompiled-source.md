# ADR-002: Test Reference Binaries, Not Decompiled Source

## Status

Accepted

## Date

2026-07-14

## Context

ILSpy output is useful for understanding Elin behavior, but it is reconstructed source. Compiler details, decompiler choices, and missing references can make that source differ from the executable contract Harmony patches at runtime.

## Decision

Use `ref/Elin.dll` as the authoritative compatibility fixture. Contract tests use Mono.Cecil to verify:

- the reviewed DLL SHA-256;
- target method declaring type, static state, return type, and parameters;
- the exact semantic IL anchors used by transpilers.

The `decompiled/` directory is ignored by Git and is never compiled or read by tests.

## Alternatives Considered

### Compile Decompiled Source

- Rejected because the decompiled project lacks the full game dependency set.
- Rejected because it would test reconstructed C# rather than shipped IL.

### Reflection-Only Method Checks

- Rejected as insufficient for transpilers: a method can keep its parameters while changing the relevant instruction sequence.

## Consequences

- Updating reference DLLs intentionally breaks the hash test until reviewed.
- Compatibility changes are visible before the game is launched.
- Proprietary DLL redistribution remains a repository policy concern independent of the tests.
