namespace OptimizedFlange.Persistence

open System.Text.Json

/// <summary>Provides the shared System.Text.Json configuration for persisted application data.</summary>
module JsonOptions =
    /// <summary>Creates serializer options for human-readable, versioned persistence DTOs.</summary>
    let create () =
        JsonSerializerOptions(
            WriteIndented = true,
            PropertyNameCaseInsensitive = false
        )
