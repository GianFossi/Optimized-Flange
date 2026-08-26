namespace OptimizedFlange.Persistence

/// <summary>JSON DTO for one explicit project acceptance criterion.</summary>
[<CLIMutable>]
type AcceptanceCriterionDto =
    {
        /// <summary>Stable criterion identifier.</summary>
        CriterionId: string
        /// <summary>Requirement level identifier.</summary>
        Level: string
        /// <summary>Requirement source identifier.</summary>
        Source: string
        /// <summary>Optional source edition; null means unspecified.</summary>
        Edition: string | null
        /// <summary>Optional source clause; null means unspecified.</summary>
        Clause: string | null
        /// <summary>Optional utilization limit; null means unspecified.</summary>
        UtilizationLimit: System.Nullable<decimal>
        /// <summary>Optional rotation limit in radians; null means unspecified.</summary>
        RotationLimitRad: System.Nullable<float>
    }

/// <summary>JSON DTO for pressure and temperature on one side of a load case.</summary>
[<CLIMutable>]
type ComponentConditionDto =
    {
        /// <summary>Pressure in pascals.</summary>
        PressurePa: float
        /// <summary>Temperature in kelvin.</summary>
        TemperatureK: float
    }

/// <summary>JSON DTO for a signed six-component joint load vector.</summary>
[<CLIMutable>]
type JointLoadVectorDto =
    {
        /// <summary>Force along local +X in newtons.</summary>
        FxN: float
        /// <summary>Force along local +Y in newtons.</summary>
        FyN: float
        /// <summary>Force along local +Z in newtons.</summary>
        FzN: float
        /// <summary>Moment about local X in newton-metres.</summary>
        MxNm: float
        /// <summary>Moment about local Y in newton-metres.</summary>
        MyNm: float
        /// <summary>Moment about local Z in newton-metres.</summary>
        MzNm: float
    }

/// <summary>JSON DTO for one project load case.</summary>
[<CLIMutable>]
type JointLoadCaseDto =
    {
        /// <summary>Stable load-case identifier.</summary>
        LoadCaseId: string
        /// <summary>Human-readable load-case name.</summary>
        Name: string
        /// <summary>Load-case kind identifier.</summary>
        Kind: string
        /// <summary>Primary-side condition.</summary>
        PrimaryCondition: ComponentConditionDto
        /// <summary>Mating-side condition.</summary>
        MatingCondition: ComponentConditionDto
        /// <summary>Signed external load vector.</summary>
        ExternalLoads: JointLoadVectorDto
    }

/// <summary>JSON DTO for corrosion, overlay, machining, and manufacturing allowances on one surface.</summary>
[<CLIMutable>]
type SurfaceAllowanceDto =
    {
        /// <summary>Corrosion allowance in metres.</summary>
        CorrosionAllowanceM: float
        /// <summary>Finished weld-overlay thickness in metres.</summary>
        WeldOverlayThicknessM: float
        /// <summary>Machining allowance in metres.</summary>
        MachiningAllowanceM: float
        /// <summary>Minus manufacturing tolerance in metres.</summary>
        MinusToleranceM: float
        /// <summary>Plus manufacturing tolerance in metres.</summary>
        PlusToleranceM: float
    }

/// <summary>JSON DTO for nominal axisymmetric dimensions of one flange or flat-cover side.</summary>
[<CLIMutable>]
type NominalSideGeometryDto =
    {
        /// <summary>Bore diameter in metres.</summary>
        BoreDiameterM: float
        /// <summary>Outside diameter in metres.</summary>
        OutsideDiameterM: float
        /// <summary>Nominal flange or cover thickness in metres.</summary>
        ThicknessM: float
        /// <summary>Bolt-circle diameter in metres.</summary>
        BoltCircleDiameterM: float
        /// <summary>Optional raised-face or seat outside diameter in metres.</summary>
        SeatOutsideDiameterM: System.Nullable<float>
    }

