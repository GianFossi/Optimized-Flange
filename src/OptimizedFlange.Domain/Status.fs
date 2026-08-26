namespace OptimizedFlange.Domain

/// <summary>Describes execution state independently from engineering acceptance.</summary>
type ExecutionStatus =
    | NotStarted
    | Running
    | Completed
    | Cancelled
    | Failed

/// <summary>Describes the engineering assessment state of a result.</summary>
type AssessmentStatus =
    | Satisfied
    | SatisfiedWithWarnings
    | Incomplete
    | NotSatisfied
    | NotApplicable

/// <summary>Describes implementation and qualification maturity.</summary>
type CalculationQualification =
    | Planned
    | Implemented
    | PartiallyImplemented
    | Validated
    | Qualified
    | Deprecated
