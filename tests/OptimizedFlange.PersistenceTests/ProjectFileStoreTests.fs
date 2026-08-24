namespace OptimizedFlange.PersistenceTests

open System
open System.IO
open OptimizedFlange.Configuration
open OptimizedFlange.Persistence
open Xunit

module ProjectFileStoreTests =
    let private projectFile =
        let createdAt = DateTimeOffset(2026, 8, 24, 10, 0, 0, TimeSpan.Zero)
        let projectConfiguration =
            Defaults.calculation
            |> ProjectCalculationConfiguration.fromDefaults
            |> PersistenceMappers.projectCalculationConfigurationToDto

        {
            SchemaVersion = ProjectFileStore.CurrentSchemaVersion
            Metadata =
                {
                    ProjectId = "project-001"
                    Name = "Project envelope test"
                    CreatedAt = createdAt
                    ModifiedAt = createdAt.AddMinutes(5.0)
                    CreatedByVersion = "test"
                }
            CalculationConfiguration = projectConfiguration
            TechnicalDataSchemaVersion = Nullable()
            TechnicalDataJson = null
        }

    let private technicalData =
        {
            SchemaVersion = ProjectFileStore.CurrentTechnicalDataSchemaVersion
            AcceptanceCriteria = Array.empty
            LoadCases = Array.empty
            JointSideGeometries = Array.empty
            BoltingAssemblies = Array.empty
            GasketAssemblies = Array.empty
            ComponentMaterials = Array.empty
            FlangedJoints = Array.empty
        }

    [<Fact>]
    let ``project file envelope round trips through versioned JSON store`` () =
        let path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ofj")

        try
            match ProjectFileStore.save path projectFile with
            | Ok () -> ()
            | Error message -> Assert.Fail($"Save failed: {message}")

            match ProjectFileStore.load path with
            | Ok loaded ->
                Assert.Equal(ProjectFileStore.CurrentSchemaVersion, loaded.SchemaVersion)
                Assert.Equal(projectFile.Metadata.ProjectId, loaded.Metadata.ProjectId)
                Assert.Equal(projectFile.Metadata.Name, loaded.Metadata.Name)
                Assert.Equal(projectFile.CalculationConfiguration.PrimaryCode, loaded.CalculationConfiguration.PrimaryCode)
                Assert.False(loaded.TechnicalDataSchemaVersion.HasValue)
                Assert.Null(loaded.TechnicalDataJson)
            | Error message ->
                Assert.Fail(message)
        finally
            if File.Exists(path) then
                File.Delete(path)

    [<Fact>]
    let ``project calculation configuration DTO maps back to project owned configuration`` () =
        let actual =
            PersistenceMappers.projectCalculationConfigurationFromDto projectFile.CalculationConfiguration

        match actual with
        | Ok configuration ->
            Assert.Equal(Defaults.calculation.PrimaryCode, configuration.PrimaryCode)
            Assert.Equal(Defaults.calculation.Solver.MaxIterations, configuration.Solver.MaxIterations)
        | Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``project file envelope embeds and extracts versioned technical data`` () =
        let path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".ofj")

        try
            let projectWithTechnicalData =
                match ProjectFileStore.withTechnicalData technicalData projectFile with
                | Ok value -> value
                | Error message -> failwith message

            match ProjectFileStore.save path projectWithTechnicalData with
            | Ok () -> ()
            | Error message -> Assert.Fail($"Save failed: {message}")

            match ProjectFileStore.load path with
            | Ok loaded ->
                Assert.True(loaded.TechnicalDataSchemaVersion.HasValue)
                Assert.NotNull(loaded.TechnicalDataJson)

                match ProjectFileStore.technicalData loaded with
                | Ok loadedTechnicalData ->
                    Assert.Equal(ProjectFileStore.CurrentTechnicalDataSchemaVersion, loadedTechnicalData.SchemaVersion)
                    Assert.Empty(loadedTechnicalData.FlangedJoints)
                | Error message ->
                    Assert.Fail(message)
            | Error message ->
                Assert.Fail(message)
        finally
            if File.Exists(path) then
                File.Delete(path)
