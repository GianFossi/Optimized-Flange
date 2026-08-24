namespace OptimizedFlange.Calculations

/// <summary>Routes calculation requests to the implemented procedure engine.</summary>
module CalculationDispatcher =
    let private notImplementedError
        (request: OptimizedFlange.Domain.CalculationRequest)
        : OptimizedFlange.Domain.CalculationError =
        {
            ErrorCode = "CALCULATION.PROCEDURE.NOT_IMPLEMENTED"
            Severity = OptimizedFlange.Domain.CheckSeverity.Error
            MessageCode = "CALCULATION.PROCEDURE.NOT_IMPLEMENTED"
            RuleId = None
        }

    /// <summary>Runs the requested calculation procedure when it is implemented by the current package.</summary>
    let run
        (request: OptimizedFlange.Domain.CalculationRequest)
        : Result<OptimizedFlange.Domain.CalculationResult, OptimizedFlange.Domain.CalculationError list> =
        match request.Procedure.Kind with
        | OptimizedFlange.Domain.StructuralValidation -> StructuralValidationEngine.run request
        | OptimizedFlange.Domain.GeometryResolutionProcedure
        | OptimizedFlange.Domain.DesignCodeAssessment
        | OptimizedFlange.Domain.Pcc1Assessment
        | OptimizedFlange.Domain.Api660Assessment
        | OptimizedFlange.Domain.IogpS614Assessment
        | OptimizedFlange.Domain.TemaAssessment
        | OptimizedFlange.Domain.ProjectPolicyAssessment -> Error [ notImplementedError request ]
