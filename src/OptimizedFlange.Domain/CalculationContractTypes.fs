namespace OptimizedFlange.Domain

/// <summary>Identifies the calculation procedure family requested by the application layer.</summary>
type CalculationProcedureKind =
    | StructuralValidation
    | GeometryResolutionProcedure
    | DesignCodeAssessment
    | Pcc1Assessment
    | Api660Assessment
    | IogpS614Assessment
    | TemaAssessment
    | ProjectPolicyAssessment

/// <summary>Represents a stable calculation procedure contract without implementing the procedure formula.</summary>
type CalculationProcedureDefinition =
    {
        /// <summary>Stable procedure identifier.</summary>
        ProcedureId: string
        /// <summary>Procedure family.</summary>
        Kind: CalculationProcedureKind
        /// <summary>Human-readable procedure name.</summary>
        Name: string
        /// <summary>Rules that this procedure may evaluate.</summary>
        Rules: EngineeringRuleMetadata list
        /// <summary>Required geometry basis for the procedure.</summary>
        RequiredGeometryBasis: GeometryStateBasis
        /// <summary>Current implementation and qualification maturity.</summary>
        Qualification: CalculationQualification
    }

/// <summary>Represents immutable input supplied to a calculation procedure.</summary>
type CalculationRequest =
    {
        /// <summary>Stable request identifier.</summary>
        RequestId: string
        /// <summary>Joint to be evaluated.</summary>
        Joint: FlangedJoint
        /// <summary>Procedure contract requested by the caller.</summary>
        Procedure: CalculationProcedureDefinition
        /// <summary>Load cases selected for evaluation. Empty means all project load cases are selected.</summary>
        SelectedLoadCaseIds: string list
        /// <summary>Project acceptance criteria selected for this procedure.</summary>
        AcceptanceCriteria: AcceptanceCriterion list
    }

/// <summary>Represents an expected calculation preparation or execution failure.</summary>
type CalculationError =
    {
        /// <summary>Stable error code.</summary>
        ErrorCode: string
        /// <summary>Diagnostic severity.</summary>
        Severity: CheckSeverity
        /// <summary>Stable message code for localization.</summary>
        MessageCode: string
        /// <summary>Optional related rule identifier.</summary>
        RuleId: string option
    }

/// <summary>Represents the function shape expected from calculation engines.</summary>
type CalculationEngine = CalculationRequest -> Result<CalculationResult, CalculationError list>