/// <summary>JSON DTO for physical hub dimensions before any code-effective transformation.</summary>
[<CLIMutable>]
type PhysicalHubGeometryDto =
    {
        /// <summary>Hub topology identifier.</summary>
        Topology: string
        /// <summary>Small-end hub thickness in metres.</summary>
        G0M: System.Nullable<float>
        /// <summary>Intermediate hub thickness in metres.</summary>
        GMidM: System.Nullable<float>
        /// <summary>Large-end hub thickness in metres.</summary>
        G1M: System.Nullable<float>
        /// <summary>Total hub length in metres.</summary>
        LengthM: System.Nullable<float>
        /// <summary>Break location for a double-taper hub in metres.</summary>
        BreakLocationM: System.Nullable<float>
    }

/// <summary>JSON DTO for one physical joint side before normative geometry resolution.</summary>
[<CLIMutable>]
type JointSideGeometryDto =
    {
        /// <summary>Stable side identifier.</summary>
        SideId: string
        /// <summary>Physical flange type identifier.</summary>
        FlangeType: string
        /// <summary>Geometry source identifier.</summary>
        Source: string
        /// <summary>Seat topology identifier.</summary>
        SeatType: string
        /// <summary>Nominal side geometry.</summary>
        Nominal: NominalSideGeometryDto
        /// <summary>Optional physical hub geometry.</summary>
        Hub: PhysicalHubGeometryDto | null
        /// <summary>Internal-surface allowances.</summary>
        InternalSurface: SurfaceAllowanceDto
        /// <summary>Gasket-seat allowances.</summary>
        GasketSeat: SurfaceAllowanceDto
        /// <summary>External-surface allowances.</summary>
        ExternalSurface: SurfaceAllowanceDto
    }

/// <summary>JSON DTO for one circular bolt pattern.</summary>
[<CLIMutable>]
type BoltPatternDto =
    {
        /// <summary>Number of equally spaced bolts or studs.</summary>
        Count: int
        /// <summary>Bolt-circle diameter in metres.</summary>
        BoltCircleDiameterM: float
        /// <summary>Angular position of the first bolt in radians.</summary>
        StartAngleRad: float
    }

/// <summary>JSON DTO for principal stud or bolt section areas.</summary>
[<CLIMutable>]
type BoltSectionAreasDto =
    {
        /// <summary>Nominal shank area in square metres.</summary>
        NominalShankAreaM2: float
        /// <summary>Tensile-stress area in square metres.</summary>
        TensileStressAreaM2: float
        /// <summary>Minimum-root area in square metres.</summary>
        MinimumRootAreaM2: float
        /// <summary>Optional reduced-shank area in square metres.</summary>
        ReducedShankAreaM2: System.Nullable<float>
        /// <summary>Governing effective resisting area in square metres.</summary>
        GoverningResistingAreaM2: float
    }

/// <summary>JSON DTO for a physical stud or bolt definition.</summary>
[<CLIMutable>]
type StudDefinitionDto =
    {
        /// <summary>Nominal diameter in metres.</summary>
        NominalDiameterM: float
        /// <summary>Thread pitch in metres.</summary>
        PitchM: float
        /// <summary>Thread standard identifier.</summary>
        ThreadStandard: string
        /// <summary>Threading construction identifier.</summary>
        ThreadingType: string
        /// <summary>Section areas.</summary>
        Areas: BoltSectionAreasDto
        /// <summary>Optional specified total stud length in metres.</summary>
        SpecifiedLengthM: System.Nullable<float>
    }

/// <summary>JSON DTO for physical preload bounds and target.</summary>
[<CLIMutable>]
type PreloadDefinitionDto =
    {
        /// <summary>Minimum assembly preload in newtons.</summary>
        MinimumPreloadN: System.Nullable<float>
        /// <summary>Target assembly preload in newtons.</summary>
        TargetPreloadN: System.Nullable<float>
        /// <summary>Maximum assembly preload in newtons.</summary>
        MaximumPreloadN: System.Nullable<float>
    }

/// <summary>JSON DTO for one complete bolting assembly.</summary>
[<CLIMutable>]
type BoltingAssemblyDto =
    {
        /// <summary>Stable assembly identifier.</summary>
        AssemblyId: string
        /// <summary>Physical arrangement identifier.</summary>
        Arrangement: string
        /// <summary>Circular bolt pattern.</summary>
        Pattern: BoltPatternDto
        /// <summary>Common stud definition.</summary>
        Stud: StudDefinitionDto
        /// <summary>Project area basis identifier.</summary>
        ProjectAreaBasis: string
        /// <summary>Tightening method identifier.</summary>
        TighteningMethod: string
        /// <summary>Preload bounds and target.</summary>
        Preload: PreloadDefinitionDto
    }

