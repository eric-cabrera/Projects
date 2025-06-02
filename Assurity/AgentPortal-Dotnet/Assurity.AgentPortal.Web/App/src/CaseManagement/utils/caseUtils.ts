export const FindAITCoverageType = function (value: string) {
  switch (value) {
    case "TwentyFourHour":
      return "24-hour";
    case "OffTheJob":
      return "Off The Job";
    default:
      return value;
  }
};

export const FindAccidentalDeathCoveragePeriod = function (value: string) {
  switch (value) {
    case "FiveYear":
      return "5 Years";
    case "SevenYear":
      return "7 Years";
    case "TenYear":
      return "10 Years";
    case "FifteenYear":
      return "15 Years";
    case "TwentyYear":
      return "20 Years";
    case "ThirtyYear":
      return "30 Years";
    case "ToAge80":
      return "To Age 80";
    default:
      return value;
  }
};

export const FindTermLifeTermPeriod = function (value: string) {
  switch (value) {
    case "TenYears":
      return "10 Years";
    case "FifteenYears":
      return "15 Years";
    case "TwentyYears":
      return "20 Years";
    case "ThirtyYears":
      return "30 Years";
    default:
      return value;
  }
};

export const FindIncomeProtectionBenefitPeriod = function (value: string) {
  switch (value) {
    case "ThirteenWeeks":
      return "13 Weeks";
    case "TwentySixWeeks":
      return "26 Weeks";
    case "OneYear":
      return "1 Year";
    case "TwoYears":
      return "2 Years";
    case "FiveYears":
      return "5 Years";
    case "TenYears":
      return "10 Years";
  }
};

export const FindIncomeProtectionEliminationPeriod = function (value: string) {
  switch (value) {
    case "ZeroAccidentDays":
      return "0 Accident Days";
    case "SevenAccidentDays":
      return "7 Accident Days";
    case "FourteenAccidentDays":
      return "14 Accident Days";
    case "ThirtyAccidentDays":
      return "30 Accident Days";
    case "SixtyAccidentDays":
      return "60 Accident Days";
    case "NinetyAccidentDays":
      return "90 Accident Days";
    case "ZeroAccidentSevenSicknessDays":
      return "0 Accident 7 Sickness Days";
    case "ZeroAccidentFourteenSicknessDays":
      return "0 Accident 14 Sickness Days";
    case "SevenAccidentSevenSicknessDays":
      return "7 Accident 7 Sickness Days";
    case "FourteenAccidentFourteenSicknessDays":
      return "14 Accident 14 Sickness Days";
    case "ThirtyAccidentThirtySicknessDays":
      return "30 Accident 30 Sickness Days";
    case "SixtyAccidentSixtySicknessDays":
      return "60 Accident 60 Sickness Days";
    case "NinetyAccidentNinetySicknessDays":
      return "90 Accident 90 Sickness Days";
  }
};

export const FindCenturyPlusTermPeriod = function (value: string) {
  switch (value) {
    case "OneYear":
      return "1 Year";
    case "TwoYears":
      return "2 Year";
    case "FiveYears":
      return "5 Year";
    case "TenYears":
      return "10 Year";
    case "ToAge65":
      return "To Age 65";
    case "ToAge67":
      return "To Age 67";
    default:
      return value;
  }
};

export const FindCenturyPlusEliminationPeriod = function (value: string) {
  switch (value) {
    case "ThirtyDays":
      return "30 Day";
    case "SixtyDays":
      return "60 Day";
    case "NinetyDays":
      return "90 Day";
    case "OneHundredEightyDays":
      return "180 Day";
    case "ThreeHundredSixtyFiveDays":
      return "365 Day";
  }
};
