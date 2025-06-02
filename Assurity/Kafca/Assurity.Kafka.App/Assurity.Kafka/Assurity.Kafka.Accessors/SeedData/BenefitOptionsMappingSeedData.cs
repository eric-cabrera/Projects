namespace Assurity.Kafka.Accessors.Extensions
{
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    public static class BenefitOptionsMappingSeedData
    {
        public static List<BenefitOptionsMapping> GetBenefitOptionsMapping => new List<BenefitOptionsMapping>
        {
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.ZeroYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.OneYear),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.TenYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.FifteenMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.EighteenMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.TwoYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.ThirtyYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.SixMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.Lifetime),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentBenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge65),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentExpenseHalfCompensation),
              Option = nameof(BenefitOptionValue.AccidentExpenseHalfCompensation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentExpenseNonEmployer),
              Option = nameof(BenefitOptionValue.AccidentExpenseNonEmployer),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentExpenseProfessionalEmployerOrganization),
              Option = nameof(BenefitOptionValue.AccidentExpenseProfessionalEmployerOrganization),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentExpenseZeroCompensation),
              Option = nameof(BenefitOptionValue.AccidentExpenseZeroCompensation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentMedicalExpense),
              Option = nameof(BenefitOptionValue.AccidentMedicalExpense),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentSickPeriod),
              Option = nameof(BenefitOptionValue.FourteenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentSickPeriod),
              Option = nameof(BenefitOptionValue.OneHundredEightyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentSickPeriod),
              Option = nameof(BenefitOptionValue.ThirtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentSickPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentSickPeriod),
              Option = nameof(BenefitOptionValue.SevenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentSickPeriod),
              Option = nameof(BenefitOptionValue.NinetyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentalDeathAndDismemberment),
              Option = nameof(BenefitOptionValue.AccidentalDeathAndDismemberment),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.ZeroConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.ZeroDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.FourteenConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.FourteenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.OneHundredEightyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.ThirtyConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.ThirtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.SixMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.SixtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.SevenConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.SevenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.NinetyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.AccidentEliminationPeriod),
              Option = nameof(BenefitOptionValue.OccupationClassD),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Aviation),
              Option = nameof(BenefitOptionValue.Aviation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Benefit),
              Option = nameof(BenefitOptionValue.CriticalIllness),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Benefit),
              Option = nameof(BenefitOptionValue.WithoutCancer),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Benefit),
              Option = nameof(BenefitOptionValue.WithoutHeartOrStroke),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.OneThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.ThousandDollarsLifeTimeBenefit),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.FifteenHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.TenThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.HundredAndFiftyDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.TwoThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.TwentyFiveHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.TwoHundredAndFiftyDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.ThreeHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.ThreeHundredAndFiftyDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.FourDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.FiveDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.FiveThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.FiveHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.SixHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.SeventyFiveHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitAmount),
              Option = nameof(BenefitOptionValue.SevenHundredAndFiftyDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.HundredThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.TwentyFiveThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.TwoHundredFiftyThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.ThirtyFiveThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.FourHundredDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.FiveThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.FiftyThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.FiveHundredDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.FiveHundredThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitMaximum),
              Option = nameof(BenefitOptionValue.FiveThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OneYear),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OneYear_OneYearBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TenDaysAccidentTenDaysSickness),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TenYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TenYears_OneYearBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TenYears_TwoYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TenYears_FiveYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TenYearsOwnOccupation),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OneHundredFourWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ElevenWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwelveMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwelveWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThirteenWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FifteenMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FifteenYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.EighteenMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OneHundredEightyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwoYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwentyYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwentyFourMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwentyFourWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwentyFiveWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwentyFiveYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.TwentySixWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeYears_OneYearBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeYears_TwoYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThirtyDaysAccidentTenDaysSickness),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThirtyMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThirtyYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThirtyOneDaysAccidentThirtyOneDaysSickness),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDaysAccidentThirtyDaysSickness),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDaysAccidentThirtyOneDaysSickness),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FourYears_OneYearBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveDaysAccidentFiveDaysSickness),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveYears_OneYearBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveYears_TwoYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveYears_FiveYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveYearsOwnOccupation),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.FiftyTwoWeeks),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.SixMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.EightYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.EightYears_TwoYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.NineYears_OneYearBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.NinetyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.NinetyDaysAccidentThirtyOneDaysSickness),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OccupationClass3A),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OccupationClass4A),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OccupationClass5A),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.OccupationClass6A),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.Lifetime),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge65),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge65_TenYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge65_TwoYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge65_FiveYearsBase),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge65OwnOccupation),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge67),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.BenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge67OwnOccupation),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Cancer),
              Option = nameof(BenefitOptionValue.Cancer),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ChiropracticVisits),
              Option = nameof(BenefitOptionValue.ChiropracticVisits10),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ChiropracticVisits),
              Option = nameof(BenefitOptionValue.ChiropracticVisits2),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ChiropracticVisits),
              Option = nameof(BenefitOptionValue.ChiropracticVisits3),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ChiropracticVisits),
              Option = nameof(BenefitOptionValue.ChiropracticVisits6),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Coinsurance),
              Option = nameof(BenefitOptionValue.CoinsurancePercentageFiftyFifty),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Coinsurance),
              Option = nameof(BenefitOptionValue.CoinsurancePercentageSeventyFiveTwentyFive),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Coinsurance),
              Option = nameof(BenefitOptionValue.CoinsurancePercentageEightyTwenty),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Coverage),
              Option = nameof(BenefitOptionValue.TwentyFourHour),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Coverage),
              Option = nameof(BenefitOptionValue.OffTheJob),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.Child),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.Child),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.Employee),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.EmployeeChild),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.EmployeeFamily),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.EmployeeSpouse),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.Family),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.Insured),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.Spouse),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CriticalIllness),
              Option = nameof(BenefitOptionValue.CriticalIllness),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CriticalIllnessHalfCompensation),
              Option = nameof(BenefitOptionValue.CriticalIllnessHalfCompensation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CriticalIllnessNonEmployer),
              Option = nameof(BenefitOptionValue.CriticalIllnessNonEmployer),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CriticalIllnessProfessionalEmployerOrganization),
              Option = nameof(BenefitOptionValue.CriticalIllnessProfessionalEmployerOrganization),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CriticalIllnessZeroCompensation),
              Option = nameof(BenefitOptionValue.CriticalIllnessZeroCompensation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.OneThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.OneHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.HundredAndFiftyDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.TwentyFiveHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.TwoHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.TwentyFiveDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.FiftyDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.FiveHundredDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Deductible),
              Option = nameof(BenefitOptionValue.FiveHundredAndSixtyDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DentalCoverage),
              Option = nameof(BenefitOptionValue.ExcludesDental),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DentalCoverage),
              Option = nameof(BenefitOptionValue.IncludesDental),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoinsurancePercentageOneHundred),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredOneHundredOneHundred),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredOneHundredTwentyFive),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredOneHundredTwentyFiveOrTwenty),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredOneHundredFifty),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredFiftyTwentyFive),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredFiftyTwentyFiveOrTwenty),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredFiftyFifty),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DependentCoverage),
              Option = nameof(BenefitOptionValue.CoveragePercentageOneHundredOneHundredOneHundred),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DisabilityIncomeAssociation),
              Option = nameof(BenefitOptionValue.DisabilityIncomeAssociation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DisabilityIncomeProfessionalEmployerOrganization),
              Option = nameof(BenefitOptionValue.DisabilityIncomeProfessionalEmployerOrganization),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.DisabilityIncomeZeroCompensation),
              Option = nameof(BenefitOptionValue.DisabilityIncomeZeroCompensation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.ZeroDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.OneYear),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.OneHundredTwentyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.FourteenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.FifteenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.OneHundredEightyConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.OneHundredEightyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.TwoYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.ThirtyConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.ThirtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.FiveYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.SixtyConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.SixtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.SevenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.SevenHundredThirtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.NinetyConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.EliminationPeriod),
              Option = nameof(BenefitOptionValue.NinetyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.Individual),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.IndividualSpouse),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.CoverageType),
              Option = nameof(BenefitOptionValue.IndividualChild),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.FlatAmount),
              Option = nameof(BenefitOptionValue.FlatAmount),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.FutureIncreaseOption),
              Option = nameof(BenefitOptionValue.FutureIncreaseOption),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.HospitalBenefit),
              Option = nameof(BenefitOptionValue.HospitalBenefit),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.HospitalConfinement),
              Option = nameof(BenefitOptionValue.HospitalConfinement),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.HospitalIndemnityHalfCompensation),
              Option = nameof(BenefitOptionValue.HospitalIndemnityHalfCompensation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.HospitalIndemnityNonEmployer),
              Option = nameof(BenefitOptionValue.HospitalIndemnityNonEmployer),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.HospitalIndemnityProfessionalEmployerOrganization),
              Option = nameof(BenefitOptionValue.HospitalIndemnityProfessionalEmployerOrganization),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.HospitalIndemnityZeroCompensation),
              Option = nameof(BenefitOptionValue.HospitalIndemnityZeroCompensation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.IndustryClass),
              Option = nameof(BenefitOptionValue.OccupationClass1),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.IndustryClass),
              Option = nameof(BenefitOptionValue.OccupationClass2),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.IndustryClass),
              Option = nameof(BenefitOptionValue.OccupationClass3),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.LifetimeAccident),
              Option = nameof(BenefitOptionValue.LifetimeAccident),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.LivingBenefit),
              Option = nameof(BenefitOptionValue.LivingBenefit),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.LongTermPremiumRefund),
              Option = nameof(BenefitOptionValue.LongTermPremiumRefund),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.LossRatio),
              Option = nameof(BenefitOptionValue.LossRatio),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.MajorMedical),
              Option = nameof(BenefitOptionValue.MajorMedical),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass1),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass1And2),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass1A),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass2),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass2A),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass3),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass3A),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass4),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClass4A),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClassA),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClassB),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClassC),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClassExcellent),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClassNormal),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClassRegular),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.OccupationClass),
              Option = nameof(BenefitOptionValue.OccupationClassSuperior),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PartialDisbursement),
              Option = nameof(BenefitOptionValue.PartialDisbursement),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyCoverage),
              Option = nameof(BenefitOptionValue.TwentyFourHour),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyCoverage),
              Option = nameof(BenefitOptionValue.HospitalIndemnityAdmission),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyCoverage),
              Option = nameof(BenefitOptionValue.HospitalIndemnityAdmissionPlus),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyCoverage),
              Option = nameof(BenefitOptionValue.OffTheJob),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyFee),
              Option = nameof(BenefitOptionValue.PolicyFee),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyFee),
              Option = nameof(BenefitOptionValue.PolicyFee2000),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyFee),
              Option = nameof(BenefitOptionValue.PolicyFee400),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyFee),
              Option = nameof(BenefitOptionValue.PolicyFee560),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyFee),
              Option = nameof(BenefitOptionValue.PolicyFee600),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyFee),
              Option = nameof(BenefitOptionValue.PolicyFee700),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyFee),
              Option = nameof(BenefitOptionValue.PolicyFee800),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyType),
              Option = nameof(BenefitOptionValue.CancerAndSpecifiedDisease),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PolicyType),
              Option = nameof(BenefitOptionValue.CancerOnly),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PregnancyAmendment),
              Option = nameof(BenefitOptionValue.PregnancyAmendment),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.PremiumRefund),
              Option = nameof(BenefitOptionValue.PremiumRefund),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.TenThousandToHundredThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.TenThousandToFiftyThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.TwentyFiveHundredToTenThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.TwentyFiveHundredToFiftenThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.ThirtyFiveHundredToTwentyFiveThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.ThirtyFiveHundredToFiftyThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.FiveThousandToTwentyThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.FiveThousandToTwentyFiveThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.FiveThousandToThirtyThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.FiveThousandToFiftyThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.FiftyFiveHundredToHundredThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.RadiationChemotherapyBenefit),
              Option = nameof(BenefitOptionValue.FiftyFiveHundredToFiftyThousandDollars),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Benefit),
              Option = nameof(BenefitOptionValue.ReducedBenefitAtSeventy),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Benefit),
              Option = nameof(BenefitOptionValue.ReturnOfPremium),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Benefit),
              Option = nameof(BenefitOptionValue.ReturnOfPremiumAndReducedBenefitAt70),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ShortTermPremiumRefund),
              Option = nameof(BenefitOptionValue.ShortTermPremiumRefund),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.ZeroYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.OneYear),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.TenYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.FifteenMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.EighteenMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.OneHundredEightyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.TwoYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.ThirtyYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.FiveYears),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.SixMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.NinetyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessBenefitPeriod),
              Option = nameof(BenefitOptionValue.ToAge65),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.ZeroConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.ZeroDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.OneDay),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.FourteenConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.FourteenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.EighteenMonths),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.OneHundredEightyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.TwoDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.ThreeDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.ThirtyConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.ThirtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.ThreeHundredSixtyFiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.FourDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.SixtyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.SevenConsecutiveDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.SevenDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SicknessEliminationPeriod),
              Option = nameof(BenefitOptionValue.NinetyDays),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SixMonthPreexist),
              Option = nameof(BenefitOptionValue.SixMonthPreexist),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.SupplementalHospitalIndemnity),
              Option = nameof(BenefitOptionValue.SupplementalHospitalIndemnity),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Combined),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Employee),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.FemaleNonTobaccoFemaleNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.FemaleNonTobaccoFemaleTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.FemaleTobaccoFemaleTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.HomeOffice),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.HomeOfficeDiscount),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.HomeOfficeDiscountNonTobacco),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.HomeOfficeDiscountTobacco),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.HomeOfficeNonTobacco),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.HomeOfficeTobacco),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Insured),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Joint),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.JointStandardNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Juvenile),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.JuvenileComposite),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.MaleNonTobaccoFemaleNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.MaleNonTobaccoFemaleTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.MaleNonTobaccoMaleNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.MaleNonTobaccoMaleTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.MaleTobaccoFemaleNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.MaleTobaccoFemaleTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.MaleTobaccoMaleTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.NonMedNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.NonMedTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.NonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.OtherInsured),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.PreferredNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.PreferredNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.PreferredPlusNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.PreferredPlusNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.PreferredTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Rated),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.ReducedPaidUpNonTobacco),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.ReducedPaidUpTobacco),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.SelectNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.SelectNonTobaccoVegetarian),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.SelectPlusNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.SelectPlusTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.SelectTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Spouse),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Standard),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.StandardNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.StandardNonTobaccoVegetarian),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.StandardPlusNonTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.StandardTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Substandard),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.SuperPreferred),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.Tobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnderwritingClass),
              Option = nameof(BenefitOptionValue.UniversalTobacco),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.UnionAssociation),
              Option = nameof(BenefitOptionValue.UnionAssociation),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Units),
              Option = nameof(BenefitOptionValue.OneUnit),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Units),
              Option = nameof(BenefitOptionValue.TwoUnits),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Units),
              Option = nameof(BenefitOptionValue.ThreeUnits),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.Units),
              Option = nameof(BenefitOptionValue.FourUnits),
              HideBenefitOption = false
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.OneThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.TenThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.TwentyFiveThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.FiftyThousandDollars),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.OneUnit25K10K5K),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.OneUnit50K20K10K),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.TwoUnits100K40K10K),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.ValuePerUnit),
              Option = nameof(BenefitOptionValue.TwoUnits50K25K10K),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.WaitingPeriod),
              Option = nameof(BenefitOptionValue.ThirtyDays),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.WaivePreexistingCondition),
              Option = nameof(BenefitOptionValue.WaivePreexistingCondition),
              HideBenefitOption = true
          },
          new BenefitOptionsMapping
          {
              Category = nameof(BenefitOptionName.WaiverOfPremium),
              Option = nameof(BenefitOptionValue.WaiverOfPremium),
              HideBenefitOption = true
          },
        };
    }
}
