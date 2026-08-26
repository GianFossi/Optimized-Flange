namespace OptimizedFlange.Calculations

open System
open OptimizedFlange.Domain

/// <summary>Input data for the IOGP S-614 paragraph 7.8.10 floating-head pressure-effect bolt-stress check.</summary>
type IogpS614FloatingHeadPressureEffectInput =
    {
        /// <summary>Minimum required gasket stress for the floating-head gasket in pascals.</summary>
        MinimumGasketStressPa: float<Pa>
        /// <summary>Floating-head gasket area in square metres.</summary>
        FloatingHeadGasketAreaM2: float<m^2>
        /// <summary>Floating-head gasket inside diameter in metres.</summary>
        FloatingHeadGasketInsideDiameterM: float<m>
        /// <summary>Floating-head outside diameter in metres.</summary>
        FloatingHeadOutsideDiameterM: float<m>
        /// <summary>Pressure acting on the tube side in pascals.</summary>
        TubeSidePressurePa: float<Pa>
        /// <summary>Pressure acting on the shell side in pascals.</summary>
        ShellSidePressurePa: float<Pa>
        /// <summary>Gasket relaxation or assembly factor used by the project for this check.</summary>
        GasketFactor: float
        /// <summary>Number of bolts participating in the floating-head cover joint.</summary>
        BoltCount: int
        /// <summary>Single-bolt root area in square metres.</summary>
        BoltRootAreaM2: float<m^2>
    }

/// <summary>Result data for the IOGP S-614 paragraph 7.8.10 floating-head pressure-effect bolt-stress check.</summary>
type IogpS614FloatingHeadPressureEffectResult =
    {
        /// <summary>Required selected assembly bolt stress in pascals.</summary>
        RequiredSelectedAssemblyBoltStressPa: float<Pa>
        /// <summary>Pressure resultant contribution in newtons.</summary>
        PressureResultantN: float<N>
        /// <summary>Gasket seating contribution in newtons.</summary>
        GasketContributionN: float<N>
    }

/// <summary>Implements currently traceable IOGP S-614 paragraph 7.8 formulas.</summary>
module IogpS614Paragraph78 =
    /// <summary>
    /// Calculates the minimum selected assembly bolt stress required by IOGP S-614 v18-12 paragraph 7.8.10 Equation (3).
    /// </summary>
    let requiredSelectedAssemblyBoltStress
        (input: IogpS614FloatingHeadPressureEffectInput)
        : Result<IogpS614FloatingHeadPressureEffectResult, string list> =
        let errors =
            [
                if input.MinimumGasketStressPa < 0.0<Pa> then
                    "Minimum gasket stress must be non-negative."
                if input.FloatingHeadGasketAreaM2 <= 0.0<m^2> then
                    "Floating-head gasket area must be positive."
                if input.FloatingHeadGasketInsideDiameterM <= 0.0<m> then
                    "Floating-head gasket inside diameter must be positive."
                if input.FloatingHeadOutsideDiameterM <= 0.0<m> then
                    "Floating-head outside diameter must be positive."
                if input.GasketFactor <= 0.0 then
                    "Gasket factor must be positive."
                if input.BoltCount <= 0 then
                    "Bolt count must be positive."
                if input.BoltRootAreaM2 <= 0.0<m^2> then
                    "Bolt root area must be positive."
            ]

        match errors with
        | _ :: _ -> Result.Error errors
        | [] ->
            let quarterPi = Math.PI / 4.0
            let gasketContribution =
                float input.MinimumGasketStressPa * float input.FloatingHeadGasketAreaM2

            let tubeSidePressureArea =
                quarterPi
                * float input.FloatingHeadGasketInsideDiameterM
                * float input.FloatingHeadGasketInsideDiameterM

            let shellSidePressureArea =
                quarterPi
                * float input.FloatingHeadOutsideDiameterM
                * float input.FloatingHeadOutsideDiameterM

            let pressureResultant =
                (tubeSidePressureArea * float input.TubeSidePressurePa)
                - (shellSidePressureArea * float input.ShellSidePressurePa)

            let boltArea = float input.BoltCount * float input.BoltRootAreaM2
            let requiredStress =
                (gasketContribution + pressureResultant)
                / (input.GasketFactor * boltArea)

            Ok
                {
                    RequiredSelectedAssemblyBoltStressPa = LanguagePrimitives.FloatWithMeasure<Pa> requiredStress
                    PressureResultantN = LanguagePrimitives.FloatWithMeasure<N> pressureResultant
                    GasketContributionN = LanguagePrimitives.FloatWithMeasure<N> gasketContribution
                }

    /// <summary>Checks whether the selected assembly bolt stress satisfies IOGP S-614 v18-12 paragraph 7.8.10 Equation (3).</summary>
    let selectedAssemblyBoltStressIsSufficient
        (selectedAssemblyBoltStressPa: float<Pa>)
        (input: IogpS614FloatingHeadPressureEffectInput)
        : Result<bool * IogpS614FloatingHeadPressureEffectResult, string list> =
        if selectedAssemblyBoltStressPa < 0.0<Pa> then
            Result.Error [ "Selected assembly bolt stress must be non-negative." ]
        else
            requiredSelectedAssemblyBoltStress input
            |> Result.map (fun result ->
                selectedAssemblyBoltStressPa >= result.RequiredSelectedAssemblyBoltStressPa, result)
