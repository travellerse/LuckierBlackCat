# ADR-005: Add Configurable Enchantment Rolls

## Status

Accepted

## Date

2026-07-27

## Context

The existing `EnchantTimes` setting changes the level passed to Elin's single
`Thing.AddEnchant` call. Its name can be mistaken for an enchantment count, but changing its
meaning would silently break existing player configurations. Allowing already-licked items to
be licked again would also conflict with Elin's property 107 state and make prayer an
unbounded equipment upgrade.

## Decision

- Keep `EnchantTimes` as the level added per Black Cat's Saliva.
- Add `EnchantCount`, defaulting to 1 and bounded to 1 through 10.
- Replace only the reviewed `Thing.AddEnchant` call inside `Thing.TryLickEnchant` with a
  plugin helper that performs the configured number of rolls.
- Use the same adjusted level for every roll.
- Permit duplicate rolls so Elin can stack their levels.
- Return the last successful result so Elin can retain its existing property 107 lick marker.
- Preserve every existing eligibility and one-lick check.

## Alternatives Considered

### Reinterpret EnchantTimes as a Roll Count

- Rejected because existing configurations use it as a level multiplier.

### Permit Repeated Licks on the Same Item

- Rejected because property 107 stores an enchantment ID rather than a lick count.
- Rejected because repeated prayer would continuously upgrade the same equipment.

### Patch Thing.AddEnchant Globally

- Rejected because unrelated game and mod enchantment workflows must remain unchanged.

## Consequences

- One lick can add multiple enchantments while the item remains marked as already licked.
- Only the last successful roll receives Elin's black-cat enchantment icon.
- Duplicate rolls may produce fewer distinct enchantment lines while increasing their levels.
- The transpiler retains one exact anchor that can be preflighted after Elin updates.
