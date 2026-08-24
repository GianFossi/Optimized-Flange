namespace OptimizedFlange.Domain

/// <summary>Identifies how the mating side of the joint is represented.</summary>
type MatingSideMode =
    | IdenticalToPrimary
    | ExplicitGeometry
    | ExternalEquivalent

/// <summary>Represents one physical side participating in the joint.</summary>
type JointSide =
    {
        /// <summary>Physical side geometry.</summary>
        Geometry: JointSideGeometry
        /// <summary>Stable material-role identifier used to resolve the side material.</summary>
        MaterialRole: string
    }

/// <summary>Represents a two-sided bolted and gasketed joint before any code-specific calculation.</summary>
type FlangedJoint =
    {
        /// <summary>Stable joint identifier.</summary>
        JointId: string
        /// <summary>Primary joint side.</summary>
        PrimarySide: JointSide
        /// <summary>How the mating side is represented.</summary>
        MatingSideMode: MatingSideMode
        /// <summary>Optional explicit mating side. Required by validation when the mode is ExplicitGeometry.</summary>
        MatingSide: JointSide option
        /// <summary>Gasket/sealing assembly owned by the joint.</summary>
        Gasket: GasketAssembly
        /// <summary>Common bolting assembly.</summary>
        Bolting: BoltingAssembly
        /// <summary>Physical project load cases.</summary>
        LoadCases: JointLoadCase list
        /// <summary>Acceptance criteria applied by selected calculation procedures.</summary>
        AcceptanceCriteria: AcceptanceCriterion list
        /// <summary>Resolved material snapshots consumed by the project.</summary>
        Materials: ComponentMaterial list
    }

/// <summary>Provides structural validation that is independent from normative ASME/API/PCC calculations.</summary>
module FlangedJoint =
    /// <summary>Returns domain-level consistency errors that can be detected without engineering formulas.</summary>
    let validateStructure (joint: FlangedJoint) =
        [
            if System.String.IsNullOrWhiteSpace joint.JointId then
                "JOINT.ID.REQUIRED"

            if joint.LoadCases.IsEmpty then
                "JOINT.LOAD_CASE.REQUIRED"

            if joint.Bolting.Pattern.Count <= 0 then
                "BOLTING.COUNT.POSITIVE_REQUIRED"

            if joint.Gasket.SealingZones.IsEmpty then
                "GASKET.SEALING_ZONE.REQUIRED"

            if joint.MatingSideMode = ExplicitGeometry && joint.MatingSide.IsNone then
                "JOINT.MATING_SIDE.EXPLICIT_GEOMETRY_REQUIRED"
        ]
