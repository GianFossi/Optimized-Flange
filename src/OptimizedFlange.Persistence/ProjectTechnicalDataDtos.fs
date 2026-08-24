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

/// <summary>JSON DTO for pressure and temperature on one side of a load case.</summary>
[<CLIMutable>]
type ComponentConditionDto =
    {
        /// <summary>Pressure in pascals.</summary>
        PressurePa: float
        /// <summary>Temperature in kelvin.</summary>
        TemperatureK: float
    }

/// <summary>JSON DTO for a signed six-component joint load vector.</summary>
[<CLIMutable>]
type JointLoadVectorDto =
    {
        /// <summary>Force along local +X in newtons.</summary>
        FxN: float
        /// <summary>Force along local +Y in newtons.</summary>
        FyN: float
        /// <summary>Force along local +Z in newtons.</summary>
        FzN: float
        /// <summary>Moment about local X in newton-metres.</summary>
        MxNm: float
        /// <summary>Moment about local Y in newton-metres.</summary>
        MyNm: float
        /// <summary>Moment about local Z in newton-metres.</summary>
        MzNm: float
    }

/// <summary>JSON DTO for one project load case.</summary>
[<CLIMutable>]
type JointLoadCaseDto =
    {
        /// <summary>Stable load-case identifier.</summary>
        LoadCaseId: string
        /// <summary>Human-readable load-case name.</summary>
        Name: string
        /// <summary>Load-case kind identifier.</summary>
        Kind: string
        /// <summary>Primary-side condition.</summary>
        PrimaryCondition: ComponentConditionDto
        /// <summary>Mating-side condition.</summary>
        MatingCondition: ComponentConditionDto
        /// <summary>Signed external load vector.</summary>
        ExternalLoads: JointLoadVectorDto
    }

/// <summary>Versioned JSON DTO for technical project data owned by an OptimizedFlange project.</summary>
[<CLIMutable>]
type ProjectTechnicalDataDto =
    {
        /// <summary>Technical data schema version.</summary>
        SchemaVersion: int
        /// <summary>Explicit project acceptance criteria.</summary>
        AcceptanceCriteria: AcceptanceCriterionDto array
        /// <summary>Physical project load cases.</summary>
        LoadCases: JointLoadCaseDto array
    }
