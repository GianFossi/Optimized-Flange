namespace OptimizedFlange.Persistence

open System.IO
open System.Text.Json

/// <summary>Provides generic JSON read/write operations for persistence DTOs.</summary>
module JsonStore =
    /// <summary>Serializes and atomically saves a persistence DTO.</summary>
    let save<'T> (path: string) (value: 'T) : Result<unit, string> =
        try
            let options = JsonOptions.create ()
            let json = JsonSerializer.Serialize(value, options)
            let validate text =
                try
                    use _document = JsonDocument.Parse(text)
                    Ok ()
                with ex ->
                    Error ex.Message
            AtomicFile.writeText validate path json
        with ex ->
            Error ex.Message

    /// <summary>Loads and deserializes a persistence DTO from JSON.</summary>
    let load<'T> (path: string) : Result<'T, string> =
        try
            if not (File.Exists(path)) then
                Error $"File not found: {path}"
            else
                let options = JsonOptions.create ()
                let json = File.ReadAllText(path)
                let value = JsonSerializer.Deserialize<'T>(json, options)
                if obj.ReferenceEquals(box value, null) then
                    Error $"Unable to deserialize: {path}"
                else
                    Ok value
        with ex ->
            Error ex.Message
