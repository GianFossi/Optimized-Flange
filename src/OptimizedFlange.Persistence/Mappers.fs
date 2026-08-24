namespace OptimizedFlange.Persistence

open System
open OptimizedFlange.Configuration

/// <summary>Maps application configuration between domain-oriented records and stable persistence DTOs.</summary>
module PersistenceMappers =
    /// <summary>Converts an option to a nullable value for persistence DTOs.</summary>
    let private optionToNullable (value: 'T option) =
        match value with
        | Some item -> Nullable item
        | None -> Nullable()

    /// <summary>Converts a nullable value from a persistence DTO to an option.</summary>
    let private nullableToOption (value: Nullable<'T>) =
        if value.HasValue then Some value.Value else None

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

    /// <summary>Maps requirement levels to stable persisted identifiers.</summary>
    let private requirementLevelToString = function
        | OptimizedFlange.Domain.Hard -> "Hard"
        | OptimizedFlange.Domain.Soft -> "Soft"
        | OptimizedFlange.Domain.Informational -> "Informational"

    /// <summary>Parses stable persisted requirement-level identifiers.</summary>
    let private requirementLevelFromString = function
        | "Hard" -> Ok OptimizedFlange.Domain.Hard
        | "Soft" -> Ok OptimizedFlange.Domain.Soft
        | "Informational" -> Ok OptimizedFlange.Domain.Informational
        | value -> Error $"Unknown requirement level: {value}"

    /// <summary>Maps requirement sources to stable persisted identifiers.</summary>
    let private requirementSourceToString = function
        | OptimizedFlange.Domain.DesignCode -> "DesignCode"
        | OptimizedFlange.Domain.Pcc1 -> "Pcc1"
        | OptimizedFlange.Domain.Api660 -> "Api660"
        | OptimizedFlange.Domain.IogpS614 -> "IogpS614"
        | OptimizedFlange.Domain.Tema -> "Tema"
        | OptimizedFlange.Domain.Project -> "Project"
        | OptimizedFlange.Domain.User -> "User"

    /// <summary>Parses stable persisted requirement-source identifiers.</summary>
    let private requirementSourceFromString = function
        | "DesignCode" -> Ok OptimizedFlange.Domain.DesignCode
        | "Pcc1" -> Ok OptimizedFlange.Domain.Pcc1
        | "Api660" -> Ok OptimizedFlange.Domain.Api660
        | "IogpS614" -> Ok OptimizedFlange.Domain.IogpS614
        | "Tema" -> Ok OptimizedFlange.Domain.Tema
        | "Project" -> Ok OptimizedFlange.Domain.Project
        | "User" -> Ok OptimizedFlange.Domain.User
        | value -> Error $"Unknown requirement source: {value}"

    /// <summary>Maps application settings to a persistence DTO.</summary>
    let applicationToDto (value: ApplicationSettings) : ApplicationSettingsDto =
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
    let applicationFromDto (dto: ApplicationSettingsDto) : Result<ApplicationSettings, string> =
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
    let recentFileToDto (value: RecentFileEntry) : RecentFileEntryDto =
        {
            Path = value.Path
            DisplayName = value.DisplayName
            LastOpenedAt = value.LastOpenedAt
            LastSavedAt = value.LastSavedAt |> optionToNullable
            FileExists = value.FileExists
            ProjectSchemaVersion = value.ProjectSchemaVersion |> optionToNullable
            Pinned = value.Pinned
        }

    /// <summary>Maps a recent-file persistence DTO to its configuration model.</summary>
    let recentFileFromDto (dto: RecentFileEntryDto) : RecentFileEntry =
        {
            Path = dto.Path
            DisplayName = dto.DisplayName
            LastOpenedAt = dto.LastOpenedAt
            LastSavedAt = nullableToOption dto.LastSavedAt
            FileExists = dto.FileExists
            ProjectSchemaVersion = nullableToOption dto.ProjectSchemaVersion
            Pinned = dto.Pinned
        }

    /// <summary>Maps a database location to a persistence DTO.</summary>
    let databaseLocationToDto (value: DatabaseLocation) : DatabaseLocationDto =
        {
            Id = value.Id
            Name = value.Name
            Path = value.Path
            Enabled = value.Enabled
            ReadOnly = value.ReadOnly
            Priority = value.Priority
            LastAccessedAt = value.LastAccessedAt |> optionToNullable
            Fingerprint =
                match value.Fingerprint with
                | Some fingerprint -> fingerprint
                | None -> (null: string | null)
        }

    /// <summary>Maps a database-location DTO to its configuration model.</summary>
    let databaseLocationFromDto (dto: DatabaseLocationDto) : DatabaseLocation =
        {
            Id = dto.Id
            Name = dto.Name
            Path = dto.Path
            Enabled = dto.Enabled
            ReadOnly = dto.ReadOnly
            Priority = dto.Priority
            LastAccessedAt = nullableToOption dto.LastAccessedAt
            Fingerprint = Option.ofObj dto.Fingerprint
        }

    /// <summary>Maps grouped database path settings to a persistence DTO.</summary>
    let databasePathsToDto (value: DatabasePathSettings) : DatabasePathSettingsDto =
        let map = List.map databaseLocationToDto >> List.toArray
        {
            SchemaVersion = value.SchemaVersion
            RootDatabaseFolder =
                match value.RootDatabaseFolder with
                | Some folder -> folder
                | None -> (null: string | null)
            Materials = map value.Materials
            Bolting = map value.Bolting
            Threads = map value.Threads
            Gaskets = map value.Gaskets
            TighteningTools = map value.TighteningTools
            ValidationCases = map value.ValidationCases
            Custom = map value.Custom
        }

    /// <summary>Maps grouped database path DTOs to the configuration model.</summary>
    let databasePathsFromDto (dto: DatabasePathSettingsDto) : DatabasePathSettings =
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
    let solverToDto (value: SolverDefaults) : SolverDefaultsDto =
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
    let solverFromDto (dto: SolverDefaultsDto) : Result<SolverDefaults, string> =
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
    let calculationDefaultsToDto (value: CalculationDefaults) : CalculationDefaultsDto =
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
    let calculationDefaultsFromDto (dto: CalculationDefaultsDto) : Result<CalculationDefaults, string> =
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

    /// <summary>Maps project-owned calculation configuration to a stable persistence DTO.</summary>
    let projectCalculationConfigurationToDto
        (value: ProjectCalculationConfiguration)
        : ProjectCalculationConfigurationDto =
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

    /// <summary>Maps a project-owned calculation configuration DTO to validated configuration values.</summary>
    let projectCalculationConfigurationFromDto
        (dto: ProjectCalculationConfigurationDto)
        : Result<ProjectCalculationConfiguration, string> =
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

    /// <summary>Maps a project acceptance criterion to a persistence DTO.</summary>
    let acceptanceCriterionToDto
        (value: OptimizedFlange.Domain.AcceptanceCriterion)
        : AcceptanceCriterionDto =
        {
            CriterionId = value.CriterionId
            Level = requirementLevelToString value.Level
            Source = requirementSourceToString value.Source
            Edition =
                match value.Edition with
                | Some edition -> edition
                | None -> null
            Clause =
                match value.Clause with
                | Some clause -> clause
                | None -> null
            UtilizationLimit = value.UtilizationLimit |> optionToNullable
            RotationLimitRad = value.RotationLimitRad |> optionToNullable
        }

    /// <summary>Maps a persistence DTO to a validated project acceptance criterion.</summary>
    let acceptanceCriterionFromDto
        (dto: AcceptanceCriterionDto)
        : Result<OptimizedFlange.Domain.AcceptanceCriterion, string> =
        match requirementLevelFromString dto.Level, requirementSourceFromString dto.Source with
        | Ok level, Ok source ->
            Ok {
                CriterionId = dto.CriterionId
                Level = level
                Source = source
                Edition = Option.ofObj dto.Edition
                Clause = Option.ofObj dto.Clause
                UtilizationLimit = nullableToOption dto.UtilizationLimit
                RotationLimitRad = nullableToOption dto.RotationLimitRad
            }
        | Error message, _ -> Error message
        | _, Error message -> Error message

    /// <summary>Maps technical project data to its versioned persistence DTO.</summary>
    let projectTechnicalDataToDto
        schemaVersion
        (acceptanceCriteria: OptimizedFlange.Domain.AcceptanceCriterion list)
        : ProjectTechnicalDataDto =
        {
            SchemaVersion = schemaVersion
            AcceptanceCriteria =
                acceptanceCriteria
                |> List.map acceptanceCriterionToDto
                |> List.toArray
        }

    /// <summary>Maps a versioned technical-data DTO to validated project technical data fragments.</summary>
    let projectTechnicalDataFromDto
        (dto: ProjectTechnicalDataDto)
        : Result<OptimizedFlange.Domain.AcceptanceCriterion list, string> =
        dto.AcceptanceCriteria
        |> Array.toList
        |> List.fold
            (fun state item ->
                match state, acceptanceCriterionFromDto item with
                | Ok items, Ok mapped -> Ok (mapped :: items)
                | Error message, _ -> Error message
                | _, Error message -> Error message)
            (Ok [])
        |> Result.map List.rev
