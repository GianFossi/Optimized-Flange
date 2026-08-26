namespace OptimizedFlange.DataSources

open OptimizedFlange.Domain

/// <summary>Identifies the technical data family exposed by a source record.</summary>
type TechnicalDataCategory =
    | Materials
    | Bolting
    | Gaskets
    | GasketParameters
    | Facings
    | StandardFlanges
    | Ratings
    | Pipes
    | Tubes
    | NozzleLoads
    | Custom

/// <summary>Represents one scalar value as read from an external data source and optionally converted to SI.</summary>
type ImportedScalar =
    {
        /// <summary>Source field name.</summary>
        Name: string
        /// <summary>Raw numeric value from the source file.</summary>
        SourceValue: float
        /// <summary>Unit declared by the source file.</summary>
        SourceUnit: string option
        /// <summary>Converted SI value when the unit is recognized.</summary>
        SiValue: float option
        /// <summary>Canonical SI unit for the converted value.</summary>
        SiUnit: string option
    }

/// <summary>Represents one searchable imported database record.</summary>
type ImportedDataRecord =
    {
        /// <summary>Stable source identifier from the configured database location.</summary>
        SourceId: string
        /// <summary>Human-readable source name.</summary>
        SourceName: string
        /// <summary>Physical source path.</summary>
        SourcePath: string
        /// <summary>Technical data category.</summary>
        Category: TechnicalDataCategory
        /// <summary>Record identifier inside the source file.</summary>
        RecordId: string
        /// <summary>Primary display name for search and selection.</summary>
        DisplayName: string
        /// <summary>Optional family/type label from the source file.</summary>
        Family: string option
        /// <summary>Optional standard/specification label from the source file.</summary>
        Standard: string option
        /// <summary>Optional grade/class label from the source file.</summary>
        Grade: string option
        /// <summary>Imported scalar fields with source and SI units.</summary>
        Scalars: ImportedScalar list
        /// <summary>Additional searchable text tokens.</summary>
        Tags: string list
        /// <summary>Provenance chain for this imported record.</summary>
        Provenance: OptimizedFlange.Domain.ProvenanceEntry list
    }

/// <summary>Defines filters used when searching imported technical data records.</summary>
type DataRecordFilter =
    {
        /// <summary>Optional category filter.</summary>
        Category: TechnicalDataCategory option
        /// <summary>Optional source identifier filter.</summary>
        SourceId: string option
        /// <summary>Optional case-insensitive text filter.</summary>
        Text: string option
        /// <summary>Optional family/type filter.</summary>
        Family: string option
        /// <summary>Optional standard/specification filter.</summary>
        Standard: string option
        /// <summary>Optional grade/class filter.</summary>
        Grade: string option
        /// <summary>Optional scalar-name presence filter.</summary>
        HasScalar: string option
    }

/// <summary>Summary returned after loading a configured technical data location.</summary>
type DataSourceLoadSummary =
    {
        /// <summary>Loaded source identifier.</summary>
        SourceId: string
        /// <summary>Loaded path.</summary>
        Path: string
        /// <summary>Number of imported records.</summary>
        RecordCount: int
        /// <summary>Warnings observed during import.</summary>
        Warnings: string list
    }

/// <summary>Result of loading all enabled configured technical data sources.</summary>
type DataCatalog =
    {
        /// <summary>Loaded data records.</summary>
        Records: ImportedDataRecord list
        /// <summary>Per-source load summaries.</summary>
        Summaries: DataSourceLoadSummary list
    }
