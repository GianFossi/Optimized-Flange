namespace OptimizedFlange.Domain

/// <summary>Identifies the engineering family of a load case.</summary>
type LoadCaseKind =
    | Design
    | Operating
    | Misoperation
    | Testing

/// <summary>Represents the signed six-component force/moment system acting at the joint reference plane.</summary>
type JointLoadVector =
    {
        /// <summary>Force along local +X in newtons.</summary>
        FxN: float<N>
        /// <summary>Force along local +Y in newtons.</summary>
        FyN: float<N>
        /// <summary>Force along local +Z, the flange axis, in newtons.</summary>
        FzN: float<N>
        /// <summary>Moment about local X in newton-metres.</summary>
        MxNm: float<N m>
        /// <summary>Moment about local Y in newton-metres.</summary>
        MyNm: float<N m>
        /// <summary>Torsional moment about local Z in newton-metres.</summary>
        MzNm: float<N m>
    }

/// <summary>Represents pressure and temperature conditions on one side of the joint.</summary>
type ComponentCondition =
    {
        /// <summary>Internal design or operating pressure in pascals.</summary>
        PressurePa: float<Pa>
        /// <summary>Component temperature in kelvin.</summary>
        TemperatureK: float<K>
    }

/// <summary>Represents one physical joint load case with side-specific conditions and one common external load system.</summary>
type JointLoadCase =
    {
        /// <summary>Stable load-case identifier.</summary>
        LoadCaseId: string
        /// <summary>Human-readable load-case name.</summary>
        Name: string
        /// <summary>Load-case family.</summary>
        Kind: LoadCaseKind
        /// <summary>Condition of the primary side.</summary>
        PrimaryCondition: ComponentCondition
        /// <summary>Condition of the mating side.</summary>
        MatingCondition: ComponentCondition
        /// <summary>Signed external force/moment system.</summary>
        ExternalLoads: JointLoadVector
    }
