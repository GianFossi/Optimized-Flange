namespace OptimizedFlange.Domain

/// <summary>Defines SI units of measure used by the calculation core.</summary>
[<AutoOpen>]
module Units =
    /// <summary>Length in metres.</summary>
    [<Measure>] type m

    /// <summary>Force in newtons.</summary>
    [<Measure>] type N

    /// <summary>Pressure in pascals.</summary>
    [<Measure>] type Pa

    /// <summary>Temperature in kelvin.</summary>
    [<Measure>] type K

    /// <summary>Mass in kilograms.</summary>
    [<Measure>] type kg

    /// <summary>Time in seconds.</summary>
    [<Measure>] type s
