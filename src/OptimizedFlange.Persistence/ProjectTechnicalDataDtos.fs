namespace OptimizedFlange.Persistence

/// <summary>JSON DTO for one explicit project acceptance criterion.</summary>
[<CLIMutable>]
type AcceptanceCriterionDto =
    {
        /// <summary>Stable criterion identifier.</summary>
        CriterionId: string
        /// <summary>Requirement level identifier.</summary>
        Level: string
        /// <summary>Requirement source identifier.</summary>
        Source: string
        /// <summary>Optional source edition; null means unspecified.</summary>
        Edition: string | null
        /// <summary>Optional source clause; null means unspecified.</summary>
        Clause: string | null
        /// <summary>Optional utilization limit; null means unspecified.</summary>
        UtilizationLimit: System.Nullable<decimal>
        /// <summary>Optional rotation limit in radians; null means unspecified.</summary>
        RotationLimitRad: System.Nullable<float>
    }

/// <summary>Versioned JSON DTO for technical project data owned by an OptimizedFlange project.</summary>
[<CLIMutable>]
type ProjectTechnicalDataDto =
    {
        /// <summary>Technical data schema version.</summary>
        SchemaVersion: int
        /// <summary>Explicit project acceptance criteria.</summary>
        AcceptanceCriteria: AcceptanceCriterionDto array
    }
