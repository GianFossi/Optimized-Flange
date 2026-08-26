namespace OptimizedFlange.Calculations

open OptimizedFlange.Domain

/// <summary>Routes partially implemented normative procedures without fabricating unresolved source-derived inputs.</summary>
module NormativeAssessmentEngine =
    let private quantity id role value unit notes =
        {
            QuantityId = id
            Role = role
            CanonicalValue = decimal value
            Unit = unit
            SourceValueId = None
            Notes = notes
        }

    let private toMillimetres (value: float<m>) = float value * 1000.0

    let private toSquareMillimetres (value: float<m^2>) = float value * 1_000_000.0

    let private toMegapascals (value: float<Pa>) = float value / 1_000_000.0

    let private toDegreesCelsius (value: float<K>) = float value - 273.15

    let private emptyTrace =
        {
            Quantities = []
            Dependencies = []
        }

    let private missingInputCheck rule messageCode notes =
        {
            CheckId = $"{rule.RuleId}.INPUTS_REQUIRED"
            Rule = rule
            Status = Incomplete
            Severity = Warning
            Comparison = None
            GoverningCase = None
            MessageCode = messageCode
            Trace =
                {
                    Quantities =
                        [
                            {
                                QuantityId = $"{rule.RuleId}.IMPLEMENTATION_STATE"
                                Role = Diagnostic
                                CanonicalValue = 0M
                                Unit = None
                                SourceValueId = None
                                Notes = Some notes
                            }
                        ]
                    Dependencies =
                        [
                            {
                                DependencyId = rule.RuleId
                                DependencyKind = "EngineeringRule"
                                Fingerprint = None
                            }
                        ]
                }
        }

    let private incompleteResult request checks =
        {
            ResultId = $"{request.RequestId}.{request.Procedure.ProcedureId}.RESULT"
            ExecutionStatus = Completed
            AssessmentStatus = Incomplete
            Qualification = PartiallyImplemented
            Checks = checks
            Trace = emptyTrace
        }

    let private completedResult request qualification checks trace =
        {
            ResultId = $"{request.RequestId}.{request.Procedure.ProcedureId}.RESULT"
            ExecutionStatus = Completed
            AssessmentStatus =
                if checks |> List.exists (fun check -> check.Status = Incomplete) then Incomplete else Satisfied
            Qualification = qualification
            Checks = checks
            Trace = trace
        }

    let private gasketDerivedInputs request =
        let envelope = request.Joint.Gasket.Envelope
        let inside = float envelope.InsideDiameterM
        let outside = float envelope.OutsideDiameterM
        let width = max 0.0 ((outside - inside) / 2.0)
        let reactionDiameter = (inside + outside) / 2.0

        match request.Joint.Gasket.SelectedGasketM, request.Joint.Gasket.SelectedGasketYPa with
        | Some gasketM, Some gasketY ->
            Microsoft.FSharp.Core.Ok(
                LanguagePrimitives.FloatWithMeasure<m> reactionDiameter,
                LanguagePrimitives.FloatWithMeasure<m> width,
                gasketM,
                gasketY)
        | None, _ -> Microsoft.FSharp.Core.Error "Selected gasket m factor is missing from the joint."
        | _, None -> Microsoft.FSharp.Core.Error "Selected gasket y seating stress is missing from the joint."

    let private governingLoadCase request =
        match request.SelectedLoadCaseIds with
        | [] -> request.Joint.LoadCases |> List.tryHead
        | selected ->
            request.Joint.LoadCases
            |> List.tryFind (fun loadCase -> selected |> List.contains loadCase.LoadCaseId)

    let private roleMatches (role: string) (selectedMaterial: ComponentMaterial) =
        selectedMaterial.ComponentRole.Contains(role, System.StringComparison.OrdinalIgnoreCase)

    let private nearestPropertyAt temperature (material: MaterialSnapshot) =
        material.Properties
        |> List.sortBy (fun property -> abs (float (property.TemperatureK - temperature)))
        |> List.tryHead

    let private allowableStressQuantity quantityId role temperature request =
        request.Joint.Materials
        |> List.tryFind (roleMatches role)
        |> Option.bind (fun selectedMaterial ->
            nearestPropertyAt temperature selectedMaterial.Material
            |> Option.bind (fun property ->
                property.AllowableStressPa
                |> Option.map (fun allowable ->
                    quantity
                        quantityId
                        DerivedInput
                        (toMegapascals allowable)
                        (Some "MPa")
                        (Some $"Resolved from selected material '{selectedMaterial.ComponentRole}' at {toDegreesCelsius property.TemperatureK} DegC."))))

    let private asmeBoltLoadCheck request (loadCase: JointLoadCase) (input: AsmeViii2Part416BoltLoadInput) result =
        let rule = NormativeProcedureCatalog.asmeViiiDivision2FlangedJointRule
        let materialQuantities =
            [
                allowableStressQuantity "ASME.VIII.2.INPUT.PRIMARY_ALLOWABLE_STRESS" "primary" loadCase.PrimaryCondition.TemperatureK request
                allowableStressQuantity "ASME.VIII.2.INPUT.MATING_ALLOWABLE_STRESS" "mating" loadCase.MatingCondition.TemperatureK request
                allowableStressQuantity "ASME.VIII.2.INPUT.BOLTING_ALLOWABLE_STRESS" "bolt" loadCase.PrimaryCondition.TemperatureK request
            ]
            |> List.choose id

        {
            CheckId = $"{rule.RuleId}.BASIC_BOLT_LOADS"
            Rule = { rule with Qualification = PartiallyImplemented }
            Status = Satisfied
            Severity = Info
            Comparison = None
            GoverningCase = None
            MessageCode = "ASME.VIII.2.FLANGE.BASIC_BOLT_LOADS.CALCULATED"
            Trace =
                {
                    Quantities =
                        [
                            quantity "ASME.VIII.2.INPUT.P" Input (toMegapascals input.PressurePa) (Some "MPa") None
                            quantity "ASME.VIII.2.INPUT.G" DerivedInput (toMillimetres input.GasketReactionDiameterM) (Some "mm") (Some "Derived from selected gasket envelope.")
                            quantity "ASME.VIII.2.INPUT.b" DerivedInput (toMillimetres input.EffectiveGasketWidthM) (Some "mm") (Some "Derived from selected gasket envelope.")
                            quantity "ASME.VIII.2.INPUT.m" Input input.GasketM None None
                            quantity "ASME.VIII.2.INPUT.y" Input (toMegapascals input.GasketYPa) (Some "MPa") None
                            quantity "ASME.VIII.2.RESULT.W_OPERATING" Result (float result.OperatingBoltLoadN) (Some "N") None
                            quantity "ASME.VIII.2.RESULT.W_SEATING" Result (float result.GasketSeatingLoadN) (Some "N") None
                        ] @ materialQuantities
                    Dependencies =
                        [
                            { DependencyId = request.Joint.Gasket.AssemblyId; DependencyKind = "GasketAssembly"; Fingerprint = None }
                            { DependencyId = rule.RuleId; DependencyKind = "EngineeringRule"; Fingerprint = None }
                        ]
                }
        }

    let private iogpPressureEffectCheck request (loadCase: JointLoadCase) (input: IogpS614FloatingHeadPressureEffectInput) result =
        let rule = NormativeProcedureCatalog.iogpS614Paragraph78AmendmentsRule
        let materialQuantities =
            [
                allowableStressQuantity "IOGP.S614.INPUT.BOLTING_ALLOWABLE_STRESS" "bolt" loadCase.PrimaryCondition.TemperatureK request
            ]
            |> List.choose id

        {
            CheckId = $"{rule.RuleId}.FLOATING_HEAD_PRESSURE_EFFECT"
            Rule = { rule with Qualification = PartiallyImplemented }
            Status = Satisfied
            Severity = Info
            Comparison = None
            GoverningCase = None
            MessageCode = "IOGP.S614.7.8.10.EQ3.CALCULATED"
            Trace =
                {
                    Quantities =
                        [
                            quantity "IOGP.S614.INPUT.SG_MIN" Input (toMegapascals input.MinimumGasketStressPa) (Some "MPa") None
                            quantity "IOGP.S614.INPUT.AG" DerivedInput (toSquareMillimetres input.FloatingHeadGasketAreaM2) (Some "mm2") None
                            quantity "IOGP.S614.INPUT.DGI" DerivedInput (toMillimetres input.FloatingHeadGasketInsideDiameterM) (Some "mm") None
                            quantity "IOGP.S614.INPUT.DFO" DerivedInput (toMillimetres input.FloatingHeadOutsideDiameterM) (Some "mm") None
                            quantity "IOGP.S614.INPUT.PT" Input (toMegapascals input.TubeSidePressurePa) (Some "MPa") None
                            quantity "IOGP.S614.INPUT.PS" Input (toMegapascals input.ShellSidePressurePa) (Some "MPa") None
                            quantity "IOGP.S614.INPUT.KG" Input input.GasketFactor None None
                            quantity "IOGP.S614.INPUT.AB_ROOT" DerivedInput (toSquareMillimetres input.BoltRootAreaM2) (Some "mm2") None
                            quantity "IOGP.S614.RESULT.SB_REQ" Result (toMegapascals result.RequiredSelectedAssemblyBoltStressPa) (Some "MPa") None
                            quantity "IOGP.S614.RESULT.F_PRESSURE" Intermediate (float result.PressureResultantN) (Some "N") None
                            quantity "IOGP.S614.RESULT.F_GASKET" Intermediate (float result.GasketContributionN) (Some "N") None
                        ] @ materialQuantities
                    Dependencies =
                        [
                            { DependencyId = request.Joint.Gasket.AssemblyId; DependencyKind = "GasketAssembly"; Fingerprint = None }
                            { DependencyId = request.Joint.Bolting.AssemblyId; DependencyKind = "BoltingAssembly"; Fingerprint = None }
                            { DependencyId = rule.RuleId; DependencyKind = "EngineeringRule"; Fingerprint = None }
                        ]
                }
        }

    /// <summary>Runs the partially implemented ASME VIII Division 2 assessment endpoint.</summary>
    let runAsmeViiiDivision2 request =
        match governingLoadCase request, gasketDerivedInputs request with
        | Some loadCase, Microsoft.FSharp.Core.Ok (reactionDiameter, effectiveWidth, gasketM, gasketY) ->
            let input =
                {
                    PressurePa = max loadCase.PrimaryCondition.PressurePa loadCase.MatingCondition.PressurePa
                    GasketReactionDiameterM = reactionDiameter
                    EffectiveGasketWidthM = effectiveWidth
                    GasketM = gasketM
                    GasketYPa = gasketY
                    SelfEnergizing = gasketM = 0.0 && gasketY = 0.0<Pa>
                    SelfEnergizingSeatingForceN = None
                }

            match AsmeViii2Part416BoltLoads.calculate input with
            | Microsoft.FSharp.Core.Ok result ->
                let check = asmeBoltLoadCheck request loadCase input result
                Microsoft.FSharp.Core.Ok(completedResult request PartiallyImplemented [ check ] check.Trace)
            | Microsoft.FSharp.Core.Error errors ->
                let notes = String.concat " " errors
                Microsoft.FSharp.Core.Ok(incompleteResult request [ missingInputCheck NormativeProcedureCatalog.asmeViiiDivision2FlangedJointRule "ASME.VIII.2.FLANGE.INPUTS_INVALID" notes ])
        | None, _ ->
            Microsoft.FSharp.Core.Ok(incompleteResult request [ missingInputCheck NormativeProcedureCatalog.asmeViiiDivision2FlangedJointRule "ASME.VIII.2.FLANGE.LOAD_CASE_REQUIRED" "At least one load case is required." ])
        | _, Microsoft.FSharp.Core.Error message ->
            Microsoft.FSharp.Core.Ok(incompleteResult request [ missingInputCheck NormativeProcedureCatalog.asmeViiiDivision2FlangedJointRule "ASME.VIII.2.FLANGE.INPUTS_REQUIRED" message ])

    /// <summary>Runs the partially implemented IOGP S-614 paragraph 7.8 assessment endpoint.</summary>
    let runIogpS614Paragraph78 request =
        match governingLoadCase request, gasketDerivedInputs request with
        | Some loadCase, Microsoft.FSharp.Core.Ok (_, _, gasketM, gasketY) ->
            let gasket = request.Joint.Gasket
            let input =
                {
                    MinimumGasketStressPa = gasketY
                    FloatingHeadGasketAreaM2 = gasket.SealingZones |> List.sumBy _.NominalAreaM2
                    FloatingHeadGasketInsideDiameterM = gasket.Envelope.InsideDiameterM
                    FloatingHeadOutsideDiameterM = request.Joint.PrimarySide.Geometry.Nominal.OutsideDiameterM
                    TubeSidePressurePa = loadCase.PrimaryCondition.PressurePa
                    ShellSidePressurePa = loadCase.MatingCondition.PressurePa
                    GasketFactor = if gasketM <= 0.0 then 1.0 else gasketM
                    BoltCount = request.Joint.Bolting.Pattern.Count
                    BoltRootAreaM2 = request.Joint.Bolting.Stud.Areas.MinimumRootAreaM2
                }

            match IogpS614Paragraph78.requiredSelectedAssemblyBoltStress input with
            | Microsoft.FSharp.Core.Ok result ->
                let check = iogpPressureEffectCheck request loadCase input result
                Microsoft.FSharp.Core.Ok(completedResult request PartiallyImplemented [ check ] check.Trace)
            | Microsoft.FSharp.Core.Error errors ->
                let notes = String.concat " " errors
                Microsoft.FSharp.Core.Ok(incompleteResult request [ missingInputCheck NormativeProcedureCatalog.iogpS614Paragraph78AmendmentsRule "IOGP.S614.7.8.INPUTS_INVALID" notes ])
        | None, _ ->
            Microsoft.FSharp.Core.Ok(incompleteResult request [ missingInputCheck NormativeProcedureCatalog.iogpS614Paragraph78AmendmentsRule "IOGP.S614.7.8.LOAD_CASE_REQUIRED" "At least one load case is required." ])
        | _, Microsoft.FSharp.Core.Error message ->
            Microsoft.FSharp.Core.Ok(incompleteResult request [ missingInputCheck NormativeProcedureCatalog.iogpS614Paragraph78AmendmentsRule "IOGP.S614.7.8.INPUTS_REQUIRED" message ])
