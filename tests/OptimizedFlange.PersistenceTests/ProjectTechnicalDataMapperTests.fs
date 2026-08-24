namespace OptimizedFlange.PersistenceTests

open OptimizedFlange.Domain
open OptimizedFlange.Persistence
open Xunit

module ProjectTechnicalDataMapperTests =
    let private allowance: SurfaceAllowance =
        {
            CorrosionAllowanceM = 0.001<m>
            WeldOverlayThicknessM = 0.002<m>
            MachiningAllowanceM = 0.0005<m>
            MinusToleranceM = 0.0002<m>
            PlusToleranceM = 0.0003<m>
        }

    let private jointSideGeometry: JointSideGeometry =
        {
            SideId = "primary-side"
            FlangeType = IntegralFlange
            Source = CustomDesigned
            SeatType = RaisedFace
            Nominal =
                {
                    BoreDiameterM = 0.5<m>
                    OutsideDiameterM = 1.2<m>
                    ThicknessM = 0.12<m>
                    BoltCircleDiameterM = 0.95<m>
                    SeatOutsideDiameterM = Some 0.72<m>
                }
            Hub =
                Some {
                    Topology = SingleTaperHub
                    G0M = Some 0.03<m>
                    GMidM = None
                    G1M = Some 0.08<m>
                    LengthM = Some 0.15<m>
                    BreakLocationM = None
                }
            InternalSurface = allowance
            GasketSeat = { allowance with CorrosionAllowanceM = 0.0<m> }
            ExternalSurface = { allowance with WeldOverlayThicknessM = 0.0<m> }
        }

    let private loadCase: JointLoadCase =
        {
            LoadCaseId = "operating-1"
            Name = "Operating 1"
            Kind = Operating
            PrimaryCondition = { PressurePa = 2.5e6<Pa>; TemperatureK = 450.0<K> }
            MatingCondition = { PressurePa = 1.0e5<Pa>; TemperatureK = 420.0<K> }
            ExternalLoads =
                {
                    FxN = -10.0<N>
                    FyN = 20.0<N>
                    FzN = -30.0<N>
                    MxNm = 40.0<N m>
                    MyNm = -50.0<N m>
                    MzNm = 60.0<N m>
                }
        }

    let private boltingAssembly: BoltingAssembly =
        {
            AssemblyId = "main-bolting"
            Arrangement = ThroughStuds
            Pattern = { Count = 16; BoltCircleDiameterM = 0.95<m>; StartAngleRad = 0.1 }
            Stud =
                {
                    NominalDiameterM = 0.024<m>
                    PitchM = 0.003<m>
                    ThreadStandard = AsmeB113Metric
                    ThreadingType = ReducedShank
                    Areas =
                        {
                            NominalShankAreaM2 = 0.000452<m^2>
                            TensileStressAreaM2 = 0.000353<m^2>
                            MinimumRootAreaM2 = 0.000330<m^2>
                            ReducedShankAreaM2 = Some 0.000300<m^2>
                            GoverningResistingAreaM2 = 0.000300<m^2>
                        }
                    SpecifiedLengthM = Some 0.18<m>
                }
            ProjectAreaBasis = GoverningResistingArea
            TighteningMethod = HydraulicTensioning
            Preload =
                {
                    MinimumPreloadN = Some 100000.0<N>
                    TargetPreloadN = Some 125000.0<N>
                    MaximumPreloadN = Some 150000.0<N>
                }
        }

    let private gasketAssembly: GasketAssembly =
        {
            AssemblyId = "main-gasket"
            Family = SpiralWound
            Envelope = { InsideDiameterM = 0.5<m>; OutsideDiameterM = 0.72<m>; ThicknessM = 0.0045<m> }
            SealingZones =
                [
                    {
                        ZoneId = "primary"
                        Role = PrimarySeal
                        Mandatory = true
                        NominalAreaM2 = 0.012<m^2>
                        MinimumAverageContactPressurePa = Some 20.0e6<Pa>
                        MaximumAverageContactPressurePa = Some 180.0e6<Pa>
                        MaterialReferenceId = Some "gasket-material"
                    }
                    {
                        ZoneId = "partition"
                        Role = PartitionSeal
                        Mandatory = true
                        NominalAreaM2 = 0.003<m^2>
                        MinimumAverageContactPressurePa = None
                        MaximumAverageContactPressurePa = None
                        MaterialReferenceId = None
                    }
                ]
            PartitionLayout =
                Some {
                    PassCount = 2
                    Ribs =
                        [
                            {
                                RibId = "rib-1"
                                OffsetM = 0.0<m>
                                OrientationRad = 1.57079632679
                                WidthM = 0.012<m>
                                EffectiveLengthM = Some 0.45<m>
                                SealingZoneId = "partition"
                            }
                        ]
                }
            HasInnerRing = true
            HasOuterRing = true
            ProjectAreaBasis = TotalNominalSealingArea
        }

    let private componentMaterial: ComponentMaterial =
        {
            ComponentRole = "primary-flange"
            Material =
                {
                    Identity =
                        {
                            MaterialId = "SA-105"
                            Specification = Some "SA-105"
                            Grade = None
                            ProductForm = Some "Forging"
                        }
                    Properties =
                        [
                            {
                                TemperatureK = 450.0<K>
                                AllowableStressPa = Some 138.0e6<Pa>
                                YieldStrengthPa = Some 250.0e6<Pa>
                                UltimateStrengthPa = Some 485.0e6<Pa>
                                ElasticModulusPa = Some 190.0e9<Pa>
                                PoissonRatio = Some 0.3
                                ThermalExpansionPerK = Some 12.0e-6
                                DensityKgPerM3 = Some 7850.0<kg/m^3>
                            }
                        ]
                    ProviderId = "materials-provider"
                    ProviderRevision = Some "rev-001"
                    SourceEdition = Some "2025"
                    Fingerprint = Some "sha256-test"
                }
        }

    let private acceptanceCriterion: AcceptanceCriterion =
        {
            CriterionId = "criterion-info"
            Level = Informational
            Source = User
            Edition = None
            Clause = None
            UtilizationLimit = None
            RotationLimitRad = None
        }

    let private flangedJoint: FlangedJoint =
        {
            JointId = "joint-1"
            PrimarySide = { Geometry = jointSideGeometry; MaterialRole = componentMaterial.ComponentRole }
            MatingSideMode = IdenticalToPrimary
            MatingSide = None
            Gasket = gasketAssembly
            Bolting = boltingAssembly
            LoadCases = [ loadCase ]
            AcceptanceCriteria = [ acceptanceCriterion ]
            Materials = [ componentMaterial ]
        }

    [<Fact>]
    let ``acceptance criterion maps through explicit DTO without losing optional limits`` () =
        let criterion: AcceptanceCriterion =
            {
                CriterionId = "criterion-rotation"
                Level = Hard
                Source = Project
                Edition = Some "project-rev-a"
                Clause = Some "REQ-12"
                UtilizationLimit = Some 0.9M
                RotationLimitRad = Some 0.01
            }

        let dto = PersistenceMappers.acceptanceCriterionToDto criterion
        let actual = PersistenceMappers.acceptanceCriterionFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(criterion.CriterionId, mapped.CriterionId)
            Assert.Equal(criterion.Level, mapped.Level)
            Assert.Equal(criterion.Source, mapped.Source)
            Assert.Equal(criterion.Edition, mapped.Edition)
            Assert.Equal(criterion.Clause, mapped.Clause)
            Assert.Equal(criterion.UtilizationLimit, mapped.UtilizationLimit)
            Assert.Equal(criterion.RotationLimitRad, mapped.RotationLimitRad)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``joint load case maps through explicit DTO without losing signs`` () =
        let dto = PersistenceMappers.jointLoadCaseToDto loadCase
        let actual = PersistenceMappers.jointLoadCaseFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(loadCase.LoadCaseId, mapped.LoadCaseId)
            Assert.Equal(loadCase.Kind, mapped.Kind)
            Assert.Equal(loadCase.PrimaryCondition.PressurePa, mapped.PrimaryCondition.PressurePa)
            Assert.Equal(loadCase.MatingCondition.TemperatureK, mapped.MatingCondition.TemperatureK)
            Assert.Equal(loadCase.ExternalLoads.FxN, mapped.ExternalLoads.FxN)
            Assert.Equal(loadCase.ExternalLoads.FzN, mapped.ExternalLoads.FzN)
            Assert.Equal(loadCase.ExternalLoads.MyNm, mapped.ExternalLoads.MyNm)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``joint side geometry maps through explicit DTO without losing optional dimensions`` () =
        let dto = PersistenceMappers.jointSideGeometryToDto jointSideGeometry
        let actual = PersistenceMappers.jointSideGeometryFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(jointSideGeometry.SideId, mapped.SideId)
            Assert.Equal(jointSideGeometry.FlangeType, mapped.FlangeType)
            Assert.Equal(jointSideGeometry.Source, mapped.Source)
            Assert.Equal(jointSideGeometry.SeatType, mapped.SeatType)
            Assert.Equal(jointSideGeometry.Nominal.SeatOutsideDiameterM, mapped.Nominal.SeatOutsideDiameterM)
            Assert.Equal(jointSideGeometry.InternalSurface.CorrosionAllowanceM, mapped.InternalSurface.CorrosionAllowanceM)
            Assert.True(mapped.Hub.IsSome)
            Assert.Equal(jointSideGeometry.Hub.Value.Topology, mapped.Hub.Value.Topology)
            Assert.Equal(jointSideGeometry.Hub.Value.G1M, mapped.Hub.Value.G1M)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``bolting assembly maps through explicit DTO without losing optional areas and preload`` () =
        let dto = PersistenceMappers.boltingAssemblyToDto boltingAssembly
        let actual = PersistenceMappers.boltingAssemblyFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(boltingAssembly.AssemblyId, mapped.AssemblyId)
            Assert.Equal(boltingAssembly.Arrangement, mapped.Arrangement)
            Assert.Equal(boltingAssembly.Pattern.Count, mapped.Pattern.Count)
            Assert.Equal(boltingAssembly.Stud.ThreadingType, mapped.Stud.ThreadingType)
            Assert.Equal(boltingAssembly.Stud.Areas.ReducedShankAreaM2, mapped.Stud.Areas.ReducedShankAreaM2)
            Assert.Equal(boltingAssembly.ProjectAreaBasis, mapped.ProjectAreaBasis)
            Assert.Equal(boltingAssembly.TighteningMethod, mapped.TighteningMethod)
            Assert.Equal(boltingAssembly.Preload.TargetPreloadN, mapped.Preload.TargetPreloadN)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``gasket assembly maps through explicit DTO without losing zones and partition layout`` () =
        let dto = PersistenceMappers.gasketAssemblyToDto gasketAssembly
        let actual = PersistenceMappers.gasketAssemblyFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(gasketAssembly.AssemblyId, mapped.AssemblyId)
            Assert.Equal(gasketAssembly.Family, mapped.Family)
            Assert.Equal(gasketAssembly.Envelope.OutsideDiameterM, mapped.Envelope.OutsideDiameterM)
            Assert.Equal(gasketAssembly.SealingZones.Length, mapped.SealingZones.Length)
            Assert.Equal(gasketAssembly.SealingZones.Head.MinimumAverageContactPressurePa, mapped.SealingZones.Head.MinimumAverageContactPressurePa)
            Assert.True(mapped.PartitionLayout.IsSome)
            Assert.Equal(gasketAssembly.PartitionLayout.Value.PassCount, mapped.PartitionLayout.Value.PassCount)
            Assert.Equal(gasketAssembly.PartitionLayout.Value.Ribs.Head.EffectiveLengthM, mapped.PartitionLayout.Value.Ribs.Head.EffectiveLengthM)
            Assert.Equal(gasketAssembly.ProjectAreaBasis, mapped.ProjectAreaBasis)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``component material maps through explicit DTO without losing provenance and optional properties`` () =
        let dto = PersistenceMappers.componentMaterialToDto componentMaterial
        let actual = PersistenceMappers.componentMaterialFromDto dto

        Assert.Equal(componentMaterial.ComponentRole, actual.ComponentRole)
        Assert.Equal(componentMaterial.Material.Identity.MaterialId, actual.Material.Identity.MaterialId)
        Assert.Equal(componentMaterial.Material.Identity.ProductForm, actual.Material.Identity.ProductForm)
        Assert.Equal(componentMaterial.Material.ProviderRevision, actual.Material.ProviderRevision)
        Assert.Equal(componentMaterial.Material.SourceEdition, actual.Material.SourceEdition)
        Assert.Equal(componentMaterial.Material.Fingerprint, actual.Material.Fingerprint)
        Assert.Single(actual.Material.Properties) |> ignore
        Assert.Equal(componentMaterial.Material.Properties.Head.AllowableStressPa, actual.Material.Properties.Head.AllowableStressPa)
        Assert.Equal(componentMaterial.Material.Properties.Head.DensityKgPerM3, actual.Material.Properties.Head.DensityKgPerM3)

    [<Fact>]
    let ``flanged joint maps through reference DTO and resolves technical fragments`` () =
        let dto = PersistenceMappers.flangedJointToDto flangedJoint
        let actual =
            PersistenceMappers.flangedJointFromDto
                [ jointSideGeometry ]
                [ gasketAssembly ]
                [ boltingAssembly ]
                [ loadCase ]
                [ acceptanceCriterion ]
                [ componentMaterial ]
                dto

        match actual with
        | Ok mapped ->
            Assert.Equal(flangedJoint.JointId, mapped.JointId)
            Assert.Equal(flangedJoint.PrimarySide.Geometry.SideId, mapped.PrimarySide.Geometry.SideId)
            Assert.Equal(flangedJoint.MatingSideMode, mapped.MatingSideMode)
            Assert.Equal(flangedJoint.Gasket.AssemblyId, mapped.Gasket.AssemblyId)
            Assert.Equal(flangedJoint.Bolting.AssemblyId, mapped.Bolting.AssemblyId)
            Assert.Single(mapped.LoadCases) |> ignore
            Assert.Single(mapped.AcceptanceCriteria) |> ignore
            Assert.Single(mapped.Materials) |> ignore
            Assert.Empty(FlangedJoint.validateStructure mapped)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``project technical data DTO maps acceptance criteria load cases geometries bolting gaskets materials and joints`` () =
        let criteria = [ acceptanceCriterion ]

        let dto =
            PersistenceMappers.projectTechnicalDataToDto
                1
                criteria
                [ loadCase ]
                [ jointSideGeometry ]
                [ boltingAssembly ]
                [ gasketAssembly ]
                [ componentMaterial ]
                [ flangedJoint ]

        let actual = PersistenceMappers.projectTechnicalDataFromDto dto

        match actual with
        | Ok (mappedCriteria, mappedLoadCases, mappedGeometries, mappedBolting, mappedGaskets, mappedMaterials, mappedJoints) ->
            Assert.Equal(1, dto.SchemaVersion)
            Assert.Single(mappedCriteria) |> ignore
            Assert.Single(mappedLoadCases) |> ignore
            Assert.Single(mappedGeometries) |> ignore
            Assert.Single(mappedBolting) |> ignore
            Assert.Single(mappedGaskets) |> ignore
            Assert.Single(mappedMaterials) |> ignore
            Assert.Single(mappedJoints) |> ignore
            Assert.Equal(criteria.Head.CriterionId, mappedCriteria.Head.CriterionId)
            Assert.Equal(loadCase.LoadCaseId, mappedLoadCases.Head.LoadCaseId)
            Assert.Equal(jointSideGeometry.SideId, mappedGeometries.Head.SideId)
            Assert.Equal(boltingAssembly.AssemblyId, mappedBolting.Head.AssemblyId)
            Assert.Equal(gasketAssembly.AssemblyId, mappedGaskets.Head.AssemblyId)
            Assert.Equal(componentMaterial.ComponentRole, mappedMaterials.Head.ComponentRole)
            Assert.Equal(flangedJoint.JointId, mappedJoints.Head.JointId)
        | Result.Error message ->
            Assert.Fail(message)
