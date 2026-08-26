namespace OptimizedFlange.Persistence

open System
open OptimizedFlange.Configuration
open OptimizedFlange.Domain.Units

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

    /// <summary>Maps load-case kinds to stable persisted identifiers.</summary>
    let private loadCaseKindToString = function
        | OptimizedFlange.Domain.Design -> "Design"
        | OptimizedFlange.Domain.Operating -> "Operating"
        | OptimizedFlange.Domain.Misoperation -> "Misoperation"
        | OptimizedFlange.Domain.Testing -> "Testing"

    /// <summary>Parses stable persisted load-case kind identifiers.</summary>
    let private loadCaseKindFromString = function
        | "Design" -> Ok OptimizedFlange.Domain.Design
        | "Operating" -> Ok OptimizedFlange.Domain.Operating
        | "Misoperation" -> Ok OptimizedFlange.Domain.Misoperation
        | "Testing" -> Ok OptimizedFlange.Domain.Testing
        | value -> Error $"Unknown load case kind: {value}"

    /// <summary>Maps flange geometry sources to stable persisted identifiers.</summary>
    let private flangeGeometrySourceToString (value: OptimizedFlange.Domain.FlangeGeometrySource) =
        match value with
        | OptimizedFlange.Domain.FlangeGeometrySource.CustomDesigned -> "CustomDesigned"
        | OptimizedFlange.Domain.FlangeGeometrySource.StandardDerived -> "StandardDerived"
        | OptimizedFlange.Domain.FlangeGeometrySource.Imported -> "Imported"

    /// <summary>Parses stable persisted flange geometry source identifiers.</summary>
    let private flangeGeometrySourceFromString value : Result<OptimizedFlange.Domain.FlangeGeometrySource, string> =
        match value with
        | "CustomDesigned" -> Ok OptimizedFlange.Domain.FlangeGeometrySource.CustomDesigned
        | "StandardDerived" -> Ok OptimizedFlange.Domain.FlangeGeometrySource.StandardDerived
        | "Imported" -> Ok OptimizedFlange.Domain.FlangeGeometrySource.Imported
        | value -> Error $"Unknown flange geometry source: {value}"

    /// <summary>Maps flange types to stable persisted identifiers.</summary>
    let private flangeTypeToString = function
        | OptimizedFlange.Domain.IntegralFlange -> "IntegralFlange"
        | OptimizedFlange.Domain.FlatCover -> "FlatCover"

    /// <summary>Parses stable persisted flange type identifiers.</summary>
    let private flangeTypeFromString = function
        | "IntegralFlange" -> Ok OptimizedFlange.Domain.IntegralFlange
        | "FlatCover" -> Ok OptimizedFlange.Domain.FlatCover
        | value -> Error $"Unknown flange type: {value}"

    /// <summary>Maps seat types to stable persisted identifiers.</summary>
    let private seatTypeToString = function
        | OptimizedFlange.Domain.FlatFace -> "FlatFace"
        | OptimizedFlange.Domain.RaisedFace -> "RaisedFace"
        | OptimizedFlange.Domain.RecessedFace -> "RecessedFace"
        | OptimizedFlange.Domain.TongueAndGroove -> "TongueAndGroove"
        | OptimizedFlange.Domain.MaleFemale -> "MaleFemale"
        | OptimizedFlange.Domain.RingTypeJointGroove -> "RingTypeJointGroove"
        | OptimizedFlange.Domain.LensSeat -> "LensSeat"
        | OptimizedFlange.Domain.CustomSeat -> "CustomSeat"

    /// <summary>Parses stable persisted seat type identifiers.</summary>
    let private seatTypeFromString = function
        | "FlatFace" -> Ok OptimizedFlange.Domain.FlatFace
        | "RaisedFace" -> Ok OptimizedFlange.Domain.RaisedFace
        | "RecessedFace" -> Ok OptimizedFlange.Domain.RecessedFace
        | "TongueAndGroove" -> Ok OptimizedFlange.Domain.TongueAndGroove
        | "MaleFemale" -> Ok OptimizedFlange.Domain.MaleFemale
        | "RingTypeJointGroove" -> Ok OptimizedFlange.Domain.RingTypeJointGroove
        | "LensSeat" -> Ok OptimizedFlange.Domain.LensSeat
        | "CustomSeat" -> Ok OptimizedFlange.Domain.CustomSeat
        | value -> Error $"Unknown seat type: {value}"

    /// <summary>Maps hub topologies to stable persisted identifiers.</summary>
    let private hubTopologyToString = function
        | OptimizedFlange.Domain.NoHub -> "NoHub"
        | OptimizedFlange.Domain.StraightHub -> "StraightHub"
        | OptimizedFlange.Domain.SingleTaperHub -> "SingleTaperHub"
        | OptimizedFlange.Domain.DoubleTaperHub -> "DoubleTaperHub"

    /// <summary>Parses stable persisted hub topology identifiers.</summary>
    let private hubTopologyFromString = function
        | "NoHub" -> Ok OptimizedFlange.Domain.NoHub
        | "StraightHub" -> Ok OptimizedFlange.Domain.StraightHub
        | "SingleTaperHub" -> Ok OptimizedFlange.Domain.SingleTaperHub
        | "DoubleTaperHub" -> Ok OptimizedFlange.Domain.DoubleTaperHub
        | value -> Error $"Unknown hub topology: {value}"

    /// <summary>Maps bolt arrangements to stable persisted identifiers.</summary>
    let private boltArrangementToString (value: OptimizedFlange.Domain.BoltArrangement) =
        match value with
        | OptimizedFlange.Domain.ThroughStuds -> "ThroughStuds"
        | OptimizedFlange.Domain.BlindTappedStuds -> "BlindTappedStuds"

    /// <summary>Parses stable persisted bolt arrangement identifiers.</summary>
    let private boltArrangementFromString value : Result<OptimizedFlange.Domain.BoltArrangement, string> =
        match value with
        | "ThroughStuds" -> Ok OptimizedFlange.Domain.ThroughStuds
        | "BlindTappedStuds" -> Ok OptimizedFlange.Domain.BlindTappedStuds
        | value -> Error $"Unknown bolt arrangement: {value}"

    /// <summary>Maps thread standards to stable persisted identifiers.</summary>
    let private threadStandardToString (value: OptimizedFlange.Domain.ThreadStandard) =
        match value with
        | OptimizedFlange.Domain.AsmeB11UnifiedInch -> "AsmeB11UnifiedInch"
        | OptimizedFlange.Domain.AsmeB113Metric -> "AsmeB113Metric"
        | OptimizedFlange.Domain.ProjectDefinedThread -> "ProjectDefinedThread"

    /// <summary>Parses stable persisted thread standard identifiers.</summary>
    let private threadStandardFromString value : Result<OptimizedFlange.Domain.ThreadStandard, string> =
        match value with
        | "AsmeB11UnifiedInch" -> Ok OptimizedFlange.Domain.AsmeB11UnifiedInch
        | "AsmeB113Metric" -> Ok OptimizedFlange.Domain.AsmeB113Metric
        | "ProjectDefinedThread" -> Ok OptimizedFlange.Domain.ProjectDefinedThread
        | value -> Error $"Unknown thread standard: {value}"

    /// <summary>Maps bolt area bases to stable persisted identifiers.</summary>
    let private boltAreaBasisToString (value: OptimizedFlange.Domain.BoltAreaBasis) =
        match value with
        | OptimizedFlange.Domain.TensileStressArea -> "TensileStressArea"
        | OptimizedFlange.Domain.MinimumRootArea -> "MinimumRootArea"
        | OptimizedFlange.Domain.GoverningResistingArea -> "GoverningResistingArea"

    /// <summary>Parses stable persisted bolt area basis identifiers.</summary>
    let private boltAreaBasisFromString value : Result<OptimizedFlange.Domain.BoltAreaBasis, string> =
        match value with
        | "TensileStressArea" -> Ok OptimizedFlange.Domain.TensileStressArea
        | "MinimumRootArea" -> Ok OptimizedFlange.Domain.MinimumRootArea
        | "GoverningResistingArea" -> Ok OptimizedFlange.Domain.GoverningResistingArea
        | value -> Error $"Unknown bolt area basis: {value}"

    /// <summary>Maps stud threading types to stable persisted identifiers.</summary>
    let private studThreadingTypeToString (value: OptimizedFlange.Domain.StudThreadingType) =
        match value with
        | OptimizedFlange.Domain.FullyThreaded -> "FullyThreaded"
        | OptimizedFlange.Domain.PartiallyThreaded -> "PartiallyThreaded"
        | OptimizedFlange.Domain.ReducedShank -> "ReducedShank"

    /// <summary>Parses stable persisted stud threading type identifiers.</summary>
    let private studThreadingTypeFromString value : Result<OptimizedFlange.Domain.StudThreadingType, string> =
        match value with
        | "FullyThreaded" -> Ok OptimizedFlange.Domain.FullyThreaded
        | "PartiallyThreaded" -> Ok OptimizedFlange.Domain.PartiallyThreaded
        | "ReducedShank" -> Ok OptimizedFlange.Domain.ReducedShank
        | value -> Error $"Unknown stud threading type: {value}"

    /// <summary>Maps tightening methods to stable persisted identifiers.</summary>
    let private tighteningMethodToString (value: OptimizedFlange.Domain.TighteningMethod) =
        match value with
        | OptimizedFlange.Domain.ManualOrUncontrolled -> "ManualOrUncontrolled"
        | OptimizedFlange.Domain.TorqueControlled -> "TorqueControlled"
        | OptimizedFlange.Domain.HydraulicTorque -> "HydraulicTorque"
        | OptimizedFlange.Domain.HydraulicTensioning -> "HydraulicTensioning"
        | OptimizedFlange.Domain.ElongationControlled -> "ElongationControlled"
        | OptimizedFlange.Domain.TurnOfNut -> "TurnOfNut"
        | OptimizedFlange.Domain.TorqueAndTurn -> "TorqueAndTurn"
        | OptimizedFlange.Domain.UserDefinedTightening -> "UserDefinedTightening"

    /// <summary>Parses stable persisted tightening method identifiers.</summary>
    let private tighteningMethodFromString value : Result<OptimizedFlange.Domain.TighteningMethod, string> =
        match value with
        | "ManualOrUncontrolled" -> Ok OptimizedFlange.Domain.ManualOrUncontrolled
        | "TorqueControlled" -> Ok OptimizedFlange.Domain.TorqueControlled
        | "HydraulicTorque" -> Ok OptimizedFlange.Domain.HydraulicTorque
        | "HydraulicTensioning" -> Ok OptimizedFlange.Domain.HydraulicTensioning
        | "ElongationControlled" -> Ok OptimizedFlange.Domain.ElongationControlled
        | "TurnOfNut" -> Ok OptimizedFlange.Domain.TurnOfNut
        | "TorqueAndTurn" -> Ok OptimizedFlange.Domain.TorqueAndTurn
        | "UserDefinedTightening" -> Ok OptimizedFlange.Domain.UserDefinedTightening
        | value -> Error $"Unknown tightening method: {value}"

    /// <summary>Maps gasket families to stable persisted identifiers.</summary>
    let private gasketFamilyToString (value: OptimizedFlange.Domain.GasketFamily) =
        match value with
        | OptimizedFlange.Domain.SpiralWound -> "SpiralWound"
        | OptimizedFlange.Domain.Kammprofile -> "Kammprofile"
        | OptimizedFlange.Domain.CorrugatedMetal -> "CorrugatedMetal"
        | OptimizedFlange.Domain.DoubleJacketed -> "DoubleJacketed"
        | OptimizedFlange.Domain.SoftFlat -> "SoftFlat"
        | OptimizedFlange.Domain.RingTypeJoint -> "RingTypeJoint"
        | OptimizedFlange.Domain.Lens -> "Lens"
        | OptimizedFlange.Domain.WeldedSeal -> "WeldedSeal"
        | OptimizedFlange.Domain.GasketFamily.CustomDesigned -> "CustomDesigned"

    /// <summary>Parses stable persisted gasket family identifiers.</summary>
    let private gasketFamilyFromString value : Result<OptimizedFlange.Domain.GasketFamily, string> =
        match value with
        | "SpiralWound" -> Ok OptimizedFlange.Domain.SpiralWound
        | "Kammprofile" -> Ok OptimizedFlange.Domain.Kammprofile
        | "CorrugatedMetal" -> Ok OptimizedFlange.Domain.CorrugatedMetal
        | "DoubleJacketed" -> Ok OptimizedFlange.Domain.DoubleJacketed
        | "SoftFlat" -> Ok OptimizedFlange.Domain.SoftFlat
        | "RingTypeJoint" -> Ok OptimizedFlange.Domain.RingTypeJoint
        | "Lens" -> Ok OptimizedFlange.Domain.Lens
        | "WeldedSeal" -> Ok OptimizedFlange.Domain.WeldedSeal
        | "CustomDesigned" -> Ok OptimizedFlange.Domain.GasketFamily.CustomDesigned
        | value -> Error $"Unknown gasket family: {value}"

    /// <summary>Maps sealing zone roles to stable persisted identifiers.</summary>
    let private sealingZoneRoleToString (value: OptimizedFlange.Domain.SealingZoneRole) =
        match value with
        | OptimizedFlange.Domain.PrimarySeal -> "PrimarySeal"
        | OptimizedFlange.Domain.SecondarySeal -> "SecondarySeal"
        | OptimizedFlange.Domain.PartitionSeal -> "PartitionSeal"
        | OptimizedFlange.Domain.SupportOnly -> "SupportOnly"
        | OptimizedFlange.Domain.InformationalZone -> "InformationalZone"

    /// <summary>Parses stable persisted sealing zone role identifiers.</summary>
    let private sealingZoneRoleFromString value : Result<OptimizedFlange.Domain.SealingZoneRole, string> =
        match value with
        | "PrimarySeal" -> Ok OptimizedFlange.Domain.PrimarySeal
        | "SecondarySeal" -> Ok OptimizedFlange.Domain.SecondarySeal
        | "PartitionSeal" -> Ok OptimizedFlange.Domain.PartitionSeal
        | "SupportOnly" -> Ok OptimizedFlange.Domain.SupportOnly
        | "InformationalZone" -> Ok OptimizedFlange.Domain.InformationalZone
        | value -> Error $"Unknown sealing zone role: {value}"

    /// <summary>Maps gasket area bases to stable persisted identifiers.</summary>
    let private gasketAreaBasisToString (value: OptimizedFlange.Domain.GasketAreaBasis) =
        match value with
        | OptimizedFlange.Domain.PeripheralNominalSealingArea -> "PeripheralNominalSealingArea"
        | OptimizedFlange.Domain.TotalNominalSealingArea -> "TotalNominalSealingArea"
        | OptimizedFlange.Domain.EffectiveSealingArea -> "EffectiveSealingArea"

    /// <summary>Parses stable persisted gasket area basis identifiers.</summary>
    let private gasketAreaBasisFromString value : Result<OptimizedFlange.Domain.GasketAreaBasis, string> =
        match value with
        | "PeripheralNominalSealingArea" -> Ok OptimizedFlange.Domain.PeripheralNominalSealingArea
        | "TotalNominalSealingArea" -> Ok OptimizedFlange.Domain.TotalNominalSealingArea
        | "EffectiveSealingArea" -> Ok OptimizedFlange.Domain.EffectiveSealingArea
        | value -> Error $"Unknown gasket area basis: {value}"

    /// <summary>Maps mating-side modes to stable persisted identifiers.</summary>
    let private matingSideModeToString (value: OptimizedFlange.Domain.MatingSideMode) =
        match value with
        | OptimizedFlange.Domain.IdenticalToPrimary -> "IdenticalToPrimary"
        | OptimizedFlange.Domain.ExplicitGeometry -> "ExplicitGeometry"
        | OptimizedFlange.Domain.ExternalEquivalent -> "ExternalEquivalent"

    /// <summary>Parses stable persisted mating-side mode identifiers.</summary>
    let private matingSideModeFromString value : Result<OptimizedFlange.Domain.MatingSideMode, string> =
        match value with
        | "IdenticalToPrimary" -> Ok OptimizedFlange.Domain.IdenticalToPrimary
        | "ExplicitGeometry" -> Ok OptimizedFlange.Domain.ExplicitGeometry
        | "ExternalEquivalent" -> Ok OptimizedFlange.Domain.ExternalEquivalent
        | value -> Error $"Unknown mating side mode: {value}"

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

    /// <summary>Maps component pressure/temperature conditions to a persistence DTO.</summary>
    let componentConditionToDto
        (value: OptimizedFlange.Domain.ComponentCondition)
        : ComponentConditionDto =
        {
            PressurePa = float value.PressurePa
            TemperatureK = float value.TemperatureK
        }

    /// <summary>Maps a component condition DTO to the domain model.</summary>
    let componentConditionFromDto
        (dto: ComponentConditionDto)
        : OptimizedFlange.Domain.ComponentCondition =
        {
            PressurePa = dto.PressurePa * 1.0<Pa>
            TemperatureK = dto.TemperatureK * 1.0<K>
        }

    /// <summary>Maps signed force and moment components to a persistence DTO.</summary>
    let jointLoadVectorToDto
        (value: OptimizedFlange.Domain.JointLoadVector)
        : JointLoadVectorDto =
        {
            FxN = float value.FxN
            FyN = float value.FyN
            FzN = float value.FzN
            MxNm = float value.MxNm
            MyNm = float value.MyNm
            MzNm = float value.MzNm
        }

    /// <summary>Maps a signed force and moment DTO to the domain model.</summary>
    let jointLoadVectorFromDto
        (dto: JointLoadVectorDto)
        : OptimizedFlange.Domain.JointLoadVector =
        {
            FxN = dto.FxN * 1.0<N>
            FyN = dto.FyN * 1.0<N>
            FzN = dto.FzN * 1.0<N>
            MxNm = dto.MxNm * 1.0<N m>
            MyNm = dto.MyNm * 1.0<N m>
            MzNm = dto.MzNm * 1.0<N m>
        }

    /// <summary>Maps one project load case to a persistence DTO.</summary>
    let jointLoadCaseToDto
        (value: OptimizedFlange.Domain.JointLoadCase)
        : JointLoadCaseDto =
        {
            LoadCaseId = value.LoadCaseId
            Name = value.Name
            Kind = loadCaseKindToString value.Kind
            PrimaryCondition = componentConditionToDto value.PrimaryCondition
            MatingCondition = componentConditionToDto value.MatingCondition
            ExternalLoads = jointLoadVectorToDto value.ExternalLoads
        }

    /// <summary>Maps a project load-case DTO to the domain model.</summary>
    let jointLoadCaseFromDto
        (dto: JointLoadCaseDto)
        : Result<OptimizedFlange.Domain.JointLoadCase, string> =
        loadCaseKindFromString dto.Kind
        |> Result.map (fun kind ->
            {
                LoadCaseId = dto.LoadCaseId
                Name = dto.Name
                Kind = kind
                PrimaryCondition = componentConditionFromDto dto.PrimaryCondition
                MatingCondition = componentConditionFromDto dto.MatingCondition
                ExternalLoads = jointLoadVectorFromDto dto.ExternalLoads
            })

    /// <summary>Maps surface allowance values to a persistence DTO.</summary>
    let surfaceAllowanceToDto
        (value: OptimizedFlange.Domain.SurfaceAllowance)
        : SurfaceAllowanceDto =
        {
            CorrosionAllowanceM = float value.CorrosionAllowanceM
            WeldOverlayThicknessM = float value.WeldOverlayThicknessM
            MachiningAllowanceM = float value.MachiningAllowanceM
            MinusToleranceM = float value.MinusToleranceM
            PlusToleranceM = float value.PlusToleranceM
        }

    /// <summary>Maps surface allowance DTO values to the domain model.</summary>
    let surfaceAllowanceFromDto
        (dto: SurfaceAllowanceDto)
        : OptimizedFlange.Domain.SurfaceAllowance =
        {
            CorrosionAllowanceM = dto.CorrosionAllowanceM * 1.0<m>
            WeldOverlayThicknessM = dto.WeldOverlayThicknessM * 1.0<m>
            MachiningAllowanceM = dto.MachiningAllowanceM * 1.0<m>
            MinusToleranceM = dto.MinusToleranceM * 1.0<m>
            PlusToleranceM = dto.PlusToleranceM * 1.0<m>
        }

    /// <summary>Maps nominal side geometry to a persistence DTO.</summary>
    let nominalSideGeometryToDto
        (value: OptimizedFlange.Domain.NominalSideGeometry)
        : NominalSideGeometryDto =
        {
            BoreDiameterM = float value.BoreDiameterM
            OutsideDiameterM = float value.OutsideDiameterM
            ThicknessM = float value.ThicknessM
            BoltCircleDiameterM = float value.BoltCircleDiameterM
            SeatOutsideDiameterM = value.SeatOutsideDiameterM |> Option.map float |> optionToNullable
        }

    /// <summary>Maps nominal side geometry DTO values to the domain model.</summary>
    let nominalSideGeometryFromDto
        (dto: NominalSideGeometryDto)
        : OptimizedFlange.Domain.NominalSideGeometry =
        {
            BoreDiameterM = dto.BoreDiameterM * 1.0<m>
            OutsideDiameterM = dto.OutsideDiameterM * 1.0<m>
            ThicknessM = dto.ThicknessM * 1.0<m>
            BoltCircleDiameterM = dto.BoltCircleDiameterM * 1.0<m>
            SeatOutsideDiameterM = dto.SeatOutsideDiameterM |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
        }

    /// <summary>Maps physical hub geometry to a persistence DTO.</summary>
    let physicalHubGeometryToDto
        (value: OptimizedFlange.Domain.PhysicalHubGeometry)
        : PhysicalHubGeometryDto =
        {
            Topology = hubTopologyToString value.Topology
            G0M = value.G0M |> Option.map float |> optionToNullable
            GMidM = value.GMidM |> Option.map float |> optionToNullable
            G1M = value.G1M |> Option.map float |> optionToNullable
            LengthM = value.LengthM |> Option.map float |> optionToNullable
            BreakLocationM = value.BreakLocationM |> Option.map float |> optionToNullable
        }

    /// <summary>Maps physical hub geometry DTO values to the domain model.</summary>
    let physicalHubGeometryFromDto
        (dto: PhysicalHubGeometryDto)
        : Result<OptimizedFlange.Domain.PhysicalHubGeometry, string> =
        hubTopologyFromString dto.Topology
        |> Result.map (fun topology ->
            {
                Topology = topology
                G0M = dto.G0M |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
                GMidM = dto.GMidM |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
                G1M = dto.G1M |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
                LengthM = dto.LengthM |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
                BreakLocationM = dto.BreakLocationM |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
            })

    /// <summary>Maps physical joint-side geometry to a persistence DTO.</summary>
    let jointSideGeometryToDto
        (value: OptimizedFlange.Domain.JointSideGeometry)
        : JointSideGeometryDto =
        {
            SideId = value.SideId
            FlangeType = flangeTypeToString value.FlangeType
            Source = flangeGeometrySourceToString value.Source
            SeatType = seatTypeToString value.SeatType
            Nominal = nominalSideGeometryToDto value.Nominal
            Hub =
                match value.Hub with
                | Some hub -> physicalHubGeometryToDto hub
                | None -> null
            InternalSurface = surfaceAllowanceToDto value.InternalSurface
            GasketSeat = surfaceAllowanceToDto value.GasketSeat
            ExternalSurface = surfaceAllowanceToDto value.ExternalSurface
        }

    /// <summary>Maps a physical joint-side geometry DTO to the domain model.</summary>
    let jointSideGeometryFromDto
        (dto: JointSideGeometryDto)
        : Result<OptimizedFlange.Domain.JointSideGeometry, string> =
        let hub =
            match Option.ofObj dto.Hub with
            | Some hubDto -> physicalHubGeometryFromDto hubDto |> Result.map Some
            | None -> Ok None

        match flangeTypeFromString dto.FlangeType, flangeGeometrySourceFromString dto.Source, seatTypeFromString dto.SeatType, hub with
        | Ok flangeType, Ok source, Ok seatType, Ok mappedHub ->
            Ok {
                SideId = dto.SideId
                FlangeType = flangeType
                Source = source
                SeatType = seatType
                Nominal = nominalSideGeometryFromDto dto.Nominal
                Hub = mappedHub
                InternalSurface = surfaceAllowanceFromDto dto.InternalSurface
                GasketSeat = surfaceAllowanceFromDto dto.GasketSeat
                ExternalSurface = surfaceAllowanceFromDto dto.ExternalSurface
            }
        | Error message, _, _, _ -> Error message
        | _, Error message, _, _ -> Error message
        | _, _, Error message, _ -> Error message
        | _, _, _, Error message -> Error message

    /// <summary>Maps a bolt pattern to a persistence DTO.</summary>
    let boltPatternToDto
        (value: OptimizedFlange.Domain.BoltPattern)
        : BoltPatternDto =
        {
            Count = value.Count
            BoltCircleDiameterM = float value.BoltCircleDiameterM
            StartAngleRad = value.StartAngleRad
        }

    /// <summary>Maps a bolt-pattern DTO to the domain model.</summary>
    let boltPatternFromDto
        (dto: BoltPatternDto)
        : OptimizedFlange.Domain.BoltPattern =
        {
            Count = dto.Count
            BoltCircleDiameterM = dto.BoltCircleDiameterM * 1.0<m>
            StartAngleRad = dto.StartAngleRad
        }

    /// <summary>Maps bolt section areas to a persistence DTO.</summary>
    let boltSectionAreasToDto
        (value: OptimizedFlange.Domain.BoltSectionAreas)
        : BoltSectionAreasDto =
        {
            NominalShankAreaM2 = float value.NominalShankAreaM2
            TensileStressAreaM2 = float value.TensileStressAreaM2
            MinimumRootAreaM2 = float value.MinimumRootAreaM2
            ReducedShankAreaM2 = value.ReducedShankAreaM2 |> Option.map float |> optionToNullable
            GoverningResistingAreaM2 = float value.GoverningResistingAreaM2
        }

    /// <summary>Maps bolt section area DTO values to the domain model.</summary>
    let boltSectionAreasFromDto
        (dto: BoltSectionAreasDto)
        : OptimizedFlange.Domain.BoltSectionAreas =
        {
            NominalShankAreaM2 = dto.NominalShankAreaM2 * 1.0<m^2>
            TensileStressAreaM2 = dto.TensileStressAreaM2 * 1.0<m^2>
            MinimumRootAreaM2 = dto.MinimumRootAreaM2 * 1.0<m^2>
            ReducedShankAreaM2 = dto.ReducedShankAreaM2 |> nullableToOption |> Option.map (fun value -> value * 1.0<m^2>)
            GoverningResistingAreaM2 = dto.GoverningResistingAreaM2 * 1.0<m^2>
        }

    /// <summary>Maps a stud definition to a persistence DTO.</summary>
    let studDefinitionToDto
        (value: OptimizedFlange.Domain.StudDefinition)
        : StudDefinitionDto =
        {
            NominalDiameterM = float value.NominalDiameterM
            PitchM = float value.PitchM
            ThreadStandard = threadStandardToString value.ThreadStandard
            ThreadingType = studThreadingTypeToString value.ThreadingType
            Areas = boltSectionAreasToDto value.Areas
            SpecifiedLengthM = value.SpecifiedLengthM |> Option.map float |> optionToNullable
        }

    /// <summary>Maps a stud definition DTO to the domain model.</summary>
    let studDefinitionFromDto
        (dto: StudDefinitionDto)
        : Result<OptimizedFlange.Domain.StudDefinition, string> =
        match threadStandardFromString dto.ThreadStandard, studThreadingTypeFromString dto.ThreadingType with
        | Ok threadStandard, Ok threadingType ->
            Ok {
                NominalDiameterM = dto.NominalDiameterM * 1.0<m>
                PitchM = dto.PitchM * 1.0<m>
                ThreadStandard = threadStandard
                ThreadingType = threadingType
                Areas = boltSectionAreasFromDto dto.Areas
                SpecifiedLengthM = dto.SpecifiedLengthM |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
            }
        | Error message, _ -> Error message
        | _, Error message -> Error message

    /// <summary>Maps a preload definition to a persistence DTO.</summary>
    let preloadDefinitionToDto
        (value: OptimizedFlange.Domain.PreloadDefinition)
        : PreloadDefinitionDto =
        {
            MinimumPreloadN = value.MinimumPreloadN |> Option.map float |> optionToNullable
            TargetPreloadN = value.TargetPreloadN |> Option.map float |> optionToNullable
            MaximumPreloadN = value.MaximumPreloadN |> Option.map float |> optionToNullable
        }

    /// <summary>Maps a preload DTO to the domain model.</summary>
    let preloadDefinitionFromDto
        (dto: PreloadDefinitionDto)
        : OptimizedFlange.Domain.PreloadDefinition =
        {
            MinimumPreloadN = dto.MinimumPreloadN |> nullableToOption |> Option.map (fun value -> value * 1.0<N>)
            TargetPreloadN = dto.TargetPreloadN |> nullableToOption |> Option.map (fun value -> value * 1.0<N>)
            MaximumPreloadN = dto.MaximumPreloadN |> nullableToOption |> Option.map (fun value -> value * 1.0<N>)
        }

    /// <summary>Maps a bolting assembly to a persistence DTO.</summary>
    let boltingAssemblyToDto
        (value: OptimizedFlange.Domain.BoltingAssembly)
        : BoltingAssemblyDto =
        {
            AssemblyId = value.AssemblyId
            Arrangement = boltArrangementToString value.Arrangement
            Pattern = boltPatternToDto value.Pattern
            Stud = studDefinitionToDto value.Stud
            ProjectAreaBasis = boltAreaBasisToString value.ProjectAreaBasis
            TighteningMethod = tighteningMethodToString value.TighteningMethod
            Preload = preloadDefinitionToDto value.Preload
        }

    /// <summary>Maps a bolting assembly DTO to the domain model.</summary>
    let boltingAssemblyFromDto
        (dto: BoltingAssemblyDto)
        : Result<OptimizedFlange.Domain.BoltingAssembly, string> =
        match
            boltArrangementFromString dto.Arrangement,
            studDefinitionFromDto dto.Stud,
            boltAreaBasisFromString dto.ProjectAreaBasis,
            tighteningMethodFromString dto.TighteningMethod
        with
        | Ok arrangement, Ok stud, Ok areaBasis, Ok tighteningMethod ->
            Ok {
                AssemblyId = dto.AssemblyId
                Arrangement = arrangement
                Pattern = boltPatternFromDto dto.Pattern
                Stud = stud
                ProjectAreaBasis = areaBasis
                TighteningMethod = tighteningMethod
                Preload = preloadDefinitionFromDto dto.Preload
            }
        | Error message, _, _, _ -> Error message
        | _, Error message, _, _ -> Error message
        | _, _, Error message, _ -> Error message
        | _, _, _, Error message -> Error message

    /// <summary>Maps a gasket envelope to a persistence DTO.</summary>
    let gasketEnvelopeToDto
        (value: OptimizedFlange.Domain.GasketEnvelope)
        : GasketEnvelopeDto =
        {
            InsideDiameterM = float value.InsideDiameterM
            OutsideDiameterM = float value.OutsideDiameterM
            ThicknessM = float value.ThicknessM
        }

    /// <summary>Maps a gasket envelope DTO to the domain model.</summary>
    let gasketEnvelopeFromDto
        (dto: GasketEnvelopeDto)
        : OptimizedFlange.Domain.GasketEnvelope =
        {
            InsideDiameterM = dto.InsideDiameterM * 1.0<m>
            OutsideDiameterM = dto.OutsideDiameterM * 1.0<m>
            ThicknessM = dto.ThicknessM * 1.0<m>
        }

    /// <summary>Maps one sealing zone to a persistence DTO.</summary>
    let sealingZoneToDto
        (value: OptimizedFlange.Domain.SealingZone)
        : SealingZoneDto =
        {
            ZoneId = value.ZoneId
            Role = sealingZoneRoleToString value.Role
            Mandatory = value.Mandatory
            NominalAreaM2 = float value.NominalAreaM2
            MinimumAverageContactPressurePa = value.MinimumAverageContactPressurePa |> Option.map float |> optionToNullable
            MaximumAverageContactPressurePa = value.MaximumAverageContactPressurePa |> Option.map float |> optionToNullable
            MaterialReferenceId =
                match value.MaterialReferenceId with
                | Some materialReferenceId -> materialReferenceId
                | None -> null
        }

    /// <summary>Maps one sealing-zone DTO to the domain model.</summary>
    let sealingZoneFromDto
        (dto: SealingZoneDto)
        : Result<OptimizedFlange.Domain.SealingZone, string> =
        sealingZoneRoleFromString dto.Role
        |> Result.map (fun role ->
            {
                ZoneId = dto.ZoneId
                Role = role
                Mandatory = dto.Mandatory
                NominalAreaM2 = dto.NominalAreaM2 * 1.0<m^2>
                MinimumAverageContactPressurePa =
                    dto.MinimumAverageContactPressurePa
                    |> nullableToOption
                    |> Option.map (fun value -> value * 1.0<Pa>)
                MaximumAverageContactPressurePa =
                    dto.MaximumAverageContactPressurePa
                    |> nullableToOption
                    |> Option.map (fun value -> value * 1.0<Pa>)
                MaterialReferenceId = Option.ofObj dto.MaterialReferenceId
            })

    /// <summary>Maps one partition rib to a persistence DTO.</summary>
    let partitionRibToDto
        (value: OptimizedFlange.Domain.PartitionRib)
        : PartitionRibDto =
        {
            RibId = value.RibId
            OffsetM = float value.OffsetM
            OrientationRad = value.OrientationRad
            WidthM = float value.WidthM
            EffectiveLengthM = value.EffectiveLengthM |> Option.map float |> optionToNullable
            SealingZoneId = value.SealingZoneId
        }

    /// <summary>Maps one partition-rib DTO to the domain model.</summary>
    let partitionRibFromDto
        (dto: PartitionRibDto)
        : OptimizedFlange.Domain.PartitionRib =
        {
            RibId = dto.RibId
            OffsetM = dto.OffsetM * 1.0<m>
            OrientationRad = dto.OrientationRad
            WidthM = dto.WidthM * 1.0<m>
            EffectiveLengthM = dto.EffectiveLengthM |> nullableToOption |> Option.map (fun value -> value * 1.0<m>)
            SealingZoneId = dto.SealingZoneId
        }

    /// <summary>Maps a partition layout to a persistence DTO.</summary>
    let partitionLayoutToDto
        (value: OptimizedFlange.Domain.PartitionLayout)
        : PartitionLayoutDto =
        {
            PassCount = value.PassCount
            Ribs = value.Ribs |> List.map partitionRibToDto |> List.toArray
        }

    /// <summary>Maps a partition-layout DTO to the domain model.</summary>
    let partitionLayoutFromDto
        (dto: PartitionLayoutDto)
        : OptimizedFlange.Domain.PartitionLayout =
        {
            PassCount = dto.PassCount
            Ribs = dto.Ribs |> Array.map partitionRibFromDto |> Array.toList
        }

    /// <summary>Maps a gasket assembly to a persistence DTO.</summary>
    let gasketAssemblyToDto
        (value: OptimizedFlange.Domain.GasketAssembly)
        : GasketAssemblyDto =
        {
            AssemblyId = value.AssemblyId
            Family = gasketFamilyToString value.Family
            Envelope = gasketEnvelopeToDto value.Envelope
            SealingZones = value.SealingZones |> List.map sealingZoneToDto |> List.toArray
            PartitionLayout =
                match value.PartitionLayout with
                | Some partitionLayout -> partitionLayoutToDto partitionLayout
                | None -> null
            HasInnerRing = value.HasInnerRing
            HasOuterRing = value.HasOuterRing
            SelectedGasketM = value.SelectedGasketM |> optionToNullable
            SelectedGasketYPa = value.SelectedGasketYPa |> Option.map float |> optionToNullable
            ProjectAreaBasis = gasketAreaBasisToString value.ProjectAreaBasis
        }

    /// <summary>Maps a gasket assembly DTO to the domain model.</summary>
    let gasketAssemblyFromDto
        (dto: GasketAssemblyDto)
        : Result<OptimizedFlange.Domain.GasketAssembly, string> =
        let zones =
            dto.SealingZones
            |> Array.toList
            |> List.fold
                (fun state item ->
                    match state, sealingZoneFromDto item with
                    | Ok items, Ok mapped -> Ok (mapped :: items)
                    | Error message, _ -> Error message
                    | _, Error message -> Error message)
                (Ok [])
            |> Result.map List.rev

        match gasketFamilyFromString dto.Family, gasketAreaBasisFromString dto.ProjectAreaBasis, zones with
        | Ok family, Ok areaBasis, Ok mappedZones ->
            Ok {
                AssemblyId = dto.AssemblyId
                Family = family
                Envelope = gasketEnvelopeFromDto dto.Envelope
                SealingZones = mappedZones
                PartitionLayout = Option.ofObj dto.PartitionLayout |> Option.map partitionLayoutFromDto
                HasInnerRing = dto.HasInnerRing
                HasOuterRing = dto.HasOuterRing
                SelectedGasketM = nullableToOption dto.SelectedGasketM
                SelectedGasketYPa =
                    nullableToOption dto.SelectedGasketYPa
                    |> Option.map LanguagePrimitives.FloatWithMeasure<Pa>
                ProjectAreaBasis = areaBasis
            }
        | Error message, _, _ -> Error message
        | _, Error message, _ -> Error message
        | _, _, Error message -> Error message

    /// <summary>Maps material identity to a persistence DTO.</summary>
    let materialIdentityToDto
        (value: OptimizedFlange.Domain.MaterialIdentity)
        : MaterialIdentityDto =
        {
            MaterialId = value.MaterialId
            Specification =
                match value.Specification with
                | Some specification -> specification
                | None -> null
            Grade =
                match value.Grade with
                | Some grade -> grade
                | None -> null
            ProductForm =
                match value.ProductForm with
                | Some productForm -> productForm
                | None -> null
        }

    /// <summary>Maps material identity DTO values to the domain model.</summary>
    let materialIdentityFromDto
        (dto: MaterialIdentityDto)
        : OptimizedFlange.Domain.MaterialIdentity =
        {
            MaterialId = dto.MaterialId
            Specification = Option.ofObj dto.Specification
            Grade = Option.ofObj dto.Grade
            ProductForm = Option.ofObj dto.ProductForm
        }

    /// <summary>Maps resolved material properties to a persistence DTO.</summary>
    let resolvedMaterialPropertiesToDto
        (value: OptimizedFlange.Domain.ResolvedMaterialProperties)
        : ResolvedMaterialPropertiesDto =
        {
            TemperatureK = float value.TemperatureK
            AllowableStressPa = value.AllowableStressPa |> Option.map float |> optionToNullable
            YieldStrengthPa = value.YieldStrengthPa |> Option.map float |> optionToNullable
            UltimateStrengthPa = value.UltimateStrengthPa |> Option.map float |> optionToNullable
            ElasticModulusPa = value.ElasticModulusPa |> Option.map float |> optionToNullable
            PoissonRatio = value.PoissonRatio |> optionToNullable
            ThermalExpansionPerK = value.ThermalExpansionPerK |> optionToNullable
            DensityKgPerM3 = value.DensityKgPerM3 |> Option.map float |> optionToNullable
        }

    /// <summary>Maps resolved material property DTO values to the domain model.</summary>
    let resolvedMaterialPropertiesFromDto
        (dto: ResolvedMaterialPropertiesDto)
        : OptimizedFlange.Domain.ResolvedMaterialProperties =
        {
            TemperatureK = dto.TemperatureK * 1.0<K>
            AllowableStressPa = dto.AllowableStressPa |> nullableToOption |> Option.map (fun value -> value * 1.0<Pa>)
            YieldStrengthPa = dto.YieldStrengthPa |> nullableToOption |> Option.map (fun value -> value * 1.0<Pa>)
            UltimateStrengthPa = dto.UltimateStrengthPa |> nullableToOption |> Option.map (fun value -> value * 1.0<Pa>)
            ElasticModulusPa = dto.ElasticModulusPa |> nullableToOption |> Option.map (fun value -> value * 1.0<Pa>)
            PoissonRatio = dto.PoissonRatio |> nullableToOption
            ThermalExpansionPerK = dto.ThermalExpansionPerK |> nullableToOption
            DensityKgPerM3 = dto.DensityKgPerM3 |> nullableToOption |> Option.map (fun value -> value * 1.0<kg/m^3>)
        }

    /// <summary>Maps a material snapshot to a persistence DTO.</summary>
    let materialSnapshotToDto
        (value: OptimizedFlange.Domain.MaterialSnapshot)
        : MaterialSnapshotDto =
        {
            Identity = materialIdentityToDto value.Identity
            Properties = value.Properties |> List.map resolvedMaterialPropertiesToDto |> List.toArray
            ProviderId = value.ProviderId
            ProviderRevision =
                match value.ProviderRevision with
                | Some revision -> revision
                | None -> null
            SourceEdition =
                match value.SourceEdition with
                | Some edition -> edition
                | None -> null
            Fingerprint =
                match value.Fingerprint with
                | Some fingerprint -> fingerprint
                | None -> null
        }

    /// <summary>Maps a material snapshot DTO to the domain model.</summary>
    let materialSnapshotFromDto
        (dto: MaterialSnapshotDto)
        : OptimizedFlange.Domain.MaterialSnapshot =
        {
            Identity = materialIdentityFromDto dto.Identity
            Properties = dto.Properties |> Array.map resolvedMaterialPropertiesFromDto |> Array.toList
            ProviderId = dto.ProviderId
            ProviderRevision = Option.ofObj dto.ProviderRevision
            SourceEdition = Option.ofObj dto.SourceEdition
            Fingerprint = Option.ofObj dto.Fingerprint
        }

    /// <summary>Maps a component material association to a persistence DTO.</summary>
    let componentMaterialToDto
        (value: OptimizedFlange.Domain.ComponentMaterial)
        : ComponentMaterialDto =
        {
            ComponentRole = value.ComponentRole
            Material = materialSnapshotToDto value.Material
        }

    /// <summary>Maps a component material DTO to the domain model.</summary>
    let componentMaterialFromDto
        (dto: ComponentMaterialDto)
        : OptimizedFlange.Domain.ComponentMaterial =
        {
            ComponentRole = dto.ComponentRole
            Material = materialSnapshotFromDto dto.Material
        }

    let private validateRequiredText value label =
        if String.IsNullOrWhiteSpace(value) then
            Error $"Missing {label}."
        else
            Ok ()

    /// <summary>Maps one joint side to a reference DTO.</summary>
    let jointSideToReferenceDto
        (value: OptimizedFlange.Domain.JointSide)
        : JointSideReferenceDto =
        {
            GeometrySideId = value.Geometry.SideId
            MaterialRole = value.MaterialRole
        }

    /// <summary>Maps one joint-side reference DTO to the domain model using already mapped side geometries.</summary>
    let jointSideFromReferenceDto
        (geometries: OptimizedFlange.Domain.JointSideGeometry list)
        (dto: JointSideReferenceDto)
        : Result<OptimizedFlange.Domain.JointSide, string> =
        if obj.ReferenceEquals(dto, null) then
            Error "Missing joint side reference."
        else
            match
                validateRequiredText dto.GeometrySideId "joint side geometry reference",
                validateRequiredText dto.MaterialRole "joint side material role"
            with
            | Ok (), Ok () ->
                match geometries |> List.tryFind (fun geometry -> geometry.SideId = dto.GeometrySideId) with
                | Some geometry ->
                    Ok {
                        Geometry = geometry
                        MaterialRole = dto.MaterialRole
                    }
                | None -> Error $"Unknown joint side geometry reference: {dto.GeometrySideId}"
            | Error message, _ -> Error message
            | _, Error message -> Error message

    /// <summary>Maps a flanged-joint composition to a reference-based persistence DTO.</summary>
    let flangedJointToDto
        (value: OptimizedFlange.Domain.FlangedJoint)
        : FlangedJointDto =
        {
            JointId = value.JointId
            PrimarySide = jointSideToReferenceDto value.PrimarySide
            MatingSideMode = matingSideModeToString value.MatingSideMode
            MatingSide =
                match value.MatingSide with
                | Some matingSide -> jointSideToReferenceDto matingSide
                | None -> null
            GasketAssemblyId = value.Gasket.AssemblyId
            BoltingAssemblyId = value.Bolting.AssemblyId
            LoadCaseIds = value.LoadCases |> List.map (fun loadCase -> loadCase.LoadCaseId) |> List.toArray
            AcceptanceCriterionIds =
                value.AcceptanceCriteria
                |> List.map (fun criterion -> criterion.CriterionId)
                |> List.toArray
            ComponentMaterialRoles =
                value.Materials
                |> List.map (fun material -> material.ComponentRole)
                |> List.toArray
        }

    let private resolveManyById requestedIds items getId label =
        requestedIds
        |> Array.toList
        |> List.fold
            (fun state requestedId ->
                match state, items |> List.tryFind (fun item -> getId item = requestedId) with
                | Ok resolved, Some item -> Ok (item :: resolved)
                | Ok _, None -> Error $"Unknown {label} reference: {requestedId}"
                | Error message, _ -> Error message)
            (Ok [])
        |> Result.map List.rev

    let private validateUniqueById items getId label =
        items
        |> Array.map getId
        |> Array.countBy id
        |> Array.tryFind (fun (_, count) -> count > 1)
        |> function
            | Some (duplicateId, _) -> Error $"Duplicate {label} identifier: {duplicateId}"
            | None -> Ok ()

    let private validateRequiredIdentifiers items getId label =
        items
        |> Array.map getId
        |> Array.tryFind String.IsNullOrWhiteSpace
        |> function
            | Some _ -> Error $"Missing {label} identifier."
            | None -> Ok ()

    let private validateRequiredArray items label =
        if obj.ReferenceEquals(items, null) then
            Error $"Technical data collection is missing: {label}"
        else
            Ok ()

    let private validateUniqueReferences requestedIds label =
        requestedIds
        |> Array.countBy id
        |> Array.tryFind (fun (_, count) -> count > 1)
        |> function
            | Some (duplicateId, _) -> Error $"Duplicate {label} reference: {duplicateId}"
            | None -> Ok ()

    let private validateRequiredReferences requestedIds label =
        if obj.ReferenceEquals(requestedIds, null) then
            Error $"Missing {label} reference collection."
        else
            requestedIds
            |> Array.tryFind String.IsNullOrWhiteSpace
            |> function
                | Some _ -> Error $"Missing {label} reference."
                | None -> Ok ()

    /// <summary>Maps a reference-based flanged-joint DTO to the domain model using already mapped technical fragments.</summary>
    let flangedJointFromDto
        (geometries: OptimizedFlange.Domain.JointSideGeometry list)
        (gaskets: OptimizedFlange.Domain.GasketAssembly list)
        (boltingAssemblies: OptimizedFlange.Domain.BoltingAssembly list)
        (loadCases: OptimizedFlange.Domain.JointLoadCase list)
        (acceptanceCriteria: OptimizedFlange.Domain.AcceptanceCriterion list)
        (componentMaterials: OptimizedFlange.Domain.ComponentMaterial list)
        (dto: FlangedJointDto)
        : Result<OptimizedFlange.Domain.FlangedJoint, string> =
        let primarySide = jointSideFromReferenceDto geometries dto.PrimarySide
        let matingSide =
            match Option.ofObj dto.MatingSide with
            | Some matingSideDto -> jointSideFromReferenceDto geometries matingSideDto |> Result.map Some
            | None -> Ok None

        let gasket =
            match validateRequiredText dto.GasketAssemblyId "gasket assembly reference" with
            | Ok () ->
                match gaskets |> List.tryFind (fun gasket -> gasket.AssemblyId = dto.GasketAssemblyId) with
                | Some mappedGasket -> Ok mappedGasket
                | None -> Error $"Unknown gasket assembly reference: {dto.GasketAssemblyId}"
            | Error message -> Error message

        let bolting =
            match validateRequiredText dto.BoltingAssemblyId "bolting assembly reference" with
            | Ok () ->
                match boltingAssemblies |> List.tryFind (fun bolting -> bolting.AssemblyId = dto.BoltingAssemblyId) with
                | Some mappedBolting -> Ok mappedBolting
                | None -> Error $"Unknown bolting assembly reference: {dto.BoltingAssemblyId}"
            | Error message -> Error message

        let uniqueLoadCaseReferences =
            validateUniqueReferences dto.LoadCaseIds "load case"

        let uniqueAcceptanceCriterionReferences =
            validateUniqueReferences dto.AcceptanceCriterionIds "acceptance criterion"

        let uniqueComponentMaterialReferences =
            validateUniqueReferences dto.ComponentMaterialRoles "component material"

        let requiredLoadCaseReferences =
            validateRequiredReferences dto.LoadCaseIds "load case"

        let requiredAcceptanceCriterionReferences =
            validateRequiredReferences dto.AcceptanceCriterionIds "acceptance criterion"

        let requiredComponentMaterialReferences =
            validateRequiredReferences dto.ComponentMaterialRoles "component material"

        let resolvedLoadCases =
            resolveManyById dto.LoadCaseIds loadCases (fun loadCase -> loadCase.LoadCaseId) "load case"
        let resolvedAcceptanceCriteria =
            resolveManyById dto.AcceptanceCriterionIds acceptanceCriteria (fun criterion -> criterion.CriterionId) "acceptance criterion"
        let resolvedMaterials =
            resolveManyById dto.ComponentMaterialRoles componentMaterials (fun material -> material.ComponentRole) "component material"

        match
            matingSideModeFromString dto.MatingSideMode,
            primarySide,
            matingSide,
            gasket,
            bolting,
            requiredLoadCaseReferences,
            requiredAcceptanceCriterionReferences,
            requiredComponentMaterialReferences,
            uniqueLoadCaseReferences,
            uniqueAcceptanceCriterionReferences,
            uniqueComponentMaterialReferences,
            resolvedLoadCases,
            resolvedAcceptanceCriteria,
            resolvedMaterials
        with
        | Ok matingSideMode, Ok mappedPrimarySide, Ok mappedMatingSide, Ok mappedGasket, Ok mappedBolting, Ok (), Ok (), Ok (), Ok (), Ok (), Ok (), Ok mappedLoadCases, Ok mappedCriteria, Ok mappedMaterials ->
            Ok {
                JointId = dto.JointId
                PrimarySide = mappedPrimarySide
                MatingSideMode = matingSideMode
                MatingSide = mappedMatingSide
                Gasket = mappedGasket
                Bolting = mappedBolting
                LoadCases = mappedLoadCases
                AcceptanceCriteria = mappedCriteria
                Materials = mappedMaterials
            }
        | Error message, _, _, _, _, _, _, _, _, _, _, _, _, _ -> Error message
        | _, Error message, _, _, _, _, _, _, _, _, _, _, _, _ -> Error message
        | _, _, Error message, _, _, _, _, _, _, _, _, _, _, _ -> Error message
        | _, _, _, Error message, _, _, _, _, _, _, _, _, _, _ -> Error message
        | _, _, _, _, Error message, _, _, _, _, _, _, _, _, _ -> Error message
        | _, _, _, _, _, Error message, _, _, _, _, _, _, _, _ -> Error message
        | _, _, _, _, _, _, Error message, _, _, _, _, _, _, _ -> Error message
        | _, _, _, _, _, _, _, Error message, _, _, _, _, _, _ -> Error message
        | _, _, _, _, _, _, _, _, Error message, _, _, _, _, _ -> Error message
        | _, _, _, _, _, _, _, _, _, Error message, _, _, _, _ -> Error message
        | _, _, _, _, _, _, _, _, _, _, Error message, _, _, _ -> Error message
        | _, _, _, _, _, _, _, _, _, _, _, Error message, _, _ -> Error message
        | _, _, _, _, _, _, _, _, _, _, _, _, Error message, _ -> Error message
        | _, _, _, _, _, _, _, _, _, _, _, _, _, Error message -> Error message

    /// <summary>Maps technical project data to its versioned persistence DTO.</summary>
    let projectTechnicalDataToDto
        schemaVersion
        (acceptanceCriteria: OptimizedFlange.Domain.AcceptanceCriterion list)
        (loadCases: OptimizedFlange.Domain.JointLoadCase list)
        (jointSideGeometries: OptimizedFlange.Domain.JointSideGeometry list)
        (boltingAssemblies: OptimizedFlange.Domain.BoltingAssembly list)
        (gasketAssemblies: OptimizedFlange.Domain.GasketAssembly list)
        (componentMaterials: OptimizedFlange.Domain.ComponentMaterial list)
        (flangedJoints: OptimizedFlange.Domain.FlangedJoint list)
        : ProjectTechnicalDataDto =
        {
            SchemaVersion = schemaVersion
            AcceptanceCriteria =
                acceptanceCriteria
                |> List.map acceptanceCriterionToDto
                |> List.toArray
            LoadCases =
                loadCases
                |> List.map jointLoadCaseToDto
                |> List.toArray
            JointSideGeometries =
                jointSideGeometries
                |> List.map jointSideGeometryToDto
                |> List.toArray
            BoltingAssemblies =
                boltingAssemblies
                |> List.map boltingAssemblyToDto
                |> List.toArray
            GasketAssemblies =
                gasketAssemblies
                |> List.map gasketAssemblyToDto
                |> List.toArray
            ComponentMaterials =
                componentMaterials
                |> List.map componentMaterialToDto
                |> List.toArray
            FlangedJoints =
                flangedJoints
                |> List.map flangedJointToDto
                |> List.toArray
        }

    /// <summary>Maps a versioned technical-data DTO to validated project technical data fragments.</summary>
    let projectTechnicalDataFromDto
        (dto: ProjectTechnicalDataDto)
        : Result<OptimizedFlange.Domain.AcceptanceCriterion list * OptimizedFlange.Domain.JointLoadCase list * OptimizedFlange.Domain.JointSideGeometry list * OptimizedFlange.Domain.BoltingAssembly list * OptimizedFlange.Domain.GasketAssembly list * OptimizedFlange.Domain.ComponentMaterial list * OptimizedFlange.Domain.FlangedJoint list, string> =
        let acceptanceCriteriaDto =
            if obj.ReferenceEquals(dto.AcceptanceCriteria, null) then [||] else dto.AcceptanceCriteria

        let loadCasesDto =
            if obj.ReferenceEquals(dto.LoadCases, null) then [||] else dto.LoadCases

        let jointSideGeometriesDto =
            if obj.ReferenceEquals(dto.JointSideGeometries, null) then [||] else dto.JointSideGeometries

        let boltingAssembliesDto =
            if obj.ReferenceEquals(dto.BoltingAssemblies, null) then [||] else dto.BoltingAssemblies

        let gasketAssembliesDto =
            if obj.ReferenceEquals(dto.GasketAssemblies, null) then [||] else dto.GasketAssemblies

        let componentMaterialsDto =
            if obj.ReferenceEquals(dto.ComponentMaterials, null) then [||] else dto.ComponentMaterials

        let flangedJointsDto =
            if obj.ReferenceEquals(dto.FlangedJoints, null) then [||] else dto.FlangedJoints

        let requiredCollections =
            [
                validateRequiredArray dto.AcceptanceCriteria "AcceptanceCriteria"
                validateRequiredArray dto.LoadCases "LoadCases"
                validateRequiredArray dto.JointSideGeometries "JointSideGeometries"
                validateRequiredArray dto.BoltingAssemblies "BoltingAssemblies"
                validateRequiredArray dto.GasketAssemblies "GasketAssemblies"
                validateRequiredArray dto.ComponentMaterials "ComponentMaterials"
                validateRequiredArray dto.FlangedJoints "FlangedJoints"
            ]
            |> List.tryPick (function
                | Ok () -> None
                | Error message -> Some message)

        let uniqueIdentifiers =
            [
                validateUniqueById acceptanceCriteriaDto (fun item -> item.CriterionId) "acceptance criterion"
                validateUniqueById loadCasesDto (fun item -> item.LoadCaseId) "load case"
                validateUniqueById jointSideGeometriesDto (fun item -> item.SideId) "joint side geometry"
                validateUniqueById boltingAssembliesDto (fun item -> item.AssemblyId) "bolting assembly"
                validateUniqueById gasketAssembliesDto (fun item -> item.AssemblyId) "gasket assembly"
                validateUniqueById componentMaterialsDto (fun item -> item.ComponentRole) "component material"
                validateUniqueById flangedJointsDto (fun item -> item.JointId) "flanged joint"
            ]
            |> List.tryPick (function
                | Ok () -> None
                | Error message -> Some message)

        let requiredIdentifiers =
            [
                validateRequiredIdentifiers acceptanceCriteriaDto (fun item -> item.CriterionId) "acceptance criterion"
                validateRequiredIdentifiers loadCasesDto (fun item -> item.LoadCaseId) "load case"
                validateRequiredIdentifiers jointSideGeometriesDto (fun item -> item.SideId) "joint side geometry"
                validateRequiredIdentifiers boltingAssembliesDto (fun item -> item.AssemblyId) "bolting assembly"
                validateRequiredIdentifiers gasketAssembliesDto (fun item -> item.AssemblyId) "gasket assembly"
                validateRequiredIdentifiers componentMaterialsDto (fun item -> item.ComponentRole) "component material"
                validateRequiredIdentifiers flangedJointsDto (fun item -> item.JointId) "flanged joint"
            ]
            |> List.tryPick (function
                | Ok () -> None
                | Error message -> Some message)

        let criteria =
            acceptanceCriteriaDto
            |> Array.toList
            |> List.fold
                (fun state item ->
                    match state, acceptanceCriterionFromDto item with
                    | Ok items, Ok mapped -> Ok (mapped :: items)
                    | Error message, _ -> Error message
                    | _, Error message -> Error message)
                (Ok [])
            |> Result.map List.rev

        let loadCases =
            loadCasesDto
            |> Array.toList
            |> List.fold
                (fun state item ->
                    match state, jointLoadCaseFromDto item with
                    | Ok items, Ok mapped -> Ok (mapped :: items)
                    | Error message, _ -> Error message
                    | _, Error message -> Error message)
                (Ok [])
            |> Result.map List.rev

        let jointSideGeometries =
            jointSideGeometriesDto
            |> Array.toList
            |> List.fold
                (fun state item ->
                    match state, jointSideGeometryFromDto item with
                    | Ok items, Ok mapped -> Ok (mapped :: items)
                    | Error message, _ -> Error message
                    | _, Error message -> Error message)
                (Ok [])
            |> Result.map List.rev

        let boltingAssemblies =
            boltingAssembliesDto
            |> Array.toList
            |> List.fold
                (fun state item ->
                    match state, boltingAssemblyFromDto item with
                    | Ok items, Ok mapped -> Ok (mapped :: items)
                    | Error message, _ -> Error message
                    | _, Error message -> Error message)
                (Ok [])
            |> Result.map List.rev

        let gasketAssemblies =
            gasketAssembliesDto
            |> Array.toList
            |> List.fold
                (fun state item ->
                    match state, gasketAssemblyFromDto item with
                    | Ok items, Ok mapped -> Ok (mapped :: items)
                    | Error message, _ -> Error message
                    | _, Error message -> Error message)
                (Ok [])
            |> Result.map List.rev

        let componentMaterials =
            componentMaterialsDto
            |> Array.map componentMaterialFromDto
            |> Array.toList

        match requiredCollections, requiredIdentifiers, uniqueIdentifiers, criteria, loadCases, jointSideGeometries, boltingAssemblies, gasketAssemblies with
        | Some message, _, _, _, _, _, _, _ -> Error message
        | None, Some message, _, _, _, _, _, _ -> Error message
        | None, None, Some message, _, _, _, _, _ -> Error message
        | None, None, None, Ok mappedCriteria, Ok mappedLoadCases, Ok mappedGeometries, Ok mappedBolting, Ok mappedGaskets ->
            let flangedJoints =
                flangedJointsDto
                |> Array.toList
                |> List.fold
                    (fun state item ->
                        match
                            state,
                            flangedJointFromDto
                                mappedGeometries
                                mappedGaskets
                                mappedBolting
                                mappedLoadCases
                                mappedCriteria
                                componentMaterials
                                item
                        with
                        | Ok items, Ok mapped -> Ok (mapped :: items)
                        | Error message, _ -> Error message
                        | _, Error message -> Error message)
                    (Ok [])
                |> Result.map List.rev

            flangedJoints
            |> Result.map (fun mappedJoints ->
                (mappedCriteria, mappedLoadCases, mappedGeometries, mappedBolting, mappedGaskets, componentMaterials, mappedJoints))
        | None, None, None, Error message, _, _, _, _ -> Error message
        | None, None, None, _, Error message, _, _, _ -> Error message
        | None, None, None, _, _, Error message, _, _ -> Error message
        | None, None, None, _, _, _, Error message, _ -> Error message
        | None, None, None, _, _, _, _, Error message -> Error message
