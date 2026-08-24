namespace OptimizedFlange.Domain

/// <summary>Identifies the origin of an engineering or configuration value.</summary>
type EngineeringValueSource =
    | BuiltInDefault
    | UserDefault
    | ProjectValue
    | StandardDerived
    | Imported
    | Migrated

/// <summary>Records one step in the provenance chain of a value.</summary>
type ProvenanceEntry =
    {
        /// <summary>Stable source identifier.</summary>
        SourceId: string
        /// <summary>Human-readable source description.</summary>
        Description: string
        /// <summary>Optional source revision, edition, or commit identifier.</summary>
        Revision: string option
    }
