namespace OptimizedFlange.Domain

/// <summary>Identifies a material independently from the external material-provider implementation.</summary>
type MaterialIdentity =
    {
        /// <summary>Stable material identifier.</summary>
        MaterialId: string
        /// <summary>Material specification, when applicable.</summary>
        Specification: string option
        /// <summary>Grade or type designation, when applicable.</summary>
        Grade: string option
        /// <summary>Product form, when applicable.</summary>
        ProductForm: string option
    }

/// <summary>Represents one resolved material-property set at a specific calculation condition.</summary>
type ResolvedMaterialProperties =
    {
        /// <summary>Temperature at which the properties were resolved.</summary>
        TemperatureK: float<K>
        /// <summary>Allowable stress in pascals, when applicable.</summary>
        AllowableStressPa: float<Pa> option
        /// <summary>Yield strength in pascals, when available.</summary>
        YieldStrengthPa: float<Pa> option
        /// <summary>Ultimate tensile strength in pascals, when available.</summary>
        UltimateStrengthPa: float<Pa> option
        /// <summary>Elastic modulus in pascals, when available.</summary>
        ElasticModulusPa: float<Pa> option
        /// <summary>Poisson ratio, when available.</summary>
        PoissonRatio: float option
        /// <summary>Mean coefficient of thermal expansion in inverse kelvin, when available.</summary>
        ThermalExpansionPerK: float option
        /// <summary>Density in kilograms per cubic metre, when available.</summary>
        DensityKgPerM3: float<kg/m^3> option
    }

/// <summary>Represents the reproducible snapshot of material data consumed by a project.</summary>
type MaterialSnapshot =
    {
        /// <summary>Material identity.</summary>
        Identity: MaterialIdentity
        /// <summary>Resolved property sets needed by the project's load cases.</summary>
        Properties: ResolvedMaterialProperties list
        /// <summary>External provider or repository identifier.</summary>
        ProviderId: string
        /// <summary>External data revision, commit, or release identifier.</summary>
        ProviderRevision: string option
        /// <summary>Optional standard/code edition associated with the property source.</summary>
        SourceEdition: string option
        /// <summary>Stable fingerprint of the consumed material snapshot.</summary>
        Fingerprint: string option
    }

/// <summary>Associates one project component role with its selected material snapshot.</summary>
type ComponentMaterial =
    {
        /// <summary>Stable component role such as primary flange, mating cover, internal bolting, or gasket zone.</summary>
        ComponentRole: string
        /// <summary>Selected material snapshot.</summary>
        Material: MaterialSnapshot
    }
