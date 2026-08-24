namespace OptimizedFlange.Domain

/// <summary>Identifies the semantic role of a recorded calculation quantity.</summary>
type CalculationQuantityRole =
    | Input
    | DerivedInput
    | Intermediate
    | Limit
    | Result
    | Diagnostic

/// <summary>Represents one named scalar value recorded for calculation traceability.</summary>
type TraceQuantity =
    {
        /// <summary>Stable quantity identifier.</summary>
        QuantityId: string
        /// <summary>Quantity role in the check or calculation.</summary>
        Role: CalculationQuantityRole
        /// <summary>Canonical numeric value in SI units or dimensionless form.</summary>
        CanonicalValue: decimal
        /// <summary>Canonical unit symbol, when the quantity is dimensional.</summary>
        Unit: string option
        /// <summary>Original source engineering value, when the quantity came directly from project data.</summary>
        SourceValueId: string option
        /// <summary>Optional notes for non-normative diagnostics or implementation traceability.</summary>
        Notes: string option
    }

/// <summary>Represents a dependency on a project input, external-data snapshot, option, or rule.</summary>
type CalculationDependency =
    {
        /// <summary>Stable dependency identifier.</summary>
        DependencyId: string
        /// <summary>Dependency category such as project input, material snapshot, option, or rule.</summary>
        DependencyKind: string
        /// <summary>Optional fingerprint that should change when this dependency changes materially.</summary>
        Fingerprint: string option
    }

/// <summary>Represents trace data captured while evaluating one check or calculation path.</summary>
type CalculationTrace =
    {
        /// <summary>Inputs, intermediate values, limits, results, and diagnostics recorded by the calculation.</summary>
        Quantities: TraceQuantity list
        /// <summary>Dependencies that affect reproducibility or invalidation.</summary>
        Dependencies: CalculationDependency list
    }