/// <summary>JSON DTO for one physical annular gasket envelope.</summary>
[<CLIMutable>]
type GasketEnvelopeDto =
    {
        /// <summary>Physical inside diameter in metres.</summary>
        InsideDiameterM: float
        /// <summary>Physical outside diameter in metres.</summary>
        OutsideDiameterM: float
        /// <summary>Installed gasket thickness in metres.</summary>
        ThicknessM: float
    }

/// <summary>JSON DTO for one sealing or support zone.</summary>
[<CLIMutable>]
type SealingZoneDto =
    {
        /// <summary>Stable zone identifier.</summary>
        ZoneId: string
        /// <summary>Zone role identifier.</summary>
        Role: string
        /// <summary>Whether failure of this zone is mandatory for the overall joint assessment.</summary>
        Mandatory: bool
        /// <summary>Nominal contact area in square metres.</summary>
        NominalAreaM2: float
        /// <summary>Optional minimum average contact pressure in pascals.</summary>
        MinimumAverageContactPressurePa: System.Nullable<float>
        /// <summary>Optional maximum average contact pressure in pascals.</summary>
        MaximumAverageContactPressurePa: System.Nullable<float>
        /// <summary>Optional material or sealing-system reference.</summary>
        MaterialReferenceId: string | null
    }

/// <summary>JSON DTO for one straight partition sealing rib.</summary>
[<CLIMutable>]
type PartitionRibDto =
    {
        /// <summary>Stable rib identifier.</summary>
        RibId: string
        /// <summary>Rib offset from gasket centre in metres.</summary>
        OffsetM: float
        /// <summary>Rib orientation in radians.</summary>
        OrientationRad: float
        /// <summary>Nominal rib width in metres.</summary>
        WidthM: float
        /// <summary>Optional effective rib length in metres.</summary>
        EffectiveLengthM: System.Nullable<float>
        /// <summary>Identifier of the sealing zone represented by the rib.</summary>
        SealingZoneId: string
    }

/// <summary>JSON DTO for optional partition layout.</summary>
[<CLIMutable>]
type PartitionLayoutDto =
    {
        /// <summary>Number of passes.</summary>
        PassCount: int
        /// <summary>Individual partition ribs.</summary>
        Ribs: PartitionRibDto array
    }

/// <summary>JSON DTO for one complete gasket or sealing assembly.</summary>
[<CLIMutable>]
type GasketAssemblyDto =
    {
        /// <summary>Stable assembly identifier.</summary>
        AssemblyId: string
        /// <summary>Gasket family identifier.</summary>
        Family: string
        /// <summary>Physical gasket envelope.</summary>
        Envelope: GasketEnvelopeDto
        /// <summary>Individual sealing/support zones.</summary>
        SealingZones: SealingZoneDto array
        /// <summary>Optional partition layout.</summary>
        PartitionLayout: PartitionLayoutDto | null
        /// <summary>Whether an inner support ring is physically present.</summary>
        HasInnerRing: bool
        /// <summary>Whether an outer centering ring is physically present.</summary>
        HasOuterRing: bool
        /// <summary>Optional selected gasket operating factor.</summary>
        SelectedGasketM: System.Nullable<float>
        /// <summary>Optional selected gasket seating stress in pascals.</summary>
        SelectedGasketYPa: System.Nullable<float>
        /// <summary>Project gasket area basis identifier.</summary>
        ProjectAreaBasis: string
    }

/// <summary>JSON DTO for material identity resolved by an external provider.</summary>
[<CLIMutable>]
type MaterialIdentityDto =
    {
        /// <summary>Stable material identifier.</summary>
        MaterialId: string
        /// <summary>Optional material specification.</summary>
        Specification: string | null
        /// <summary>Optional material grade.</summary>
        Grade: string | null
        /// <summary>Optional product form.</summary>
        ProductForm: string | null
    }

