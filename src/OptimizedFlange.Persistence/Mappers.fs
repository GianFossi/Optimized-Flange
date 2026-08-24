namespace OptimizedFlange.Persistence

open System
open OptimizedFlange.Configuration

/// <summary>Maps application configuration between domain-oriented records and stable persistence DTOs.</summary>
module PersistenceMappers =
    /// <summary>Maps the display unit-system union to its stable persisted identifier.</summary>
    let private unitSystemToString = function
        | SI -> "SI"
        | USCustomary -> "USCustomary"

    /// <summary>Parses a persisted display unit-system identifier.</summary>
    let private unitSystemFromString = function
        | "SI" -> Ok SI
        | "USCustomary" -> Ok USCustomary
        | value -> Error $"Unknown display unit system: {value}"

    /// <summary>Maps the primary design code to its stable persisted identifier.</summary>
    let private primaryCodeToString = function
        | AsmeViiiDivision1 -> "ASME_VIII_DIV1"
        | AsmeViiiDivision2 -> "ASME_VIII_DIV2"

    /// <summary>Parses a persisted primary design-code identifier.</summary>
    let private primaryCodeFromString = function
        | "ASME_VIII_DIV1" -> Ok AsmeViiiDivision1
        | "ASME_VIII_DIV2" -> Ok AsmeViiiDivision2
        | value -> Error $"Unknown primary design code: {value}"

    /// <summary>Maps a solver type to its stable persisted identifier.</summary>
    let private solverTypeToString = function
        | FixedPoint -> "FixedPoint"
        | DampedFixedPoint -> "DampedFixedPoint"
        | NewtonRaphson -> "NewtonRaphson"
        | Hybrid -> "Hybrid"

    /// <summary>Parses a persisted solver identifier.</summary>
    let private solverTypeFromString = function
        | "FixedPoint" -> Ok FixedPoint
        | "DampedFixedPoint" -> Ok DampedFixedPoint
        | "NewtonRaphson" -> Ok NewtonRaphson
        | "Hybrid" -> Ok Hybrid
        | value -> Error $"Unknown solver type: {value}"

    /// <summary>Maps application settings to a persistence DTO.</summary>
    let applicationToDto (value: ApplicationSettings) =
        {
            SchemaVersion = value.SchemaVersion
            Language = value.Language
            DefaultUnitSystem = unitSystemToString value.DefaultUnitSystem
            NumericCulture = value.NumericCulture
            AutoSaveEnabled = value.AutoSaveEnabled
            AutoSaveIntervalMinutes = value.AutoSaveIntervalMinutes
            DefaultProjectFolder = value.DefaultProjectFolder
            DefaultReportFolder = value.DefaultReportFolder
            DefaultBackupFolder = value.DefaultBackupFolder
        }

    /// <summary>Maps a persistence DTO to validated application settings.</summary>
    let applicationFromDto (dto: ApplicationSettingsDto) =
        unitSystemFromString dto.DefaultUnitSystem
        |> Result.map (fun units ->
            {
                SchemaVersion = dto.SchemaVersion
                Language = dto.Language
                DefaultUnitSystem = units
                NumericCulture = dto.NumericCulture
                AutoSaveEnabled = dto.AutoSaveEnabled
                AutoSaveIntervalMinutes = dto.AutoSaveIntervalMinutes
                DefaultProjectFolder = dto.DefaultProjectFolder
                DefaultReportFolder = dto.DefaultReportFolder
                DefaultBackupFolder = dto.DefaultBackupFolder
            })

    /// <summary>Maps a recent-file entry to a persistence DTO.</summary>
    let recentFileToDto (value: RecentFileEntry) =
        {
            Path = value.Path
            DisplayName = value.DisplayName
            LastOpenedAt = value.LastOpenedAt
            LastSavedAt = value.LastSavedAt |> Option.toNullable
            FileExists = value.FileExists
            ProjectSchemaVersion = value.ProjectSchemaVersion |> Option.toNullable
            Pinned = value.Pinned
        }

    /// <summary>Maps a recent-file persistence DTO to its configuration model.</summary>
    let recentFileFromDto (dto: RecentFileEntryDto) =
        {
            Path = dto.Path
            DisplayName = dto.DisplayName
            LastOpenedAt = dto.LastOpenedAt
            LastSavedAt = Option.ofNullable dto.LastSavedAt
            FileExists = dto.FileExists
            ProjectSchemaVersion = Option.ofNullable dto.ProjectSchemaVersion
            Pinned = dto.Pinned
        }

    /// <summary>Maps a database location to a persistence DTO.</summary>
    let databaseLocationToDto (value: DatabaseLocation) =
        {
            Id = value.Id
            Name = value.Name
            Path = value.Path
            Enabled = value.Enabled
            ReadOnly = value.ReadOnly
            Priority = value.Priority
            LastAccessedAt = value.LastAccessedAt |> Option.toNullable
            Fingerprint = value.Fingerprint |> Option.defaultValue null
        }

    /// <summary>Maps a database-location DTO to its configuration model.</summary>
    let databaseLocationFromDto (dto: DatabaseLocationDto) =
        {
            Id = dto.Id
            Name = dto.Name
            Path = dto.Path
            Enabled = dto.Enabled
            ReadOnly = dto.ReadOnly
            Priority = dto.Priority
            LastAccessedAt = Option.ofNullable dto.LastAccessedAt
            Fingerprint = Option.ofObj dto.Fingerprint
        }

    /// <summary>Maps grouped database path settings to a persistence DTO.</summary>
    let databasePathsToDto (value: DatabasePathSettings) =
        let map = List.map databaseLocationToDto >> List.toArray
        {
            SchemaVersion = value.SchemaVersion
            RootDatabaseFolder = value.RootDatabaseFolder |> Option.defaultValue null
            Materials = map value.Materials
            Bolting = map value.Bolting
            Threads = map value.Threads
            Gaskets = map value.Gaskets
            TighteningTools = map value.TighteningTools
            ValidationCases = map value.ValidationCases
            Custom = map value.Custom
        }

    /// <summary>Maps grouped database path DTOs to the configuration model.</summary>
    let databasePathsFromDto (dto: DatabasePathSettingsDto) =
        let map (items: DatabaseLocationDto array) = items |> Array.map databaseLocationFromDto |> Array.toList
        {
            SchemaVersion = dto.SchemaVersion
            RootDatabaseFolder = Option.ofObj dto.RootDatabaseFolder
            Materials = map dto.Materials
            Bolting = map dto.Bolting
            Threads = map dto.Threads
            Gaskets = map dto.Gaskets
            TighteningTools = map dto.TighteningTools
            ValidationCases = map dto.ValidationCases
            Custom = map dto.Custom
        }

    /// <summary>Maps solver defaults to a persistence DTO.</summary>
    let solverToDto (value: SolverDefaults) =
        {
            SolverType = solverTypeToString value.SolverType
            RelativeTolerance = value.RelativeTolerance
            ForceToleranceN = value.ForceToleranceN
            PressureTolerancePa = value.PressureTolerancePa
            DisplacementToleranceM = value.DisplacementToleranceM
            StiffnessToleranceNPerM = value.StiffnessToleranceNPerM
            MaxIterations = value.MaxIterations
            InitialDamping = value.InitialDamping
            MinimumDamping = value.MinimumDamping
            MaxSubdivisions = value.MaxSubdivisions
        }

    /// <summary>Maps a solver DTO to validated solver defaults.</summary>
    let solverFromDto (dto: SolverDefaultsDto) =
        solverTypeFromString dto.SolverType
        |> Result.map (fun solverType ->
            {
                SolverType = solverType
                RelativeTolerance = dto.RelativeTolerance
                ForceToleranceN = dto.ForceToleranceN
                PressureTolerancePa = dto.PressureTolerancePa
                DisplacementToleranceM = dto.DisplacementToleranceM
                StiffnessToleranceNPerM = dto.StiffnessToleranceNPerM
                MaxIterations = dto.MaxIterations
                InitialDamping = dto.InitialDamping
                MinimumDamping = dto.MinimumDamping
                MaxSubdivisions = dto.MaxSubdivisions
            })

    /// <summary>Maps calculation defaults to a stable persistence DTO.</summary>
    let calculationDefaultsToDto (value: CalculationDefaults) =
        {
            SchemaVersion = value.SchemaVersion
            PrimaryCode = primaryCodeToString value.PrimaryCode
            Pcc1Enabled = value.Pcc1Enabled
            Api660Enabled = value.Api660Enabled
            IogpS614Enabled = value.IogpS614Enabled
            Wm1FactorK = value.Wm1FactorK
            TargetUtilization = value.TargetUtilization
            Solver = solverToDto value.Solver
        }

    /// <summary>Maps a calculation-defaults DTO to validated configuration values.</summary>
    let calculationDefaultsFromDto (dto: CalculationDefaultsDto) =
        match primaryCodeFromString dto.PrimaryCode, solverFromDto dto.Solver with
        | Ok primaryCode, Ok solver ->
            Ok {
                SchemaVersion = dto.SchemaVersion
                PrimaryCode = primaryCode
                Pcc1Enabled = dto.Pcc1Enabled
                Api660Enabled = dto.Api660Enabled
                IogpS614Enabled = dto.IogpS614Enabled
                Wm1FactorK = dto.Wm1FactorK
                TargetUtilization = dto.TargetUtilization
                Solver = solver
            }
        | Error message, _ -> Error message
        | _, Error message -> Error message
