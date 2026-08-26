namespace OptimizedFlange.Calculations

open OptimizedFlange.Domain

/// <summary>Provides planned normative calculation procedure definitions without implementing formulas.</summary>
module NormativeProcedureCatalog =
    let private standardSource name edition clause =
        {
            SourceKind = Standard
            Name = name
            Edition = Some edition
            Clause = clause
            FormulaReference = None
        }

    let private recommendedPracticeSource name edition clause =
        {
            SourceKind = RecommendedPractice
            Name = name
            Edition = Some edition
            Clause = clause
            FormulaReference = None
        }

    let private companySpecificationSource name edition clause =
        {
            SourceKind = CompanySpecification
            Name = name
            Edition = Some edition
            Clause = clause
            FormulaReference = None
        }

    let private plannedRule ruleId name ownerModule source =
        {
            RuleId = ruleId
            Name = name
            Module = ownerModule
            Source = source
            Qualification = Planned
            ValidationEvidence = []
        }

    /// <summary>Planned ASME VIII Division 1 flange assessment rule metadata.</summary>
    let asmeViiiDivision1FlangeRule =
        plannedRule
            "ASME.VIII.1.FLANGE.PLACEHOLDER"
            "ASME VIII Division 1 flange assessment"
            FlangeStressAssessment
            (standardSource "ASME BPVC Section VIII Division 1" "2025" (Some "Mandatory Appendix 2"))

    /// <summary>Planned ASME VIII Division 2 flanged-joint assessment rule metadata.</summary>
    let asmeViiiDivision2FlangedJointRule =
        plannedRule
            "ASME.VIII.2.FLANGE.PLACEHOLDER"
            "ASME VIII Division 2 flanged-joint assessment"
            FlangeStressAssessment
            (standardSource "ASME BPVC Section VIII Division 2" "2025" (Some "Part 4"))

    /// <summary>Planned ASME PCC-1 Appendix O assembly assessment rule metadata.</summary>
    let pcc1AppendixORule =
        plannedRule
            "ASME.PCC.1.ASSEMBLY.PLACEHOLDER"
            "ASME PCC-1 Appendix O assembly assessment"
            AssemblyAssessment
            (recommendedPracticeSource "ASME PCC-1" "2022" (Some "Appendix O"))

    /// <summary>Planned API 660 paragraph 7.8 exchanger flange requirement metadata.</summary>
    let api660Paragraph78Rule =
        plannedRule
            "API.660.EXCHANGER.PLACEHOLDER"
            "API 660 paragraph 7.8 exchanger flange assessment"
            ProjectPolicy
            (standardSource "API Standard 660" "2015" (Some "7.8"))

    /// <summary>Planned IOGP S-614 amendments to API 660 paragraph 7.8 metadata.</summary>
    let iogpS614Paragraph78AmendmentsRule =
        plannedRule
            "IOGP.S-614.EXCHANGER.PLACEHOLDER"
            "IOGP S-614 amendments to API 660 paragraph 7.8"
            ProjectPolicy
            (companySpecificationSource "IOGP S-614" "v18-12 / December 2018" (Some "7.8"))

    /// <summary>Procedure definition for planned ASME VIII Division 1 assessment.</summary>
    let asmeViiiDivision1 =
        {
            ProcedureId = "ASME.VIII.1.PROCEDURE.FLANGE_ASSESSMENT"
            Kind = DesignCodeAssessment
            Name = "ASME VIII Division 1 flange assessment"
            Rules = [ asmeViiiDivision1FlangeRule ]
            RequiredGeometryBasis = CodeEffective
            Qualification = Planned
        }

    /// <summary>Procedure definition for planned ASME VIII Division 2 assessment.</summary>
    let asmeViiiDivision2 =
        {
            ProcedureId = "ASME.VIII.2.PROCEDURE.FLANGED_JOINT_ASSESSMENT"
            Kind = DesignCodeAssessment
            Name = "ASME VIII Division 2 flanged-joint assessment"
            Rules = [ asmeViiiDivision2FlangedJointRule ]
            RequiredGeometryBasis = CodeEffective
            Qualification = Planned
        }

    /// <summary>Procedure definition for planned ASME PCC-1 Appendix O assessment.</summary>
    let pcc1AppendixO =
        {
            ProcedureId = "ASME.PCC.1.2022.APPENDIX.O.PROCEDURE"
            Kind = Pcc1Assessment
            Name = "ASME PCC-1 Appendix O assembly assessment"
            Rules = [ pcc1AppendixORule ]
            RequiredGeometryBasis = CodeEffective
            Qualification = Planned
        }

    /// <summary>Procedure definition for planned API 660 paragraph 7.8 assessment.</summary>
    let api660Paragraph78 =
        {
            ProcedureId = "API.660.2015.PARAGRAPH.7.8.PROCEDURE"
            Kind = Api660Assessment
            Name = "API 660 paragraph 7.8 exchanger flange assessment"
            Rules = [ api660Paragraph78Rule ]
            RequiredGeometryBasis = CodeEffective
            Qualification = Planned
        }

    /// <summary>Procedure definition for planned IOGP S-614 amendments to API 660 paragraph 7.8.</summary>
    let iogpS614Paragraph78Amendments =
        {
            ProcedureId = "IOGP.S-614.V18-12.PARAGRAPH.7.8.PROCEDURE"
            Kind = IogpS614Assessment
            Name = "IOGP S-614 amendments to API 660 paragraph 7.8"
            Rules = [ iogpS614Paragraph78AmendmentsRule ]
            RequiredGeometryBasis = CodeEffective
            Qualification = Planned
        }

    /// <summary>All planned normative procedures known to the calculation package.</summary>
    let all =
        [
            asmeViiiDivision1
            asmeViiiDivision2
            pcc1AppendixO
            api660Paragraph78
            iogpS614Paragraph78Amendments
        ]