/// <summary>JSON DTO for one resolved material-property set.</summary>
[<CLIMutable>]
type ResolvedMaterialPropertiesDto =
    {
        /// <summary>Temperature at which properties were resolved, in kelvin.</summary>
        TemperatureK: float
        /// <summary>Optional allowable stress in pascals.</summary>
        AllowableStressPa: System.Nullable<float>
        /// <summary>Optional yield strength in pascals.</summary>
        YieldStrengthPa: System.Nullable<float>
        /// <summary>Optional ultimate tensile strength in pascals.</summary>
        UltimateStrengthPa: System.Nullable<float>
        /// <summary>Optional elastic modulus in pascals.</summary>
        ElasticModulusPa: System.Nullable<float>
        /// <summary>Optional Poisson ratio.</summary>
        PoissonRatio: System.Nullable<float>
        /// <summary>Optional mean coefficient of thermal expansion in inverse kelvin.</summary>
        ThermalExpansionPerK: System.Nullable<float>
        /// <summary>Optional density in kilograms per cubic metre.</summary>
        DensityKgPerM3: System.Nullable<float>
    }

/// <summary>JSON DTO for a reproducible material snapshot consumed by a project.</summary>
[<CLIMutable>]
type MaterialSnapshotDto =
    {
        /// <summary>Material identity.</summary>
        Identity: MaterialIdentityDto
        /// <summary>Resolved property sets.</summary>
        Properties: ResolvedMaterialPropertiesDto array
        /// <summary>External provider or repository identifier.</summary>
        ProviderId: string
        /// <summary>Optional external provider revision.</summary>
        ProviderRevision: string | null
        /// <summary>Optional source standard or code edition.</summary>
        SourceEdition: string | null
        /// <summary>Optional stable fingerprint.</summary>
        Fingerprint: string | null
    }

/// <summary>JSON DTO associating one component role with a material snapshot.</summary>
[<CLIMutable>]
type ComponentMaterialDto =
    {
        /// <summary>Stable component role.</summary>
        ComponentRole: string
        /// <summary>Selected material snapshot.</summary>
        Material: MaterialSnapshotDto
    }

/// <summary>JSON DTO for one joint side reference inside a flanged-joint composition.</summary>
[<CLIMutable>]
type JointSideReferenceDto =
    {
        /// <summary>Referenced physical side geometry identifier.</summary>
        GeometrySideId: string
        /// <summary>Stable material role used by the side.</summary>
        MaterialRole: string
    }

/// <summary>JSON DTO for a two-sided flanged-joint composition referencing technical data fragments.</summary>
[<CLIMutable>]
type FlangedJointDto =
    {
        /// <summary>Stable joint identifier.</summary>
        JointId: string
        /// <summary>Primary joint side reference.</summary>
        PrimarySide: JointSideReferenceDto
        /// <summary>Mating-side representation mode identifier.</summary>
        MatingSideMode: string
        /// <summary>Optional explicit mating-side reference.</summary>
        MatingSide: JointSideReferenceDto | null
        /// <summary>Referenced gasket assembly identifier.</summary>
        GasketAssemblyId: string
        /// <summary>Referenced bolting assembly identifier.</summary>
        BoltingAssemblyId: string
        /// <summary>Referenced load-case identifiers.</summary>
        LoadCaseIds: string array
        /// <summary>Referenced acceptance-criterion identifiers.</summary>
        AcceptanceCriterionIds: string array
        /// <summary>Referenced component material roles.</summary>
        ComponentMaterialRoles: string array
    }

/// <summary>Versioned JSON DTO for technical project data owned by an OptimizedFlange project.</summary>
[<CLIMutable>]
type ProjectTechnicalDataDto =
    {
        /// <summary>Technical data schema version.</summary>
        SchemaVersion: int
        /// <summary>Explicit project acceptance criteria.</summary>
        AcceptanceCriteria: AcceptanceCriterionDto array
        /// <summary>Physical project load cases.</summary>
        LoadCases: JointLoadCaseDto array
        /// <summary>Physical joint-side geometries.</summary>
        JointSideGeometries: JointSideGeometryDto array
        /// <summary>Physical bolting assemblies.</summary>
        BoltingAssemblies: BoltingAssemblyDto array
        /// <summary>Physical gasket assemblies.</summary>
        GasketAssemblies: GasketAssemblyDto array
        /// <summary>Component material snapshots.</summary>
        ComponentMaterials: ComponentMaterialDto array
        /// <summary>Flanged-joint compositions.</summary>
        FlangedJoints: FlangedJointDto array
    }
