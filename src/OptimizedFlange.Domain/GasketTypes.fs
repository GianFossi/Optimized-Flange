namespace OptimizedFlange.Domain

/// <summary>Identifies the gasket or sealing-system family.</summary>
type GasketFamily =
    | SpiralWound
    | Kammprofile
    | CorrugatedMetal
    | DoubleJacketed
    | SoftFlat
    | RingTypeJoint
    | Lens
    | WeldedSeal
    | CustomDesigned

/// <summary>Identifies the function of a sealing zone in the overall gasket assembly.</summary>
type SealingZoneRole =
    | PrimarySeal
    | SecondarySeal
    | PartitionSeal
    | SupportOnly
    | InformationalZone

/// <summary>Identifies the contact-area basis used for a non-normative gasket/bolting ratio policy.</summary>
type GasketAreaBasis =
    | PeripheralNominalSealingArea
    | TotalNominalSealingArea
    | EffectiveSealingArea

/// <summary>Represents one physical annular gasket envelope.</summary>
type GasketEnvelope =
    {
        /// <summary>Physical inside diameter in metres.</summary>
        InsideDiameterM: float<m>
        /// <summary>Physical outside diameter in metres.</summary>
        OutsideDiameterM: float<m>
        /// <summary>Installed gasket thickness in metres.</summary>
        ThicknessM: float<m>
    }

/// <summary>Represents one sealing or support zone independently from code-effective gasket geometry.</summary>
type SealingZone =
    {
        /// <summary>Stable zone identifier.</summary>
        ZoneId: string
        /// <summary>Zone function.</summary>
        Role: SealingZoneRole
        /// <summary>Whether failure of this zone is mandatory for the overall joint assessment.</summary>
        Mandatory: bool
        /// <summary>Nominal contact area in square metres.</summary>
        NominalAreaM2: float<m^2>
        /// <summary>Optional minimum allowable average contact pressure in pascals.</summary>
        MinimumAverageContactPressurePa: float<Pa> option
        /// <summary>Optional maximum allowable average contact pressure in pascals.</summary>
        MaximumAverageContactPressurePa: float<Pa> option
        /// <summary>Optional material or sealing-system reference.</summary>
        MaterialReferenceId: string option
    }

/// <summary>Represents one straight partition sealing rib.</summary>
type PartitionRib =
    {
        /// <summary>Stable rib identifier.</summary>
        RibId: string
        /// <summary>Rib offset from gasket centre in metres.</summary>
        OffsetM: float<m>
        /// <summary>Rib orientation in radians.</summary>
        OrientationRad: float
        /// <summary>Nominal rib width in metres.</summary>
        WidthM: float<m>
        /// <summary>Effective rib length in metres after geometry resolution.</summary>
        EffectiveLengthM: float<m> option
        /// <summary>Identifier of the sealing zone represented by the rib.</summary>
        SealingZoneId: string
    }

/// <summary>Represents the optional partition layout used by multipass exchanger joints.</summary>
type PartitionLayout =
    {
        /// <summary>Number of passes, restricted by project validation to the supported range.</summary>
        PassCount: int
        /// <summary>Individual partition ribs.</summary>
        Ribs: PartitionRib list
    }

/// <summary>Represents the complete gasket/sealing assembly independently from normative code transformations.</summary>
type GasketAssembly =
    {
        /// <summary>Stable assembly identifier.</summary>
        AssemblyId: string
        /// <summary>Sealing-system family.</summary>
        Family: GasketFamily
        /// <summary>Physical envelope.</summary>
        Envelope: GasketEnvelope
        /// <summary>Individual sealing/support zones.</summary>
        SealingZones: SealingZone list
        /// <summary>Optional partition layout.</summary>
        PartitionLayout: PartitionLayout option
        /// <summary>Whether an inner support ring is physically present.</summary>
        HasInnerRing: bool
        /// <summary>Whether an outer centering ring is physically present.</summary>
        HasOuterRing: bool
        /// <summary>Area basis used by the project gasket/bolting ratio policy.</summary>
        ProjectAreaBasis: GasketAreaBasis
    }
