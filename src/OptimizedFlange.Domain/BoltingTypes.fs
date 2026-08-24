namespace OptimizedFlange.Domain

/// <summary>Identifies the physical bolting arrangement.</summary>
type BoltArrangement =
    | ThroughStuds
    | BlindTappedStuds

/// <summary>Identifies the thread standard family used to resolve thread geometry.</summary>
type ThreadStandard =
    | AsmeB11UnifiedInch
    | AsmeB113Metric
    | ProjectDefinedThread

/// <summary>Identifies the bolting area basis used by a requirement or project policy.</summary>
type BoltAreaBasis =
    | TensileStressArea
    | MinimumRootArea
    | GoverningResistingArea

/// <summary>Identifies the longitudinal construction of a stud.</summary>
type StudThreadingType =
    | FullyThreaded
    | PartiallyThreaded
    | ReducedShank

/// <summary>Identifies the tightening method selected for the bolted joint.</summary>
type TighteningMethod =
    | ManualOrUncontrolled
    | TorqueControlled
    | HydraulicTorque
    | HydraulicTensioning
    | ElongationControlled
    | TurnOfNut
    | TorqueAndTurn
    | UserDefinedTightening

/// <summary>Represents one circular bolt pattern.</summary>
type BoltPattern =
    {
        /// <summary>Number of equally spaced bolts or studs.</summary>
        Count: int
        /// <summary>Bolt-circle diameter in metres.</summary>
        BoltCircleDiameterM: float<m>
        /// <summary>Angular position of the first bolt in radians, measured CCW from local +X while looking along +Z.</summary>
        StartAngleRad: float
    }

/// <summary>Represents the principal section areas of one stud or bolt.</summary>
type BoltSectionAreas =
    {
        /// <summary>Nominal shank area in square metres.</summary>
        NominalShankAreaM2: float<m^2>
        /// <summary>Tensile-stress area in square metres.</summary>
        TensileStressAreaM2: float<m^2>
        /// <summary>Minimum-root area in square metres.</summary>
        MinimumRootAreaM2: float<m^2>
        /// <summary>Optional reduced-shank area in square metres.</summary>
        ReducedShankAreaM2: float<m^2> option
        /// <summary>Governing effective resisting area in square metres when the applicable rule permits it.</summary>
        GoverningResistingAreaM2: float<m^2>
    }

/// <summary>Represents the physical stud or bolt definition independently from code sizing equations.</summary>
type StudDefinition =
    {
        /// <summary>Nominal diameter in metres.</summary>
        NominalDiameterM: float<m>
        /// <summary>Thread pitch in metres.</summary>
        PitchM: float<m>
        /// <summary>Thread-standard family.</summary>
        ThreadStandard: ThreadStandard
        /// <summary>Threading construction.</summary>
        ThreadingType: StudThreadingType
        /// <summary>Calculated or catalogued section areas.</summary>
        Areas: BoltSectionAreas
        /// <summary>Optional specified total stud length in metres.</summary>
        SpecifiedLengthM: float<m> option
    }

/// <summary>Represents physical preload bounds and target for a bolting assembly.</summary>
type PreloadDefinition =
    {
        /// <summary>Minimum assembly preload in newtons.</summary>
        MinimumPreloadN: float<N> option
        /// <summary>Target assembly preload in newtons.</summary>
        TargetPreloadN: float<N> option
        /// <summary>Maximum assembly preload in newtons.</summary>
        MaximumPreloadN: float<N> option
    }

/// <summary>Represents the complete bolting assembly at domain level.</summary>
type BoltingAssembly =
    {
        /// <summary>Stable assembly identifier.</summary>
        AssemblyId: string
        /// <summary>Physical arrangement.</summary>
        Arrangement: BoltArrangement
        /// <summary>Circular bolt pattern.</summary>
        Pattern: BoltPattern
        /// <summary>Common stud definition used by the pattern.</summary>
        Stud: StudDefinition
        /// <summary>Area basis selected by the project for non-normative ratio policies.</summary>
        ProjectAreaBasis: BoltAreaBasis
        /// <summary>Selected tightening method.</summary>
        TighteningMethod: TighteningMethod
        /// <summary>Preload bounds and target.</summary>
        Preload: PreloadDefinition
    }
