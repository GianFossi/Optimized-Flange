namespace OptimizedFlange.PersistenceTests

open System.IO
open OptimizedFlange.Configuration
open OptimizedFlange.Persistence
open Xunit

module DatabasePathSettingsStoreTests =
    let private databaseRoot = @"C:\Users\ganfossi\Documents\DataBase\data"

    [<Fact>]
    let ``database path defaults are portable until a root folder is supplied`` () =
        let defaults = Defaults.databasePaths

        Assert.Equal(Defaults.DatabasePathSettingsSchemaVersion, defaults.SchemaVersion)
        Assert.Equal(None, defaults.RootDatabaseFolder)
        Assert.Empty(defaults.Materials)
        Assert.Empty(defaults.Bolting)
        Assert.Empty(defaults.Gaskets)
        Assert.Empty(defaults.Custom)

    [<Fact>]
    let ``database path defaults can be created from an external root folder`` () =
        let defaults = Defaults.databasePathsFromRootFolder databaseRoot

        Assert.Equal(Defaults.DatabasePathSettingsSchemaVersion, defaults.SchemaVersion)
        Assert.Equal(Some databaseRoot, defaults.RootDatabaseFolder)
        Assert.All(defaults.Materials @ defaults.Bolting @ defaults.Gaskets @ defaults.Custom, fun location ->
            Assert.True(location.Enabled)
            Assert.True(location.ReadOnly)
            Assert.False(System.String.IsNullOrWhiteSpace(location.Path)))
        Assert.Contains(defaults.Materials, fun location -> location.Id = "DB.MATERIALS.MYLIB")
        Assert.Contains(defaults.Gaskets, fun location -> location.Path = Path.Combine(databaseRoot, "Gaskets.xml"))

    [<Fact>]
    let ``database path settings round trip through versioned JSON store`` () =
        let path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json")
        let settings = Defaults.databasePathsFromRootFolder databaseRoot

        try
            let saveResult = DatabasePathSettingsStore.save path settings
            match saveResult with
            | Ok () -> ()
            | Error message -> Assert.Fail($"Save failed: {message}")

            let loadResult = DatabasePathSettingsStore.load path

            match loadResult with
            | Ok loaded ->
                Assert.Equal(settings.RootDatabaseFolder, loaded.RootDatabaseFolder)
                Assert.Equal(settings.Gaskets.Length, loaded.Gaskets.Length)
                Assert.Equal(settings.Custom.Length, loaded.Custom.Length)
            | Error message -> Assert.Fail(message)
        finally
            if File.Exists(path) then
                File.Delete(path)
