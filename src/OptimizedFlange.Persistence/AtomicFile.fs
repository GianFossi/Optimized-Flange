namespace OptimizedFlange.Persistence

open System
open System.IO

/// <summary>Implements atomic-style replacement for important persisted files.</summary>
module AtomicFile =
    /// <summary>Writes text to a temporary file, flushes it, validates it, and replaces the destination only when validation succeeds.</summary>
    let writeText
        (validate: string -> Result<unit, string>)
        (path: string)
        (content: string)
        : Result<unit, string> =
        try
            let directory = Path.GetDirectoryName(path)
            match directory with
            | null -> ()
            | value when String.IsNullOrWhiteSpace(value) -> ()
            | value -> Directory.CreateDirectory(value) |> ignore

            let tempPath = path + ".tmp"
            do
                use stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None)
                use writer = new StreamWriter(stream)
                writer.Write(content)
                writer.Flush()
                stream.Flush(true)

            match validate content with
            | Error message ->
                File.Delete(tempPath)
                Error message
            | Ok () ->
                if File.Exists(path) then
                    let backupPath = path + ".bak"
                    File.Replace(tempPath, path, backupPath, true)
                else
                    File.Move(tempPath, path)
                Ok ()
        with ex ->
            Error ex.Message
