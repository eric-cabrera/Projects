namespace Assurity.PolicyInfo.Contracts.V1.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum BenefitOptionValue
    {
        [Display(Name = "$100")]
        OneHundredDollars,

        [Display(Name = "$1,000")]
        OneThousandDollars,

        [Display(Name = "$10,000")]
        TenThousandDollars,

        [Display(Name = "$10,000 / $100,000")]
        TenThousandToHundredThousandDollars,

        [Display(Name = "$10,000 / $50,000")]
        TenThousandToFiftyThousandDollars,

        [Display(Name = "$100,000")]
        HundredThousandDollars,

        [Display(Name = "$1,000 Lifetime Benefit")]
        ThousandDollarsLifeTimeBenefit,

        [Display(Name = "$150")]
        HundredAndFiftyDollars,

        [Display(Name = "$1,500")]
        FifteenHundredDollars,

        [Display(Name = "$200")]
        TwoHundredDollars,

        [Display(Name = "$2,000")]
        TwoThousandDollars,

        [Display(Name = "$25")]
        TwentyFiveDollars,

        [Display(Name = "$250")]
        TwoHundredAndFiftyDollars,

        [Display(Name = "$2,500")]
        TwentyFiveHundredDollars,

        [Display(Name = "$2,500 / $10,000")]
        TwentyFiveHundredToTenThousandDollars,

        [Display(Name = "$2,500 / $15,000")]
        TwentyFiveHundredToFiftenThousandDollars,

        [Display(Name = "$25,000")]
        TwentyFiveThousandDollars,

        [Display(Name = "$250,000")]
        TwoHundredFiftyThousandDollars,

        [Display(Name = "$300")]
        ThreeHundredDollars,

        [Display(Name = "$350")]
        ThreeHundredAndFiftyDollars,

        [Display(Name = "$3,500 / $25,000")]
        ThirtyFiveHundredToTwentyFiveThousandDollars,

        [Display(Name = "$3,500 / $50,000")]
        ThirtyFiveHundredToFiftyThousandDollars,

        [Display(Name = "$35,000")]
        ThirtyFiveThousandDollars,

        [Display(Name = "$4")]
        FourDollars,

        [Display(Name = "$400")]
        FourHundredDollars,

        [Display(Name = "$5")]
        FiveDollars,

        [Display(Name = "$50")]
        FiftyDollars,

        [Display(Name = "$500")]
        FiveHundredDollars,

        [Display(Name = "$5,000")]
        FiveThousandDollars,

        [Display(Name = "$5,000 / $20,000")]
        FiveThousandToTwentyThousandDollars,

        [Display(Name = "$5,000 / $25,000")]
        FiveThousandToTwentyFiveThousandDollars,

        [Display(Name = "$5,000 / $30,000")]
        FiveThousandToThirtyThousandDollars,

        [Display(Name = "$5,000 / $50,000")]
        FiveThousandToFiftyThousandDollars,

        [Display(Name = "$50,000")]
        FiftyThousandDollars,

        [Display(Name = "$500,000")]
        FiveHundredThousandDollars,

        [Display(Name = "$5500_$100000")]
        FiftyFiveHundredToHundredThousandDollars,

        [Display(Name = "$5500_$50000")]
        FiftyFiveHundredToFiftyThousandDollars,

        [Display(Name = "$560")]
        FiveHundredAndSixtyDollars,

        [Display(Name = "$600")]
        SixHundredDollars,

        [Display(Name = "$750")]
        SevenHundredAndFiftyDollars,

        [Display(Name = "$7,500")]
        SeventyFiveHundredDollars,

        [Display(Name = "Accidental Death And Dismemberment")]
        AccidentalDeathAndDismemberment,

        [Display(Name = "Accident Expense Half Compensation")]
        AccidentExpenseHalfCompensation,

        [Display(Name = "Accident Expense Non-Employer")]
        AccidentExpenseNonEmployer,

        [Display(Name = "Accident Expense Professional Employer Organization")]
        AccidentExpenseProfessionalEmployerOrganization,

        [Display(Name = "Accident Expense Zero Compensation")]
        AccidentExpenseZeroCompensation,

        [Display(Name = "Accident Medical Expense")]
        AccidentMedicalExpense,

        [Display(Name = "Aviation")]
        Aviation,

        [Display(Name = "Cancer")]
        Cancer,

        [Display(Name = "Cancer and Specified Disease")]
        CancerAndSpecifiedDisease,

        [Display(Name = "Cancer Only")]
        CancerOnly,

        [Display(Name = "Child")]
        Child,

        [Display(Name = "Child")]
        ChildNoCharge,

        [Display(Name = "10")]
        ChiropracticVisits10,

        [Display(Name = "2")]
        ChiropracticVisits2,

        [Display(Name = "3")]
        ChiropracticVisits3,

        [Display(Name = "6")]
        ChiropracticVisits6,

        [Display(Name = "80% / 20%")]
        CoinsurancePercentageEightyTwenty,

        [Display(Name = "50% / 50%")]
        CoinsurancePercentageFiftyFifty,

        [Display(Name = "100%")]
        CoinsurancePercentageOneHundred,

        [Display(Name = "75% / 25%")]
        CoinsurancePercentageSeventyFiveTwentyFive,

        [Display(Name = "100% / 50% / 50%")]
        CoveragePercentageOneHundredFiftyFifty,

        [Display(Name = "100% / 50% / 25%")]
        CoveragePercentageOneHundredFiftyTwentyFive,

        [Display(Name = "100% / 50% / 25% or 20%")]
        CoveragePercentageOneHundredFiftyTwentyFiveOrTwenty,

        [Display(Name = "100% / 100% / 50%")]
        CoveragePercentageOneHundredOneHundredFifty,

        [Display(Name = "100% / 100% / 25%")]
        CoveragePercentageOneHundredOneHundredTwentyFive,

        [Display(Name = "100% / 100% / 25% or 20%")]
        CoveragePercentageOneHundredOneHundredTwentyFiveOrTwenty,

        [Display(Name = "100% / 100% / 100%")]
        CoveragePercentageOneHundredOneHundredOneHundred,

        [Display(Name = "Critical Illness")]
        CriticalIllness,

        [Display(Name = "Critical Illness Half Compensation")]
        CriticalIllnessHalfCompensation,

        [Display(Name = "Critical Illness Non-Employer")]
        CriticalIllnessNonEmployer,

        [Display(Name = "Critical Illness Professional Employer Organization")]
        CriticalIllnessProfessionalEmployerOrganization,

        [Display(Name = "Critical Illness Zero Compensation")]
        CriticalIllnessZeroCompensation,

        [Display(Name = "Disability Income Association")]
        DisabilityIncomeAssociation,

        [Display(Name = "Disability Income Professional Employer Organization")]
        DisabilityIncomeProfessionalEmployerOrganization,

        [Display(Name = "Disability Income Zero Compensation")]
        DisabilityIncomeZeroCompensation,

        [Display(Name = "18 Months")]
        EighteenMonths,

        [Display(Name = "8 Years")]
        EightYears,

        [Display(Name = "8 Years (2 Years Base)")]
        EightYears_TwoYearsBase,

        [Display(Name = "11 Weeks")]
        ElevenWeeks,

        [Display(Name = "Employee")]
        Employee,

        [Display(Name = "Employee and Family")]
        EmployeeFamily,

        [Display(Name = "Employee and Child")]
        EmployeeChild,

        [Display(Name = "Employee and Spouse")]
        EmployeeSpouse,

        [Display(Name = "Excludes Dental")]
        ExcludesDental,

        [Display(Name = "Family")]
        Family,

        [Display(Name = "15 Days")]
        FifteenDays,

        [Display(Name = "15 Months")]
        FifteenMonths,

        [Display(Name = "15 Years")]
        FifteenYears,

        [Display(Name = "52 Weeks")]
        FiftyTwoWeeks,

        [Display(Name = "5 Days/5 Days")]
        FiveDaysAccidentFiveDaysSickness,

        [Display(Name = "5 Years")]
        FiveYears,

        [Display(Name = "5 Years (5 Years Base)")]
        FiveYears_FiveYearsBase,

        [Display(Name = "5 Years (1 Year Base)")]
        FiveYears_OneYearBase,

        [Display(Name = "5 Years (2 Years Base)")]
        FiveYears_TwoYearsBase,

        [Display(Name = "5 Years Own Occupation")]
        FiveYearsOwnOccupation,

        [Display(Name = "Flat Amount")]
        FlatAmount,

        [Display(Name = "4 Days")]
        FourDays,

        [Display(Name = "14 Consecutive Days")]
        FourteenConsecutiveDays,

        [Display(Name = "14 Days")]
        FourteenDays,

        [Display(Name = "4 Units")]
        FourUnits,

        [Display(Name = "4 Years (1 Year Base)")]
        FourYears_OneYearBase,

        [Display(Name = "Future Increase Option")]
        FutureIncreaseOption,

        [Display(Name = "Hospital Benefit")]
        HospitalBenefit,

        [Display(Name = "Hospital Confinement")]
        HospitalConfinement,

        [Display(Name = "Hospital Indemnity Admission")]
        HospitalIndemnityAdmission,

        [Display(Name = "Hospital Indemnity Admission Plus")]
        HospitalIndemnityAdmissionPlus,

        [Display(Name = "Hospital Indemnity Half Compensation")]
        HospitalIndemnityHalfCompensation,

        [Display(Name = "Hospital Indemnity Non-Employer")]
        HospitalIndemnityNonEmployer,

        [Display(Name = "Hospital Indemnity Professional Employer Organization")]
        HospitalIndemnityProfessionalEmployerOrganization,

        [Display(Name = "Hospital Indemnity Zero Compensation")]
        HospitalIndemnityZeroCompensation,

        [Display(Name = "Includes Dental")]
        IncludesDental,

        [Display(Name = "Insured Only")]
        Individual,

        [Display(Name = "Insured/Spouse")]
        IndividualSpouse,

        [Display(Name = "Insured/Children")]
        IndividualChild,

        [Display(Name = "Class 1")]
        IndustryClass1,

        [Display(Name = "Class 2")]
        IndustryClass2,

        [Display(Name = "Class 3")]
        IndustryClass3,

        [Display(Name = "Class 4")]
        IndustryClass4,

        [Display(Name = "Insured")]
        Insured,

        [Display(Name = "Lifetime")]
        Lifetime,

        [Display(Name = "Lifetime Accident")]
        LifetimeAccident,

        [Display(Name = "Living Benefit")]
        LivingBenefit,

        [Display(Name = "Long Term Premium Refund")]
        LongTermPremiumRefund,

        [Display(Name = "Loss Ratio")]
        LossRatio,

        [Display(Name = "Major Medical")]
        MajorMedical,

        [Display(Name = "90 Consecutive Days")]
        NinetyConsecutiveDays,

        [Display(Name = "90 Days")]
        NinetyDays,

        [Display(Name = "90 Days/31 Days")]
        NinetyDaysAccidentThirtyOneDaysSickness,

        [Display(Name = "9 Years (1 Year Base)")]
        NineYears_OneYearBase,

        [Display(Name = "Class 1")]
        OccupationClass1,

        [Display(Name = "Class 1A")]
        OccupationClass1A,

        [Display(Name = "Class 1 and 2")]
        OccupationClass1And2,

        [Display(Name = "Class 2")]
        OccupationClass2,

        [Display(Name = "Class 2A")]
        OccupationClass2A,

        [Display(Name = "Class 3")]
        OccupationClass3,

        [Display(Name = "Class 3A")]
        OccupationClass3A,

        [Display(Name = "Class 4")]
        OccupationClass4,

        [Display(Name = "Class 4A")]
        OccupationClass4A,

        [Display(Name = "Class 5A")]
        OccupationClass5A,

        [Display(Name = "Class 6A")]
        OccupationClass6A,

        [Display(Name = "Class A")]
        OccupationClassA,

        [Display(Name = "Class B")]
        OccupationClassB,

        [Display(Name = "Class C")]
        OccupationClassC,

        [Display(Name = "Class D")]
        OccupationClassD,

        [Display(Name = "Excellent")]
        OccupationClassExcellent,

        [Display(Name = "Normal")]
        OccupationClassNormal,

        [Display(Name = "Regular")]
        OccupationClassRegular,

        [Display(Name = "Superior")]
        OccupationClassSuperior,

        [Display(Name = "Off-the-Job")]
        OffTheJob,

        [Display(Name = "1 Day")]
        OneDay,

        [Display(Name = "180 Consecutive Days")]
        OneHundredEightyConsecutiveDays,

        [Display(Name = "180 Days")]
        OneHundredEightyDays,

        [Display(Name = "104 Weeks")]
        OneHundredFourWeeks,

        [Display(Name = "120 Days")]
        OneHundredTwentyDays,

        [Display(Name = "1 Unit")]
        OneUnit,

        [Display(Name = "1 Unit 25K/10K/5K")]
        OneUnit25K10K5K,

        [Display(Name = "1 Unit 50K/20K/10K")]
        OneUnit50K20K10K,

        [Display(Name = "1 Year")]
        OneYear,

        [Display(Name = "1 Year (1 Year Base)")]
        OneYear_OneYearBase,

        [Display(Name = "Partial Disbursement")]
        PartialDisbursement,

        [Display(Name = "Policy Fee")]
        PolicyFee,

        [Display(Name = "Policy Fee 2000")]
        PolicyFee2000,

        [Display(Name = "Policy Fee 400")]
        PolicyFee400,

        [Display(Name = "Policy Fee 560")]
        PolicyFee560,

        [Display(Name = "Policy Fee 600")]
        PolicyFee600,

        [Display(Name = "Policy Fee 700")]
        PolicyFee700,

        [Display(Name = "Policy Fee 800")]
        PolicyFee800,

        [Display(Name = "Pregnancy Amendment")]
        PregnancyAmendment,

        [Display(Name = "Premium Refund")]
        PremiumRefund,

        [Display(Name = "Reduced Benefit at Age 70")]
        ReducedBenefitAtSeventy,

        [Display(Name = "Return Of Premium")]
        ReturnOfPremium,

        [Display(Name = "Return of Premium and Reduced Benefit at Age 70")]
        ReturnOfPremiumAndReducedBenefitAt70,

        [Display(Name = "7 Consecutive Days")]
        SevenConsecutiveDays,

        [Display(Name = "7 Days")]
        SevenDays,

        [Display(Name = "730 Days")]
        SevenHundredThirtyDays,

        [Display(Name = "Short Term Premium Refund")]
        ShortTermPremiumRefund,

        [Display(Name = "Six Month Pre-exist")]
        SixMonthPreexist,

        [Display(Name = "6 Months")]
        SixMonths,

        [Display(Name = "60 Consecutive Days")]
        SixtyConsecutiveDays,

        [Display(Name = "60 Days")]
        SixtyDays,

        [Display(Name = "Spouse")]
        Spouse,

        [Display(Name = "Supplemental Hospital Indemnity")]
        SupplementalHospitalIndemnity,

        [Display(Name = "10 Days/10 Days")]
        TenDaysAccidentTenDaysSickness,

        [Display(Name = "10 Years")]
        TenYears,

        [Display(Name = "10 Years (5 Years Base)")]
        TenYears_FiveYearsBase,

        [Display(Name = "10 Years (1 Year Base)")]
        TenYears_OneYearBase,

        [Display(Name = "10 Years (2 Years Base)")]
        TenYears_TwoYearsBase,

        [Display(Name = "10 Years Own Occupation")]
        TenYearsOwnOccupation,

        [Display(Name = "2 Days")]
        TwoDays,

        [Display(Name = "13 Weeks")]
        ThirteenWeeks,

        [Display(Name = "30 Consecutive Days")]
        ThirtyConsecutiveDays,

        [Display(Name = "30 Days")]
        ThirtyDays,

        [Display(Name = "30 Days/10 Days")]
        ThirtyDaysAccidentTenDaysSickness,

        [Display(Name = "30 Months")]
        ThirtyMonths,

        [Display(Name = "31 Days/31 Days")]
        ThirtyOneDaysAccidentThirtyOneDaysSickness,

        [Display(Name = "30 Years")]
        ThirtyYears,

        [Display(Name = "3 Days")]
        ThreeDays,

        [Display(Name = "365 Consecutive Days")]
        ThreeHundredSixtyFiveConsecutiveDays,

        [Display(Name = "365 Days")]
        ThreeHundredSixtyFiveDays,

        [Display(Name = "365 Days/30 Days")]
        ThreeHundredSixtyFiveDaysAccidentThirtyDaysSickness,

        [Display(Name = "365 Days/31 Days")]
        ThreeHundredSixtyFiveDaysAccidentThirtyOneDaysSickness,

        [Display(Name = "3 Months")]
        ThreeMonths,

        [Display(Name = "3 Units")]
        ThreeUnits,

        [Display(Name = "3 Years")]
        ThreeYears,

        [Display(Name = "3 Years (1 Year Base)")]
        ThreeYears_OneYearBase,

        [Display(Name = "3 Years (2 Years Base)")]
        ThreeYears_TwoYearsBase,

        [Display(Name = "To Age 65")]
        ToAge65,

        [Display(Name = "To Age 65 (5 Years Base)")]
        ToAge65_FiveYearsBase,

        [Display(Name = "To Age 65 (10 Years Base)")]
        ToAge65_TenYearsBase,

        [Display(Name = "To Age 65 (2 Years Base)")]
        ToAge65_TwoYearsBase,

        [Display(Name = "To Age 65 Own Occupation")]
        ToAge65OwnOccupation,

        [Display(Name = "To Age 67")]
        ToAge67,

        [Display(Name = "To Age 67 Own Occupation")]
        ToAge67OwnOccupation,

        [Display(Name = "12 Months")]
        TwelveMonths,

        [Display(Name = "12 Weeks")]
        TwelveWeeks,

        [Display(Name = "25 Weeks")]
        TwentyFiveWeeks,

        [Display(Name = "25 Years")]
        TwentyFiveYears,

        [Display(Name = "24-Hour")]
        TwentyFourHour,

        [Display(Name = "24 Months")]
        TwentyFourMonths,

        [Display(Name = "24 Weeks")]
        TwentyFourWeeks,

        [Display(Name = "26 Weeks")]
        TwentySixWeeks,

        [Display(Name = "20 Years")]
        TwentyYears,

        [Display(Name = "2 Units")]
        TwoUnits,

        [Display(Name = "2 Units 100K/40K/10K")]
        TwoUnits100K40K10K,

        [Display(Name = "2 Units 50K/25K/10K")]
        TwoUnits50K25K10K,

        [Display(Name = "2 Years")]
        TwoYears,

        [Display(Name = "Union Association")]
        UnionAssociation,

        [Display(Name = "Waive Preexisting Condition")]
        WaivePreexistingCondition,

        [Display(Name = "Waiver Of Premium")]
        WaiverOfPremium,

        [Display(Name = "Without Cancer")]
        WithoutCancer,

        [Display(Name = "Without Heart or Stroke")]
        WithoutHeartOrStroke,

        [Display(Name = "0 Consecutive Days")]
        ZeroConsecutiveDays,

        [Display(Name = "0 Days")]
        ZeroDays,

        [Display(Name = "0 Years")]
        ZeroYears,

        // Enums related to Underwriting
        [Display(Name = "Combined")]
        Combined,

        [Display(Name = "Female Non-Tobacco Female Non-Tobacco")]
        FemaleNonTobaccoFemaleNonTobacco,

        [Display(Name = "Female Non-Tobacco Female Tobacco")]
        FemaleNonTobaccoFemaleTobacco,

        [Display(Name = "Female Tobacco Female Tobacco")]
        FemaleTobaccoFemaleTobacco,

        [Display(Name = "Home Office")]
        HomeOffice,

        [Display(Name = "Home Office Discount")]
        HomeOfficeDiscount,

        [Display(Name = "Home Office Discount Non-Tobacco")]
        HomeOfficeDiscountNonTobacco,

        [Display(Name = "Home Office Discount Tobacco")]
        HomeOfficeDiscountTobacco,

        [Display(Name = "Home Office Non-Tobacco")]
        HomeOfficeNonTobacco,

        [Display(Name = "Home Office Tobacco")]
        HomeOfficeTobacco,

        [Display(Name = "Joint")]
        Joint,

        [Display(Name = "Joint Standard Non-Tobacco")]
        JointStandardNonTobacco,

        [Display(Name = "Juvenile")]
        Juvenile,

        [Display(Name = "Juvenile Composite")]
        JuvenileComposite,

        [Display(Name = "Male Non-Tobacco Female Non-Tobacco")]
        MaleNonTobaccoFemaleNonTobacco,

        [Display(Name = "Male Non-Tobacco Female Tobacco")]
        MaleNonTobaccoFemaleTobacco,

        [Display(Name = "Male Non-Tobacco Male Non-Tobacco")]
        MaleNonTobaccoMaleNonTobacco,

        [Display(Name = "Male Non-Tobacco Male Tobacco")]
        MaleNonTobaccoMaleTobacco,

        [Display(Name = "Male Tobacco Female Non-Tobacco")]
        MaleTobaccoFemaleNonTobacco,

        [Display(Name = "Male Tobacco Female Tobacco")]
        MaleTobaccoFemaleTobacco,

        [Display(Name = "Male Tobacco Male Tobacco")]
        MaleTobaccoMaleTobacco,

        [Display(Name = "Non-Med Non-Tobacco")]
        NonMedNonTobacco,

        [Display(Name = "Non-Med Tobacco")]
        NonMedTobacco,

        [Display(Name = "Non-Tobacco")]
        NonTobacco,

        [Display(Name = "Other Insured")]
        OtherInsured,

        [Display(Name = "Preferred Non-Tobacco")]
        Preferred,

        [Display(Name = "Preferred Non-Tobacco")]
        PreferredNonTobacco,

        [Display(Name = "Preferred Plus Non-Tobacco")]
        PreferredPlus,

        [Display(Name = "Preferred Plus Non-Tobacco")]
        PreferredPlusNonTobacco,

        [Display(Name = "Preferred Tobacco")]
        PreferredTobacco,

        [Display(Name = "Rated")]
        Rated,

        [Display(Name = "Reduced Paid-Up Non-Tobacco")]
        ReducedPaidUpNonTobacco,

        [Display(Name = "Reduced Paid-Up Tobacco")]
        ReducedPaidUpTobacco,

        [Display(Name = "Select Non-Tobacco")]
        SelectNonTobacco,

        [Display(Name = "Select Non-Tobacco Vegetarian")]
        SelectNonTobaccoVegetarian,

        [Display(Name = "Select Plus Non-Tobacco")]
        SelectPlusNonTobacco,

        [Display(Name = "Select Plus Tobacco")]
        SelectPlusTobacco,

        [Display(Name = "Select Tobacco")]
        SelectTobacco,

        [Display(Name = "Standard")]
        Standard,

        [Display(Name = "Standard Non-Tobacco")]
        StandardNonTobacco,

        [Display(Name = "Standard Non-Tobacco Vegetarian")]
        StandardNonTobaccoVegetarian,

        [Display(Name = "Standard Plus Non-Tobacco")]
        StandardPlusNonTobacco,

        [Display(Name = "Standard Tobacco")]
        StandardTobacco,

        [Display(Name = "Substandard")]
        Substandard,

        [Display(Name = "Super Preferred")]
        SuperPreferred,

        [Display(Name = "Tobacco")]
        Tobacco,

        [Display(Name = "Uni-Tobacco")]
        UniversalTobacco,

        [Display(Name = "Unknown")]
        Unknown
    }
}
