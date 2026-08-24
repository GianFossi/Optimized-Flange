# Core Calculation Contracts — Step 3

## Purpose

Step 3 defines the core contracts required before implementing ASME, PCC, API, IOGP, TEMA, EN, or project-policy calculations.

It deliberately contains **no normative engineering formulas**. The goal is to make future calculations traceable, testable, reproducible, and separable from UI, persistence, and reporting.

## Domain additions

```text
CalculationRequest
├── RequestId
├── FlangedJoint
├── CalculationProcedureDefinition
├── SelectedLoadCaseIds[]
└── AcceptanceCriteria[]

CalculationProcedureDefinition
├── ProcedureId
├── Kind
├── Name
├── Rules[]
├── RequiredGeometryBasis
└── Qualification

EngineeringRuleMetadata
├── RuleId
├── Name
├── Module
├── Source
├── Qualification
└── ValidationEvidence[]

CalculationResult
├── ResultId
├── ExecutionStatus
├── AssessmentStatus
├── Qualification
├── Checks[]
└── Trace

CheckResult
├── CheckId
├── Rule
├── Status
├── Severity
├── Comparison?
├── GoverningCase?
├── MessageCode
└── Trace
```

## Important boundaries

- A rule metadata record may reference a standard, edition, clause, and formula identifier, but it does not implement or paraphrase the formula.
- A check result records status, severity, governing case, message code, comparison values, and trace data. It is not reduced to a Boolean pass/fail.
- Trace values separate inputs, derived inputs, intermediate quantities, limits, results, and diagnostics.
- Dependencies are explicit so later calculation invalidation and reproducibility can be based on project inputs, external-data snapshots, selected options, and rule metadata.
- Calculation engines are represented only by the function shape `CalculationRequest -> Result<CalculationResult, CalculationError list>`.
- Human-readable text remains outside the core. The core emits stable message codes.

## Next direction

The next project phase is the testing and validation campaign skeleton:

1. add test projects and test categories;
2. add unit tests for non-normative domain behavior;
3. add placeholders/registries for future clause, reference, regression, integration, and benchmark evidence;
4. only then implement normative calculation engines with source/edition/clause references and tests.
