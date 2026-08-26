namespace OptimizedFlange.DataSources

open System

/// <summary>Searches imported technical data records using simple deterministic filters.</summary>
module Search =
    let private containsIgnoreCase (needle: string) (haystack: string) =
        haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0

    let private equalsIgnoreCase (expected: string) (actual: string) =
        String.Equals(expected, actual, StringComparison.OrdinalIgnoreCase)

    let private optionalEquals expected actual =
        match expected, actual with
        | None, _ -> true
        | Some value, Some actualValue -> equalsIgnoreCase value actualValue
        | Some _, None -> false

    let private textMatches text (record: ImportedDataRecord) =
        match text with
        | None -> true
        | Some value when String.IsNullOrWhiteSpace(value) -> true
        | Some value ->
            [
                record.RecordId
                record.DisplayName
                yield! record.Tags
                yield! record.Family |> Option.toList
                yield! record.Standard |> Option.toList
                yield! record.Grade |> Option.toList
            ]
            |> List.exists (containsIgnoreCase value)

    let private scalarMatches scalarName (record: ImportedDataRecord) =
        match scalarName with
        | None -> true
        | Some value when String.IsNullOrWhiteSpace(value) -> true
        | Some value -> record.Scalars |> List.exists (fun scalar -> equalsIgnoreCase value scalar.Name)

    /// <summary>Returns records matching all supplied filters.</summary>
    let filter (criteria: DataRecordFilter) (records: ImportedDataRecord list) : ImportedDataRecord list =
        records
        |> List.filter (fun record ->
            (criteria.Category |> Option.map ((=) record.Category) |> Option.defaultValue true)
            && optionalEquals criteria.SourceId (Some record.SourceId)
            && optionalEquals criteria.Family record.Family
            && optionalEquals criteria.Standard record.Standard
            && optionalEquals criteria.Grade record.Grade
            && textMatches criteria.Text record
            && scalarMatches criteria.HasScalar record)

    /// <summary>Empty filter that returns every imported record.</summary>
    let all : DataRecordFilter =
        {
            Category = None
            SourceId = None
            Text = None
            Family = None
            Standard = None
            Grade = None
            HasScalar = None
        }
