namespace OptimizedFlange.PersistenceTests

open System.IO
open OptimizedFlange.Configuration
open OptimizedFlange.Persistence
open Xunit

module CalculationDefaultsStoreTests =
    [<Fact>]
    let ``calculation defaults round trip through versioned JSON store`` () =
        let path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json")

        try
            let defaults: CalculationDefaults = Defaults.calculation
            let saveResult = CalculationDefaultsStore.save path defaults
            match saveResult with
            | Ok () -> ()
            | Error message -> Assert.Fail($"Save failed: {message}")

            let loadResult = CalculationDefaultsStore.load path

            match loadResult with
            | Ok loaded ->
                Assert.Equal(Defaults.CalculationDefaultsSchemaVersion, loaded.SchemaVersion)
                Assert.Equal(defaults.PrimaryCode, loaded.PrimaryCode)
                Assert.Equal(defaults.Solver.MaxIterations, loaded.Solver.MaxIterations)
                Assert.Equal(defaults.TargetUtilization, loaded.TargetUtilization)
            | Error message ->
                Assert.Fail(message)
        finally
            if File.Exists(path) then
                File.Delete(path)
