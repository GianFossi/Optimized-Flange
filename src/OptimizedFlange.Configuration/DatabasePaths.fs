namespace OptimizedFlange.Configuration

open System

/// <summary>Defines one configurable external database location.</summary>
type DatabaseLocation =
    {
        /// <summary>Stable database location identifier.</summary>
        Id: string
        /// <summary>Human-readable database name.</summary>
        Name: string
        /// <summary>Configured filesystem path.</summary>
        Path: string
        /// <summary>Whether this location participates in resolution.</summary>
        Enabled: bool
        /// <summary>Whether the location must be treated as read-only.</summary>
        ReadOnly: bool
        /// <summary>Lower values have higher resolution priority.</summary>
        Priority: int
        /// <summary>Last access timestamp if known.</summary>
        LastAccessedAt: DateTimeOffset option
        /// <summary>Optional content fingerprint.</summary>
        Fingerprint: string option
    }

/// <summary>Groups all configurable technical database locations.</summary>
type DatabasePathSettings =
    {
        /// <summary>Schema version of this document.</summary>
        SchemaVersion: int
        /// <summary>Optional root database folder.</summary>
        RootDatabaseFolder: string option
        /// <summary>Materials database locations.</summary>
        Materials: DatabaseLocation list
        /// <summary>Bolting database locations.</summary>
        Bolting: DatabaseLocation list
        /// <summary>Thread database locations.</summary>
        Threads: DatabaseLocation list
        /// <summary>Gasket database locations.</summary>
        Gaskets: DatabaseLocation list
        /// <summary>Tightening tool database locations.</summary>
        TighteningTools: DatabaseLocation list
        /// <summary>Validation/reference-case database locations.</summary>
        ValidationCases: DatabaseLocation list
        /// <summary>Additional project or user-defined database locations.</summary>
        Custom: DatabaseLocation list
    }
