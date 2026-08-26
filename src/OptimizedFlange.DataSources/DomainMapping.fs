namespace OptimizedFlange.DataSources

open System
open OptimizedFlange.Domain

/// <summary>Maps imported technical data records into domain objects when enough sourced values are present.</summary>
module DomainMapping =
    let private tryScalarSi name unitName (record: ImportedDataRecord) =
        record.Scalars
        |> List.tryFind (fun scalar ->
            String.Equals(scalar.Name, name, StringComparison.OrdinalIgnoreCase)
            && scalar.SiUnit = Some unitName)
        |> Microsoft.FSharp.Core.Option.bind _.SiValue

    let private sourceRevision (record: ImportedDataRecord) =
        record.Provenance |> List.tryHead |> Microsoft.FSharp.Core.Option.bind _.Revision

    let private materialSnapshotAtAmbient (record: ImportedDataRecord) : Result<MaterialSnapshot, string> =
        let yieldPa = tryScalarSi "specifiedMinimumYieldStrength" "Pa" record
        let ultimatePa = tryScalarSi "specifiedMinimumUltimateStrength" "Pa" record

        if record.Category <> Materials then
            Microsoft.FSharp.Core.Error $"Record '{record.RecordId}' is not a material record."
        else
            Microsoft.FSharp.Core.Ok
                {
                    Identity =
                        {
                            MaterialId = record.RecordId
                            Specification = record.Standard
                            Grade = record.Grade
                            ProductForm = record.Tags |> List.tryHead
                        }
                    Properties =
                        [
                            {
                                TemperatureK = 293.15<K>
                                AllowableStressPa = None
                                YieldStrengthPa = yieldPa |> Microsoft.FSharp.Core.Option.map LanguagePrimitives.FloatWithMeasure<Pa>
                                UltimateStrengthPa = ultimatePa |> Microsoft.FSharp.Core.Option.map LanguagePrimitives.FloatWithMeasure<Pa>
                                ElasticModulusPa = None
                                PoissonRatio = None
                                ThermalExpansionPerK = None
                                DensityKgPerM3 = None
                            }
                        ]
                    ProviderId = record.SourceId
                    ProviderRevision = sourceRevision record
                    SourceEdition = None
                    Fingerprint = None
                }

    let private boltingAssembly (boltCount: int) (boltCircleDiameterM: float<m>) (record: ImportedDataRecord) : Result<BoltingAssembly, string> =
        let required name unitName =
            match tryScalarSi name unitName record with
            | Some value -> Microsoft.FSharp.Core.Ok value
            | None -> Microsoft.FSharp.Core.Error $"Record '{record.RecordId}' misses SI scalar '{name}' as '{unitName}'."

        if record.Category <> Bolting then
            Microsoft.FSharp.Core.Error $"Record '{record.RecordId}' is not a bolting record."
        elif boltCount <= 0 then
            Microsoft.FSharp.Core.Error "Bolt count must be positive."
        else
            match required "NominalDiameter" "m", required "Pitch" "m", required "TensileStressArea" "m2" with
            | Microsoft.FSharp.Core.Ok nominalDiameter, Microsoft.FSharp.Core.Ok pitch, Microsoft.FSharp.Core.Ok tensileArea ->
                let rootDiameter = tryScalarSi "ThreadRootDiameter_d3" "m" record
                let nominalArea =
                    Math.PI * nominalDiameter * nominalDiameter / 4.0
                let rootArea =
                    rootDiameter
                    |> Microsoft.FSharp.Core.Option.map (fun diameter -> Math.PI * diameter * diameter / 4.0)
                    |> Microsoft.FSharp.Core.Option.defaultValue tensileArea

                Microsoft.FSharp.Core.Ok
                    {
                        AssemblyId = $"bolting:{record.RecordId}"
                        Arrangement = ThroughStuds
                        Pattern =
                            {
                                Count = boltCount
                                BoltCircleDiameterM = boltCircleDiameterM
                                StartAngleRad = 0.0
                            }
                        Stud =
                            {
                                NominalDiameterM = LanguagePrimitives.FloatWithMeasure<m> nominalDiameter
                                PitchM = LanguagePrimitives.FloatWithMeasure<m> pitch
                                ThreadStandard = AsmeB11UnifiedInch
                                ThreadingType = FullyThreaded
                                Areas =
                                    {
                                        NominalShankAreaM2 = LanguagePrimitives.FloatWithMeasure<m^2> nominalArea
                                        TensileStressAreaM2 = LanguagePrimitives.FloatWithMeasure<m^2> tensileArea
                                        MinimumRootAreaM2 = LanguagePrimitives.FloatWithMeasure<m^2> rootArea
                                        ReducedShankAreaM2 = None
                                        GoverningResistingAreaM2 = LanguagePrimitives.FloatWithMeasure<m^2> (min tensileArea rootArea)
                                    }
                                SpecifiedLengthM = None
                            }
                        ProjectAreaBasis = TensileStressArea
                        TighteningMethod = UserDefinedTightening
                        Preload =
                            {
                                MinimumPreloadN = None
                                TargetPreloadN = None
                                MaximumPreloadN = None
                            }
                    }
            | Microsoft.FSharp.Core.Error message, _, _
            | _, Microsoft.FSharp.Core.Error message, _
            | _, _, Microsoft.FSharp.Core.Error message -> Microsoft.FSharp.Core.Error message

    let private ringGasketAssembly (parameters: ImportedDataRecord option) (record: ImportedDataRecord) : Result<GasketAssembly, string> =
        let pitch = tryScalarSi "P" "m" record
        let width = tryScalarSi "A" "m" record
        let height = tryScalarSi "H" "m" record
        let selectedM =
            parameters
            |> Option.bind (fun parameter -> parameter.Scalars |> List.tryFind (fun scalar -> scalar.Name = "m"))
            |> Option.map _.SourceValue
        let selectedY =
            parameters
            |> Option.bind (tryScalarSi "y" "Pa")
            |> Option.map LanguagePrimitives.FloatWithMeasure<Pa>

        match record.Category, pitch, width, height with
        | Gaskets, Some pitchM, Some widthM, Some heightM ->
            let inside = max 0.0 (pitchM - widthM)
            let outside = pitchM + widthM
            let area = Math.PI * (outside * outside - inside * inside) / 4.0
            Microsoft.FSharp.Core.Ok
                {
                    AssemblyId = $"gasket:{record.RecordId}"
                    Family = RingTypeJoint
                    Envelope =
                        {
                            InsideDiameterM = LanguagePrimitives.FloatWithMeasure<m> inside
                            OutsideDiameterM = LanguagePrimitives.FloatWithMeasure<m> outside
                            ThicknessM = LanguagePrimitives.FloatWithMeasure<m> heightM
                        }
                    SealingZones =
                        [
                            {
                                ZoneId = "primary"
                                Role = PrimarySeal
                                Mandatory = true
                                NominalAreaM2 = LanguagePrimitives.FloatWithMeasure<m^2> area
                                MinimumAverageContactPressurePa = None
                                MaximumAverageContactPressurePa = None
                                MaterialReferenceId = None
                            }
                        ]
                    PartitionLayout = None
                    HasInnerRing = false
                    HasOuterRing = false
                    SelectedGasketM = selectedM
                    SelectedGasketYPa = selectedY
                    ProjectAreaBasis = PeripheralNominalSealingArea
                }
        | Gaskets, _, _, _ ->
            Microsoft.FSharp.Core.Error $"Record '{record.RecordId}' misses ring-gasket P/A/H dimensions."
        | _ ->
            Microsoft.FSharp.Core.Error $"Record '{record.RecordId}' is not a gasket record."

    let private zeroAllowance =
        {
            CorrosionAllowanceM = 0.0<m>
            WeldOverlayThicknessM = 0.0<m>
            MachiningAllowanceM = 0.0<m>
            MinusToleranceM = 0.0<m>
            PlusToleranceM = 0.0<m>
        }

    let private jointSideGeometry (sideId: string) (record: ImportedDataRecord) : Result<JointSideGeometry, string> =
        let bore =
            tryScalarSi "Bore" "m" record
            |> Option.orElseWith (fun () -> tryScalarSi "B" "m" record)
            |> Option.orElseWith (fun () -> tryScalarSi "HubSmallDiam" "m" record)
        let outside =
            tryScalarSi "OutsideDiameter" "m" record
            |> Option.orElseWith (fun () -> tryScalarSi "O" "m" record)
            |> Option.orElseWith (fun () -> tryScalarSi "RingOD" "m" record)
        let thickness =
            tryScalarSi "Thickness" "m" record
            |> Option.orElseWith (fun () -> tryScalarSi "t" "m" record)
            |> Option.orElseWith (fun () -> tryScalarSi "RingWT" "m" record)
        let boltCircle =
            tryScalarSi "BoltCircleDiameter" "m" record
            |> Option.orElseWith (fun () -> tryScalarSi "BC" "m" record)
            |> Option.orElseWith (fun () -> tryScalarSi "BoltCircDiam" "m" record)
        let hubSmall = tryScalarSi "HubSmallDiam" "m" record
        let hubLarge = tryScalarSi "HubLargeDiam" "m" record
        let hubLength = tryScalarSi "HubLength" "m" record
        let seatOutside = tryScalarSi "FacingDiameterRF" "m" record

        match record.Category, bore, outside, thickness, boltCircle with
        | StandardFlanges, Some boreM, Some outsideM, Some thicknessM, Some boltCircleM ->
            let hub =
                match hubSmall, hubLarge, hubLength with
                | Some g0, Some g1, Some length when length > 0.0 ->
                    Some
                        {
                            Topology = SingleTaperHub
                            G0M = Some(LanguagePrimitives.FloatWithMeasure<m> g0)
                            GMidM = None
                            G1M = Some(LanguagePrimitives.FloatWithMeasure<m> g1)
                            LengthM = Some(LanguagePrimitives.FloatWithMeasure<m> length)
                            BreakLocationM = None
                        }
                | _ -> None
            Microsoft.FSharp.Core.Ok
                {
                    SideId = sideId
                    FlangeType = IntegralFlange
                    Source = FlangeGeometrySource.Imported
                    SeatType = RaisedFace
                    Nominal =
                        {
                            BoreDiameterM = LanguagePrimitives.FloatWithMeasure<m> boreM
                            OutsideDiameterM = LanguagePrimitives.FloatWithMeasure<m> outsideM
                            ThicknessM = LanguagePrimitives.FloatWithMeasure<m> thicknessM
                            BoltCircleDiameterM = LanguagePrimitives.FloatWithMeasure<m> boltCircleM
                            SeatOutsideDiameterM = seatOutside |> Microsoft.FSharp.Core.Option.map LanguagePrimitives.FloatWithMeasure<m>
                        }
                    Hub = hub
                    InternalSurface = zeroAllowance
                    GasketSeat = zeroAllowance
                    ExternalSurface = zeroAllowance
                }
        | StandardFlanges, _, _, _, _ ->
            Microsoft.FSharp.Core.Error $"Record '{record.RecordId}' does not expose the required flange geometry scalar names yet."
        | _ ->
            Microsoft.FSharp.Core.Error $"Record '{record.RecordId}' is not a standard flange record."

    /// <summary>Maps an imported material record to a material snapshot using available ambient properties.</summary>
    let toMaterialSnapshot record = materialSnapshotAtAmbient record

    /// <summary>Maps an imported bolting record to a bolting assembly using project bolt count and bolt-circle diameter.</summary>
    let toBoltingAssembly boltCount boltCircleDiameterM record = boltingAssembly boltCount boltCircleDiameterM record

    /// <summary>Maps an imported ring-gasket record to a gasket assembly.</summary>
    let toGasketAssembly record = ringGasketAssembly None record

    /// <summary>Maps an imported ring-gasket record with selected gasket design parameters to a gasket assembly.</summary>
    let toGasketAssemblyWithParameters parameters record = ringGasketAssembly (Some parameters) record

    /// <summary>Maps an imported standard-flange record to side geometry when required scalar names are available.</summary>
    let toJointSideGeometry sideId record = jointSideGeometry sideId record
