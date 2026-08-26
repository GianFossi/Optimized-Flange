namespace OptimizedFlange.DataSources

open OptimizedFlange.Configuration
open OptimizedFlange.Domain

/// <summary>Associates a calculation procedure with imported data records that can resolve its future inputs.</summary>
type ProcedureDataResolution =
    {
        /// <summary>Procedure identifier requested by the caller.</summary>
        ProcedureId: string
        /// <summary>Candidate records matching the procedure's data categories.</summary>
        CandidateRecords: ImportedDataRecord list
        /// <summary>Messages describing data categories still missing from the configured sources.</summary>
        MissingCategories: TechnicalDataCategory list
    }

/// <summary>Resolves searchable data candidates for known calculation procedures.</summary>
module ProcedureDataResolver =
    let private requiredCategories (procedure: CalculationProcedureDefinition) : TechnicalDataCategory list =
        match procedure.Kind with
        | DesignCodeAssessment ->
            [ Materials; Bolting; Gaskets; GasketParameters; Facings; StandardFlanges; Ratings ]
        | Pcc1Assessment ->
            [ Materials; Bolting; Gaskets; GasketParameters; Facings ]
        | Api660Assessment
        | IogpS614Assessment ->
            [ Materials; Bolting; Gaskets; GasketParameters; Facings; StandardFlanges; Ratings; Pipes ]
        | GeometryResolutionProcedure ->
            [ Bolting; Gaskets; Facings; StandardFlanges; Pipes; Tubes ]
        | StructuralValidation
        | TemaAssessment
        | ProjectPolicyAssessment ->
            [ Materials; Bolting; Gaskets; GasketParameters; Facings; StandardFlanges; Pipes ]

    let private hasCategory (category: TechnicalDataCategory) (records: ImportedDataRecord list) =
        records |> List.exists (fun record -> record.Category = category)

    /// <summary>Loads the configured catalog and returns procedure-relevant data candidates.</summary>
    let resolve (settings: DatabasePathSettings) (procedure: CalculationProcedureDefinition) : Result<ProcedureDataResolution, string> =
        LocalDatabaseLoaders.loadCatalog settings
        |> Result.map (fun catalog ->
            let categories = requiredCategories procedure
            let candidates =
                catalog.Records
                |> List.filter (fun record -> categories |> List.contains record.Category)
            {
                ProcedureId = procedure.ProcedureId
                CandidateRecords = candidates
                MissingCategories =
                    categories
                    |> List.filter (fun category -> not (hasCategory category candidates))
            })

    /// <summary>Loads procedure-relevant candidates and applies an additional user search filter.</summary>
    let search (settings: DatabasePathSettings) (procedure: CalculationProcedureDefinition) (criteria: DataRecordFilter) : Result<ImportedDataRecord list, string> =
        resolve settings procedure
        |> Result.map (fun resolution ->
            Search.filter criteria resolution.CandidateRecords)
