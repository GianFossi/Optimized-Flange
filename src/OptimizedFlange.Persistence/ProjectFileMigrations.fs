namespace OptimizedFlange.Persistence

/// <summary>Provides explicit project-file schema migrations up to the current envelope version.</summary>
module ProjectFileMigrations =
    /// <summary>Current project-file envelope schema version.</summary>
    [<Literal>]
    let CurrentSchemaVersion = 1

    /// <summary>Migrates a project-file DTO to the current schema version, rejecting unsupported versions.</summary>
    let migrateToCurrent (dto: ProjectFileDto) =
        match dto.SchemaVersion with
        | CurrentSchemaVersion -> Ok dto
        | version when version > CurrentSchemaVersion ->
            Error $"Unsupported future project file schema version: {version}"
        | version ->
            Error $"Unsupported legacy project file schema version: {version}"
