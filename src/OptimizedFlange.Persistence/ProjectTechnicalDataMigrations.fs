namespace OptimizedFlange.Persistence

/// <summary>Provides explicit technical-data schema migrations up to the current payload version.</summary>
module ProjectTechnicalDataMigrations =
    /// <summary>Current technical-data schema version stored inside a project file envelope.</summary>
    [<Literal>]
    let CurrentSchemaVersion = 1

    /// <summary>Migrates a technical-data DTO to the current schema version, rejecting unsupported versions.</summary>
    let migrateToCurrent (dto: ProjectTechnicalDataDto) =
        match dto.SchemaVersion with
        | CurrentSchemaVersion -> Ok dto
        | version when version > CurrentSchemaVersion ->
            Error $"Unsupported future technical data schema version: {version}"
        | version ->
            Error $"Unsupported legacy technical data schema version: {version}"
