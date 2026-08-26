namespace OptimizedFlange.Calculations

open OptimizedFlange.Domain

/// <summary>Routes partially implemented normative procedures without fabricating unresolved source-derived inputs.</summary>
module NormativeAssessmentEngine =
    let private emptyTrace =
        {
            Quantities = []
            Dependencies = []
        }

    let private missingInputCheck rule messageCode notes =
        {
            CheckId = $"{rule.RuleId}.INPUTS_REQUIRED"
            Rule = rule
            Status = Incomplete
            Severity = Warning
            Comparison = None
            GoverningCase = None
            MessageCode = messageCode
            Trace =
                {
                    Quantities =
                        [
                            {
                                QuantityId = $"{rule.RuleId}.IMPLEMENTATION_STATE"
                                Role = Diagnostic
                                CanonicalValue = 0M
                                Unit = None
                                SourceValueId = None
                                Notes = Some notes
                            }
                        ]
                    Dependencies =
                        [
                            {
                                DependencyId = rule.RuleId
                                DependencyKind = "EngineeringRule"
                                Fingerprint = None
                            }
                        ]
                }
        }

    let private incompleteResult request checks =
        {
            ResultId = $"{request.RequestId}.{request.Procedure.ProcedureId}.RESULT"
            ExecutionStatus = Completed
            AssessmentStatus = Incomplete
            Qualification = PartiallyImplemented
            Checks = checks
            Trace = emptyTrace
        }

    /// <summary>Runs the partially implemented ASME VIII Division 2 assessment endpoint.</summary>
    let runAsmeViiiDivision2 request =
        let checks =
            [
                missingInputCheck
                    NormativeProcedureCatalog.asmeViiiDivision2FlangedJointRule
                    "ASME.VIII.2.FLANGE.INPUTS_REQUIRED"
                    "ASME VIII-2 Part 4.16 basic bolt-load helper formulas are implemented, but the dispatcher needs resolved G, b, m, y, self-energizing state, and validation case selection before producing numeric procedure results."
            ]

        Ok(incompleteResult request checks)

    /// <summary>Runs the partially implemented IOGP S-614 paragraph 7.8 assessment endpoint.</summary>
    let runIogpS614Paragraph78 request =
        let checks =
            [
                missingInputCheck
                    NormativeProcedureCatalog.iogpS614Paragraph78AmendmentsRule
                    "IOGP.S614.7.8.INPUTS_REQUIRED"
                    "IOGP S-614 paragraph 7.8.10 Equation (3) helper is implemented, but the dispatcher needs resolved floating-head inputs before producing numeric procedure results."
            ]

        Ok(incompleteResult request checks)
