# ADR-004: Preserve the Version 2 Behavior Scope

## Status

Accepted

## Date

2026-07-14

## Context

The reliability refactor must not silently redesign gameplay. Several original documentation claims did not match the implementation, especially party-only lickers and ranged items.

## Decision

Version 2 preserves these reviewed semantics:

- Lickers are selected from characters on the current map, not only party members.
- Disabling the ability requirement uses the player as the licker.
- Eligibility follows Elin `TryLickEnchant`: equipment, not cursed, above normal rarity, and no existing lick state.
- Pickup licking occurs only after an item becomes player-owned.
- Existing inventory items, failed full-backpack pickups, and NPC pickups do not trigger.
- The returned pickup item controls eligibility, including stacked or transformed results.
- Both active and passive successful prayers trigger for the player; NPC prayers do not.
- Enchantment level remains `base + saliva * multiplier`, with normalized negative inputs and saturating overflow.

## Alternatives Considered

### Restrict Lickers to Party Members

- Deferred as a gameplay change rather than a reliability fix.

### Disable Passive Prayer Licking

- Deferred because the original patch handled both active and passive calls.

### Expand to Slotless Ranged Weapons

- Deferred because Elin's current `TryLickEnchant` starts with `IsEquipment`.

## Consequences

- README wording matches actual behavior.
- Future gameplay changes must supersede this ADR and add rule tests.
