namespace OptimizedFlange.Configuration

/// <summary>Represents technical calculation configuration frozen inside an individual project.</summary>
type ProjectCalculationConfiguration =
    {
        /// <summary>Schema version of the project calculation configuration.</summary>
        SchemaVersion: int
        /// <summary>Primary design code selected for this project.</summary>
        PrimaryCode: PrimaryDesignCode
        /// <summary>Whether PCC-1 checks are active for this project.</summary>
        Pcc1Enabled: bool
        /// <summary>Whether API 660 checks are active for this project.</summary>
        Api660Enabled: bool
        /// <summary>Whether IOGP S-614 checks are active for this project.</summary>
        IogpS614Enabled: bool
        /// <summary>Project-specific Wm1 multiplier.</summary>
        Wm1FactorK: decimal
        /// <summary>Project-specific optimization target utilization.</summary>
        TargetUtilization: decimal
        /// <summary>Project-specific solver settings.</summary>
        Solver: SolverDefaults
    }

/// <summary>Creates a project-owned copy of global calculation defaults.</summary>
module ProjectCalculationConfiguration =
    /// <summary>Copies calculation defaults into a new project so later changes to global defaults cannot alter the existing project.</summary>
    let fromDefaults (defaults: CalculationDefaults) =
        {
            SchemaVersion = defaults.SchemaVersion
            PrimaryCode = defaults.PrimaryCode
            Pcc1Enabled = defaults.Pcc1Enabled
            Api660Enabled = defaults.Api660Enabled
            IogpS614Enabled = defaults.IogpS614Enabled
            Wm1FactorK = defaults.Wm1FactorK
            TargetUtilization = defaults.TargetUtilization
            Solver = defaults.Solver
        }
