namespace OptimizedFlange.Configuration

/// <summary>Provides built-in defaults used only when no persisted user defaults are available.</summary>
module Defaults =
    /// <summary>Current application settings schema version.</summary>
    [<Literal>]
    let ApplicationSettingsSchemaVersion = 1

    /// <summary>Current calculation-defaults schema version.</summary>
    [<Literal>]
    let CalculationDefaultsSchemaVersion = 1

    /// <summary>Creates the built-in solver defaults established by the V1 architecture.</summary>
    let solver =
        {
            SolverType = Hybrid
            RelativeTolerance = 1e-6
            ForceToleranceN = 1.0
            PressureTolerancePa = 1.0
            DisplacementToleranceM = 1e-9
            StiffnessToleranceNPerM = 1.0
            MaxIterations = 50
            InitialDamping = 1.0
            MinimumDamping = 0.05
            MaxSubdivisions = 12
        }

    /// <summary>Creates built-in calculation defaults for a new installation.</summary>
    let calculation : CalculationDefaults =
        {
            SchemaVersion = CalculationDefaultsSchemaVersion
            PrimaryCode = AsmeViiiDivision1
            Pcc1Enabled = true
            Api660Enabled = false
            IogpS614Enabled = false
            Wm1FactorK = 1.00M
            TargetUtilization = 0.90M
            Solver = solver
        }
