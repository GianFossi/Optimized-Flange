namespace OptimizedFlange.Calculations

open OptimizedFlange.Domain

/// <summary>Executes non-normative structural validation checks for calculation requests.</summary>
module StructuralValidationEngine =
    let private emptyTrace =
        {
            Quantities = []
            Dependencies =
                [
                    {
                        DependencyId = "REQUEST.JOINT"
                        DependencyKind = "ProjectInput"
                        Fingerprint = None
                    }
                    {
                        DependencyId = ProcedureCatalog.structuralCompletenessRule.RuleId
                        DependencyKind = "Rule"
                        Fingerprint = None
                    }
                ]
        }

    let private checkFromError index code =
        {
            CheckId = $"OF.STRUCTURE.CHECK.{index}"
            Rule = ProcedureCatalog.structuralCompletenessRule
            Status = NotSatisfied
            Severity = Error
            Comparison = None
            GoverningCase = None
            MessageCode = code
            Trace = emptyTrace
        }

    /// <summary>Runs structural validation for a calculation request without evaluating normative engineering formulas.</summary>
    let run (request: CalculationRequest) : Result<CalculationResult, CalculationError list> =
        let errors = FlangedJoint.validateStructure request.Joint
        let checks = errors |> List.mapi checkFromError

        let result =
            {
                ResultId = $"{request.RequestId}.STRUCTURAL_VALIDATION"
                ExecutionStatus = Completed
                AssessmentStatus =
                    if checks.IsEmpty then
                        Satisfied
                    else
                        NotSatisfied
                Qualification = Implemented
                Checks = checks
                Trace = emptyTrace
            }

        Ok result
