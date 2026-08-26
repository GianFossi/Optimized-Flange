namespace OptimizedFlange.DataSources

open System
open System.Globalization
open System.IO
open System.Text.Json
open System.Xml.Linq
open Microsoft.Data.Sqlite
open OptimizedFlange.Configuration

/// <summary>Loads configured local technical database files into searchable imported records.</summary>
module LocalDatabaseLoaders =
    let private invariant = CultureInfo.InvariantCulture

    let private tryParseFloat (value: string) =
        match Double.TryParse(value, NumberStyles.Float, invariant) with
        | true, parsed -> Some parsed
        | false, _ -> None

    let private attr name (element: XElement) =
        match element.Attribute(XName.Get name) with
        | null -> None
        | value -> Some value.Value

    let private childValue name (element: XElement) =
        match element.Element(XName.Get name) with
        | null -> None
        | value -> Some value.Value

    let private childUnit name (element: XElement) =
        match element.Element(XName.Get name) with
        | null -> None
        | child -> attr "unit" child

    let private scalar name unitName value : ImportedScalar =
        let converted = UnitConversion.toSi unitName value
        {
            Name = name
            SourceValue = value
            SourceUnit = unitName
            SiValue = converted |> Option.map fst
            SiUnit = converted |> Option.map snd
        }

    let private scalarFromAttribute name (element: XElement) =
        attr name element
        |> Option.bind tryParseFloat
        |> Option.map (scalar name (attr "unit" element))

    let private scalarFromChild name (element: XElement) =
        childValue name element
        |> Option.bind tryParseFloat
        |> Option.map (scalar name (childUnit name element))

    let private provenance sourceId sourceName path : OptimizedFlange.Domain.ProvenanceEntry list =
        [
            {
                SourceId = sourceId
                Description = $"{sourceName}: {path}"
                Revision = None
            }
        ]

    let private categoryFromLocation (location: DatabaseLocation) =
        match location.Id with
        | id when id.Contains("MATERIALS", StringComparison.OrdinalIgnoreCase) -> Materials
        | id when id.Contains("BOLTING", StringComparison.OrdinalIgnoreCase) -> Bolting
        | id when id.Contains("GASKETS.DESIGN", StringComparison.OrdinalIgnoreCase) -> GasketParameters
        | id when id.Contains("GASKETS.MOMENT", StringComparison.OrdinalIgnoreCase) -> GasketParameters
        | id when id.Contains("GASKETS", StringComparison.OrdinalIgnoreCase) -> Gaskets
        | id when id.Contains("FACINGS", StringComparison.OrdinalIgnoreCase) -> Facings
        | id when id.Contains("FLANGES", StringComparison.OrdinalIgnoreCase) -> StandardFlanges
        | id when id.Contains("RATINGS", StringComparison.OrdinalIgnoreCase) -> Ratings
        | id when id.Contains("PIPES", StringComparison.OrdinalIgnoreCase) -> Pipes
        | id when id.Contains("TUBES", StringComparison.OrdinalIgnoreCase) -> Tubes
        | _ -> Custom

    let private makeRecord (location: DatabaseLocation) category recordId displayName family standard grade scalars tags : ImportedDataRecord =
        {
            SourceId = location.Id
            SourceName = location.Name
            SourcePath = location.Path
            Category = category
            RecordId = recordId
            DisplayName = displayName
            Family = family
            Standard = standard
            Grade = grade
            Scalars = scalars
            Tags = tags
            Provenance = provenance location.Id location.Name location.Path
        }

    let private loadBolting (location: DatabaseLocation) =
        let document = XDocument.Load(location.Path)
        document.Descendants(XName.Get "TieRod")
        |> Seq.choose (fun tieRod ->
            attr "designation" tieRod
            |> Option.map (fun designation ->
                let scalars =
                    [ "NominalDiameter"; "Pitch"; "HoleDiameter"; "ThreadRootDiameter_d3"; "TensileStressArea" ]
                    |> List.choose (fun name -> scalarFromChild name tieRod)
                makeRecord
                    location
                    Bolting
                    designation
                    designation
                    (Some "TieRod")
                    (Some "TEMA")
                    None
                    scalars
                    [ "thread"; "bolt"; "stud" ]))
        |> Seq.toList

    let private loadGaskets (location: DatabaseLocation) =
        let document = XDocument.Load(location.Path)
        let root =
            match document.Root with
            | null -> failwith $"Database source '{location.Id}' has no XML root."
            | value -> value
        let unitName = attr "unit" root

        let ringRecords =
            document.Descendants(XName.Get "Ring")
            |> Seq.choose (fun ring ->
                attr "id" ring
                |> Option.map (fun id ->
                    let scalars =
                        [ "P"; "A"; "B"; "H"; "C"; "R1" ]
                        |> List.choose (fun name ->
                            attr name ring
                            |> Option.bind tryParseFloat
                            |> Option.map (scalar name unitName))
                    makeRecord
                        location
                        Gaskets
                        $"Ring-{id}"
                        $"Ring gasket {id}"
                        (Some "RingJoint")
                        (root |> attr "source")
                        None
                        scalars
                        [ "ring"; "rj"; "gasket" ]))

        let gasketRecords =
            document.Descendants()
            |> Seq.filter (fun element -> element.Name.LocalName.EndsWith("Gasket", StringComparison.OrdinalIgnoreCase))
            |> Seq.choose (fun gasket ->
                attr "id" gasket
                |> Option.orElseWith (fun () -> attr "designation" gasket)
                |> Option.map (fun id ->
                    let scalars =
                        gasket.Attributes()
                        |> Seq.choose (fun attribute ->
                            attribute.Value
                            |> tryParseFloat
                            |> Option.map (scalar attribute.Name.LocalName unitName))
                        |> Seq.toList
                    makeRecord
                        location
                        Gaskets
                        id
                        id
                        (Some gasket.Name.LocalName)
                        (root |> attr "source")
                        None
                        scalars
                        [ "gasket"; gasket.Name.LocalName ]))

        Seq.append ringRecords gasketRecords |> Seq.distinctBy (fun record -> record.RecordId) |> Seq.toList

    let private loadGasketParameters (location: DatabaseLocation) =
        let document = XDocument.Load(location.Path)
        let root =
            match document.Root with
            | null -> failwith $"Database source '{location.Id}' has no XML root."
            | value -> value
        let unitY = attr "unit_y" root

        root.Descendants(XName.Get "Gasket")
        |> Seq.mapi (fun index gasket ->
            let manufacturer =
                gasket.Parent
                |> Option.ofObj
                |> Option.bind (attr "id")
            let description = attr "description" gasket |> Option.defaultValue $"Gasket parameter {index + 1}"
            let recordId =
                manufacturer
                |> Option.map (fun maker -> $"{maker}:{index + 1}")
                |> Option.defaultValue $"GasketParameter-{index + 1}"
            let scalars =
                [
                    yield! attr "m" gasket |> Option.bind tryParseFloat |> Option.map (scalar "m" None) |> Option.toList
                    yield! attr "y" gasket |> Option.bind tryParseFloat |> Option.map (scalar "y" unitY) |> Option.toList
                ]
            makeRecord
                location
                GasketParameters
                recordId
                description
                manufacturer
                (attr "source" root)
                None
                scalars
                [ description ])
        |> Seq.toList

    let private loadGenericXml (location: DatabaseLocation) category =
        let document = XDocument.Load(location.Path)
        let root =
            match document.Root with
            | null -> failwith $"Database source '{location.Id}' has no XML root."
            | value -> value
        root.Elements()
        |> Seq.mapi (fun index element ->
            let recordId =
                attr "id" element
                |> Option.orElseWith (fun () -> attr "designation" element)
                |> Option.orElseWith (fun () -> attr "name" element)
                |> Option.defaultValue $"{element.Name.LocalName}-{index + 1}"
            let scalars =
                element.Attributes()
                |> Seq.choose (fun attribute ->
                    attribute.Value
                    |> tryParseFloat
                    |> Option.map (scalar attribute.Name.LocalName (attr "unit" root)))
                |> Seq.toList
            makeRecord
                location
                category
                recordId
                recordId
                (Some element.Name.LocalName)
                (attr "standard" root)
                None
                scalars
                [ element.Name.LocalName ])
        |> Seq.toList

    let private loadFlanges (location: DatabaseLocation) =
        let document = XDocument.Load(location.Path)
        let toScalar name (flange: XElement) =
            childValue name flange
            |> Option.bind tryParseFloat
            |> Option.map (scalar name (Some "mm"))
        let toDimensionlessScalar name (flange: XElement) =
            childValue name flange
            |> Option.bind tryParseFloat
            |> Option.map (scalar name None)

        document.Descendants(XName.Get "Flange")
        |> Seq.mapi (fun index flange ->
            let standard = attr "Standard" flange
            let flangeType = attr "Type" flange
            let nps = childValue "NPS" flange
            let rating = childValue "Rating" flange
            let recordId =
                match standard, flangeType, nps, rating with
                | Some std, Some typ, Some size, Some cls -> $"{std}:{typ}:NPS{size}:CL{cls}"
                | _ -> $"Flange-{index + 1}"
            let facingDiameters =
                flange.Elements(XName.Get "FacingDiamRF")
                |> Seq.choose (fun element -> element.Value |> tryParseFloat)
                |> Seq.toList
            let facingScalars =
                [
                    match facingDiameters with
                    | diameter :: height :: _ ->
                        yield scalar "FacingDiameterRF" (Some "mm") diameter
                        yield scalar "FacingHeightRF" (Some "mm") height
                    | diameter :: _ ->
                        yield scalar "FacingDiameterRF" (Some "mm") diameter
                    | [] -> ()
                ]
            let scalars =
                [
                    yield! [ "RingOD"; "RingWT"; "HubLargeDiam"; "HubSmallDiam"; "HubLength"; "Height_WN"; "Height_LWN"; "BoltCircDiam"; "BoltHoleDiam2"; "BoltNomSize2"; "BoltMinLenRF"; "BoltMinLenRJ"; "BoltBackMinSpacing" ]
                           |> List.choose (fun name -> toScalar name flange)
                    yield! [ "NPS"; "Rating"; "DN"; "BoltNum"; "BoltHoleDiam1"; "BoltNomSize1" ]
                           |> List.choose (fun name -> toDimensionlessScalar name flange)
                    yield! facingScalars
                ]
            makeRecord
                location
                StandardFlanges
                recordId
                recordId
                flangeType
                standard
                rating
                scalars
                [ yield! nps |> Option.map (fun value -> $"NPS {value}") |> Option.toList
                  yield! rating |> Option.map (fun value -> $"Class {value}") |> Option.toList
                  yield! attr "Edition" flange |> Option.map (fun value -> $"Edition {value}") |> Option.toList ])
        |> Seq.toList

    let private loadMaterialsJson (location: DatabaseLocation) =
        use document = JsonDocument.Parse(File.ReadAllText(location.Path))
        let materials = document.RootElement.GetProperty("materials")

        materials.EnumerateArray()
        |> Seq.choose (fun material ->
            let getString (name: string) =
                match material.TryGetProperty(name) with
                | true, property when property.ValueKind = JsonValueKind.String -> Option.ofObj (property.GetString())
                | _ -> None

            match getString "id", getString "name" with
            | Some id, Some name ->
                let scalars =
                    match material.TryGetProperty("basicProperties") with
                    | true, basic when basic.ValueKind = JsonValueKind.Object ->
                        [
                            "specifiedMinimumYieldStrength"
                            "specifiedMinimumUltimateStrength"
                        ]
                        |> List.choose (fun propertyName ->
                            match basic.TryGetProperty(propertyName) with
                            | true, value when value.ValueKind = JsonValueKind.Number ->
                                Some(scalar propertyName (Some "MPa") (value.GetDouble()))
                            | _ -> None)
                    | _ -> []

                makeRecord
                    location
                    Materials
                    id
                    name
                    (getString "family")
                    (getString "specification")
                    (getString "grade")
                    scalars
                    [ yield! getString "productForm" |> Option.toList
                      yield! getString "nominalComposition" |> Option.toList
                      yield! getString "alloyIdentificationUns" |> Option.toList ]
                |> Some
            | _ -> None)
        |> Seq.toList

    let private sqliteString (reader: SqliteDataReader) ordinal =
        if reader.IsDBNull(ordinal) then None else Some(reader.GetString(ordinal))

    let private sqliteDouble (reader: SqliteDataReader) ordinal =
        if reader.IsDBNull(ordinal) then None else Some(reader.GetDouble(ordinal))

    let private loadAsmeMaterialsSqlite (location: DatabaseLocation) =
        use connection = new SqliteConnection($"Data Source={location.Path};Mode=ReadOnly")
        connection.Open()
        let hasColumn table column =
            use command = connection.CreateCommand()
            command.CommandText <- $"PRAGMA table_info({table})"
            use reader = command.ExecuteReader()
            let mutable found = false
            while reader.Read() do
                if String.Equals(reader.GetString(1), column, StringComparison.OrdinalIgnoreCase) then
                    found <- true
            found

        let normalizedSchema = hasColumn "materials" "name"
        use command = connection.CreateCommand()
        if normalizedSchema then
            command.CommandText <-
                """
                SELECT id, name, revision, nominal_composition, product_form, specification,
                       type_grade, alloy_designation, smus_mpa, smys_mpa,
                       poisson_factor, density_kg_m3
                FROM materials
                ORDER BY id
                """
        else
            command.CommandText <-
                """
                SELECT ID, NULL AS name, Revision, NominalComposition, ProductForm, Specification,
                       TypeGrade, AlloyDesignationNumber, SMTS, SMYS,
                       PoissonFactor, Density
                FROM Materials
                ORDER BY ID
                """
        use reader = command.ExecuteReader()

        [
            while reader.Read() do
                let id = reader.GetInt32(0).ToString(invariant)
                let specification = sqliteString reader 5
                let grade = sqliteString reader 6
                let name =
                    sqliteString reader 1
                    |> Option.orElseWith (fun () ->
                        match specification, grade with
                        | Some spec, Some typeGrade -> Some $"{spec} {typeGrade}"
                        | Some spec, None -> Some spec
                        | None, Some typeGrade -> Some typeGrade
                        | None, None -> None)
                    |> Option.defaultValue id
                let revision = sqliteString reader 2
                let nominalComposition = sqliteString reader 3
                let productForm = sqliteString reader 4
                let alloy = sqliteString reader 7
                let smus = sqliteDouble reader 8
                let smys = sqliteDouble reader 9
                let poisson = sqliteDouble reader 10
                let density = sqliteDouble reader 11

                let scalars =
                    [
                        yield! smys |> Option.map (scalar "specifiedMinimumYieldStrength" (Some "MPa")) |> Option.toList
                        yield! smus |> Option.map (scalar "specifiedMinimumUltimateStrength" (Some "MPa")) |> Option.toList
                        yield! poisson |> Option.map (scalar "PoissonRatio" None) |> Option.toList
                        yield! density |> Option.map (scalar "Density" (Some "kg/m3")) |> Option.toList
                    ]

                {
                    SourceId = location.Id
                    SourceName = location.Name
                    SourcePath = location.Path
                    Category = Materials
                    RecordId = id
                    DisplayName = name
                    Family = nominalComposition
                    Standard = specification
                    Grade = grade
                    Scalars = scalars
                    Tags = [ yield! productForm |> Option.toList; yield! alloy |> Option.toList ]
                    Provenance =
                        [
                            {
                                SourceId = location.Id
                                Description = $"{location.Name}: materials row {id}"
                                Revision = revision
                            }
                        ]
                }
        ]

    let private loadLocation (location: DatabaseLocation) =
        if not location.Enabled then
            Microsoft.FSharp.Core.Ok { Records = []; Summaries = [] }
        elif not (File.Exists location.Path) then
            Microsoft.FSharp.Core.Error $"Database source '{location.Id}' not found: {location.Path}"
        else
            let category = categoryFromLocation location
            let extension =
                match Path.GetExtension(location.Path) with
                | null -> String.Empty
                | value -> value.ToLowerInvariant()
            let records =
                match location.Id, extension with
                | id, ".json" when id.Contains("MATERIALS", StringComparison.OrdinalIgnoreCase) -> loadMaterialsJson location
                | id, ".sqlite3" when id.Contains("MATERIALS", StringComparison.OrdinalIgnoreCase) -> loadAsmeMaterialsSqlite location
                | id, ".db" when id.Contains("MATERIALS", StringComparison.OrdinalIgnoreCase) -> loadAsmeMaterialsSqlite location
                | id, ".xml" when id.Contains("BOLTING", StringComparison.OrdinalIgnoreCase) -> loadBolting location
                | id, ".xml" when id.Contains("GASKETS.DESIGN", StringComparison.OrdinalIgnoreCase) -> loadGasketParameters location
                | id, ".xml" when id.Contains("GASKETS.GEOMETRY", StringComparison.OrdinalIgnoreCase) -> loadGaskets location
                | id, ".xml" when id.Contains("FLANGES", StringComparison.OrdinalIgnoreCase) -> loadFlanges location
                | _, ".xml" -> loadGenericXml location category
                | _, _ -> []
            Microsoft.FSharp.Core.Ok
                {
                    Records = records
                    Summaries =
                        [
                            {
                                SourceId = location.Id
                                Path = location.Path
                                RecordCount = records.Length
                                Warnings = if records.IsEmpty then [ "No records imported by the current loader." ] else []
                            }
                        ]
                }

    /// <summary>Loads all enabled configured database paths into a searchable data catalog.</summary>
    let loadCatalog (settings: DatabasePathSettings) : Result<DataCatalog, string> =
        let locations =
            settings.Materials
            @ settings.Bolting
            @ settings.Threads
            @ settings.Gaskets
            @ settings.TighteningTools
            @ settings.ValidationCases
            @ settings.Custom
            |> List.filter (fun (location: DatabaseLocation) -> location.Enabled)
            |> List.sortBy _.Priority

        let folder state location =
            match state with
            | Microsoft.FSharp.Core.Error _ as error -> error
            | Microsoft.FSharp.Core.Ok catalog ->
                match loadLocation location with
                | Microsoft.FSharp.Core.Ok loaded ->
                    Microsoft.FSharp.Core.Ok
                        {
                            Records = catalog.Records @ loaded.Records
                            Summaries = catalog.Summaries @ loaded.Summaries
                        }
                | Microsoft.FSharp.Core.Error message -> Microsoft.FSharp.Core.Error message

        locations |> List.fold folder (Microsoft.FSharp.Core.Ok { Records = []; Summaries = [] })

    /// <summary>Loads all enabled configured database paths and applies a search filter.</summary>
    let search (settings: DatabasePathSettings) (criteria: DataRecordFilter) : Result<ImportedDataRecord list, string> =
        loadCatalog settings
        |> Result.map (fun catalog -> Search.filter criteria catalog.Records)
