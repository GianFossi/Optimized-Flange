namespace OptimizedFlange.Configuration

/// <summary>Provides built-in defaults used only when no persisted user defaults are available.</summary>
module Defaults =
    /// <summary>Current application settings schema version.</summary>
    [<Literal>]
    let ApplicationSettingsSchemaVersion = 1

    /// <summary>Current calculation-defaults schema version.</summary>
    [<Literal>]
    let CalculationDefaultsSchemaVersion = 1

    /// <summary>Current database-path settings schema version.</summary>
    [<Literal>]
    let DatabasePathSettingsSchemaVersion = 1

    let private databaseLocation rootFolder id name relativePath priority =
        {
            Id = id
            Name = name
            Path = System.IO.Path.Combine(rootFolder, relativePath)
            Enabled = true
            ReadOnly = true
            Priority = priority
            LastAccessedAt = None
            Fingerprint = None
        }

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

    /// <summary>Creates built-in database path defaults without binding the application to a workstation path.</summary>
    let databasePaths : DatabasePathSettings =
        {
            SchemaVersion = DatabasePathSettingsSchemaVersion
            RootDatabaseFolder = None
            Materials = []
            Bolting = []
            Threads = []
            Gaskets = []
            TighteningTools = []
            ValidationCases = []
            Custom = []
        }

    /// <summary>Creates local technical database path defaults from an externally configured root folder.</summary>
    let databasePathsFromRootFolder rootFolder : DatabasePathSettings =
        {
            SchemaVersion = DatabasePathSettingsSchemaVersion
            RootDatabaseFolder = Some rootFolder
            Materials =
                [
                    databaseLocation rootFolder "DB.MATERIALS.MYLIB" "User material library defaults" "MyLib.json" 5
                    databaseLocation rootFolder "DB.MATERIALS.ASME.SQLITE" "ASME materials SQLite database" "asme_sec2_partd_metric.sqlite3" 10
                    databaseLocation rootFolder "DB.MATERIALS.ASME.WORKING" "ASME materials working database" "asme_materials.working.db" 20
                ]
            Bolting =
                [
                    databaseLocation rootFolder "DB.BOLTING.TIE_RODS" "Bolting tie rod database" "Boltings.xml" 10
                ]
            Threads = []
            Gaskets =
                [
                    databaseLocation rootFolder "DB.GASKETS.GEOMETRY" "Gasket geometry database" "Gaskets.xml" 10
                    databaseLocation rootFolder "DB.GASKETS.DESIGN_PARAMETERS" "Gasket design parameter database" "GasketDesignParameters.xml" 20
                    databaseLocation rootFolder "DB.GASKETS.MOMENT_FACTORS" "Gasket moment factor database" "GasketMomentFactor.xml" 30
                    databaseLocation rootFolder "DB.FACINGS" "Flange facing database" "Facings.xml" 40
                ]
            TighteningTools = []
            ValidationCases = []
            Custom =
                [
                    databaseLocation rootFolder "DB.FLANGES.STANDARD" "Standard flange database" "Flanges.xml" 10
                    databaseLocation rootFolder "DB.PIPES.STANDARD" "Pipe dimension database" "Pipes.xml" 20
                    databaseLocation rootFolder "DB.RATINGS.ASME_B16" "ASME B16 pressure-temperature rating database" "ASME_B16_Ratings.xml" 30
                    databaseLocation rootFolder "DB.TUBES.BWG" "Tube BWG series database" "TubesBWGSerie.xml" 40
                ]
        }
