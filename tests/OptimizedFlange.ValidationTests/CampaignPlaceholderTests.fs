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
    let ``selected source standards record editions and documents`` () =
        use document = readJsonDocument "standards-support.json"
        let standards = document.RootElement.GetProperty("standards")

        let selected =
            standards.EnumerateArray()
            |> Seq.filter (fun standard -> standard.GetProperty("supportStatus").GetString() = "SourceSelected")
            |> Seq.toList

        Assert.True(selected.Length >= 3)

        for standard in selected do
            Assert.False(System.String.IsNullOrWhiteSpace(standard.GetProperty("edition").GetString()))
            Assert.True(standard.GetProperty("sourceDocuments").GetArrayLength() > 0)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``engineering rule registry has no implemented normative placeholder rules without evidence`` () =
        use document = readJsonDocument "engineering-rules.json"
        let rules = document.RootElement.GetProperty("rules")
        Assert.True(rules.GetArrayLength() > 0)

        for rule in rules.EnumerateArray() do
            let normative = rule.GetProperty("normative").GetBoolean()
            let implementationStatus = rule.GetProperty("implementationStatus").GetString()
            if normative then
                match rule.GetProperty("ruleId").GetString() with
                | null -> Assert.Fail("Rule identifier is missing.")
                | ruleId ->
                    if ruleId.EndsWith(".PLACEHOLDER") then
                        Assert.Equal("Planned", implementationStatus)
                    elif implementationStatus = "Implemented" then
                        Assert.True(rule.GetProperty("validationEvidence").GetArrayLength() > 0)

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

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``formula inventory requires manual clause inventory before implementation`` () =
        use document = readJsonDocument "formula-inventory.json"
        let root = document.RootElement
        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32())

        let referenceGuideIds = root.GetProperty("referenceGuideIds")
        Assert.Contains(
            referenceGuideIds.EnumerateArray(),
            fun referenceGuideId -> referenceGuideId.GetString() = "GUIDE.FAST.FLANGE.DESIGN.R09")

        let sourceDocuments = root.GetProperty("sourceDocuments")
        Assert.True(sourceDocuments.GetArrayLength() > 0)

        for sourceDocument in sourceDocuments.EnumerateArray() do
            match sourceDocument.GetProperty("path").GetString() with
            | null -> Assert.Fail("Source document path is missing.")
            | path ->
                Assert.False(System.String.IsNullOrWhiteSpace(path))
                Assert.True(File.Exists(Path.Combine(repositoryRoot, path)))

        let formulaGroups = root.GetProperty("formulaGroups")
        Assert.True(formulaGroups.GetArrayLength() > 0)

        for formulaGroup in formulaGroups.EnumerateArray() do
            let status = formulaGroup.GetProperty("status").GetString()
            Assert.Contains(status, [| "NeedsManualClauseInventory"; "ClauseInventoryStarted" |])

            for formula in formulaGroup.GetProperty("formulas").EnumerateArray() do
                Assert.Equal("Implemented", formula.GetProperty("status").GetString())
                Assert.False(System.String.IsNullOrWhiteSpace(formula.GetProperty("clauseReference").GetString()))
                Assert.False(System.String.IsNullOrWhiteSpace(formula.GetProperty("formulaReference").GetString()))
                Assert.True(formula.GetProperty("validationCaseIds").GetArrayLength() > 0)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``formula inventory has no implementation-ready formulas without validation cases`` () =
        use document = readJsonDocument "formula-inventory.json"
        let formulaGroups = document.RootElement.GetProperty("formulaGroups")

        for formulaGroup in formulaGroups.EnumerateArray() do
            for formula in formulaGroup.GetProperty("formulas").EnumerateArray() do
                let status = formula.GetProperty("status").GetString()
                if status = "ReadyForImplementation" || status = "Implemented" then
                    Assert.False(System.String.IsNullOrWhiteSpace(formula.GetProperty("clauseReference").GetString()))
                    Assert.False(System.String.IsNullOrWhiteSpace(formula.GetProperty("formulaReference").GetString()))
                    Assert.True(formula.GetProperty("validationCaseIds").GetArrayLength() > 0)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``IOGP S-614 equation 3 implementation is registered in the amendment inventory`` () =
        use document = readJsonDocument "formula-inventory.json"
        let formulaGroups = document.RootElement.GetProperty("formulaGroups").EnumerateArray() |> Seq.toList

        let iogpGroup =
            formulaGroups
            |> List.find (fun group ->
                group.GetProperty("formulaGroupId").GetString() = "IOGP.S-614.V18-12.PARAGRAPH.7.8.AMENDMENTS.INVENTORY")

        let formulas = iogpGroup.GetProperty("formulas").EnumerateArray() |> Seq.toList
        let equation3 =
            formulas
            |> List.find (fun formula -> formula.GetProperty("formulaId").GetString() = "IOGP.S-614.V18-12.7.8.10.EQ3")

        Assert.Equal("Implemented", equation3.GetProperty("status").GetString())
        Assert.Equal("7.8.10", equation3.GetProperty("clauseReference").GetString())
        Assert.Equal("Equation (3)", equation3.GetProperty("formulaReference").GetString())
        Assert.True(equation3.GetProperty("validationCaseIds").GetArrayLength() > 0)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``IOGP S-614 paragraph 7.8 inventory amends API 660 paragraph 7.8`` () =
        use document = readJsonDocument "formula-inventory.json"
        let formulaGroups = document.RootElement.GetProperty("formulaGroups").EnumerateArray() |> Seq.toList

        let apiGroup =
            formulaGroups
            |> List.find (fun group ->
                group.GetProperty("formulaGroupId").GetString() = "API.660.2015.PARAGRAPH.7.8.INVENTORY")

        let iogpGroup =
            formulaGroups
            |> List.find (fun group ->
                group.GetProperty("formulaGroupId").GetString() = "IOGP.S-614.V18-12.PARAGRAPH.7.8.AMENDMENTS.INVENTORY")

        Assert.Equal("API.660", apiGroup.GetProperty("standardId").GetString())
        Assert.Equal("IOGP.S-614", iogpGroup.GetProperty("standardId").GetString())
        Assert.Equal(
            apiGroup.GetProperty("formulaGroupId").GetString(),
            iogpGroup.GetProperty("amendsFormulaGroupId").GetString())
        Assert.Equal("ClauseInventoryStarted", iogpGroup.GetProperty("status").GetString())

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``reference workbook guides are non normative and macro execution is disabled`` () =
        use document = readJsonDocument "reference-guides.json"
        let root = document.RootElement
        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32())

        let guides = root.GetProperty("referenceGuides")
        Assert.True(guides.GetArrayLength() > 0)

        for guide in guides.EnumerateArray() do
            match guide.GetProperty("path").GetString() with
            | null -> Assert.Fail("Reference guide path is missing.")
            | path ->
                Assert.False(System.String.IsNullOrWhiteSpace(path))
                Assert.True(File.Exists(Path.Combine(repositoryRoot, path)))

            Assert.False(guide.GetProperty("trustedAsNormativeSource").GetBoolean())
            Assert.False(guide.GetProperty("macroExecutionAllowed").GetBoolean())

            if guide.GetProperty("format").GetString() = "ExcelMacroEnabledWorkbook" then
                Assert.True(guide.GetProperty("containsMacros").GetBoolean())

            Assert.True(guide.GetProperty("sheetCount").GetInt32() > 0)
            Assert.True(guide.GetProperty("totalFormulaCells").GetInt32() > 0)
            Assert.True(guide.GetProperty("definedNameCount").GetInt32() > 0)
            Assert.True(guide.GetProperty("uniqueDefinedNameCount").GetInt32() > 0)
            Assert.True(guide.GetProperty("worksheets").GetArrayLength() > 0)

            let clusters = guide.GetProperty("definedNameClusters")
            Assert.True(clusters.GetProperty("bolting").GetArrayLength() > 0)
            Assert.True(clusters.GetProperty("gasket").GetArrayLength() > 0)
            Assert.True(clusters.GetProperty("flange").GetArrayLength() > 0)
            Assert.True(clusters.GetProperty("loads").GetArrayLength() > 0)

            Assert.True(guide.GetProperty("linkedFormulaGroupIds").GetArrayLength() > 0)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``reference guide linked formula groups exist in formula inventory`` () =
        use guidesDocument = readJsonDocument "reference-guides.json"
        use inventoryDocument = readJsonDocument "formula-inventory.json"

        let formulaGroupIds =
            inventoryDocument.RootElement.GetProperty("formulaGroups").EnumerateArray()
            |> Seq.choose (fun formulaGroup ->
                match formulaGroup.GetProperty("formulaGroupId").GetString() with
                | null -> None
                | formulaGroupId -> Some formulaGroupId)
            |> Set.ofSeq

        for guide in guidesDocument.RootElement.GetProperty("referenceGuides").EnumerateArray() do
            for linkedFormulaGroupId in guide.GetProperty("linkedFormulaGroupIds").EnumerateArray() do
                match linkedFormulaGroupId.GetString() with
                | null -> Assert.Fail("Linked formula group identifier is missing.")
                | groupId -> Assert.Contains(groupId, formulaGroupIds)

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``symbol maps are candidate mappings and reference registered guides`` () =
        use symbolDocument = readJsonDocument "symbol-map.json"
        use guideDocument = readJsonDocument "reference-guides.json"

        let guideIds =
            guideDocument.RootElement.GetProperty("referenceGuides").EnumerateArray()
            |> Seq.choose (fun guide ->
                match guide.GetProperty("referenceGuideId").GetString() with
                | null -> None
                | guideId -> Some guideId)
            |> Set.ofSeq

        let maps = symbolDocument.RootElement.GetProperty("symbolMaps")
        Assert.True(maps.GetArrayLength() > 0)

        for symbolMap in maps.EnumerateArray() do
            Assert.Equal("ReferenceGuide", symbolMap.GetProperty("sourceKind").GetString())
            Assert.False(symbolMap.GetProperty("normativeAuthority").GetBoolean())
            Assert.False(symbolMap.GetProperty("macroExecutionRequired").GetBoolean())
            Assert.Equal("CandidateMapping", symbolMap.GetProperty("status").GetString())

            match symbolMap.GetProperty("referenceGuideId").GetString() with
            | null -> Assert.Fail("Symbol map reference guide identifier is missing.")
            | guideId -> Assert.Contains(guideId, guideIds)

            let entries = symbolMap.GetProperty("entries")
            Assert.True(entries.GetArrayLength() > 0)

            for entry in entries.EnumerateArray() do
                Assert.False(System.String.IsNullOrWhiteSpace(entry.GetProperty("externalSymbol").GetString()))
                Assert.False(System.String.IsNullOrWhiteSpace(entry.GetProperty("domainPath").GetString()))
                Assert.False(System.String.IsNullOrWhiteSpace(entry.GetProperty("quantityKind").GetString()))
                Assert.False(System.String.IsNullOrWhiteSpace(entry.GetProperty("canonicalUnit").GetString()))
                Assert.Contains(
                    entry.GetProperty("mappingStatus").GetString(),
                    [| "Candidate"; "NeedsReview"; "NeedsSourceFormula" |])

    [<Trait("Campaign", "Registry")>]
    [<Fact>]
    let ``workbook comparison cases are not qualification evidence`` () =
        use document = readJsonDocument "workbook-comparison-cases.json"
        let root = document.RootElement
        Assert.Equal(1, root.GetProperty("schemaVersion").GetInt32())
        Assert.Equal("ReferenceWorkbookComparisonOnly", root.GetProperty("comparisonStatus").GetString())
        Assert.False(root.GetProperty("macroExecutionRequired").GetBoolean())
        Assert.False(root.GetProperty("qualificationEvidence").GetBoolean())

        let cases = root.GetProperty("cases")
        Assert.True(cases.GetArrayLength() > 0)

        for comparisonCase in cases.EnumerateArray() do
            Assert.False(System.String.IsNullOrWhiteSpace(comparisonCase.GetProperty("caseId").GetString()))
            Assert.False(System.String.IsNullOrWhiteSpace(comparisonCase.GetProperty("sheetName").GetString()))
            Assert.False(System.String.IsNullOrWhiteSpace(comparisonCase.GetProperty("implementedFormulaId").GetString()))
            Assert.True(comparisonCase.GetProperty("sourceCells").EnumerateObject() |> Seq.length > 0)
