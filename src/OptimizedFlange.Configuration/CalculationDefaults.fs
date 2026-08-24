namespace OptimizedFlange.Configuration

/// <summary>Identifies the primary design-code family used by a new project.</summary>
type PrimaryDesignCode =
    | AsmeViiiDivision1
    | AsmeViiiDivision2

/// <summary>Defines the default numerical solver algorithm.</summary>
type SolverType =
    | FixedPoint
    | DampedFixedPoint
    | NewtonRaphson
    | Hybrid

/// <summary>Defines numerical solver defaults. These are implementation settings, not normative engineering limits.</summary>
type SolverDefaults =
    {
        /// <summary>Preferred solver algorithm.</summary>
        SolverType: SolverType
        /// <summary>Relative convergence tolerance.</summary>
        RelativeTolerance: float
        /// <summary>Force convergence tolerance in newtons.</summary>
        ForceToleranceN: float
        /// <summary>Pressure convergence tolerance in pascals.</summary>
        PressureTolerancePa: float
        /// <summary>Displacement convergence tolerance in metres.</summary>
        DisplacementToleranceM: float
        /// <summary>Stiffness convergence tolerance in newtons per metre.</summary>
        StiffnessToleranceNPerM: float
        /// <summary>Maximum nonlinear iterations.</summary>
        MaxIterations: int
        /// <summary>Initial damping factor.</summary>
        InitialDamping: float
        /// <summary>Minimum permitted damping factor.</summary>
        MinimumDamping: float
        /// <summary>Maximum automatic subdivisions for a transition step.</summary>
        MaxSubdivisions: int
    }

/// <summary>Defines calculation defaults copied into a project when it is created.</summary>
type CalculationDefaults =
    {
        /// <summary>Schema version of this defaults document.</summary>
        SchemaVersion: int
        /// <summary>Primary design code used for a new project.</summary>
        PrimaryCode: PrimaryDesignCode
        /// <summary>Whether PCC-1 verification is enabled for new projects.</summary>
        Pcc1Enabled: bool
        /// <summary>Whether API 660 verification is enabled for new projects.</summary>
        Api660Enabled: bool
        /// <summary>Whether IOGP S-614 verification is enabled for new projects.</summary>
        IogpS614Enabled: bool
        /// <summary>Default Wm1 multiplier.</summary>
        Wm1FactorK: decimal
        /// <summary>Default optimizer target utilization.</summary>
        TargetUtilization: decimal
        /// <summary>Default solver configuration.</summary>
        Solver: SolverDefaults
    }
