namespace OptimizedFlange.DataSources

open OptimizedFlange.Domain

/// <summary>Inputs that cannot be inferred from catalog records when composing a calculation-ready joint.</summary>
type JointSelectionInputs =
    {
        /// <summary>Stable joint identifier.</summary>
        JointId: string
        /// <summary>Primary material role identifier.</summary>
        PrimaryMaterialRole: string
        /// <summary>Mating material role identifier.</summary>
        MatingMaterialRole: string
        /// <summary>Bolt count selected for the joint.</summary>
        BoltCount: int
        /// <summary>Bolt-circle diameter selected for the joint.</summary>
        BoltCircleDiameter: float<m>
        /// <summary>Primary-side pressure for the initial calculation case.</summary>
        PrimaryPressure: float<Pa>
        /// <summary>Mating-side pressure for the initial calculation case.</summary>
        MatingPressure: float<Pa>
        /// <summary>Primary-side temperature for the initial calculation case.</summary>
        PrimaryTemperature: float<K>
        /// <summary>Mating-side temperature for the initial calculation case.</summary>
        MatingTemperature: float<K>
    }

/// <summary>Records selected by the user or project defaults to compose a flanged joint.</summary>
type JointSelectedRecords =
    {
        /// <summary>Selected primary flange record.</summary>
        PrimaryFlange: ImportedDataRecord
        /// <summary>Optional explicit mating flange record. When omitted, primary geometry is reused.</summary>
        MatingFlange: ImportedDataRecord option
        /// <summary>Selected gasket record.</summary>
        Gasket: ImportedDataRecord
        /// <summary>Selected gasket design-parameter record with m/y values.</summary>
        GasketParameters: ImportedDataRecord option
        /// <summary>Selected bolting record.</summary>
        Bolting: ImportedDataRecord
        /// <summary>Selected primary material record.</summary>
        PrimaryMaterial: ImportedDataRecord
        /// <summary>Selected mating material record.</summary>
        MatingMaterial: ImportedDataRecord option
    }

/// <summary>Composes domain joints from explicit imported-data selections.</summary>
module JointSelectionBuilder =
    let private zeroLoads =
        {
            FxN = 0.0<N>
            FyN = 0.0<N>
            FzN = 0.0<N>
            MxNm = 0.0<N m>
            MyNm = 0.0<N m>
            MzNm = 0.0<N m>
        }

    /// <summary>Builds a flanged joint from selected imported records and explicit project inputs.</summary>
    let buildJoint (inputs: JointSelectionInputs) (selection: JointSelectedRecords) : Result<FlangedJoint, string> =
        let primaryGeometryResult = DomainMapping.toJointSideGeometry "primary" selection.PrimaryFlange
        let gasketResult =
            selection.GasketParameters
            |> Option.map (fun parameters -> DomainMapping.toGasketAssemblyWithParameters parameters selection.Gasket)
            |> Option.defaultValue (DomainMapping.toGasketAssembly selection.Gasket)
        let boltingResult = DomainMapping.toBoltingAssembly inputs.BoltCount inputs.BoltCircleDiameter selection.Bolting
        let primaryMaterialResult = DomainMapping.toMaterialSnapshot selection.PrimaryMaterial

        match primaryGeometryResult, gasketResult, boltingResult, primaryMaterialResult with
        | Microsoft.FSharp.Core.Ok primaryGeometry,
          Microsoft.FSharp.Core.Ok gasket,
          Microsoft.FSharp.Core.Ok bolting,
          Microsoft.FSharp.Core.Ok primaryMaterial ->
            let matingMaterialResult =
                selection.MatingMaterial
                |> Option.map DomainMapping.toMaterialSnapshot
                |> Option.defaultValue (Microsoft.FSharp.Core.Ok primaryMaterial)

            let matingGeometryResult =
                selection.MatingFlange
                |> Option.map (DomainMapping.toJointSideGeometry "mating")
                |> Option.defaultValue (Microsoft.FSharp.Core.Ok primaryGeometry)

            match matingMaterialResult, matingGeometryResult with
            | Microsoft.FSharp.Core.Ok matingMaterial, Microsoft.FSharp.Core.Ok matingGeometry ->
                Microsoft.FSharp.Core.Ok
                    {
                        JointId = inputs.JointId
                        PrimarySide =
                            {
                                Geometry = primaryGeometry
                                MaterialRole = inputs.PrimaryMaterialRole
                            }
                        MatingSideMode =
                            if selection.MatingFlange.IsSome then ExplicitGeometry else IdenticalToPrimary
                        MatingSide =
                            if selection.MatingFlange.IsSome then
                                Some
                                    {
                                        Geometry = matingGeometry
                                        MaterialRole = inputs.MatingMaterialRole
                                    }
                            else
                                None
                        Gasket = gasket
                        Bolting = bolting
                        LoadCases =
                            [
                                {
                                    LoadCaseId = "LC-001"
                                    Name = "Selected design case"
                                    Kind = LoadCaseKind.Design
                                    PrimaryCondition =
                                        {
                                            PressurePa = inputs.PrimaryPressure
                                            TemperatureK = inputs.PrimaryTemperature
                                        }
                                    MatingCondition =
                                        {
                                            PressurePa = inputs.MatingPressure
                                            TemperatureK = inputs.MatingTemperature
                                        }
                                    ExternalLoads = zeroLoads
                                }
                            ]
                        AcceptanceCriteria = []
                        Materials =
                            [
                                {
                                    ComponentRole = inputs.PrimaryMaterialRole
                                    Material = primaryMaterial
                                }
                                {
                                    ComponentRole = inputs.MatingMaterialRole
                                    Material = matingMaterial
                                }
                            ]
                    }
            | Microsoft.FSharp.Core.Error message, _
            | _, Microsoft.FSharp.Core.Error message -> Microsoft.FSharp.Core.Error message
        | Microsoft.FSharp.Core.Error message, _, _, _
        | _, Microsoft.FSharp.Core.Error message, _, _
        | _, _, Microsoft.FSharp.Core.Error message, _
        | _, _, _, Microsoft.FSharp.Core.Error message -> Microsoft.FSharp.Core.Error message
