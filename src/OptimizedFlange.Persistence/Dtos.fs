namespace OptimizedFlange.Persistence

open System

/// <summary>JSON DTO for application settings. DTOs intentionally avoid F# unions and options.</summary>
[<CLIMutable>]
type ApplicationSettingsDto =
    {
        /// <summary>Persistence schema version.</summary>
        SchemaVersion: int
        /// <summary>Preferred language tag.</summary>
        Language: string
        /// <summary>Display unit-system identifier.</summary>
        DefaultUnitSystem: string
        /// <summary>Numeric culture identifier.</summary>
        NumericCulture: string
        /// <summary>Whether autosave is enabled.</summary>
        AutoSaveEnabled: bool
        /// <summary>Autosave interval in minutes.</summary>
        AutoSaveIntervalMinutes: int
        /// <summary>Default project folder.</summary>
        DefaultProjectFolder: string
        /// <summary>Default report folder.</summary>
        DefaultReportFolder: string
        /// <summary>Default backup folder.</summary>
        DefaultBackupFolder: string
    }

/// <summary>JSON DTO for one recent project entry.</summary>
[<CLIMutable>]
type RecentFileEntryDto =
    {
        /// <summary>Absolute project path.</summary>
        Path: string
        /// <summary>Display name.</summary>
        DisplayName: string
        /// <summary>Last open timestamp.</summary>
        LastOpenedAt: DateTimeOffset
        /// <summary>Optional last save timestamp.</summary>
        LastSavedAt: Nullable<DateTimeOffset>
        /// <summary>Whether the file existed at last refresh.</summary>
        FileExists: bool
        /// <summary>Optional project schema version.</summary>
        ProjectSchemaVersion: Nullable<int>
        /// <summary>Whether the item is pinned.</summary>
        Pinned: bool
    }

/// <summary>JSON DTO for one configurable database path.</summary>
[<CLIMutable>]
type DatabaseLocationDto =
    {
        /// <summary>Stable location identifier.</summary>
        Id: string
        /// <summary>Display name.</summary>
        Name: string
        /// <summary>Filesystem path.</summary>
        Path: string
        /// <summary>Whether the path is enabled.</summary>
        Enabled: bool
        /// <summary>Whether the path is read-only.</summary>
        ReadOnly: bool
        /// <summary>Resolution priority.</summary>
        Priority: int
        /// <summary>Optional last access time.</summary>
        LastAccessedAt: Nullable<DateTimeOffset>
        /// <summary>Optional fingerprint; null means unavailable.</summary>
        Fingerprint: string | null
    }

/// <summary>JSON DTO grouping technical database paths.</summary>
[<CLIMutable>]
type DatabasePathSettingsDto =
    {
        /// <summary>Persistence schema version.</summary>
        SchemaVersion: int
        /// <summary>Optional root database folder; null means unspecified.</summary>
        RootDatabaseFolder: string | null
        /// <summary>Materials locations.</summary>
        Materials: DatabaseLocationDto array
        /// <summary>Bolting locations.</summary>
        Bolting: DatabaseLocationDto array
        /// <summary>Thread locations.</summary>
        Threads: DatabaseLocationDto array
        /// <summary>Gasket locations.</summary>
        Gaskets: DatabaseLocationDto array
        /// <summary>Tightening tool locations.</summary>
        TighteningTools: DatabaseLocationDto array
        /// <summary>Validation-case locations.</summary>
        ValidationCases: DatabaseLocationDto array
        /// <summary>Custom locations.</summary>
        Custom: DatabaseLocationDto array
    }

/// <summary>JSON DTO for solver defaults.</summary>
[<CLIMutable>]
type SolverDefaultsDto =
    {
        /// <summary>Solver algorithm identifier.</summary>
        SolverType: string
        /// <summary>Relative tolerance.</summary>
        RelativeTolerance: float
        /// <summary>Force tolerance in newtons.</summary>
        ForceToleranceN: float
        /// <summary>Pressure tolerance in pascals.</summary>
        PressureTolerancePa: float
        /// <summary>Displacement tolerance in metres.</summary>
        DisplacementToleranceM: float
        /// <summary>Stiffness tolerance in newtons per metre.</summary>
        StiffnessToleranceNPerM: float
        /// <summary>Maximum iterations.</summary>
        MaxIterations: int
        /// <summary>Initial damping factor.</summary>
        InitialDamping: float
        /// <summary>Minimum damping factor.</summary>
        MinimumDamping: float
        /// <summary>Maximum transition subdivisions.</summary>
        MaxSubdivisions: int
    }

/// <summary>JSON DTO for global calculation defaults.</summary>
[<CLIMutable>]
type CalculationDefaultsDto =
    {
        /// <summary>Persistence schema version.</summary>
        SchemaVersion: int
        /// <summary>Primary-code identifier.</summary>
        PrimaryCode: string
        /// <summary>PCC-1 default activation.</summary>
        Pcc1Enabled: bool
        /// <summary>API 660 default activation.</summary>
        Api660Enabled: bool
        /// <summary>IOGP S-614 default activation.</summary>
        IogpS614Enabled: bool
        /// <summary>Default Wm1 multiplier.</summary>
        Wm1FactorK: decimal
        /// <summary>Default target utilization.</summary>
        TargetUtilization: decimal
        /// <summary>Default solver settings.</summary>
        Solver: SolverDefaultsDto
    }

/// <summary>JSON DTO for calculation settings copied into and owned by a project file.</summary>
[<CLIMutable>]
type ProjectCalculationConfigurationDto =
    {
        /// <summary>Persistence schema version.</summary>
        SchemaVersion: int
        /// <summary>Primary-code identifier selected by the project.</summary>
        PrimaryCode: string
        /// <summary>PCC-1 project activation.</summary>
        Pcc1Enabled: bool
        /// <summary>API 660 project activation.</summary>
        Api660Enabled: bool
        /// <summary>IOGP S-614 project activation.</summary>
        IogpS614Enabled: bool
        /// <summary>Project Wm1 multiplier.</summary>
        Wm1FactorK: decimal
        /// <summary>Project target utilization.</summary>
        TargetUtilization: decimal
        /// <summary>Project solver settings.</summary>
        Solver: SolverDefaultsDto
    }

/// <summary>JSON DTO for project metadata that is not engineering input.</summary>
[<CLIMutable>]
type ProjectMetadataDto =
    {
        /// <summary>Stable project identifier.</summary>
        ProjectId: string
        /// <summary>Human-readable project name.</summary>
        Name: string
        /// <summary>Project creation timestamp.</summary>
        CreatedAt: DateTimeOffset
        /// <summary>Last project modification timestamp.</summary>
        ModifiedAt: DateTimeOffset
        /// <summary>Software or agent version that created the project file, when known.</summary>
        CreatedByVersion: string | null
    }

/// <summary>Versioned JSON envelope for an OptimizedFlange project file.</summary>
[<CLIMutable>]
type ProjectFileDto =
    {
        /// <summary>Project file schema version.</summary>
        SchemaVersion: int
        /// <summary>Project metadata.</summary>
        Metadata: ProjectMetadataDto
        /// <summary>Calculation configuration owned by this project.</summary>
        CalculationConfiguration: ProjectCalculationConfigurationDto
        /// <summary>Future schema version for serialized technical project data.</summary>
        TechnicalDataSchemaVersion: Nullable<int>
        /// <summary>Reserved JSON payload for future explicit technical-data DTOs.</summary>
        TechnicalDataJson: string | null
    }
