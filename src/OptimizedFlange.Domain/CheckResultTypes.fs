namespace OptimizedFlange.Domain

/// <summary>Identifies the severity of a check result or diagnostic message.</summary>
type CheckSeverity =
    | Info
    | Warning
    | Error
    | Critical

/// <summary>Identifies the load case or condition that governs a result.</summary>
type GoverningCase =
    {
        /// <summary>Stable load-case identifier, when the check is load-case specific.</summary>
        LoadCaseId: string option
        /// <summary>Stable side identifier, when the check is side specific.</summary>
        SideId: string option
        /// <summary>Stable sealing-zone identifier, when the check is zone specific.</summary>
        SealingZoneId: string option
        /// <summary>Stable bolt or bolt-group identifier, when the check is bolt specific.</summary>
        BoltReferenceId: string option
    }

/// <summary>Represents the numerical comparison produced by a technical check.</summary>
type CheckComparison =
    {
        /// <summary>Actual calculated value.</summary>
        Actual: TraceQuantity option
        /// <summary>Limit or allowable value.</summary>
        Limit: TraceQuantity option
        /// <summary>Utilization ratio, where a value above 1 usually indicates exceedance for ratio checks.</summary>
        Utilization: decimal option
    }

/// <summary>Represents one structured technical check result.</summary>
type CheckResult =
    {
        /// <summary>Stable check identifier.</summary>
        CheckId: string
        /// <summary>Rule metadata used to evaluate or define the check.</summary>
        Rule: EngineeringRuleMetadata
        /// <summary>Engineering assessment status.</summary>
        Status: AssessmentStatus
        /// <summary>Diagnostic severity.</summary>
        Severity: CheckSeverity
        /// <summary>Numerical comparison, if this check has a scalar actual/limit form.</summary>
        Comparison: CheckComparison option
        /// <summary>Governing case or component, when applicable.</summary>
        GoverningCase: GoverningCase option
        /// <summary>Stable message code for rendering localized text outside the core.</summary>
        MessageCode: string
        /// <summary>Trace values and dependencies used by this check.</summary>
        Trace: CalculationTrace
    }

/// <summary>Represents the aggregate result of a calculation procedure.</summary>
type CalculationResult =
    {
        /// <summary>Stable result identifier.</summary>
        ResultId: string
        /// <summary>Execution state of the calculation procedure.</summary>
        ExecutionStatus: ExecutionStatus
        /// <summary>Overall engineering assessment state.</summary>
        AssessmentStatus: AssessmentStatus
        /// <summary>Qualification maturity of the calculation procedure.</summary>
        Qualification: CalculationQualification
        /// <summary>Individual structured check results.</summary>
        Checks: CheckResult list
        /// <summary>Procedure-level trace values and dependencies.</summary>
        Trace: CalculationTrace
    }
