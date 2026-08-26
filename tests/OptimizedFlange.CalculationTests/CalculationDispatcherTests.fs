namespace OptimizedFlange.CalculationTests

open OptimizedFlange.Calculations
open OptimizedFlange.Domain
open Xunit

module CalculationDispatcherTests =
    let private allowance =
        {
            CorrosionAllowanceM = 0.0<m>
            WeldOverlayThicknessM = 0.0<m>
            MachiningAllowanceM = 0.0<m>
            MinusToleranceM = 0.0<m>
            PlusToleranceM = 0.0<m>
        }

    let private side =
        {
            Geometry =
                {
                    SideId = "side-a"
                    FlangeType = IntegralFlange
                    Source = CustomDesigned
                    SeatType = RaisedFace
                    Nominal =
                        {
                            BoreDiameterM = 0.5<m>
                            OutsideDiameterM = 1.0<m>
                            ThicknessM = 0.1<m>
                            BoltCircleDiameterM = 0.8<m>
                            SeatOutsideDiameterM = Some 0.7<m>
                        }
                    Hub = None
                    InternalSurface = allowance
                    GasketSeat = allowance
                    ExternalSurface = allowance
                }
            MaterialRole = "flange"
        }

    let private bolting =
        {
            AssemblyId = "bolting"
            Arrangement = ThroughStuds
            Pattern = { Count = 8; BoltCircleDiameterM = 0.8<m>; StartAngleRad = 0.0 }
            Stud =
                {
                    NominalDiameterM = 0.02<m>
                    PitchM = 0.0025<m>
                    ThreadStandard = AsmeB113Metric
                    ThreadingType = FullyThreaded
                    Areas =
                        {
                            NominalShankAreaM2 = 0.000314<m^2>
                            TensileStressAreaM2 = 0.000245<m^2>
                            MinimumRootAreaM2 = 0.000225<m^2>
                            ReducedShankAreaM2 = None
                            GoverningResistingAreaM2 = 0.000225<m^2>
                        }
                    SpecifiedLengthM = None
                }
            ProjectAreaBasis = GoverningResistingArea
            TighteningMethod = TorqueControlled
            Preload = { MinimumPreloadN = None; TargetPreloadN = None; MaximumPreloadN = None }
        }

    let private gasket =
        {
            AssemblyId = "gasket"
            Family = SpiralWound
            Envelope = { InsideDiameterM = 0.5<m>; OutsideDiameterM = 0.7<m>; ThicknessM = 0.004<m> }
            SealingZones =
                [
                    {
                        ZoneId = "primary"
                        Role = PrimarySeal
                        Mandatory = true
                        NominalAreaM2 = 0.01<m^2>
                        MinimumAverageContactPressurePa = None
                        MaximumAverageContactPressurePa = None
                        MaterialReferenceId = None
                    }
                ]
            PartitionLayout = None
            HasInnerRing = true
            HasOuterRing = true
            SelectedGasketM = Some 3.0
            SelectedGasketYPa = Some 70.0e6<Pa>
            ProjectAreaBasis = TotalNominalSealingArea
        }

    let private loadCase =
        {
            LoadCaseId = "design"
            Name = "Design"
            Kind = Design
            PrimaryCondition = { PressurePa = 1.0e6<Pa>; TemperatureK = 293.15<K> }
            MatingCondition = { PressurePa = 1.0e6<Pa>; TemperatureK = 293.15<K> }
            ExternalLoads =
                {
                    FxN = 0.0<N>
                    FyN = 0.0<N>
                    FzN = 0.0<N>
                    MxNm = 0.0<N m>
                    MyNm = 0.0<N m>
                    MzNm = 0.0<N m>
                }
        }

    let private material role allowable =
        {
            ComponentRole = role
            Material =
                {
                    Identity =
                        {
                            MaterialId = $"{role}-material"
                            Specification = Some "Test"
                            Grade = Some "A"
                            ProductForm = None
                        }
                    Properties =
                        [
                            {
                                TemperatureK = 293.15<K>
                                AllowableStressPa = Some allowable
                                YieldStrengthPa = None
                                UltimateStrengthPa = None
                                ElasticModulusPa = None
                                PoissonRatio = None
                                ThermalExpansionPerK = None
                                DensityKgPerM3 = None
                            }
                        ]
                    ProviderId = "test"
                    ProviderRevision = None
                    SourceEdition = None
                    Fingerprint = None
                }
        }

    let private joint =
        {
            JointId = "joint"
            PrimarySide = side
            MatingSideMode = IdenticalToPrimary
            MatingSide = None
            Gasket = gasket
            Bolting = bolting
            LoadCases = [ loadCase ]
            AcceptanceCriteria = []
            Materials =
                [
                    material "primary flange" 120.0e6<Pa>
                    material "mating flange" 110.0e6<Pa>
                    material "bolting" 100.0e6<Pa>
                ]
        }

    let private request procedure =
        {
            RequestId = "request"
            Joint = joint
            Procedure = procedure
            SelectedLoadCaseIds = []
            AcceptanceCriteria = []
        }

    [<Fact>]
    let ``dispatcher runs implemented structural validation`` () =
        let actual = CalculationDispatcher.run (request ProcedureCatalog.structuralValidation)

        match actual with
        | Ok result ->
            Assert.Equal(Completed, result.ExecutionStatus)
            Assert.Equal(Satisfied, result.AssessmentStatus)
            Assert.Empty(result.Checks)
        | Result.Error errors ->
            Assert.Fail($"Unexpected errors: {errors}")

    [<Fact>]
    let ``dispatcher returns not implemented for normative assessment procedures`` () =
        let procedure =
            {
                ProcedureCatalog.structuralValidation with
                    ProcedureId = "ASME.PLACEHOLDER"
                    Kind = DesignCodeAssessment
                    Name = "ASME placeholder"
                    Qualification = PartiallyImplemented
            }

        let actual = CalculationDispatcher.run (request procedure)

        match actual with
        | Ok _ -> Assert.Fail("Expected unsupported procedure error.")
        | Result.Error errors ->
            Assert.Single(errors) |> ignore
            Assert.Equal("CALCULATION.PROCEDURE.NOT_IMPLEMENTED", errors.Head.ErrorCode)

    [<Fact>]
    let ``procedure catalog exposes planned normative procedures`` () =
        let procedures = ProcedureCatalog.all

        Assert.Contains(
            procedures,
            fun procedure -> procedure.ProcedureId = "ASME.VIII.1.PROCEDURE.FLANGE_ASSESSMENT")
        Assert.Contains(
            procedures,
            fun procedure -> procedure.ProcedureId = "ASME.VIII.2.PROCEDURE.FLANGED_JOINT_ASSESSMENT")
        Assert.Contains(
            procedures,
            fun procedure -> procedure.ProcedureId = "ASME.PCC.1.2022.APPENDIX.O.PROCEDURE")
        Assert.Contains(
            procedures,
            fun procedure -> procedure.ProcedureId = "API.660.2015.PARAGRAPH.7.8.PROCEDURE")
        Assert.Contains(
            procedures,
            fun procedure -> procedure.ProcedureId = "IOGP.S-614.V18-12.PARAGRAPH.7.8.PROCEDURE")

    [<Fact>]
    let ``planned normative procedures remain unimplemented through dispatcher`` () =
        let procedures =
            NormativeProcedureCatalog.all
            |> List.except [
                NormativeProcedureCatalog.asmeViiiDivision2
                NormativeProcedureCatalog.iogpS614Paragraph78Amendments
            ]

        for procedure in procedures do
            Assert.Equal(Planned, procedure.Qualification)
            Assert.All(procedure.Rules, fun rule -> Assert.Equal(Planned, rule.Qualification))

            let actual = CalculationDispatcher.run (request procedure)

            match actual with
            | Ok _ -> Assert.Fail($"Expected not implemented error for {procedure.ProcedureId}.")
            | Result.Error errors ->
                Assert.Single(errors) |> ignore
                Assert.Equal("CALCULATION.PROCEDURE.NOT_IMPLEMENTED", errors.Head.ErrorCode)

    [<Fact>]
    let ``dispatcher resolves and runs partially implemented ASME VIII Division 2 helper procedure`` () =
        let actual = CalculationDispatcher.run (request NormativeProcedureCatalog.asmeViiiDivision2)

        match actual with
        | Result.Error errors -> Assert.Fail($"Unexpected errors: {errors}")
        | Ok result ->
            Assert.Equal(Completed, result.ExecutionStatus)
            Assert.Equal(Satisfied, result.AssessmentStatus)
            Assert.Equal(PartiallyImplemented, result.Qualification)
            Assert.Single(result.Checks) |> ignore
            Assert.Equal("ASME.VIII.2.FLANGE.BASIC_BOLT_LOADS.CALCULATED", result.Checks.Head.MessageCode)
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "ASME.VIII.2.RESULT.W_OPERATING")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "ASME.VIII.2.RESULT.W_SEATING")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "ASME.VIII.2.INPUT.PRIMARY_ALLOWABLE_STRESS")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "ASME.VIII.2.INPUT.BOLTING_ALLOWABLE_STRESS")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "ASME.VIII.2.INPUT.P" && quantity.Unit = Some "MPa")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "ASME.VIII.2.INPUT.G" && quantity.Unit = Some "mm")

    [<Fact>]
    let ``dispatcher resolves and runs partially implemented IOGP S-614 helper procedure`` () =
        let actual = CalculationDispatcher.run (request NormativeProcedureCatalog.iogpS614Paragraph78Amendments)

        match actual with
        | Result.Error errors -> Assert.Fail($"Unexpected errors: {errors}")
        | Ok result ->
            Assert.Equal(Completed, result.ExecutionStatus)
            Assert.Equal(Satisfied, result.AssessmentStatus)
            Assert.Equal(PartiallyImplemented, result.Qualification)
            Assert.Single(result.Checks) |> ignore
            Assert.Equal("IOGP.S614.7.8.10.EQ3.CALCULATED", result.Checks.Head.MessageCode)
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "IOGP.S614.RESULT.SB_REQ")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "IOGP.S614.INPUT.BOLTING_ALLOWABLE_STRESS")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "IOGP.S614.RESULT.SB_REQ" && quantity.Unit = Some "MPa")
            Assert.Contains(result.Trace.Quantities, fun quantity -> quantity.QuantityId = "IOGP.S614.INPUT.AG" && quantity.Unit = Some "mm2")
