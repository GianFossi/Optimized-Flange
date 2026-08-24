namespace OptimizedFlange.UnitTests

open OptimizedFlange.Domain
open Xunit

module FlangedJointStructureTests =
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

    let private bolting count =
        {
            AssemblyId = "bolting"
            Arrangement = ThroughStuds
            Pattern = { Count = count; BoltCircleDiameterM = 0.8<m>; StartAngleRad = 0.0 }
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

    let private gasket zones =
        {
            AssemblyId = "gasket"
            Family = SpiralWound
            Envelope = { InsideDiameterM = 0.5<m>; OutsideDiameterM = 0.7<m>; ThicknessM = 0.004<m> }
            SealingZones = zones
            PartitionLayout = None
            HasInnerRing = true
            HasOuterRing = true
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

    let private sealingZone =
        {
            ZoneId = "primary"
            Role = PrimarySeal
            Mandatory = true
            NominalAreaM2 = 0.01<m^2>
            MinimumAverageContactPressurePa = None
            MaximumAverageContactPressurePa = None
            MaterialReferenceId = None
        }

    let private validJoint =
        {
            JointId = "joint"
            PrimarySide = side
            MatingSideMode = IdenticalToPrimary
            MatingSide = None
            Gasket = gasket [ sealingZone ]
            Bolting = bolting 8
            LoadCases = [ loadCase ]
            AcceptanceCriteria = []
            Materials = []
        }

    [<Fact>]
    let ``validateStructure accepts a structurally complete joint`` () =
        let actual = FlangedJoint.validateStructure validJoint

        Assert.Empty(actual)

    [<Fact>]
    let ``validateStructure reports explicit mating side without mating geometry`` () =
        let joint = { validJoint with MatingSideMode = ExplicitGeometry; MatingSide = None }

        let actual = FlangedJoint.validateStructure joint

        Assert.Contains("JOINT.MATING_SIDE.EXPLICIT_GEOMETRY_REQUIRED", actual)

    [<Fact>]
    let ``validateStructure reports missing non normative structural inputs`` () =
        let joint =
            {
                validJoint with
                    JointId = ""
                    LoadCases = []
                    Gasket = gasket []
                    Bolting = bolting 0
            }

        let actual = FlangedJoint.validateStructure joint

        Assert.Contains("JOINT.ID.REQUIRED", actual)
        Assert.Contains("JOINT.LOAD_CASE.REQUIRED", actual)
        Assert.Contains("GASKET.SEALING_ZONE.REQUIRED", actual)
        Assert.Contains("BOLTING.COUNT.POSITIVE_REQUIRED", actual)
