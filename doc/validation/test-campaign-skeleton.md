# Test Campaign Skeleton

## Purpose

This phase creates the first automated test structure required before any sourced normative calculation rules are implemented.

## Test projects

```text
tests/
├── OptimizedFlange.UnitTests/
├── OptimizedFlange.PersistenceTests/
└── OptimizedFlange.ValidationTests/
```

## Current coverage

- Non-normative domain structure validation for `FlangedJoint`.
- Recent-file list behavior, including pinned entry preservation and the 20-entry non-pinned cap.
- Calculation-default persistence round trip through the JSON store.
- Explicit placeholders for future clause, reference, and regression campaigns.

## Boundaries

The tests added in this phase do not validate ASME, PCC-1, API 660, IOGP S-614, TEMA, EN 1591, or project-policy engineering equations.

Future normative rule tests must identify source standard, edition, clause/formula reference, reference inputs, expected outputs, and tolerances.
