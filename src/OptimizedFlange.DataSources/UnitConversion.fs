namespace OptimizedFlange.DataSources

/// <summary>Converts known external database units into canonical SI values.</summary>
module UnitConversion =
    /// <summary>Converts a scalar to SI when the source unit is recognized.</summary>
    let toSi (unitName: string option) (value: float) : (float * string) option =
        match unitName |> Option.map (fun unit -> unit.Trim().ToLowerInvariant()) with
        | Some "mm" -> Some(value / 1000.0, "m")
        | Some "mm2" | Some "mm^2" | Some "mm²" -> Some(value / 1_000_000.0, "m2")
        | Some "bar" -> Some(value * 100_000.0, "Pa")
        | Some "mpa" -> Some(value * 1_000_000.0, "Pa")
        | Some "psi" -> Some(value * 6894.757293168, "Pa")
        | Some "c" | Some "°c" | Some "degc" | Some "celsius" -> Some(value + 273.15, "K")
        | Some "k" -> Some(value, "K")
        | Some "pa" -> Some(value, "Pa")
        | Some "m" -> Some(value, "m")
        | Some "m2" | Some "m^2" | Some "m²" -> Some(value, "m2")
        | Some "kg/m3" | Some "kg/m^3" | Some "kg/m³" -> Some(value, "kg/m3")
        | _ -> None
