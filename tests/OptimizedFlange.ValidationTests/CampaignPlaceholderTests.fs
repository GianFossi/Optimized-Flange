namespace OptimizedFlange.ValidationTests

open System.IO
open System.Text.Json
open Xunit

module CampaignPlaceholderTests =
    let private repositoryRoot =
        let mutable directory = DirectoryInfo(Directory.GetCurrentDirectory())
        while not (File.Exists(Path.Combine(directory.FullName, "OptimizedFlange.sln"))) do
            match directory.Parent with
            | null -> failwith "Unable to locate repository root."
            | parent -> directory <- parent
        directory.FullName

    let private registryPath fileName =
        Path.Combine(repositoryRoot, "registry", fileName)

    let private readJsonDocument fileName =
        let path = registryPath fileName
        use stream = File.OpenRead(path)
        JsonDocument.Parse(stream)

    [<Trait("Campaign", "Clause")>]
    [<Fact>]
    let ``clause tests are reserved for future sourced normative rules`` () =
        Assert.True(true)

    [<Trait("Campaign", "Reference")>]
    [<Fact>]
    let ``reference tests are reserved for independent published examples`` () =
        Assert.True(true)

    [<Trait("Campaign", "Regression")>]
    [<Fact>]
    let ``regression tests are reserved for accepted result baselines`` () =
        Assert.True(true)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``standards support registry is valid and contains only planned normative support`` () =
        use document = readJsonDocument "standards-support.json"
        let root = document.RootElement
        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32())

        let standards = root.GetProperty("standards")
        Assert.True(standards.GetArrayLength() > 0)

        for standard in standards.EnumerateArray() do
            Assert.False(standard.GetProperty("normativeFormulasImplemented").GetBoolean())

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``engineering rule registry has no implemented normative placeholder rules`` () =
        use document = readJsonDocument "engineering-rules.json"
        let rules = document.RootElement.GetProperty("rules")
        Assert.True(rules.GetArrayLength() > 0)

        for rule in rules.EnumerateArray() do
            let normative = rule.GetProperty("normative").GetBoolean()
            let implementationStatus = rule.GetProperty("implementationStatus").GetString()
            if normative then
                Assert.Equal("Planned", implementationStatus)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``qualification registry declares no qualified normative calculations`` () =
        use document = readJsonDocument "qualification.json"
        let scope = document.RootElement.GetProperty("qualificationScope")
        Assert.False(scope.GetProperty("normativeCalculationsQualified").GetBoolean())
        Assert.Equal(0, scope.GetProperty("qualifiedStandards").GetArrayLength())

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``normative interpretation registry starts empty`` () =
        use document = readJsonDocument "normative-interpretations.json"
        let interpretations = document.RootElement.GetProperty("interpretations")
        Assert.Equal(0, interpretations.GetArrayLength())
