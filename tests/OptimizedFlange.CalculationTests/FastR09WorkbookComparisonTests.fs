namespace OptimizedFlange.CalculationTests

open OptimizedFlange.Calculations
open OptimizedFlange.Domain
open Xunit

module FastR09WorkbookComparisonTests =
    type private FastR09GasketSeatingCase =
        {
            SheetName: string
            EffectiveGasketWidthMm: float
            GasketReactionDiameterMm: float
            GasketYMPa: float
            ExpectedBaseGasketSeatingLoadKN: float
        }

    let private comparisonCases =
        [
            {
                SheetName = "Floating Head"
                EffectiveGasketWidthMm = 7.127411872482185
                GasketReactionDiameterMm = 932.7451762550356
                GasketYMPa = 70.0
                ExpectedBaseGasketSeatingLoadKN = 1461.9845415613163
            }
            {
                SheetName = "Main Girth Flange SH"
                EffectiveGasketWidthMm = 9.759610647971568
                GasketReactionDiameterMm = 1806.4807787040568
                GasketYMPa = 69.0
                ExpectedBaseGasketSeatingLoadKN = 3821.7722313472964
            }
            {
                SheetName = "Main Girth Flange CH"
                EffectiveGasketWidthMm = 11.269427669584646
                GasketReactionDiameterMm = 1135.4611446608308
                GasketYMPa = 69.0
                ExpectedBaseGasketSeatingLoadKN = 2773.7869540921392
            }
        ]

    [<Fact>]
    let ``ASME gasket seating helper matches FAST R09 cached Wg1 base cases`` () =
        for testCase in comparisonCases do
            let input =
                {
                    PressurePa = 0.0<Pa>
                    GasketReactionDiameterM = LanguagePrimitives.FloatWithMeasure<m> (testCase.GasketReactionDiameterMm / 1000.0)
                    EffectiveGasketWidthM = LanguagePrimitives.FloatWithMeasure<m> (testCase.EffectiveGasketWidthMm / 1000.0)
                    GasketM = 0.0
                    GasketYPa = LanguagePrimitives.FloatWithMeasure<Pa> (testCase.GasketYMPa * 1.0e6)
                    SelfEnergizing = false
                    SelfEnergizingSeatingForceN = None
                }

            let actual = AsmeViii2Part416BoltLoads.calculate input

            match actual with
            | Result.Error errors -> Assert.Fail($"Unexpected errors for {testCase.SheetName}: {errors}")
            | Ok result ->
                let actualKN = float result.GasketSeatingLoadN / 1000.0
                Assert.InRange(
                    actualKN,
                    testCase.ExpectedBaseGasketSeatingLoadKN - 0.000001,
                    testCase.ExpectedBaseGasketSeatingLoadKN + 0.000001)
