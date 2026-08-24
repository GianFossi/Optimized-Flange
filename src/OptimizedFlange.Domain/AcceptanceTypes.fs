namespace OptimizedFlange.Domain

/// <summary>Identifies whether a requirement is mandatory, preferential, or informational.</summary>
type RequirementLevel =
    | Hard
    | Soft
    | Informational

/// <summary>Identifies the source category of an acceptance requirement.</summary>
type RequirementSource =
    | DesignCode
    | Pcc1
    | Api660
    | IogpS614
    | Tema
    | Project
    | User

/// <summary>Represents an explicit acceptance criterion without embedding a normative formula.</summary>
type AcceptanceCriterion =
    {
        /// <summary>Stable criterion identifier.</summary>
        CriterionId: string
        /// <summary>Requirement level.</summary>
        Level: RequirementLevel
        /// <summary>Requirement source category.</summary>
        Source: RequirementSource
        /// <summary>Optional source edition.</summary>
        Edition: string option
        /// <summary>Optional source clause.</summary>
        Clause: string option
        /// <summary>Optional utilization target. Acceptance remains independent from optimization target margin.</summary>
        UtilizationLimit: decimal option
        /// <summary>Optional physical rotation limit in radians.</summary>
        RotationLimitRad: float option
    }
