namespace OptimizedFlange.PersistenceTests

open OptimizedFlange.Domain
open OptimizedFlange.Persistence
open Xunit

module ProjectTechnicalDataMapperTests =
    let private loadCase: JointLoadCase =
        {
            LoadCaseId = "operating-1"
            Name = "Operating 1"
            Kind = Operating
            PrimaryCondition = { PressurePa = 2.5e6<Pa>; TemperatureK = 450.0<K> }
            MatingCondition = { PressurePa = 1.0e5<Pa>; TemperatureK = 420.0<K> }
            ExternalLoads =
                {
                    FxN = -10.0<N>
                    FyN = 20.0<N>
                    FzN = -30.0<N>
                    MxNm = 40.0<N m>
                    MyNm = -50.0<N m>
                    MzNm = 60.0<N m>
                }
        }

    [<Fact>]
    let ``acceptance criterion maps through explicit DTO without losing optional limits`` () =
        let criterion: AcceptanceCriterion =
            {
                CriterionId = "criterion-rotation"
                Level = Hard
                Source = Project
                Edition = Some "project-rev-a"
                Clause = Some "REQ-12"
                UtilizationLimit = Some 0.9M
                RotationLimitRad = Some 0.01
            }

        let dto = PersistenceMappers.acceptanceCriterionToDto criterion
        let actual = PersistenceMappers.acceptanceCriterionFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(criterion.CriterionId, mapped.CriterionId)
            Assert.Equal(criterion.Level, mapped.Level)
            Assert.Equal(criterion.Source, mapped.Source)
            Assert.Equal(criterion.Edition, mapped.Edition)
            Assert.Equal(criterion.Clause, mapped.Clause)
            Assert.Equal(criterion.UtilizationLimit, mapped.UtilizationLimit)
            Assert.Equal(criterion.RotationLimitRad, mapped.RotationLimitRad)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``joint load case maps through explicit DTO without losing signs`` () =
        let dto = PersistenceMappers.jointLoadCaseToDto loadCase
        let actual = PersistenceMappers.jointLoadCaseFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(loadCase.LoadCaseId, mapped.LoadCaseId)
            Assert.Equal(loadCase.Kind, mapped.Kind)
            Assert.Equal(loadCase.PrimaryCondition.PressurePa, mapped.PrimaryCondition.PressurePa)
            Assert.Equal(loadCase.MatingCondition.TemperatureK, mapped.MatingCondition.TemperatureK)
            Assert.Equal(loadCase.ExternalLoads.FxN, mapped.ExternalLoads.FxN)
            Assert.Equal(loadCase.ExternalLoads.FzN, mapped.ExternalLoads.FzN)
            Assert.Equal(loadCase.ExternalLoads.MyNm, mapped.ExternalLoads.MyNm)
        | Result.Error message ->
            Assert.Fail(message)

    [<Fact>]
    let ``project technical data DTO maps acceptance criteria and load case collections`` () =
        let criteria: AcceptanceCriterion list =
            [
                {
                    CriterionId = "criterion-info"
                    Level = Informational
                    Source = User
                    Edition = None
                    Clause = None
                    UtilizationLimit = None
                    RotationLimitRad = None
                }
            ]

        let dto = PersistenceMappers.projectTechnicalDataToDto 1 criteria [ loadCase ]
        let actual = PersistenceMappers.projectTechnicalDataFromDto dto

        match actual with
        | Ok (mappedCriteria, mappedLoadCases) ->
            Assert.Equal(1, dto.SchemaVersion)
            Assert.Single(mappedCriteria) |> ignore
            Assert.Single(mappedLoadCases) |> ignore
            Assert.Equal(criteria.Head.CriterionId, mappedCriteria.Head.CriterionId)
            Assert.Equal(loadCase.LoadCaseId, mappedLoadCases.Head.LoadCaseId)
        | Result.Error message ->
            Assert.Fail(message)
