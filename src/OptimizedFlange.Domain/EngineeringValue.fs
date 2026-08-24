namespace OptimizedFlange.Domain

/// <summary>Represents a canonical engineering value together with its original input and provenance.</summary>
type EngineeringValue =
    {
        /// <summary>Stable semantic identifier of the engineering value.</summary>
        Id: string
        /// <summary>Canonical numeric value used by calculations, expressed in the canonical SI unit for the value kind.</summary>
        CanonicalValue: decimal
        /// <summary>Original textual value entered or imported before normalization.</summary>
        OriginalInput: string option
        /// <summary>Original unit symbol associated with the input.</summary>
        OriginalUnit: string option
        /// <summary>Source category for this value.</summary>
        Source: EngineeringValueSource
        /// <summary>Ordered provenance chain.</summary>
        Provenance: ProvenanceEntry list
    }
