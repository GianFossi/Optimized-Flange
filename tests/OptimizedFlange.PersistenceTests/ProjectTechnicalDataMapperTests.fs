namespace OptimizedFlange.PersistenceTests

open OptimizedFlange.Domain
open OptimizedFlange.Persistence
open Xunit

module ProjectTechnicalDataMapperTests =
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
    let ``project technical data DTO maps acceptance criteria collection`` () =
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

        let dto = PersistenceMappers.projectTechnicalDataToDto 1 criteria
        let actual = PersistenceMappers.projectTechnicalDataFromDto dto

        match actual with
        | Ok mapped ->
            Assert.Equal(1, dto.SchemaVersion)
            Assert.Single(mapped) |> ignore
            Assert.Equal(criteria.Head.CriterionId, mapped.Head.CriterionId)
        | Result.Error message ->
            Assert.Fail(message)
