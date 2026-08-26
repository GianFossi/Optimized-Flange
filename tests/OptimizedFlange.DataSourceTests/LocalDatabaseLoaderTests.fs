namespace OptimizedFlange.DataSourceTests

open System
open OptimizedFlange.Calculations
open OptimizedFlange.Configuration
open OptimizedFlange.DataSources
open OptimizedFlange.Domain
open Xunit

module LocalDatabaseLoaderTests =
    let private databaseRoot = @"C:\Users\ganfossi\Documents\DataBase\data"

    let private settings =
        Defaults.databasePathsFromRootFolder databaseRoot

    [<Fact>]
    let ``local database catalog loads configured XML and JSON records`` () =
        match LocalDatabaseLoaders.loadCatalog settings with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok catalog ->
            Assert.True(catalog.Records.Length > 0)
            Assert.Contains(catalog.Summaries, fun summary -> summary.SourceId = "DB.MATERIALS.MYLIB" && summary.RecordCount > 0)
            Assert.Contains(catalog.Summaries, fun summary -> summary.SourceId = "DB.BOLTING.TIE_RODS" && summary.RecordCount > 0)
            Assert.Contains(catalog.Summaries, fun summary -> summary.SourceId = "DB.GASKETS.GEOMETRY" && summary.RecordCount > 0)

    [<Fact>]
    let ``bolting loader converts millimetres and square millimetres to SI`` () =
        let criteria =
            {
                Search.all with
                    Category = Some Bolting
                    Text = Some "1/2 x 13"
                    HasScalar = Some "TensileStressArea"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            let record = Assert.Single(records)
            let tensileArea = record.Scalars |> List.find (fun scalar -> scalar.Name = "TensileStressArea")
            let nominalDiameter = record.Scalars |> List.find (fun scalar -> scalar.Name = "NominalDiameter")

            Assert.Equal(Some "m2", tensileArea.SiUnit)
            Assert.Equal(81.0 / 1_000_000.0, tensileArea.SiValue.Value, 12)
            Assert.Equal(Some "m", nominalDiameter.SiUnit)
            Assert.Equal(12.7 / 1000.0, nominalDiameter.SiValue.Value, 12)

    [<Fact>]
    let ``material search filters by specification grade and converts MPa to Pa`` () =
        let criteria =
            {
                Search.all with
                    Category = Some Materials
                    Standard = Some "SA-193"
                    Grade = Some "B7"
                    HasScalar = Some "specifiedMinimumYieldStrength"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            let record =
                records
                |> List.find (fun candidate -> candidate.DisplayName.Contains("SA-193 B7", StringComparison.OrdinalIgnoreCase))
            let yieldStrength =
                record.Scalars |> List.find (fun scalar -> scalar.Name = "specifiedMinimumYieldStrength")

            Assert.Equal(Some "Pa", yieldStrength.SiUnit)
            Assert.True(yieldStrength.SiValue.Value > 500_000_000.0)

    [<Fact>]
    let ``gasket search filters ring-joint records with imported dimensions`` () =
        let criteria =
            {
                Search.all with
                    Category = Some Gaskets
                    Text = Some "Ring gasket 11"
                    HasScalar = Some "P"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            let record = Assert.Single(records)
            let pitch = record.Scalars |> List.find (fun scalar -> scalar.Name = "P")

            Assert.Equal(Some "m", pitch.SiUnit)
            Assert.Equal(34.14 / 1000.0, pitch.SiValue.Value, 12)
            Assert.NotEmpty(record.Provenance)

    [<Fact>]
    let ``gasket parameter records expose m and y in SI`` () =
        let criteria =
            {
                Search.all with
                    Category = Some GasketParameters
                    Text = Some "Ring Joint: Iron or Soft Steel"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            let record = Assert.Single(records)
            Assert.Contains(record.Scalars, fun scalar -> scalar.Name = "m" && scalar.SourceValue = 5.5)
            Assert.Contains(record.Scalars, fun scalar -> scalar.Name = "y" && scalar.SiUnit = Some "Pa")

    [<Fact>]
    let ``procedure resolver returns data candidates for IOGP S-614 assessment`` () =
        match ProcedureDataResolver.resolve settings NormativeProcedureCatalog.iogpS614Paragraph78Amendments with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok resolution ->
            Assert.Equal(NormativeProcedureCatalog.iogpS614Paragraph78Amendments.ProcedureId, resolution.ProcedureId)
            Assert.Contains(resolution.CandidateRecords, fun record -> record.Category = Materials)
            Assert.Contains(resolution.CandidateRecords, fun record -> record.Category = Bolting)
            Assert.Contains(resolution.CandidateRecords, fun record -> record.Category = Gaskets)
            Assert.Contains(resolution.CandidateRecords, fun record -> record.Category = StandardFlanges)
            Assert.DoesNotContain(NozzleLoads, resolution.MissingCategories)

    [<Fact>]
    let ``procedure search can filter ASME candidates by material text`` () =
        let criteria =
            {
                Search.all with
                    Category = Some Materials
                    Text = Some "SA-516"
            }

        match ProcedureDataResolver.search settings NormativeProcedureCatalog.asmeViiiDivision2 criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            Assert.NotEmpty(records)
            Assert.All(records, fun record -> Assert.Equal(Materials, record.Category))

    [<Fact>]
    let ``SQLite ASME material database is parsed as material records`` () =
        let sqliteOnly =
            {
                settings with
                    Materials =
                        settings.Materials
                        |> List.filter (fun location -> location.Id = "DB.MATERIALS.ASME.SQLITE")
                    Bolting = []
                    Gaskets = []
                    Custom = []
            }

        let criteria =
            {
                Search.all with
                    Category = Some Materials
                    Text = Some "SA-1008"
                    HasScalar = Some "specifiedMinimumYieldStrength"
            }

        match LocalDatabaseLoaders.search sqliteOnly criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            Assert.NotEmpty(records)
            let record = records.Head
            Assert.Equal(Some "SA-1008", record.Standard)
            Assert.Contains(record.Scalars, fun scalar -> scalar.Name = "specifiedMinimumYieldStrength" && scalar.SiUnit = Some "Pa")

    [<Fact>]
    let ``material imported record maps to material snapshot`` () =
        let criteria =
            {
                Search.all with
                    Category = Some Materials
                    Text = Some "SA-193 B7"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            let record = records |> List.find (fun record -> record.Standard = Some "SA-193" && record.Grade = Some "B7")
            match DomainMapping.toMaterialSnapshot record with
            | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
            | Microsoft.FSharp.Core.Ok snapshot ->
                Assert.Equal(record.RecordId, snapshot.Identity.MaterialId)
                Assert.Equal(Some "SA-193", snapshot.Identity.Specification)
                Assert.Equal(Some "B7", snapshot.Identity.Grade)
                Assert.True(snapshot.Properties.Head.YieldStrengthPa.IsSome)

    [<Fact>]
    let ``bolting imported record maps to bolting assembly`` () =
        let criteria =
            {
                Search.all with
                    Category = Some Bolting
                    Text = Some "3/4 x 10"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            match DomainMapping.toBoltingAssembly 16 0.5<m> records.Head with
            | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
            | Microsoft.FSharp.Core.Ok assembly ->
                Assert.Equal(16, assembly.Pattern.Count)
                Assert.Equal(0.5<m>, assembly.Pattern.BoltCircleDiameterM)
                Assert.True(assembly.Stud.Areas.TensileStressAreaM2 > 0.0<m^2>)

    [<Fact>]
    let ``ring gasket imported record maps to gasket assembly`` () =
        let criteria =
            {
                Search.all with
                    Category = Some Gaskets
                    Text = Some "Ring gasket 11"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            match DomainMapping.toGasketAssembly records.Head with
            | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
            | Microsoft.FSharp.Core.Ok assembly ->
                Assert.Equal(RingTypeJoint, assembly.Family)
                Assert.True(assembly.Envelope.OutsideDiameterM > assembly.Envelope.InsideDiameterM)
                Assert.NotEmpty(assembly.SealingZones)

    [<Fact>]
    let ``standard flange record maps to joint side geometry`` () =
        let criteria =
            {
                Search.all with
                    Category = Some StandardFlanges
                    Text = Some "NPS .5000"
                    Grade = Some "150"
            }

        match LocalDatabaseLoaders.search settings criteria with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok records ->
            Assert.NotEmpty(records)
            match DomainMapping.toJointSideGeometry "primary" records.Head with
            | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
            | Microsoft.FSharp.Core.Ok geometry ->
                Assert.Equal("primary", geometry.SideId)
                Assert.Equal(FlangeGeometrySource.Imported, geometry.Source)
                Assert.True(geometry.Nominal.OutsideDiameterM > 0.0<m>)
                Assert.True(geometry.Nominal.BoltCircleDiameterM > 0.0<m>)
                Assert.True(geometry.Hub.IsSome)

    [<Fact>]
    let ``selected records compose a structurally valid flanged joint for dispatcher`` () =
        let first criteria =
            match LocalDatabaseLoaders.search settings criteria with
            | Microsoft.FSharp.Core.Error message -> failwith message
            | Microsoft.FSharp.Core.Ok records ->
                records
                |> List.tryHead
                |> Option.defaultWith (fun () -> failwith "No record matched selection criteria.")

        let flange =
            first
                {
                    Search.all with
                        Category = Some StandardFlanges
                        Text = Some "NPS .5000"
                        Grade = Some "150"
                }
        let gasket =
            first
                {
                    Search.all with
                        Category = Some Gaskets
                        Text = Some "Ring gasket 11"
                }
        let gasketParameters =
            first
                {
                    Search.all with
                        Category = Some GasketParameters
                        Text = Some "Ring Joint: Iron or Soft Steel"
                }
        let bolting =
            first
                {
                    Search.all with
                        Category = Some Bolting
                        Text = Some "1/2 x 13"
                }
        let material =
            first
                {
                    Search.all with
                        Category = Some Materials
                        Text = Some "SA-516 70"
                }

        let inputs =
            {
                JointId = "joint-from-selected-records"
                PrimaryMaterialRole = "primary"
                MatingMaterialRole = "mating"
                BoltCount = 4
                BoltCircleDiameter = 0.0603<m>
                PrimaryPressure = 1_000_000.0<Pa>
                MatingPressure = 0.0<Pa>
                PrimaryTemperature = 293.15<K>
                MatingTemperature = 293.15<K>
            }

        let selection =
            {
                PrimaryFlange = flange
                MatingFlange = None
                Gasket = gasket
                GasketParameters = Some gasketParameters
                Bolting = bolting
                PrimaryMaterial = material
                MatingMaterial = None
            }

        match JointSelectionBuilder.buildJoint inputs selection with
        | Microsoft.FSharp.Core.Error message -> Assert.Fail(message)
        | Microsoft.FSharp.Core.Ok joint ->
            Assert.Equal(Some 5.5, joint.Gasket.SelectedGasketM)
            Assert.True(joint.Gasket.SelectedGasketYPa.IsSome)
            let request =
                {
                    RequestId = "REQ-SELECTED-001"
                    Joint = joint
                    Procedure = ProcedureCatalog.structuralValidation
                    SelectedLoadCaseIds = []
                    AcceptanceCriteria = []
                }

            match CalculationDispatcher.run request with
            | Microsoft.FSharp.Core.Error errors -> Assert.Fail($"Dispatcher returned {errors.Length} errors.")
            | Microsoft.FSharp.Core.Ok result ->
                Assert.Equal(Completed, result.ExecutionStatus)
                Assert.Equal(Satisfied, result.AssessmentStatus)
