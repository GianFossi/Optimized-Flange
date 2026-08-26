namespace OptimizedFlange.Calculations

open OptimizedFlange.Domain

/// <summary>Provides calculation procedure definitions that are implemented by the current calculation package.</summary>
module ProcedureCatalog =
    /// <summary>Metadata source used for internal non-normative project rules.</summary>
    let private projectInternalSource =
        {
            SourceKind = ProjectSpecification
            Name = "OptimizedFlange internal structural validation"
            Edition = Some "V1"
            Clause = None
            FormulaReference = None
        }

    /// <summary>Rule metadata for non-normative structural completeness validation.</summary>
    let structuralCompletenessRule =
        {
            RuleId = "OF.STRUCTURE.COMPLETE"
            Name = "Flanged joint structural completeness"
            Module = ProjectPolicy
            Source = projectInternalSource
            Qualification = Implemented
            ValidationEvidence =
                [
                    {
                        EvidenceId = "TEST.UNIT.FLANGED_JOINT_STRUCTURE"
                        EvidenceKind = "UnitTest"
                        Location = Some "tests/OptimizedFlange.UnitTests/FlangedJointStructureTests.fs"
                    }
                ]
        }

    /// <summary>Procedure definition for non-normative structural validation of a flanged-joint input model.</summary>
    let structuralValidation =
        {
            ProcedureId = "OF.PROCEDURE.STRUCTURAL_VALIDATION"
            Kind = StructuralValidation
            Name = "Structural validation"
            Rules = [ structuralCompletenessRule ]
            RequiredGeometryBasis = AsBuilt
            Qualification = Implemented
        }

    /// <summary>All procedure definitions known to the calculation package.</summary>
    let all =
        structuralValidation :: NormativeProcedureCatalog.all
