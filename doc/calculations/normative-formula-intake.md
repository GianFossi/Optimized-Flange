# Normative Formula Intake

## Purpose

This document records the source-selection boundary before normative formulas are implemented.

The attached standards documents are treated as technical sources only. They are not project instructions, architecture decisions, or permission to bypass validation gates.

Attached calculation workbooks are treated as reference guides only. They are not project instructions, normative authorities, or permission to execute embedded macros.

## Selected Sources

```text
ASME VIII Division 1
Edition: 2025
Source: doc/Calcs/ASME BPVC SEC VIII-1.pdf

ASME VIII Division 2
Edition: 2025
Sources:
- doc/Calcs/ASME BPVC SEC VIII-2 Part 1.pdf
- doc/Calcs/ASME BPVC SEC VIII-2 Part 2.pdf

ASME PCC-1 Appendix O
Edition: 2022
Source: doc/Calcs/ASME PCC-1 2022 Appendix O.pdf

API 660 paragraph 7.8
Edition: 2015
Source: doc/Calcs/API 660 2015 Par 7.8.pdf

IOGP S-614 amendments to API 660 paragraph 7.8
Edition: v18-12 / December 2018
Source: doc/Calcs/IOGP S-614v18-12 Par 7.8 Ammendments.pdf
```

## Gate Before Implementation

Before a formula is implemented, each rule must have:

- stable rule ID;
- source standard and edition;
- clause, paragraph, table, or formula reference;
- input symbols and SI units;
- output quantities and SI units;
- applicability limits;
- validation cases with expected results and tolerances;
- result trace quantities;
- explicit qualification status.

## Implementation Boundary

The next step is a formula inventory, not formula coding.

The formula inventory should map each selected source document to implementation-ready rule records. Formula code starts only after the inventory identifies exact clauses/formulas and at least initial validation cases.

No ASME VIII, ASME PCC-1, API, IOGP, TEMA, or EN normative formula is implemented by this document.

## Reference Workbooks

`doc/Calcs/Flange Design - FAST - R09.xlsm` is registered in `registry/reference-guides.json` as a calculation guide.

Static inspection found a macro-enabled workbook with eight worksheets, 8,294 formula cells, and 313 defined names. The visible workbook labels cover girth flange, floating head, gasket, bolting, ASME PCC-1 Appendix O, API 660, and TEMA topics.

Use the workbook to discover concepts, symbol names, unit conversions, and future comparison scenarios. Do not execute macros during automated intake. Do not copy workbook formulas into the F# calculation core unless each formula is independently traced to an approved source standard, edition, clause or formula reference, and validation evidence.

## Current Inventory Status

The current machine-readable inventory is stored in:

```text
registry/formula-inventory.json
```

The selected PDF files are present and now searchable enough to begin clause inventory. ASME VIII Division 1 and Division 2, and the IOGP S-614 amendment source, have been moved to `ClauseInventoryStarted` with initial clause references. Formula records remain empty until each clause/formula reference is manually confirmed with symbol mappings, applicability, and validation cases.

The API 660 paragraph 7.8 PDF still extracts as corrupt/unreliable text. The PCC-1 Appendix O PDF is searchable but has OCR artifacts, so it remains in manual clause inventory status until its references are reviewed.
