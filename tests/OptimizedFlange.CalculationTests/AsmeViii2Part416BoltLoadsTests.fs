namespace OptimizedFlange.CalculationTests

open OptimizedFlange.Calculations
open OptimizedFlange.Domain
open Xunit

module AsmeViii2Part416BoltLoadsTests =
    [<Fact>]
    let ``non self energizing gasket bolt loads include pressure and seating components`` () =
        let input =
            {
                PressurePa = 2.0e6<Pa>
                GasketReactionDiameterM = 0.5<m>
                EffectiveGasketWidthM = 0.006<m>
                GasketM = 3.0
                GasketYPa = 70.0e6<Pa>
                SelfEnergizing = false
                SelfEnergizingSeatingForceN = None
            }

        let actual = AsmeViii2Part416BoltLoads.calculate input

        match actual with
        | Result.Error errors -> Assert.Fail($"Unexpected errors: {errors}")
        | Ok result ->
            Assert.InRange(float result.OperatingBoltLoadN, 505596.0, 505598.0)
            Assert.InRange(float result.GasketSeatingLoadN, 659734.0, 659735.0)

    [<Fact>]
    let ``self energizing gasket bolt loads use pressure load and optional seating force`` () =
        let input =
            {
                PressurePa = 2.0e6<Pa>
                GasketReactionDiameterM = 0.5<m>
                EffectiveGasketWidthM = 0.006<m>
                GasketM = 3.0
                GasketYPa = 70.0e6<Pa>
                SelfEnergizing = true
                SelfEnergizingSeatingForceN = Some 12_000.0<N>
            }

        let actual = AsmeViii2Part416BoltLoads.calculate input

        match actual with
        | Result.Error errors -> Assert.Fail($"Unexpected errors: {errors}")
        | Ok result ->
            Assert.InRange(float result.OperatingBoltLoadN, 392499.0, 392501.0)
            Assert.InRange(float result.GasketSeatingLoadN, 11999.0, 12001.0)

    [<Fact>]
    let ``invalid ASME bolt load helper inputs are rejected`` () =
        let input =
            {
                PressurePa = -1.0<Pa>
                GasketReactionDiameterM = 0.5<m>
                EffectiveGasketWidthM = 0.006<m>
                GasketM = 3.0
                GasketYPa = 70.0e6<Pa>
                SelfEnergizing = false
                SelfEnergizingSeatingForceN = None
            }

        let actual = AsmeViii2Part416BoltLoads.calculate input

        match actual with
        | Ok _ -> Assert.Fail("Expected invalid-input errors.")
        | Result.Error errors -> Assert.Contains("Pressure must be non-negative.", errors)
