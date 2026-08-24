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
            let validate (text: string) =
                try
                    use _document = JsonDocument.Parse(text)
                    Ok ()
                with ex ->
                    Error ex.Message
            AtomicFile.writeText validate path json
        with ex ->
            Error ex.Message

    /// <summary>Loads and deserializes a persistence DTO from JSON.</summary>
    let load<'T when 'T: not null> (path: string) : Result<'T, string> =
        try
            if not (File.Exists(path)) then
                Error $"File not found: {path}"
            else
                let options = JsonOptions.create ()
                let json = File.ReadAllText(path)
                let valueOrNull = JsonSerializer.Deserialize(json, typeof<'T>, options)
                if isNull valueOrNull then
                    Error $"Unable to deserialize: {path}"
                else
                    Ok (valueOrNull :?> 'T)
        with ex ->
            Error ex.Message
