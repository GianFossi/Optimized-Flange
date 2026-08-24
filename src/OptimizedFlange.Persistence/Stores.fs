namespace OptimizedFlange.Persistence

open OptimizedFlange.Configuration

/// <summary>Provides typed persistence for application settings using explicit DTO mapping.</summary>
module ApplicationSettingsStore =
    /// <summary>Saves application settings as versioned JSON.</summary>
    let save path (settings: ApplicationSettings) =
        settings |> PersistenceMappers.applicationToDto |> JsonStore.save path

    /// <summary>Loads application settings from versioned JSON.</summary>
    let load path =
        JsonStore.load<ApplicationSettingsDto> path
        |> Result.bind PersistenceMappers.applicationFromDto

/// <summary>Provides typed persistence for calculation defaults using explicit DTO mapping.</summary>
module CalculationDefaultsStore =
    /// <summary>Saves calculation defaults as versioned JSON.</summary>
    let save path (settings: CalculationDefaults) =
        settings |> PersistenceMappers.calculationDefaultsToDto |> JsonStore.save path

    /// <summary>Loads calculation defaults from versioned JSON.</summary>
    let load path =
        JsonStore.load<CalculationDefaultsDto> path
        |> Result.bind PersistenceMappers.calculationDefaultsFromDto

/// <summary>Provides typed persistence for database paths using explicit DTO mapping.</summary>
module DatabasePathSettingsStore =
    /// <summary>Saves database paths as versioned JSON.</summary>
    let save path (settings: DatabasePathSettings) =
        settings |> PersistenceMappers.databasePathsToDto |> JsonStore.save path

    /// <summary>Loads database paths from versioned JSON.</summary>
    let load path =
        JsonStore.load<DatabasePathSettingsDto> path
        |> Result.map PersistenceMappers.databasePathsFromDto

/// <summary>Provides typed persistence for the recent-file list using explicit DTO mapping.</summary>
module RecentFilesStore =
    /// <summary>Saves recent-file entries as JSON.</summary>
    let save path (items: RecentFileEntry list) =
        items
        |> List.map PersistenceMappers.recentFileToDto
        |> List.toArray
        |> JsonStore.save path

    /// <summary>Loads recent-file entries from JSON.</summary>
    let load path =
        JsonStore.load<RecentFileEntryDto array> path
        |> Result.map (Array.map PersistenceMappers.recentFileFromDto >> Array.toList)
