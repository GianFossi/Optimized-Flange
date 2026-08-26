open System
open OptimizedFlange.Calculations
open OptimizedFlange.Configuration
open OptimizedFlange.DataSources
open OptimizedFlange.Domain

let databaseRoot = @"C:\Users\ganfossi\Documents\DataBase\data"
let settings = Defaults.databasePathsFromRootFolder databaseRoot

let formatMeasure value unitName =
    match value with
    | Some number -> $"{number:g6} {unitName}"
    | None -> "n/a"

let first label criteria =
    match LocalDatabaseLoaders.search settings criteria with
    | Microsoft.FSharp.Core.Error message -> failwith message
    | Microsoft.FSharp.Core.Ok records ->
        match records with
        | record :: _ ->
            printfn "Selected %-10s: %s [%s]" label record.DisplayName record.SourceId
            record
        | [] -> failwith $"No record found for {label}."

let scalar name unitName (record: ImportedDataRecord) =
    record.Scalars
    |> List.tryFind (fun scalar ->
        scalar.Name = name
        && if String.IsNullOrWhiteSpace(unitName) then scalar.SiUnit.IsNone else scalar.SiUnit = Some unitName)
    |> Option.bind (fun scalar ->
        scalar.SiValue
        |> Option.orElse (Some scalar.SourceValue))

printfn "OptimizedFlange text demo"
printfn "Database root: %s" databaseRoot
printfn ""

let flange =
    first
        "flange"
        {
            Search.all with
                Category = Some StandardFlanges
                Text = Some "NPS .5000"
                Grade = Some "150"
        }

let gasket =
    first
        "gasket"
        {
            Search.all with
                Category = Some Gaskets
                Text = Some "Ring gasket 11"
        }

let gasketParameters =
    first
        "gasket m/y"
        {
            Search.all with
                Category = Some GasketParameters
                Text = Some "Ring Joint: Iron or Soft Steel"
        }

let bolting =
    first
        "bolting"
        {
            Search.all with
                Category = Some Bolting
                Text = Some "1/2 x 13"
        }

let material =
    first
        "material"
        {
            Search.all with
                Category = Some Materials
                Text = Some "SA-516 70"
        }

printfn ""
printfn "Key imported SI values"
printfn "Flange OD       : %s" (formatMeasure (scalar "RingOD" "m" flange) "m")
printfn "Flange thickness: %s" (formatMeasure (scalar "RingWT" "m" flange) "m")
printfn "Bolt circle     : %s" (formatMeasure (scalar "BoltCircDiam" "m" flange) "m")
printfn "Gasket pitch P  : %s" (formatMeasure (scalar "P" "m" gasket) "m")
printfn "Gasket m        : %s" (formatMeasure (scalar "m" "" gasketParameters) "")
printfn "Gasket y        : %s" (formatMeasure (scalar "y" "Pa" gasketParameters) "Pa")
printfn "Bolt stress area: %s" (formatMeasure (scalar "TensileStressArea" "m2" bolting) "m2")
printfn "Material SMYS   : %s" (formatMeasure (scalar "specifiedMinimumYieldStrength" "Pa" material) "Pa")

let boltCount =
    scalar "BoltNum" "" flange
    |> Option.map int
    |> Option.defaultValue 4

let boltCircle =
    scalar "BoltCircDiam" "m" flange
    |> Option.map LanguagePrimitives.FloatWithMeasure<m>
    |> Option.defaultValue 0.0603<m>

let inputs =
    {
        JointId = "TEST-FLANGED-JOINT-001"
        PrimaryMaterialRole = "primary-flange"
        MatingMaterialRole = "mating-flange"
        BoltCount = boltCount
        BoltCircleDiameterM = boltCircle
        PrimaryPressurePa = 1_000_000.0<Pa>
        MatingPressurePa = 0.0<Pa>
        PrimaryTemperatureK = 293.15<K>
        MatingTemperatureK = 293.15<K>
    }

let selection =
    {
        PrimaryFlange = flange
        MatingFlange = None
        Gasket = gasket
        GasketParameters = Some gasketParameters
        Bolting = bolting
        PrimaryMaterial = material
        MatingMaterial = None
    }

printfn ""
printfn "Building flanged joint..."

match JointSelectionBuilder.buildJoint inputs selection with
| Microsoft.FSharp.Core.Error message ->
    printfn "Joint build failed: %s" message
    Environment.ExitCode <- 1
| Microsoft.FSharp.Core.Ok joint ->
    printfn "Joint id          : %s" joint.JointId
    printfn "Mating mode       : %A" joint.MatingSideMode
    printfn "Bolt count        : %i" joint.Bolting.Pattern.Count
    printfn "Gasket family     : %A" joint.Gasket.Family
    printfn "Load cases        : %i" joint.LoadCases.Length
    printfn "Material snapshots: %i" joint.Materials.Length

    let runProcedure procedure =
        {
            RequestId = "REQ-TEXT-DEMO-001"
            Joint = joint
            Procedure = procedure
            SelectedLoadCaseIds = []
            AcceptanceCriteria = []
        }
        |> CalculationDispatcher.run

    printfn ""
    printfn "Running dispatcher procedures"

    for procedure in [ ProcedureCatalog.structuralValidation; NormativeProcedureCatalog.asmeViiiDivision2; NormativeProcedureCatalog.iogpS614Paragraph78Amendments ] do
        printfn ""
        printfn "%s" procedure.Name
        match runProcedure procedure with
        | Microsoft.FSharp.Core.Error errors ->
            printfn "Dispatcher failed with %i error(s)." errors.Length
            for error in errors do
                printfn "- %s / %s" error.ErrorCode error.MessageCode
            Environment.ExitCode <- 1
        | Microsoft.FSharp.Core.Ok result ->
            printfn "Execution status  : %A" result.ExecutionStatus
            printfn "Assessment status : %A" result.AssessmentStatus
            printfn "Qualification     : %A" result.Qualification
            printfn "Checks            : %i" result.Checks.Length
            for check in result.Checks do
                printfn "- %s => %A" check.MessageCode check.Status
            for quantity in result.Trace.Quantities |> List.filter (fun q -> q.Role = Result) do
                printfn "  %s = %M %s" quantity.QuantityId quantity.CanonicalValue (quantity.Unit |> Option.defaultValue "")

    printfn ""
    printfn "Demo completed. ASME/IOGP outputs are PartiallyImplemented helper results, not qualified final design checks."
