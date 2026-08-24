namespace OptimizedFlange.Domain

/// <summary>Identifies the technical module that owns an engineering rule.</summary>
type EngineeringRuleModule =
    | GeometryResolution
    | LoadResolution
    | BoltingAssessment
    | GasketAssessment
    | FlangeStressAssessment
    | RotationAssessment
    | AssemblyAssessment
    | ProjectPolicy

/// <summary>Identifies the kind of technical source behind an engineering rule.</summary>
type EngineeringRuleSourceKind =
    | Standard
    | RecommendedPractice
    | CompanySpecification
    | ProjectSpecification
    | UserDefinedPolicy

/// <summary>Represents the source reference for a rule without embedding the rule formula.</summary>
type EngineeringRuleSource =
    {
        /// <summary>Source family such as ASME, PCC-1, API, IOGP, TEMA, EN, project, or user.</summary>
        SourceKind: EngineeringRuleSourceKind
        /// <summary>Formal source name.</summary>
        Name: string
        /// <summary>Source edition, revision, or issue identifier.</summary>
        Edition: string option
        /// <summary>Clause, paragraph, table, appendix, or section reference.</summary>
        Clause: string option
        /// <summary>Formula, table row, figure, or equation reference when applicable.</summary>
        FormulaReference: string option
    }

/// <summary>Links an engineering rule to validation or qualification evidence.</summary>
type ValidationEvidenceReference =
    {
        /// <summary>Stable evidence identifier.</summary>
        EvidenceId: string
        /// <summary>Evidence kind, such as unit test, clause test, reference case, or regression case.</summary>
        EvidenceKind: string
        /// <summary>Optional path, URI, or registry identifier for the evidence artifact.</summary>
        Location: string option
    }

/// <summary>Represents stable metadata for a rule or check before implementation of its formula.</summary>
type EngineeringRuleMetadata =
    {
        /// <summary>Stable rule identifier.</summary>
        RuleId: string
        /// <summary>Human-readable rule name.</summary>
        Name: string
        /// <summary>Technical module that owns the rule.</summary>
        Module: EngineeringRuleModule
        /// <summary>Source reference for the rule.</summary>
        Source: EngineeringRuleSource
        /// <summary>Current implementation and qualification maturity.</summary>
        Qualification: CalculationQualification
        /// <summary>Validation or qualification evidence references.</summary>
        ValidationEvidence: ValidationEvidenceReference list
    }
