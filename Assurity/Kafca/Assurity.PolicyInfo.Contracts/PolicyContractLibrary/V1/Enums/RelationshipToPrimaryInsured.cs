namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum RelationshipToPrimaryInsured
    {
        Aunt,

        [Display(Name = "Authorized Person")]
        Authorized_Person,

        Brother,
        Business,

        [Display(Name = "Business Associate")]
        Business_Associate,

        [Display(Name = "Business Partner")]
        Business_Partner,

        Charity,
        Child,

        [Display(Name = "Contingent Beneficiary")]
        Contingent_Beneficiary,

        Corporation,
        Cousin,
        Creditor,
        Custodian,
        Daughter,

        [Display(Name = "Daughter In Law")]
        Daughter_In_Law,

        Dependent,

        [Display(Name = "Disabled Dependent")]
        Disabled_Dependent,

        [Display(Name = "Domestic Partner")]
        Domestic_Partner,

        Employee,
        Employer,
        Estate,
        Father,

        [Display(Name = "Father In Law")]
        Father_In_Law,

        Fiance,
        Fiancee,

        [Display(Name = "Former Husband")]
        Former_Husband,

        [Display(Name = "Former Spouse")]
        Former_Spouse,

        [Display(Name = "Former Wife")]
        Former_Wife,

        Friend,

        [Display(Name = "God Parent")]
        God_Parent,

        Godfather,
        Godmother,

        [Display(Name = "Grand Child")]
        Grand_Child,

        [Display(Name = "Grand Parent")]
        Grand_Parent,

        Granddaughter,
        Grandfather,
        Grandmother,
        Grandson,

        [Display(Name = "Grantor Trust")]
        Grantor_Trust,

        [Display(Name = "Great Aunt")]
        Great_Aunt,

        [Display(Name = "Great Granddaughter")]
        Great_Granddaughter,

        [Display(Name = "Great Grandfather")]
        Great_Grandfather,

        [Display(Name = "Great Grandmother")]
        Great_Grandmother,

        [Display(Name = "Great Grandson")]
        Great_Grandson,

        [Display(Name = "Great Uncle")]
        Great_Uncle,

        Guardian,

        [Display(Name = "Half Brother")]
        Half_Brother,

        [Display(Name = "Half Sister")]
        Half_Sister,

        Husband,
        Institution,
        Joint,
        Mother,

        [Display(Name = "Mother In Law")]
        Mother_In_Law,

        Nephew,
        Niece,
        Other,
        Parent,
        Partner,
        Partnership,

        [Display(Name = "Power Of Attorney")]
        Power_Of_Attorney,

        Self,
        Sibling,
        Sister,

        [Display(Name = "Sister In Law")]
        Sister_In_Law,

        [Display(Name = "Solo Proprietorship")]
        Sole_Proprietorship,

        Son,

        [Display(Name = "Son In Law")]
        Son_In_Law,

        Spouse,

        [Display(Name = "Step Brother")]
        Step_Brother,

        [Display(Name = "Step Child")]
        Step_Child,

        [Display(Name = "Step Daughter")]
        Step_Daughter,

        [Display(Name = "Step Father")]
        Step_Father,

        [Display(Name = "Step Mother")]
        Step_Mother,

        [Display(Name = "Step Parent")]
        Step_Parent,

        [Display(Name = "Step Sibling")]
        Step_Sibling,

        [Display(Name = "Step Sister")]
        Step_Sister,

        [Display(Name = "Step Son")]
        Step_Son,

        Trust,
        Trustee,
        Uncle,
        Unknown,
        Wife,

        // Values added below this comment were not present in the original list.
        // Adding at the bottom to preserve enumeration order.
        Additional
    }
}