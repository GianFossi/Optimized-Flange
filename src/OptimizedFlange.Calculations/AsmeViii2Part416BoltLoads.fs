namespace OptimizedFlange.Calculations

open System
open OptimizedFlange.Domain

/// <summary>Input data for ASME VIII Division 2 Part 4.16 basic bolt-load helper formulas.</summary>
type AsmeViii2Part416BoltLoadInput =
    {
        /// <summary>Design pressure in pascals. Use a non-negative absolute pressure for this helper.</summary>
        PressurePa: float<Pa>
        /// <summary>Diameter at the gasket load reaction in metres.</summary>
        GasketReactionDiameterM: float<m>
        /// <summary>Effective gasket contact width in metres.</summary>
        EffectiveGasketWidthM: float<m>
        /// <summary>Gasket factor for the operating condition.</summary>
        GasketM: float
        /// <summary>Gasket seating stress in pascals.</summary>
        GasketYPa: float<Pa>
        /// <summary>True when the gasket is self-energizing for the applicable ASME helper equation.</summary>
        SelfEnergizing: bool
        /// <summary>Optional externally determined axial seating force for self-energizing gaskets.</summary>
        SelfEnergizingSeatingForceN: float<N> option
    }

/// <summary>Result data for ASME VIII Division 2 Part 4.16 basic bolt-load helper formulas.</summary>
type AsmeViii2Part416BoltLoadResult =
    {
        /// <summary>Operating-condition design bolt load in newtons.</summary>
        OperatingBoltLoadN: float<N>
        /// <summary>Required gasket seating load component in newtons.</summary>
        GasketSeatingLoadN: float<N>
    }

/// <summary>Implements currently traceable ASME VIII Division 2 Part 4.16 basic bolt-load helper formulas.</summary>
module AsmeViii2Part416BoltLoads =
    /// <summary>Calculates ASME VIII Division 2 2025 Part 4.16 operating and gasket seating helper loads.</summary>
    let calculate (input: AsmeViii2Part416BoltLoadInput) : Result<AsmeViii2Part416BoltLoadResult, string list> =
        let errors =
            [
                if input.PressurePa < 0.0<Pa> then
                    "Pressure must be non-negative."
                if input.GasketReactionDiameterM <= 0.0<m> then
                    "Gasket reaction diameter must be positive."
                if input.EffectiveGasketWidthM < 0.0<m> then
                    "Effective gasket width must be non-negative."
                if input.GasketM < 0.0 then
                    "Gasket operating factor must be non-negative."
                if input.GasketYPa < 0.0<Pa> then
                    "Gasket seating stress must be non-negative."
                match input.SelfEnergizingSeatingForceN with
                | Some force when force < 0.0<N> -> "Self-energizing seating force must be non-negative."
                | _ -> ()
            ]

        match errors with
        | _ :: _ -> Result.Error errors
        | [] ->
            let reactionDiameter = float input.GasketReactionDiameterM
            let pressure = float input.PressurePa
            let hydrostaticEndLoad = 0.785 * reactionDiameter * reactionDiameter * pressure

            let operatingBoltLoad =
                if input.SelfEnergizing then
                    hydrostaticEndLoad
                else
                    hydrostaticEndLoad
                    + (2.0 * float input.EffectiveGasketWidthM * Math.PI * reactionDiameter * input.GasketM * pressure)

            let gasketSeatingLoad =
                if input.SelfEnergizing then
                    input.SelfEnergizingSeatingForceN |> Option.defaultValue 0.0<N> |> float
                else
                    Math.PI
                    * float input.EffectiveGasketWidthM
                    * reactionDiameter
                    * float input.GasketYPa

            Ok
                {
                    OperatingBoltLoadN = LanguagePrimitives.FloatWithMeasure<N> operatingBoltLoad
                    GasketSeatingLoadN = LanguagePrimitives.FloatWithMeasure<N> gasketSeatingLoad
                }
