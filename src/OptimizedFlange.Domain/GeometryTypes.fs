namespace OptimizedFlange.Domain

/// <summary>Identifies the source from which flange geometry originates.</summary>
type FlangeGeometrySource =
    | CustomDesigned
    | StandardDerived
    | Imported

/// <summary>Identifies the physical flange topology represented by a side of the joint.</summary>
type FlangeType =
    | IntegralFlange
    | FlatCover

/// <summary>Identifies the gasket-seat topology on a joint side.</summary>
type SeatType =
    | FlatFace
    | RaisedFace
    | RecessedFace
    | TongueAndGroove
    | MaleFemale
    | RingTypeJointGroove
    | LensSeat
    | CustomSeat

/// <summary>Identifies the physical hub topology. Code-effective hub classification is resolved separately.</summary>
type HubTopology =
    | NoHub
    | StraightHub
    | SingleTaperHub
    | DoubleTaperHub

/// <summary>Identifies which geometry state is required by a calculation or check.</summary>
type GeometryStateBasis =
    | AsBuilt
    | ManufacturingMinimum
    | Corroded
    | CodeEffective

/// <summary>Represents corrosion, overlay, machining, and manufacturing allowances for one surface zone.</summary>
type SurfaceAllowance =
    {
        /// <summary>Corrosion allowance in metres.</summary>
        CorrosionAllowanceM: float<m>
        /// <summary>Finished weld-overlay thickness in metres.</summary>
        WeldOverlayThicknessM: float<m>
        /// <summary>Machining allowance in metres.</summary>
        MachiningAllowanceM: float<m>
        /// <summary>Minus manufacturing tolerance in metres.</summary>
        MinusToleranceM: float<m>
        /// <summary>Plus manufacturing tolerance in metres.</summary>
        PlusToleranceM: float<m>
    }

/// <summary>Represents the nominal axisymmetric dimensions of one flange or flat-cover side.</summary>
type NominalSideGeometry =
    {
        /// <summary>Inside or bore diameter in metres.</summary>
        BoreDiameterM: float<m>
        /// <summary>Outside diameter in metres.</summary>
        OutsideDiameterM: float<m>
        /// <summary>Nominal flange or cover thickness in metres.</summary>
        ThicknessM: float<m>
        /// <summary>Nominal bolt-circle diameter in metres.</summary>
        BoltCircleDiameterM: float<m>
        /// <summary>Optional raised-face or seat outside diameter in metres.</summary>
        SeatOutsideDiameterM: float<m> option
    }

/// <summary>Represents physical hub dimensions independently from any code-effective transformation.</summary>
type PhysicalHubGeometry =
    {
        /// <summary>Hub topology.</summary>
        Topology: HubTopology
        /// <summary>Small-end hub thickness g0 in metres.</summary>
        G0M: float<m> option
        /// <summary>Intermediate thickness used by double-taper hubs in metres.</summary>
        GMidM: float<m> option
        /// <summary>Large-end hub thickness g1 in metres.</summary>
        G1M: float<m> option
        /// <summary>Total hub length in metres.</summary>
        LengthM: float<m> option
        /// <summary>Break location for a double-taper hub, measured from the small end, in metres.</summary>
        BreakLocationM: float<m> option
    }

/// <summary>Represents one physical joint side before normative geometry resolution.</summary>
type JointSideGeometry =
    {
        /// <summary>Stable identifier of the side.</summary>
        SideId: string
        /// <summary>Physical type of the side.</summary>
        FlangeType: FlangeType
        /// <summary>Geometry source.</summary>
        Source: FlangeGeometrySource
        /// <summary>Seat topology.</summary>
        SeatType: SeatType
        /// <summary>Nominal axisymmetric geometry.</summary>
        Nominal: NominalSideGeometry
        /// <summary>Optional physical hub geometry.</summary>
        Hub: PhysicalHubGeometry option
        /// <summary>Internal-surface allowances.</summary>
        InternalSurface: SurfaceAllowance
        /// <summary>Gasket-seat allowances.</summary>
        GasketSeat: SurfaceAllowance
        /// <summary>External-surface allowances.</summary>
        ExternalSurface: SurfaceAllowance
    }
