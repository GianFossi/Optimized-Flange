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
- Registry checks for planned standards support, rule placeholders, qualification status, and empty normative interpretations.
- Formula-inventory checks that block implementation-ready formulas unless references and validation cases are recorded.
- Reference-guide checks that keep calculation workbooks non-normative and block macro execution during intake.

## Boundaries

The tests added in this phase do not validate ASME, PCC-1, API 660, IOGP S-614, TEMA, EN 1591, or project-policy engineering equations.

Future normative rule tests must identify source standard, edition, clause/formula reference, reference inputs, expected outputs, and tolerances.

The registry tests intentionally fail if planned normative placeholders are marked implemented before the supporting rule implementation and validation evidence are added.

Formula-inventory tests intentionally keep selected source groups in manual or started inventory status until reliable clause/formula extraction and validation cases exist.

Reference-guide tests intentionally fail if a workbook guide is marked as a normative source or macro-executable intake artifact.
