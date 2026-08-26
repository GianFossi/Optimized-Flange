namespace OptimizedFlange.CalculationTests

open OptimizedFlange.Calculations
open OptimizedFlange.Domain
open Xunit

module IogpS614Paragraph78Tests =
    [<Fact>]
    let ``required selected assembly bolt stress includes gasket and pressure effects`` () =
        let input =
            {
                MinimumGasketStressPa = 50.0e6<Pa>
                FloatingHeadGasketAreaM2 = 0.010<m^2>
                FloatingHeadGasketInsideDiameterM = 0.500<m>
                FloatingHeadOutsideDiameterM = 0.800<m>
                TubeSidePressurePa = 2.0e6<Pa>
                ShellSidePressurePa = 1.0e6<Pa>
                GasketFactor = 0.8
                BoltCount = 20
                BoltRootAreaM2 = 245.0e-6<m^2>
            }

        let actual = IogpS614Paragraph78.requiredSelectedAssemblyBoltStress input

        match actual with
        | Result.Error errors -> Assert.Fail($"Unexpected errors: {errors}")
        | Ok result ->
            Assert.InRange(float result.GasketContributionN, 499999.9, 500000.1)
            Assert.InRange(float result.PressureResultantN, -109956.0, -109955.5)
            Assert.InRange(float result.RequiredSelectedAssemblyBoltStressPa, 99501000.0, 99501200.0)

    [<Fact>]
    let ``selected assembly bolt stress sufficiency is checked against required stress`` () =
        let input =
            {
                MinimumGasketStressPa = 50.0e6<Pa>
                FloatingHeadGasketAreaM2 = 0.010<m^2>
                FloatingHeadGasketInsideDiameterM = 0.500<m>
                FloatingHeadOutsideDiameterM = 0.800<m>
                TubeSidePressurePa = 2.0e6<Pa>
                ShellSidePressurePa = 1.0e6<Pa>
                GasketFactor = 0.8
                BoltCount = 20
                BoltRootAreaM2 = 245.0e-6<m^2>
            }

        let actual =
            IogpS614Paragraph78.selectedAssemblyBoltStressIsSufficient 100.0e6<Pa> input

        match actual with
        | Result.Error errors -> Assert.Fail($"Unexpected errors: {errors}")
        | Ok (isSufficient, _) -> Assert.True(isSufficient)

    [<Fact>]
    let ``invalid pressure effect inputs are rejected`` () =
        let input =
            {
                MinimumGasketStressPa = 0.0<Pa>
                FloatingHeadGasketAreaM2 = 0.0<m^2>
                FloatingHeadGasketInsideDiameterM = 0.500<m>
                FloatingHeadOutsideDiameterM = 0.800<m>
                TubeSidePressurePa = 2.0e6<Pa>
                ShellSidePressurePa = 1.0e6<Pa>
                GasketFactor = 0.8
                BoltCount = 20
                BoltRootAreaM2 = 245.0e-6<m^2>
            }

        let actual = IogpS614Paragraph78.requiredSelectedAssemblyBoltStress input

        match actual with
        | Ok _ -> Assert.Fail("Expected invalid-input errors.")
        | Result.Error errors -> Assert.Contains("Floating-head gasket area must be positive.", errors)
