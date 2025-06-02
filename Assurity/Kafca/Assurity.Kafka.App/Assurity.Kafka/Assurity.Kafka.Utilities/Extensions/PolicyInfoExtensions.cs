namespace Assurity.Kafka.Utilities.Extensions
{
    using System;
    using System.Reflection;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using NewRelic.Api.Agent;

    public static class PolicyInfoExtensions
    {
        public static Status ToBenefitStatus(
            this string? statusCode,
            string? statusReason)
        {
            switch (statusCode)
            {
                case "A":
                case "S":
                    return Status.Active;
                case "T":
                    return Status.Terminated;
                case "P":
                    return Status.Pending;
            }

            return Status.Terminated; // Default should never be hit. Only base benefits of sequence 0 or UV benefits of sequence 99 are unpopulated
        }

        public static StatusReason ToBenefitStatusReason(
            this string? statusReason,
            string? statusCode)
        {
            if (string.IsNullOrWhiteSpace(statusReason))
            {
                return StatusReason.None;
            }

            switch (statusReason)
            {
                case "RI":
                    if (statusCode == "A")
                    {
                        return StatusReason.Reinstated;
                    }
                    else if (statusCode == "P")
                    {
                        return StatusReason.ReadyToIssue;
                    }
                    else
                    {
                        return StatusReason.Reissued;
                    }

                case "RS":
                    return statusCode == "A"
                        ? StatusReason.Restored
                        : StatusReason.Reserved;
                case "AN":
                    return StatusReason.AnniversaryProcessingError;
                case "BE":
                    return StatusReason.BillingError;
                case "CP":
                    return StatusReason.ConversionPending;
                case "DP":
                    return StatusReason.DeathPending;
                case "HO":
                    return StatusReason.HomeOfficeSuspended;
                case "LP":
                    return statusCode == "S"
                        ? StatusReason.LapsePending
                        : StatusReason.Lapsed;
                case "MP":
                    return StatusReason.ModeaversaryProcessingError;
                case "MX":
                    return StatusReason.MonthaversaryProcessingError;
                case "PC":
                    return StatusReason.PolicyChange;
                case "RP":
                    return statusCode == "S"
                        ? StatusReason.RenewalPending
                        : StatusReason.Replaced;
                case "SP":
                    return StatusReason.SurrenderPending;
                case "WP":
                    return StatusReason.WaiverPending;
                case "NI":
                    return StatusReason.NotIssued;
                case "NT":
                case "NN":
                case "NR":
                    return StatusReason.NotTaken;
                case "PO":
                    return StatusReason.Postponed;
                case "SR":
                    return StatusReason.Surrendered;
                case "WI":
                    return StatusReason.Withdrawn;
                case "IC":
                case "MB":
                case "RW":
                    return StatusReason.Incomplete;
                case "SM":
                    return StatusReason.Submitted;
                case "CA":
                    return StatusReason.Canceled;
                case "CE":
                    return StatusReason.ETIConversion;
                case "CR":
                    return StatusReason.RPIConversion;
                case "CV":
                    return StatusReason.Conversion;
                case "DC":
                    return StatusReason.DeathClaim;
                case "DE":
                case "DH":
                case "DN":
                case "DO":
                case "DR":
                case "DS":
                    return StatusReason.Declined;
                case "EB":
                    return StatusReason.ExchangeBenefit;
                case "EX":
                    return StatusReason.Expire;
                case "MA":
                    return StatusReason.Matured;
                case "MM":
                    return StatusReason.MPRtoMDVConversion;
                case "UC":
                    return StatusReason.UnappliedCash;
                case "RE":
                    return StatusReason.ReturnedEft;
                case "IE":
                case "ID":
                case "IH":
                case "IJ":
                case "IM":
                case "IP":
                case "IR":
                case "IS":
                case "IW":
                case "IY":
                case "RJ":
                    return StatusReason.Ineligible;
                case "DU":
                    return StatusReason.DeclinedFullUnd;
                case "ER":
                    return StatusReason.Error;
                case "PW":
                    return StatusReason.WithdrawnPayor;
                case "IA":
                    return StatusReason.IneligibleAge;
                case "OW":
                    return StatusReason.WithdrawnOwner;
                case "ON":
                    return StatusReason.NotTakenOwner;
                case "RC":
                    return StatusReason.ReturnedCheck;
                case "PN":
                    return StatusReason.NotTakenPayor;
                case "RL":
                    return StatusReason.ReplacementPending;
                default:
                    return StatusReason.Unknown;
            }
        }

        public static BillingForm ToBillingForm(this string? billingForm)
        {
            switch (billingForm)
            {
                case "GVT":
                    return BillingForm.GovernmentAllotment;

                case "CRD":
                    return BillingForm.CreditCard;

                case "DIR":
                    return BillingForm.Direct;

                case "PAC":
                    return BillingForm.AutomaticBankWithdrawal;

                case "LST":
                    return BillingForm.ListBill;

                case "":
                    return BillingForm.None;

                default:
                    return BillingForm.Unknown;
            }
        }

        public static BillingMode ToBillingMode(
            this short billingMode,
            string? specialMode)
        {
            switch (billingMode)
            {
                case 0:
                    return BillingMode.None;

                case 1:
                    if (specialMode == null)
                    {
                        return BillingMode.Monthly;
                    }

                    var ninthlySpecialModeList = new List<string>
                    {
                        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"
                    };

                    var tenthlySpecialModeList = new List<string>
                    {
                        "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X"
                    };

                    if (ninthlySpecialModeList.Contains(specialMode))
                    {
                        return BillingMode.Ninthly;
                    }
                    else if (tenthlySpecialModeList.Contains(specialMode))
                    {
                        return BillingMode.Tenthly;
                    }
                    else
                    {
                        return BillingMode.Monthly;
                    }

                case 3:
                    return BillingMode.Quarterly;

                case 6:
                    return BillingMode.SemiAnnually;

                case 7:
                    return BillingMode.Weekly;

                case 9:
                    return BillingMode.Ninthly;

                case 10:
                    return BillingMode.Tenthly;

                case 12:
                    if (specialMode == "1")
                    {
                        return BillingMode.Calendar;
                    }
                    else
                    {
                        return BillingMode.Annually;
                    }

                case 13:
                    return BillingMode.Thirteenthly;

                case 14:
                    return BillingMode.Biweekly;

                case 26:
                    return BillingMode.TwentySixPay;

                case 52:
                    return BillingMode.FiftyTwoPay;

                default:
                    return BillingMode.Unknown;
            }
        }

        public static BillingStatus ToBillingCode(this string? billingCode)
        {
            switch (billingCode)
            {
                case "A":
                    return BillingStatus.Active;

                case "H":
                    return BillingStatus.HoldBilling;

                case "S":
                    return BillingStatus.Suspended;

                case "":
                    return BillingStatus.None;

                default:
                    return BillingStatus.Unknown;
            }
        }

        public static BillingReason ToBillingReason(this string? billingReason)
        {
            switch (billingReason)
            {
                case "PC":
                    return BillingReason.PolicyChange;
                case "SP":
                    return BillingReason.PaidUp;
                case "VB":
                    return BillingReason.VanishBase;
                case "VN":
                    return BillingReason.Vanish;
                case "WD":
                    return BillingReason.WaiverDisability;
                case "AJ":
                    return BillingReason.Adjustment;
                case "BE":
                    return BillingReason.BillingError;
                case "DP":
                    return BillingReason.DeathPending;
                case "EP":
                    return BillingReason.EarlyPaidUp;
                case "ET":
                    return BillingReason.ExtendedTerm;
                case "NF":
                    return BillingReason.NonForfeiture;
                case "PU":
                    return BillingReason.PaidUp;
                case "RU":
                    return BillingReason.ReducedPaidUp;
                case "ST":
                    return BillingReason.StoppedPremium;
                case "WP":
                    return BillingReason.WaiverPending;
                case "MA":
                case "MD":
                case "ZP":
                case "":
                    return BillingReason.None;
                default:
                    return BillingReason.Unknown;
            }
        }

        public static Country? ToCountry(this string? country)
        {
            switch (country?.Trim().ToUpper())
            {
                case "AFGHANISTAN":
                    return Country.AFGHANISTAN;
                case "ALBANIA":
                    return Country.ALBANIA;
                case "ALGERIA":
                    return Country.ALGERIA;
                case "AMERICAN SAMOA":
                    return Country.AMERICAN_SAMOA;
                case "ANDORRA":
                    return Country.ANDORRA;
                case "ANGOLA":
                    return Country.ANGOLA;
                case "ANGUILLA":
                    return Country.ANGUILLA;
                case "ANTARCTICA":
                    return Country.ANTARCTICA;
                case "ANTIGUA BARBUDA":
                    return Country.ANTIGUA_BARBUDA;
                case "ARGENTINA":
                    return Country.ARGENTINA;
                case "ARMENIA":
                    return Country.ARMENIA;
                case "ARUBA":
                    return Country.ARUBA;
                case "ASCENSION ISLANDS":
                    return Country.ASCENSION_ISLANDS;
                case "AUSTRALIA":
                case "AUSTRAILIA":
                    return Country.AUSTRALIA;
                case "AUSTRIA":
                    return Country.AUSTRIA;
                case "AZERBAIJAN":
                    return Country.AZERBAIJAN;
                case "BAHAMAS":
                    return Country.BAHAMAS;
                case "BAHRAIN":
                    return Country.BAHRAIN;
                case "BANGLADESH":
                    return Country.BANGLADESH;
                case "BARBADOS":
                    return Country.BARBADOS;
                case "BELARUS":
                    return Country.BELARUS;
                case "BELGIUM":
                    return Country.BELGIUM;
                case "BELIZE":
                    return Country.BELIZE;
                case "BENIN":
                    return Country.BENIN;
                case "BERMUDA":
                    return Country.BERMUDA;
                case "BHUTAN":
                    return Country.BHUTAN;
                case "BOLIVIA":
                    return Country.BOLIVIA;
                case "BOSNIA HERZEGOVINA":
                    return Country.BOSNIA_HERZEGOVINA;
                case "BOTSWANA":
                    return Country.BOTSWANA;
                case "BOUVET ISLAND":
                    return Country.BOUVET_ISLAND;
                case "BRAZIL":
                    return Country.BRAZIL;
                case "BRITISH INDIAN OCNTERR":
                    return Country.BRITISH_INDIAN_OCNTERR;
                case "BRUNEI":
                    return Country.BRUNEI;
                case "BULGARIA":
                    return Country.BULGARIA;
                case "BURKINA FASO":
                    return Country.BURKINA_FASO;
                case "BURMA":
                    return Country.BURMA;
                case "BURUNDI":
                    return Country.BURUNDI;
                case "CAMBODIA":
                    return Country.CAMBODIA;
                case "CAMEROON":
                    return Country.CAMEROON;
                case "CANADA":
                case "CANANDA":
                    return Country.CANADA;
                case "CAPEVERDE ISLAND":
                    return Country.CAPEVERDE_ISLAND;
                case "CAYMAN IS":
                    return Country.CAYMAN_IS;
                case "CENTRAL AFRICAN REP":
                    return Country.CENTRAL_AFRICAN_REP;
                case "CHAD":
                    return Country.CHAD;
                case "CHILE":
                    return Country.CHILE;
                case "CHINA":
                    return Country.CHINA;
                case "CHRISTMAS ISLANDS":
                    return Country.CHRISTMAS_ISLANDS;
                case "COCOA ISLANDS":
                    return Country.COCOA_ISLANDS;
                case "COCOS":
                    return Country.COCOS;
                case "COCOS KEELING":
                    return Country.COCOS_KEELING;
                case "COLUMBIA":
                case "COLOMBIA":
                    return Country.COLUMBIA;
                case "COMOROS":
                    return Country.COMOROS;
                case "CONGO":
                    return Country.CONGO;
                case "CONGO DEM REP":
                    return Country.CONGO_DEM_REP;
                case "COOK ISLANDS":
                    return Country.COOK_ISLANDS;
                case "COSTA RICA":
                    return Country.COSTA_RICA;
                case "COTE DIVORIE":
                    return Country.COTE_DIVORIE;
                case "CROATIA":
                    return Country.CROATIA;
                case "CURACAO":
                    return Country.CURACAO;
                case "CUBA":
                    return Country.CUBA;
                case "CYPRUS":
                    return Country.CYPRUS;
                case "CZECH REPUBLIC":
                    return Country.CZECH_REPUBLIC;
                case "DENMARK":
                    return Country.DENMARK;
                case "DJIBOUTI":
                    return Country.DJIBOUTI;
                case "DOMINICA":
                    return Country.DOMINICA;
                case "DOMINICAN REPUBLIC":
                    return Country.DOMINICAN_REPUBLIC;
                case "ECUADOR":
                    return Country.ECUADOR;
                case "EGYPT":
                    return Country.EGYPT;
                case "EL SALVADOR":
                    return Country.EL_SALVADOR;
                case "EQUATORIAL GUINEA":
                    return Country.EQUATORIAL_GUINEA;
                case "ERITREA":
                    return Country.ERITREA;
                case "ESTONIA":
                    return Country.ESTONIA;
                case "ETHIOPIA":
                    return Country.ETHIOPIA;
                case "FALKLAND ISLANDS":
                    return Country.FALKLAND_ISLANDS;
                case "FAEROE ISLANDS":
                    return Country.FAEROE_ISLANDS;
                case "FIJI":
                    return Country.FIJI;
                case "FINLAND":
                    return Country.FINLAND;
                case "FRANCE":
                    return Country.FRANCE;
                case "FRENCH GUIANA":
                    return Country.FRENCH_GUIANA;
                case "FRENCH POLYNESIA":
                    return Country.FRENCH_POLYNESIA;
                case "FRENCH SOUTHERN TERRITORIES":
                    return Country.FRENCH_SOUTHERN_TERRITORIES;
                case "GABON":
                    return Country.GABON;
                case "GAMBIA":
                    return Country.GAMBIA;
                case "GEORGIA":
                    return Country.GEORGIA;
                case "GERMANY":
                    return Country.GERMANY;
                case "GHANA":
                    return Country.GHANA;
                case "GIBRALTAR":
                    return Country.GIBRALTAR;
                case "GREECE":
                    return Country.GREECE;
                case "GREENLAND":
                    return Country.GREENLAND;
                case "GRENADA":
                    return Country.GRENADA;
                case "GUADALOUPE":
                    return Country.GUADALOUPE;
                case "GUAM":
                    return Country.GUAM;
                case "GUATEMALA":
                    return Country.GUATEMALA;
                case "GUERNSEY":
                    return Country.GUERNSEY;
                case "GUINEA":
                    return Country.GUINEA;
                case "GUINEA BISSAU":
                    return Country.GUINEA_BISSAU;
                case "GUYANA":
                    return Country.GUYANA;
                case "HAITI":
                    return Country.HAITI;
                case "HEARD ISLAND":
                    return Country.HEARD_ISLAND;
                case "HOLY SEE":
                    return Country.HOLY_SEE;
                case "HONDURAS":
                    return Country.HONDURAS;
                case "HONG KONG":
                    return Country.HONG_KONG;
                case "HUNGARY":
                    return Country.HUNGARY;
                case "ICELAND":
                    return Country.ICELAND;
                case "INDIA":
                    return Country.INDIA;
                case "INDONESIA":
                    return Country.INDONESIA;
                case "IRAN":
                    return Country.IRAN;
                case "IRAQ":
                    return Country.IRAQ;
                case "IRELAND":
                    return Country.IRELAND;
                case "ISLE OF MAN":
                    return Country.ISLE_OF_MAN;
                case "ISRAEL":
                    return Country.ISRAEL;
                case "ITALY":
                    return Country.ITALY;
                case "IVORY COAST":
                    return Country.IVORY_COAST;
                case "JAMAICA":
                    return Country.JAMAICA;
                case "JAPAN":
                    return Country.JAPAN;
                case "JERSEY":
                    return Country.JERSEY;
                case "JORDAN":
                    return Country.JORDAN;
                case "KAZAKHSTAN":
                    return Country.KAZAKHSTAN;
                case "KENYA":
                    return Country.KENYA;
                case "KIRIBATI":
                    return Country.KIRIBATI;
                case "KOREA DEM PEOPLE REP":
                    return Country.KOREA_DEM_PEOPLE_REP;
                case "KOREA REPUBLIC":
                    return Country.KOREA_REPUBLIC;
                case "KOSOVO":
                    return Country.KOSOVO;
                case "KUWAIT":
                    return Country.KUWAIT;
                case "KYRGYZSTAN":
                    return Country.KYRGYZSTAN;
                case "LAOS":
                    return Country.LAOS;
                case "LATVIA":
                    return Country.LATVIA;
                case "LEBANON":
                    return Country.LEBANON;
                case "LESOTHO":
                    return Country.LESOTHO;
                case "LIBERIA":
                    return Country.LIBERIA;
                case "LIBYA":
                    return Country.LIBYA;
                case "LIECHTENSTEIN":
                    return Country.LIECHTENSTEIN;
                case "LITHUANIA":
                    return Country.LITHUANIA;
                case "LUXEMBOURG":
                    return Country.LUXEMBOURG;
                case "MACAO":
                    return Country.MACAO;
                case "MACEDONIA":
                    return Country.MACEDONIA;
                case "MADAGASCAR":
                    return Country.MADAGASCAR;
                case "MALAWI":
                    return Country.MALAWI;
                case "MALAYSIA":
                    return Country.MALAYSIA;
                case "MALDIVES":
                    return Country.MALDIVES;
                case "MALI":
                    return Country.MALI;
                case "MALTA":
                    return Country.MALTA;
                case "MARSHALL ISLANDS":
                    return Country.MARSHALL_ISLANDS;
                case "MARTINIQUE":
                    return Country.MARTINIQUE;
                case "MAURITANIA":
                    return Country.MAURITANIA;
                case "MAURITIUS":
                    return Country.MAURITIUS;
                case "MAYOTTE ISLAND":
                    return Country.MAYOTTE_ISLAND;
                case "MEXICO":
                    return Country.MEXICO;
                case "MICRONESIA":
                    return Country.MICRONESIA;
                case "MOLDOVA":
                    return Country.MOLDOVA;
                case "MONACO":
                    return Country.MONACO;
                case "MONGOLIA":
                    return Country.MONGOLIA;
                case "MONTENEGRO":
                    return Country.MONTENEGRO;
                case "MONTSERRAT":
                    return Country.MONTSERRAT;
                case "MOROCCO":
                    return Country.MOROCCO;
                case "MOZAMBIQUE":
                    return Country.MOZAMBIQUE;
                case "MYANMAR":
                    return Country.MYANMAR;
                case "NAMIBIA":
                    return Country.NAMIBIA;
                case "NAURU":
                    return Country.NAURU;
                case "NEPAL":
                    return Country.NEPAL;
                case "NETHERLANDS":
                    return Country.NETHERLANDS;
                case "NETHERLANDS ANTILLES":
                    return Country.NETHERLANDS_ANTILLES;
                case "NEW CALEDONIA":
                    return Country.NEW_CALEDONIA;
                case "NEW ZEALAND":
                    return Country.NEW_ZEALAND;
                case "NICARAGUA":
                    return Country.NICARAGUA;
                case "NIGER":
                    return Country.NIGER;
                case "NIGERIA":
                    return Country.NIGERIA;
                case "NIUE":
                    return Country.NIUE;
                case "NORFOLK ISLAND":
                    return Country.NORFOLK_ISLAND;
                case "NORTH YEMEN":
                    return Country.NORTH_YEMEN;
                case "NORTH MARIANA ISLANDS":
                    return Country.NORTH_MARIANA_ISLANDS;
                case "NORWAY":
                    return Country.NORWAY;
                case "OMAN":
                    return Country.OMAN;
                case "PAKISTAN":
                    return Country.PAKISTAN;
                case "PALAU":
                    return Country.PALAU;
                case "PALESTINE":
                    return Country.PALESTINE;
                case "PANAMA":
                    return Country.PANAMA;
                case "PAPUA NEW GUINEA":
                    return Country.PAPUA_NEW_GUINEA;
                case "PARAGUAY":
                    return Country.PARAGUAY;
                case "PERU":
                    return Country.PERU;
                case "PHILIPPINES":
                case "PHILIPINES":
                    return Country.PHILIPPINES;
                case "PITCARIN ISLANDS":
                    return Country.PITCARIN_ISLANDS;
                case "POLAND":
                    return Country.POLAND;
                case "PORTUGAL":
                    return Country.PORTUGAL;
                case "PUERTO RICO":
                    return Country.PUERTO_RICO;
                case "QATAR":
                    return Country.QATAR;
                case "REUNION ISLAND":
                    return Country.REUNION_ISLAND;
                case "ROMANIA":
                    return Country.ROMANIA;
                case "RUSSIA":
                    return Country.RUSSIA;
                case "RWANDA":
                    return Country.RWANDA;
                case "SAIPAN":
                    return Country.SAIPAN;
                case "SAMOA":
                    return Country.SAMOA;
                case "SAN_MARINO":
                    return Country.SAN_MARINO;
                case "SAO TOME":
                    return Country.SAO_TOME;
                case "SAUDI ARABIA":
                    return Country.SAUDI_ARABIA;
                case "SENEGAL":
                    return Country.SENEGAL;
                case "SERBIA":
                    return Country.SERBIA;
                case "SEYCHELLES":
                    return Country.SEYCHELLES;
                case "SIERRA LEONE":
                    return Country.SIERRA_LEONE;
                case "SINGAPORE":
                    return Country.SINGAPORE;
                case "SLOVAKIA":
                    return Country.SLOVAKIA;
                case "SLOVENIA":
                    return Country.SLOVENIA;
                case "SOLOMON ISLANDS":
                    return Country.SOLOMON_ISLANDS;
                case "SOMALIA":
                    return Country.SOMALIA;
                case "SOUTH AFRICA":
                    return Country.SOUTH_AFRICA;
                case "SOUTH GEORGIA SANDWICH":
                    return Country.SOUTH_GEORGIA_SANDWICH;
                case "SOUTH SUDAN":
                    return Country.SOUTH_SUDAN;
                case "SOUTH YEMEN":
                    return Country.SOUTH_YEMEN;
                case "SPAIN":
                    return Country.SPAIN;
                case "SRI LANKA":
                    return Country.SRI_LANKA;
                case "ST PIERRE":
                    return Country.ST_PIERRE;
                case "ST HELENA":
                    return Country.ST_HELENA;
                case "ST KITTS NEVIS":
                    return Country.ST_KITTS_NEVIS;
                case "ST LUCIA":
                    return Country.ST_LUCIA;
                case "ST VINCENT":
                    return Country.ST_VINCENT;
                case "SUDAN":
                    return Country.SUDAN;
                case "SURINAME":
                    return Country.SURINAME;
                case "SVALBARD ISLAND":
                    return Country.SVALBARD_ISLAND;
                case "SWAZILAND":
                    return Country.SWAZILAND;
                case "SWEDEN":
                    return Country.SWEDEN;
                case "SWITZERLAND":
                    return Country.SWITZERLAND;
                case "SYRIA":
                    return Country.SYRIA;
                case "TAIWAN":
                    return Country.TAIWAN;
                case "TAJIKISTAN":
                    return Country.TAJIKISTAN;
                case "TANZANIA":
                    return Country.TANZANIA;
                case "THAILAND":
                    return Country.THAILAND;
                case "EAST TIMOR":
                    return Country.EAST_TIMOR;
                case "TOGO":
                    return Country.TOGO;
                case "TOKELAU":
                    return Country.TOKELAU;
                case "TONGA ISLANDS":
                    return Country.TONGA_ISLANDS;
                case "TRINIDAD TOBAGO":
                    return Country.TRINIDAD_TOBAGO;
                case "TUNISIA":
                    return Country.TUNISIA;
                case "TURKEY":
                    return Country.TURKEY;
                case "TURKMENISTAN":
                    return Country.TURKMENISTAN;
                case "TURKS CAICOS IS":
                    return Country.TURKS_CAICOS_IS;
                case "TUVALU":
                    return Country.TUVALU;
                case "UGANDA":
                    return Country.UGANDA;
                case "UKRAINE":
                    return Country.UKRAINE;
                case "USSR":
                    return Country.USSR;
                case "UNITED ARAB EMIRATES":
                    return Country.UNITED_ARAB_EMIRATES;
                case "UK":
                case "ENGLAND":
                case "UNITED KINGDOM":
                    return Country.UK;
                case "US MINOR OUTLYING ISLANDS":
                    return Country.US_MINOR_OUTLYING_ISLANDS;
                case "USA":
                case "US":
                case "UNITED STATE":
                case "UNITED STATES":
                case "UNITED STATES OF AMERICA":
                case "UNITEDS TATES":
                    return Country.USA;
                case "URUGUAY":
                    return Country.URUGUAY;
                case "VIRGIN ISLANDS US":
                    return Country.VIRGIN_ISLANDS_US;
                case "UZBEKISTAN":
                    return Country.UZBEKISTAN;
                case "VANUATU":
                    return Country.VANUATU;
                case "VENEZUELA":
                    return Country.VENEZUELA;
                case "VIETNAM":
                    return Country.VIETNAM;
                case "BRITISH VIRGINIS":
                    return Country.BRITISH_VIRGINIS;
                case "WALLIS ISLANDS":
                    return Country.WALLIS_ISLANDS;
                case "WESTERN SARAHA":
                    return Country.WESTERN_SARAHA;
                case "WESTERN SAMOA":
                    return Country.WESTERN_SAMOA;
                case "YEMEN":
                    return Country.YEMEN;
                case "YUGOSLAVIA":
                    return Country.YUGOSLAVIA;
                case "ZAIRE":
                    return Country.ZAIRE;
                case "ZAMBIA":
                    return Country.ZAMBIA;
                case "ZIMBABWE":
                    return Country.ZIMBABWE;
                case "":
                case null:
                    return null;
                default:
                    return Country.UNKNOWN;
            }
        }

        /// <summary>
        /// If possible, this method converts a LifePro integer to a date, but converts 0s to null. Because these integers have no time
        /// component, we default to midnight. Storing only a date in Mongo will likely also append midnight. Should the day come that Mongo
        /// supports storage of only a date, we could consider changing this method to return the DateOnly object. Doing so would require
        /// changing the following fields from DateTime to DateOnly in the contracts: ApplicationDate, IssueDate, SubmitDate, TerminationDate,
        /// Participant.Person.DateOfBirth, Requirement.AddedDate, Requirement.ObtainedDate. Additional light refactoring would also be needed
        /// in this application.
        /// </summary>
        /// <param name="lifeProDate">An 8 digit integer that represents a date. This is the date format often used in LifePro.</param>
        /// <returns>When possible, this returns a date time with a time of midnight, otherwise it returns null.</returns>
        [Trace]
        public static DateTime? ToNullableDateTime(this int lifeProDate)
        {
            var dateAsString = lifeProDate.ToString();
            DateOnly dateOnly;

            if (dateAsString.Length == 8)
            {
                var dateAsFormattedString = $"{dateAsString.Substring(4, 2)}/{dateAsString.Substring(6, 2)}/{dateAsString.Substring(0, 4)}";

                // LifePro stores 'the indeterminite future' sometimes as 99991231 and other times as 99999999.
                // Since 99999999 can't parse to a date or be used in date calculations, we'll convert it to 12/31/9999.
                if (dateAsFormattedString == "99/99/9999")
                {
                    return new DateTime(9999, 12, 31);
                }

                if (DateOnly.TryParse(dateAsFormattedString, out dateOnly))
                {
                    return dateOnly.ToDateTime(TimeOnly.MinValue);
                }
            }

            return null;
        }

        [Trace]
        public static int ToLifeProDateInteger(this int lifeProDate)
        {
            var date = lifeProDate.ToString();
            int convertedLifeProDate;
            if (date.Length == 8)
            {
                int month = int.Parse(date.Substring(0, 2));
                int day = int.Parse(date.Substring(2, 2));
                int year = int.Parse(date.Substring(4, 4));

                convertedLifeProDate = (year * 10000) + (month * 100) + day;
            }
            else if (date.Length == 7)
            {
                int month = int.Parse(date.Substring(0, 1));
                int day = int.Parse(date.Substring(1, 2));
                int year = int.Parse(date.Substring(3, 4));

                convertedLifeProDate = (year * 10000) + (month * 100) + day;
            }
            else
            {
                throw new Exception("Invalid LifePro Date.");
            }

            return convertedLifeProDate;
        }

        public static DateTime? ToNullablePaidToDate(this int lifeProPaidToDate, LineOfBusiness lineOfBusiness)
        {
            switch (lineOfBusiness)
            {
                case LineOfBusiness.UniversalLife:
                case LineOfBusiness.Annuity:
                case LineOfBusiness.ImmediateAnnuity:
                    return null;
            }

            return ToNullableDateTime(lifeProPaidToDate);
        }

        public static int ToLifeProDate(this DateTime dateTime)
        {
            return int.Parse(dateTime.ToString("yyyyMMdd"));
        }

        public static Gender ToGender(this string? gender)
        {
            switch (gender)
            {
                case "M":
                    return Gender.Male;
                case "F":
                    return Gender.Female;
                default:
                    return Gender.Unknown;
            }
        }

        public static LineOfBusiness ToLineOfBusiness(this string? lineOfBusiness)
        {
            switch (lineOfBusiness)
            {
                case "A":
                    return LineOfBusiness.Annuity;
                case "G":
                    return LineOfBusiness.Group;
                case "H":
                    return LineOfBusiness.Health;
                case "I":
                    return LineOfBusiness.ImmediateAnnuity;
                case "L":
                    return LineOfBusiness.TraditionalLife;
                case "S":
                    return LineOfBusiness.TrueGroupCensus;
                case "U":
                    return LineOfBusiness.UniversalLife;
                default:
                    return LineOfBusiness.Unknown;
            }
        }

        public static RelationshipToPrimaryInsured ToRelationshipToPrimaryInsured(
            this string? insured)
        {
            switch (insured)
            {
                case "AUNT":
                    return RelationshipToPrimaryInsured.Aunt;
                case "AUTHORIZED-PERSON":
                    return RelationshipToPrimaryInsured.Authorized_Person;
                case "BROTHER":
                    return RelationshipToPrimaryInsured.Brother;
                case "BUSINESS":
                    return RelationshipToPrimaryInsured.Business;
                case "BUS ASSOCIATE":
                    return RelationshipToPrimaryInsured.Business_Associate;
                case "BUS PARTNER":
                    return RelationshipToPrimaryInsured.Business_Partner;
                case "CHARITY":
                    return RelationshipToPrimaryInsured.Charity;
                case "CHILD":
                    return RelationshipToPrimaryInsured.Child;
                case "CONTINGENT-BENEFICIARY":
                    return RelationshipToPrimaryInsured.Contingent_Beneficiary;
                case "CORPORATION":
                    return RelationshipToPrimaryInsured.Corporation;
                case "COUSIN":
                    return RelationshipToPrimaryInsured.Cousin;
                case "CREDITOR":
                    return RelationshipToPrimaryInsured.Creditor;
                case "CUSTODIAN":
                    return RelationshipToPrimaryInsured.Custodian;
                case "DAUGHTER":
                    return RelationshipToPrimaryInsured.Daughter;
                case "DAUGHTERINLAW":
                    return RelationshipToPrimaryInsured.Daughter_In_Law;
                case "DEPENDENT":
                    return RelationshipToPrimaryInsured.Dependent;
                case "DISBLD DEP":
                    return RelationshipToPrimaryInsured.Disabled_Dependent;
                case "DOMEST PAR":
                    return RelationshipToPrimaryInsured.Domestic_Partner;
                case "EMPLOYEE":
                    return RelationshipToPrimaryInsured.Employee;
                case "EMPLOYER":
                    return RelationshipToPrimaryInsured.Employer;
                case "ESTATE":
                    return RelationshipToPrimaryInsured.Estate;
                case "FATHER":
                    return RelationshipToPrimaryInsured.Father;
                case "FATHERINLAW":
                    return RelationshipToPrimaryInsured.Father_In_Law;
                case "FIANCE":
                    return RelationshipToPrimaryInsured.Fiance;
                case "FIANCEE":
                    return RelationshipToPrimaryInsured.Fiancee;
                case "EX-HUSBAND":
                    return RelationshipToPrimaryInsured.Former_Husband;
                case "EXSPOUSE":
                    return RelationshipToPrimaryInsured.Former_Spouse;
                case "EX-WIFE":
                    return RelationshipToPrimaryInsured.Former_Wife;
                case "FRIEND":
                    return RelationshipToPrimaryInsured.Friend;
                case "GODPARENT":
                    return RelationshipToPrimaryInsured.God_Parent;
                case "GODFATHER":
                    return RelationshipToPrimaryInsured.Godfather;
                case "GODMOTHER":
                    return RelationshipToPrimaryInsured.Godmother;
                case "GRANDCHILD":
                    return RelationshipToPrimaryInsured.Grand_Child;
                case "GRANDPARENT":
                    return RelationshipToPrimaryInsured.Grand_Parent;
                case "GRANDDAUGHTER":
                    return RelationshipToPrimaryInsured.Granddaughter;
                case "GRANDFATHER":
                    return RelationshipToPrimaryInsured.Grandfather;
                case "GRANDMOTHER":
                    return RelationshipToPrimaryInsured.Grandmother;
                case "GRANT TRUST":
                    return RelationshipToPrimaryInsured.Grantor_Trust;
                case "GRANDSON":
                    return RelationshipToPrimaryInsured.Grandson;
                case "GREAT-AUNT":
                    return RelationshipToPrimaryInsured.Great_Aunt;
                case "GREAT-GRANDDAUGHTER":
                    return RelationshipToPrimaryInsured.Great_Granddaughter;
                case "GREAT-GRANDFATHER":
                    return RelationshipToPrimaryInsured.Great_Grandfather;
                case "GREAT-GRANDMOTHER":
                    return RelationshipToPrimaryInsured.Great_Grandmother;
                case "GREAT-GRANDSON":
                    return RelationshipToPrimaryInsured.Great_Grandson;
                case "GREAT-UNCLE":
                    return RelationshipToPrimaryInsured.Great_Uncle;
                case "GUARDIAN":
                    return RelationshipToPrimaryInsured.Guardian;
                case "HALFBROTHER":
                    return RelationshipToPrimaryInsured.Half_Brother;
                case "HALFSISTER":
                    return RelationshipToPrimaryInsured.Half_Sister;
                case "HUSBAND":
                    return RelationshipToPrimaryInsured.Husband;
                case "INSTITUTION":
                    return RelationshipToPrimaryInsured.Institution;
                case "JOINT":
                    return RelationshipToPrimaryInsured.Joint;
                case "MOTHER":
                    return RelationshipToPrimaryInsured.Mother;
                case "MOTHERINLAW":
                    return RelationshipToPrimaryInsured.Mother_In_Law;
                case "NEPHEW":
                    return RelationshipToPrimaryInsured.Nephew;
                case "NIECE":
                    return RelationshipToPrimaryInsured.Niece;
                case "OTHER":
                    return RelationshipToPrimaryInsured.Other;
                case "PARENT":
                    return RelationshipToPrimaryInsured.Parent;
                case "PARTNER":
                    return RelationshipToPrimaryInsured.Partner;
                case "PARTNERSHIP":
                    return RelationshipToPrimaryInsured.Partnership;
                case "POWER-OF-AT":
                    return RelationshipToPrimaryInsured.Power_Of_Attorney;
                case "SELF":
                    return RelationshipToPrimaryInsured.Self;
                case "SIBLING":
                    return RelationshipToPrimaryInsured.Sibling;
                case "SISTER":
                    return RelationshipToPrimaryInsured.Sister;
                case "SISTERINLAW":
                    return RelationshipToPrimaryInsured.Sister_In_Law;
                case "SOLE-PROPRIETORSHIP":
                    return RelationshipToPrimaryInsured.Sole_Proprietorship;
                case "SON":
                    return RelationshipToPrimaryInsured.Son;
                case "SONINLAW":
                    return RelationshipToPrimaryInsured.Son_In_Law;
                case "SPOUSE":
                    return RelationshipToPrimaryInsured.Spouse;
                case "STEPBROTHER":
                    return RelationshipToPrimaryInsured.Step_Brother;
                case "STEPCHILD":
                    return RelationshipToPrimaryInsured.Step_Child;
                case "STEPDAUGHTER":
                    return RelationshipToPrimaryInsured.Step_Daughter;
                case "STEP-FATHER":
                    return RelationshipToPrimaryInsured.Step_Father;
                case "STEP-MOTHER":
                    return RelationshipToPrimaryInsured.Step_Mother;
                case "STEP-PARENT":
                    return RelationshipToPrimaryInsured.Step_Parent;
                case "STEPSIBLING":
                    return RelationshipToPrimaryInsured.Step_Sibling;
                case "STEPSISTER":
                    return RelationshipToPrimaryInsured.Step_Sister;
                case "STEPSON":
                    return RelationshipToPrimaryInsured.Step_Son;
                case "TRUST":
                    return RelationshipToPrimaryInsured.Trust;
                case "TRUSTEE":
                    return RelationshipToPrimaryInsured.Trustee;
                case "UNCLE":
                    return RelationshipToPrimaryInsured.Uncle;
                case "UNKNOWN":
                    return RelationshipToPrimaryInsured.Unknown;
                case "WIFE":
                    return RelationshipToPrimaryInsured.Wife;
                default:
                    return RelationshipToPrimaryInsured.Unknown;
            }
        }

        public static AnnuitantType ToAnnuitantType(this string? relateCode)
        {
            switch (relateCode)
            {
                case "A1":
                    return AnnuitantType.Primary;
                case "A2":
                    return AnnuitantType.Secondary;
                case "A3":
                    return AnnuitantType.Tertiary;
            }

            return AnnuitantType.Primary;
        }

        public static BeneficiaryType ToBeneficiaryType(this string? relateCode)
        {
            switch (relateCode)
            {
                case "B1":
                    return BeneficiaryType.Primary;
                case "B2":
                    return BeneficiaryType.Contingent;
                case "ZT":
                    return BeneficiaryType.Tertiary;
                case "ZU":
                    return BeneficiaryType.Quaternary;
                case "J1":
                    return BeneficiaryType.JointInsuredPrimary;
                case "J2":
                    return BeneficiaryType.JointInsuredContingent;
                case "XP":
                    return BeneficiaryType.PerStirpesPrimary;
                case "XC":
                    return BeneficiaryType.PerStirpesContingent;
                case "XT":
                    return BeneficiaryType.PerStirpesTertiary;
            }

            return BeneficiaryType.Primary;
        }

        public static OwnerType ToOwnerType(this string? relateCode)
        {
            switch (relateCode)
            {
                case "PO":
                    return OwnerType.Primary;
                case "O1":
                    return OwnerType.Additional;
                case "ZC":
                    return OwnerType.Contingent;
            }

            return OwnerType.Primary;
        }

        public static PayorType ToPayorType(this string? relateCode)
        {
            switch (relateCode)
            {
                case "PA":
                    return PayorType.Primary;
                case "P1":
                    return PayorType.Additional;
                case "UL":
                case "U2":
                    return PayorType.SecondaryAddressee;
            }

            return PayorType.Primary;
        }

        public static OwnerType ToRelateCode(this string? relateCode)
        {
            switch (relateCode)
            {
                case "PO":
                    return OwnerType.Primary;
                case "O1":
                    return OwnerType.Additional;
                case "ZC":
                    return OwnerType.Contingent;
            }

            return OwnerType.Contingent;
        }

        public static Status ToPolicyStatus(
            this string? contractCode)
        {
            switch (contractCode)
            {
                case "A":
                case "S":
                    return Status.Active;
                case "T":
                    return Status.Terminated;
                case "P":
                    return Status.Pending;
            }

            return Status.Terminated; // not sure what the defualt return should be
        }

        public static State? ToState(this string? stateAbbreviation)
        {
            switch (stateAbbreviation)
            {
                case "AK":
                    return State.AK;
                case "AL":
                    return State.AL;
                case "AR":
                    return State.AR;
                case "AZ":
                    return State.AZ;
                case "CA":
                    return State.CA;
                case "CO":
                    return State.CO;
                case "CT":
                    return State.CT;
                case "DC":
                    return State.DC;
                case "DE":
                    return State.DE;
                case "FL":
                    return State.FL;
                case "GA":
                    return State.GA;
                case "HI":
                    return State.HI;
                case "IA":
                    return State.IA;
                case "ID":
                    return State.ID;
                case "IL":
                    return State.IL;
                case "IN":
                    return State.IN;
                case "KS":
                    return State.KS;
                case "KY":
                    return State.KY;
                case "LA":
                    return State.LA;
                case "MA":
                    return State.MA;
                case "MD":
                    return State.MD;
                case "MI":
                    return State.MI;
                case "MN":
                    return State.MN;
                case "MO":
                    return State.MO;
                case "MS":
                    return State.MS;
                case "MT":
                    return State.MT;
                case "NC":
                    return State.NC;
                case "ND":
                    return State.ND;
                case "NE":
                    return State.NE;
                case "NH":
                    return State.NH;
                case "NJ":
                    return State.NJ;
                case "NM":
                    return State.NM;
                case "NV":
                    return State.NV;
                case "NY":
                    return State.NY;
                case "OH":
                    return State.OH;
                case "OK":
                    return State.OK;
                case "OR":
                    return State.OR;
                case "PA":
                    return State.PA;
                case "PR":
                    return State.PR;
                case "RI":
                    return State.RI;
                case "SC":
                    return State.SC;
                case "SD":
                    return State.SD;
                case "TN":
                    return State.TN;
                case "TX":
                    return State.TX;
                case "UT":
                    return State.UT;
                case "VA":
                    return State.VA;
                case "VI":
                    return State.VI;
                case "VT":
                    return State.VT;
                case "WA":
                    return State.WA;
                case "WI":
                    return State.WI;
                case "WV":
                    return State.WV;
                case "WY":
                    return State.WY;
                default:
                    return null;
            }
        }

        public static StatusReason ToPolicyStatusReason(
            this string? contractReason,
            string? contractCode)
        {
            if (string.IsNullOrWhiteSpace(contractReason))
            {
                return StatusReason.None;
            }

            switch (contractReason)
            {
                case "AE":
                    return StatusReason.AccumAPLError;
                case "AL":
                    return StatusReason.AllocationError;
                case "AM":
                    return StatusReason.Amendment;
                case "AN":
                    return StatusReason.AnniversaryProcessingError;
                case "AP":
                    return StatusReason.Approved;
                case "AU":
                    return StatusReason.AutoNumbered;
                case "BE":
                    return StatusReason.BillingError;
                case "CA":
                    return StatusReason.Canceled;
                case "CE":
                    return StatusReason.ETIConversion;
                case "CP":
                    return StatusReason.ConversionPending;
                case "CR":
                    return StatusReason.RPIConversion;
                case "CV":
                    return StatusReason.Conversion;
                case "DC":
                    return StatusReason.DeathClaim;
                case "DE":
                case "DH":
                case "DN":
                case "DO":
                case "DR":
                case "DS":
                    return StatusReason.Declined;
                case "DF":
                    return StatusReason.DeferredPolicy;
                case "DP":
                    return StatusReason.DeathPending;
                case "DU":
                    return StatusReason.DeclinedFullUnd;
                case "EB":
                    return StatusReason.ExchangeBenefit;
                case "ER":
                    return StatusReason.Error;
                case "EX":
                    return StatusReason.Expire;
                case "HO":
                    return StatusReason.HomeOfficeSuspended;
                case "IA":
                    return StatusReason.IneligibleAge;
                case "IB":
                    return StatusReason.IneligibleNotMember;
                case "IC":
                case "MB":
                case "RW":
                    return StatusReason.Incomplete;
                case "IE":
                case "ID":
                case "IH":
                case "IJ":
                case "IM":
                case "IP":
                case "IR":
                case "IS":
                case "IW":
                case "IY":
                case "RJ":
                    return StatusReason.Ineligible;
                case "LN":
                    return StatusReason.LoanProcessError;
                case "LP":
                    return contractCode == "S"
                        ? StatusReason.LapsePending
                        : StatusReason.Lapsed;
                case "MA":
                    return StatusReason.Matured;
                case "MM":
                    return StatusReason.MPRtoMDVConversion;
                case "MP":
                    return StatusReason.ModeaversaryProcessingError;
                case "MX":
                    return StatusReason.MonthaversaryProcessingError;
                case "NA":
                    return StatusReason.NotApproved;
                case "NI":
                    return StatusReason.NotIssued;
                case "NQ":
                    return StatusReason.QuestionableCostBasis;
                case "NT":
                case "NN":
                case "NR":
                    return StatusReason.NotTaken;
                case "ON":
                    return StatusReason.NotTakenOwner;
                case "OW":
                    return StatusReason.WithdrawnOwner;
                case "PN":
                    return StatusReason.NotTakenPayor;
                case "PC":
                    return StatusReason.PolicyChange;
                case "PO":
                    return StatusReason.Postponed;
                case "PT":
                    return StatusReason.PreliminaryTerm;
                case "PW":
                    return StatusReason.WithdrawnPayor;
                case "RA":
                    return StatusReason.Reissued;
                case "RC":
                    return StatusReason.ReturnedCheck;
                case "RE":
                    return StatusReason.ReturnedEft;
                case "RI":
                    if (contractCode == "A")
                    {
                        return StatusReason.Reinstated;
                    }
                    else if (contractCode == "P")
                    {
                        return StatusReason.ReadyToIssue;
                    }
                    else
                    {
                        return StatusReason.Reissued;
                    }

                case "RL":
                    return StatusReason.ReplacementPending;
                case "RS":
                    return contractCode == "A"
                        ? StatusReason.Restored
                        : StatusReason.Reserved;
                case "RP":
                    return contractCode == "S"
                        ? StatusReason.RenewalPending
                        : StatusReason.Replaced;
                case "SM":
                    return StatusReason.Submitted;
                case "SP":
                    return StatusReason.SurrenderPending;
                case "SR":
                    return StatusReason.Surrendered;
                case "TP":
                    return StatusReason.TerminationPending;
                case "UC":
                    return StatusReason.UnappliedCash;
                case "WD":
                    return StatusReason.WaiverDisability;
                case "WI":
                    return StatusReason.Withdrawn;
                case "WP":
                    return StatusReason.WaiverPending;
                case "2L":
                    return StatusReason.TwoYearLookback;
                default:
                    return StatusReason.Unknown;
            }
        }

        public static PolicyStatusDetail ToPolicyStatusDetail(
            this string? policyStatusDetail)
        {
            if (string.IsNullOrWhiteSpace(policyStatusDetail))
            {
                return PolicyStatusDetail.None;
            }

            switch (policyStatusDetail)
            {
                case "AGENT RECOMMENDED":
                    return PolicyStatusDetail.AgentRecommended;
                case "CANCELLED - INITIAL PREMIUM NOT PAID":
                    return PolicyStatusDetail.CancelledInitialPremiumNotPaid;
                case "CANCELLED DUE TO NO RESPONSE":
                    return PolicyStatusDetail.CancelledDueToNoResponse;
                case "CIRCUMSTANCES CHANGED":
                    return PolicyStatusDetail.CircumstancesChanged;
                case "CONVERSION":
                    return PolicyStatusDetail.Conversion;
                case "COVERAGE THROUGH WORK":
                    return PolicyStatusDetail.CoverageThroughWork;
                case "Death":
                    return PolicyStatusDetail.Death;
                case "ETI":
                    return PolicyStatusDetail.ETI;
                case "FREE LOOK PERIOD - BAD CHECK OR CREDIT CARD":
                    return PolicyStatusDetail.FreeLookPeriodBadCheckOrCreditCard;
                case "FREE LOOK PERIOD - INSURED REQUESTED":
                    return PolicyStatusDetail.FreeLookPeriodInsuredRequested;
                case "HOME OFFICE CANCELLED":
                    return PolicyStatusDetail.HomeOfficeCancelled;
                case "INSURED CANCELLED - LUMP SUM PAID":
                    return PolicyStatusDetail.InsuredCancelledLumpSumPaid;
                case "INSURED REQUESTED":
                    return PolicyStatusDetail.InsuredRequested;
                case "LAPSED":
                    return PolicyStatusDetail.Lapsed;
                case "MATURITY":
                    return PolicyStatusDetail.Maturity;
                case "NO LONGER NEED":
                    return PolicyStatusDetail.NoLongerNeed;
                case "NON-PAYMENT":
                    return PolicyStatusDetail.NonPayment;
                case "NOT SPECIFIED":
                    return PolicyStatusDetail.NotSpecified;
                case "OTHER COVERAGE":
                    return PolicyStatusDetail.OtherCoverage;
                case "REPLACEMENT - EXTERNAL":
                    return PolicyStatusDetail.ReplacementExternal;
                case "REPLACEMENT - INTERNAL":
                    return PolicyStatusDetail.ReplacementInternal;
                case "RESCIND":
                    return PolicyStatusDetail.Rescind;
                case "RETIRED":
                    return PolicyStatusDetail.Retired;
                case "RPU":
                    return PolicyStatusDetail.RPU;
                case "SUPPLEMENTAL CONTRACT":
                    return PolicyStatusDetail.SupplementalContract;
                case "UNABLE TO AFFORD":
                    return PolicyStatusDetail.UnableToAfford;
                default:
                    return PolicyStatusDetail.Unknown;
            }
        }

        /// <summary>
        /// Maps a policies TaxQualificationCodeDescription based on the LineOfBusiness and TaxQualificationCOde sent in.
        /// </summary>
        /// <param name="taxQualificationCode">A policies Tax Qualification Code.</param>
        /// <param name="lineOfBusiness">A policies Line Of Business.</param>
        /// <returns>Returns the mapped TaxQualificationCodeDescription as a string.</returns>
        public static TaxQualificationStatus ToTaxQualificationStatus(
            this string? taxQualificationCode,
            string? lineOfBusiness)
        {
            if (lineOfBusiness == "I")
            {
                switch (taxQualificationCode)
                {
                    case "0":
                        return TaxQualificationStatus.NotQualified;
                    case "1":
                        return TaxQualificationStatus.HR10;
                    case "2":
                        return TaxQualificationStatus.TSA;
                    case "3":
                        return TaxQualificationStatus.IRA;
                    case "4":
                        return TaxQualificationStatus.SEP;
                    case "5":
                        return TaxQualificationStatus.NonQualifiedDeferredCompensation;
                    case "6":
                        return TaxQualificationStatus.RetirementPlan;
                    case "7":
                        return TaxQualificationStatus.Sec457Plan;
                    case "8":
                        return TaxQualificationStatus.DefinedBenefit;
                    case "9":
                        return TaxQualificationStatus.DefinedContribution;
                    case "S":
                        return TaxQualificationStatus.SimpleIRA;
                    case "C":
                        return TaxQualificationStatus.HealthCafeteria;
                    case "R":
                        return TaxQualificationStatus.RothIRAContribution;
                    case "Q":
                        return TaxQualificationStatus.RothIRAConversion;
                    case "N":
                        return TaxQualificationStatus.NotQualified;
                    case "Y":
                        return TaxQualificationStatus.Qualified;
                    default:
                        return TaxQualificationStatus.Unknown;
                }
            }
            else
            {
                switch (taxQualificationCode)
                {
                    case "0":
                        return TaxQualificationStatus.NotQualified;
                    case "1":
                        return TaxQualificationStatus.HR10;
                    case "2":
                        return TaxQualificationStatus.TSA;
                    case "3":
                        return TaxQualificationStatus.IRA;
                    case "4":
                        return TaxQualificationStatus.SEP;
                    case "5":
                        return TaxQualificationStatus.DeferredCompensation;
                    case "6":
                        return TaxQualificationStatus.RetirementPlan;
                    case "7":
                        return TaxQualificationStatus.Sec457Plan;
                    case "8":
                        return TaxQualificationStatus.DefinedBenefit;
                    case "9":
                        return TaxQualificationStatus.DefinedContribution;
                    case "S":
                        return TaxQualificationStatus.SimpleIRA;
                    case "C":
                        return TaxQualificationStatus.HealthCafeteria;
                    case "R":
                        return TaxQualificationStatus.RothIRAContribution;
                    case "Q":
                        return TaxQualificationStatus.RothIRAConversion;
                    case "O":
                        return TaxQualificationStatus.OLAB;
                    case "N":
                        return TaxQualificationStatus.NonOLAB;
                    case "T":
                        return TaxQualificationStatus.HealthQualified;
                    case "G":
                        return TaxQualificationStatus.HealthGrandfathered;
                    case "A":
                        return TaxQualificationStatus.RothTSA;
                    case "K":
                        return TaxQualificationStatus.Roth401k;
                    case "V":
                        return TaxQualificationStatus.RothGovt457;
                    default:
                        return TaxQualificationStatus.Unknown;
                }
            }
        }

        public static BenefitOptionName ToBenefitOptionName(this string? benefitOptionName)
        {
            switch (benefitOptionName?.ToLower())
            {
                case "acc death and dism":
                    return BenefitOptionName.AccidentalDeathAndDismemberment;

                case "acc dth & dism rider":
                    return BenefitOptionName.AccidentalDeathAndDismemberment;

                case "acc benefit period":
                    return BenefitOptionName.AccidentBenefitPeriod;

                case "acc elim period":
                    return BenefitOptionName.AccidentEliminationPeriod;

                case "aehalfcomp":
                    return BenefitOptionName.AccidentExpenseHalfCompensation;

                case "aenonemp":
                    return BenefitOptionName.AccidentExpenseNonEmployer;

                case "aepeo":
                    return BenefitOptionName.AccidentExpenseProfessionalEmployerOrganization;

                case "aezerocomp":
                    return BenefitOptionName.AccidentExpenseZeroCompensation;

                case "accident medical exp":
                    return BenefitOptionName.AccidentMedicalExpense;

                case "acc sick period":
                    return BenefitOptionName.AccidentSickPeriod;

                case "aviation cov rider":
                    return BenefitOptionName.Aviation;

                case "benefit":
                    return BenefitOptionName.Benefit;

                case "benefit amount":
                case "benefit amounts":
                    return BenefitOptionName.BenefitAmount;

                case "benefit max":
                    return BenefitOptionName.BenefitMaximum;

                case "benefit type":
                case "benefit period":
                case "base benefit period":
                case "cat benefit period":
                case "rbr benefit period":
                case "occupation period":
                case "own occ period":
                case "own occupation":
                    return BenefitOptionName.BenefitPeriod;

                case "cancer rider":
                    return BenefitOptionName.Cancer;

                case "chiropractic visits":
                case "chiropractor visits":
                    return BenefitOptionName.ChiropracticVisits;

                case "coinsurance":
                    return BenefitOptionName.Coinsurance;

                case "coverage":
                    return BenefitOptionName.Coverage;

                case "coverage type":
                case "coverge type":
                case "coverage tyoe":
                case "covrage type":
                case "family coverage":
                    return BenefitOptionName.CoverageType;

                case "citical illness":
                case "critcal illness":
                case "critical illiness":
                case "critical illness1":
                case "critical illness":
                    return BenefitOptionName.CriticalIllness;

                case "cihalfcomp":
                    return BenefitOptionName.CriticalIllnessHalfCompensation;

                case "cinonemp":
                    return BenefitOptionName.CriticalIllnessNonEmployer;

                case "cipeo":
                    return BenefitOptionName.CriticalIllnessProfessionalEmployerOrganization;

                case "cizerocomp":
                    return BenefitOptionName.CriticalIllnessZeroCompensation;

                case "deductible":
                    return BenefitOptionName.Deductible;

                case "dental coverage":
                    return BenefitOptionName.DentalCoverage;

                case "benefit percentage":
                case "dependent coverage":
                case "family ben options":
                    return BenefitOptionName.DependentCoverage;

                case "diassoc":
                    return BenefitOptionName.DisabilityIncomeAssociation;

                case "dipeo":
                    return BenefitOptionName.DisabilityIncomeProfessionalEmployerOrganization;

                case "dizerocomp":
                    return BenefitOptionName.DisabilityIncomeZeroCompensation;

                case "base elim period":
                case "cat elimination period":
                case "cat elim period":
                case "elmiation period":
                case "elimination period 1":
                case "elimination perion":
                case "elimination period":
                    return BenefitOptionName.EliminationPeriod;

                case "fio option":
                    return BenefitOptionName.FutureIncreaseOption;

                case "flat amount":
                    return BenefitOptionName.FlatAmount;

                case "hospital benefit":
                case "hosp benefit rider":
                    return BenefitOptionName.HospitalBenefit;

                case "hospital confinement":
                case "hosp confinement ben":
                    return BenefitOptionName.HospitalConfinement;

                case "hihalfcomp":
                    return BenefitOptionName.HospitalIndemnityHalfCompensation;

                case "hinonemp":
                    return BenefitOptionName.HospitalIndemnityNonEmployer;

                case "hipeo":
                    return BenefitOptionName.HospitalIndemnityProfessionalEmployerOrganization;

                case "hizercomp":
                    return BenefitOptionName.HospitalIndemnityZeroCompensation;

                case "class":
                    return BenefitOptionName.IndustryClass;

                case "lifetime accident":
                    return BenefitOptionName.LifetimeAccident;

                case "living benefit":
                    return BenefitOptionName.LivingBenefit;

                case "long term prem ref":
                    return BenefitOptionName.LongTermPremiumRefund;

                case "loss ratio rider":
                    return BenefitOptionName.LossRatio;

                case "major medical":
                    return BenefitOptionName.MajorMedical;

                case "partial disb rider":
                    return BenefitOptionName.PartialDisbursement;

                case "occupation class":
                case "occupations class":
                case "occ class":
                case "occupation clas":
                case "occupational class":
                case "occuption class":
                    return BenefitOptionName.OccupationClass;

                case "policy coverage":
                    return BenefitOptionName.PolicyCoverage;

                case "policyfee":
                case "policyfee2000":
                case "policyfee400":
                case "policyfee560":
                case "policyfee600":
                case "policyfee700":
                case "policyfee800":
                    return BenefitOptionName.PolicyFee;

                case "policy type":
                    return BenefitOptionName.PolicyType;

                case "pregnancy amendment":
                    return BenefitOptionName.PregnancyAmendment;

                case "prem refund rider":
                    return BenefitOptionName.PremiumRefund;

                case "rad/chemo benefit":
                    return BenefitOptionName.RadiationChemotherapyBenefit;

                case "reduced ben at 70":
                case "reduced benefit at 70":
                    return BenefitOptionName.ReducedBenefitAt70;

                case "return of premium":
                case "rop":
                    return BenefitOptionName.ReturnOfPremium;

                case "rop&reduce ben at 70":
                    return BenefitOptionName.ReturnOfPremiumAndReducedBenefitAt70;

                case "short term prem ref":
                    return BenefitOptionName.ShortTermPremiumRefund;

                case "sick benefit period":
                    return BenefitOptionName.SicknessBenefitPeriod;

                case "sick elim period":
                case "sick elim  period":
                case "sickness elim period":
                    return BenefitOptionName.SicknessEliminationPeriod;

                case "six month pre exist":
                    return BenefitOptionName.SixMonthPreexist;

                case "supersupp hi":
                    return BenefitOptionName.SupplementalHospitalIndemnity;

                case "value per unit":
                    return BenefitOptionName.ValuePerUnit;

                case "unionassociation":
                    return BenefitOptionName.UnionAssociation;

                case "unit":
                case "units":
                    return BenefitOptionName.Units;

                case "waiting period":
                    return BenefitOptionName.WaitingPeriod;

                case "waiver of premium":
                    return BenefitOptionName.WaiverOfPremium;

                case "waive prex":
                    return BenefitOptionName.WaivePreexistingCondition;

                default:
                    return BenefitOptionName.Unknown;
            }
        }

        public static BenefitOptionValue ToBenefitOptionValue(this string? benefitOptionValue)
        {
            switch (benefitOptionValue?.ToLower())
            {
                case "$4.00":
                    return BenefitOptionValue.FourDollars;

                case "$5.00":
                    return BenefitOptionValue.FiveDollars;

                case "$25.00":
                    return BenefitOptionValue.TwentyFiveDollars;

                case "$50.00":
                    return BenefitOptionValue.FiftyDollars;

                case "$100":
                    return BenefitOptionValue.OneHundredDollars;

                case "$150":
                    return BenefitOptionValue.HundredAndFiftyDollars;

                case "$200":
                    return BenefitOptionValue.TwoHundredDollars;

                case "$250":
                    return BenefitOptionValue.TwoHundredAndFiftyDollars;

                case "$300":
                    return BenefitOptionValue.ThreeHundredDollars;

                case "$350":
                    return BenefitOptionValue.ThreeHundredAndFiftyDollars;

                case "$400":
                case "$400.00":
                    return BenefitOptionValue.FourHundredDollars;

                case "$500":
                    return BenefitOptionValue.FiveHundredDollars;

                case "$560":
                    return BenefitOptionValue.FiveHundredAndSixtyDollars;

                case "$600":
                    return BenefitOptionValue.SixHundredDollars;

                case "$750":
                    return BenefitOptionValue.SevenHundredAndFiftyDollars;

                case "$1000":
                case "$1,000":
                    return BenefitOptionValue.OneThousandDollars;

                case "$1000 lifetime ben":
                    return BenefitOptionValue.ThousandDollarsLifeTimeBenefit;

                case "$1,500":
                    return BenefitOptionValue.FifteenHundredDollars;

                case "$2,000":
                    return BenefitOptionValue.TwoThousandDollars;

                case "$2,500":
                    return BenefitOptionValue.TwentyFiveHundredDollars;

                case "$5000":
                case "$5,000":
                    return BenefitOptionValue.FiveThousandDollars;

                case "$7,500":
                    return BenefitOptionValue.SeventyFiveHundredDollars;

                case "$10,000":
                    return BenefitOptionValue.TenThousandDollars;

                case "$25,000":
                    return BenefitOptionValue.TwentyFiveThousandDollars;

                case "$35,000":
                    return BenefitOptionValue.ThirtyFiveThousandDollars;

                case "50,000":
                case "$50,000":
                    return BenefitOptionValue.FiftyThousandDollars;

                case "100,000":
                case "$100,000":
                    return BenefitOptionValue.HundredThousandDollars;

                case "250,000":
                case "$250,000":
                    return BenefitOptionValue.TwoHundredFiftyThousandDollars;

                case "$500,000":
                    return BenefitOptionValue.FiveHundredThousandDollars;

                case "$2,500 / $10,000":
                    return BenefitOptionValue.TwentyFiveHundredToTenThousandDollars;

                case "$2,500 / $15,000":
                    return BenefitOptionValue.TwentyFiveHundredToFiftenThousandDollars;

                case "$3,500 / $25,000":
                    return BenefitOptionValue.ThirtyFiveHundredToTwentyFiveThousandDollars;

                case "$3,500 / $50,000":
                    return BenefitOptionValue.ThirtyFiveHundredToFiftyThousandDollars;

                case "$5,000 / $20,000":
                    return BenefitOptionValue.FiveThousandToTwentyThousandDollars;

                case "$5,000 / $25,000":
                    return BenefitOptionValue.FiveThousandToTwentyFiveThousandDollars;

                case "$5,000 / $30,000":
                    return BenefitOptionValue.FiveThousandToThirtyThousandDollars;

                case "$5,000 / $50,000":
                    return BenefitOptionValue.FiveThousandToFiftyThousandDollars;

                case "$5,500 / $50,000":
                    return BenefitOptionValue.FiftyFiveHundredToFiftyThousandDollars;

                case "$5,500 / $100,000":
                    return BenefitOptionValue.FiftyFiveHundredToHundredThousandDollars;

                case "$10,000 / $50,000":
                    return BenefitOptionValue.TenThousandToFiftyThousandDollars;

                case "$10,000 / $100,000":
                    return BenefitOptionValue.TenThousandToHundredThousandDollars;

                case "0 consecutive days":
                    return BenefitOptionValue.ZeroConsecutiveDays;

                case "7 consecutive days":
                    return BenefitOptionValue.SevenConsecutiveDays;

                case "14 consecutive days":
                    return BenefitOptionValue.FourteenConsecutiveDays;

                case "30 consecutive days":
                    return BenefitOptionValue.ThirtyConsecutiveDays;

                case "60 consecutive days":
                    return BenefitOptionValue.SixtyConsecutiveDays;

                case "90 consecutive days":
                    return BenefitOptionValue.NinetyConsecutiveDays;

                case "180 consecutive days":
                    return BenefitOptionValue.OneHundredEightyConsecutiveDays;

                case "365 consecutive days":
                    return BenefitOptionValue.ThreeHundredSixtyFiveConsecutiveDays;

                case "0 days":
                    return BenefitOptionValue.ZeroDays;

                case "1 days":
                    return BenefitOptionValue.OneDay;

                case "2 days":
                    return BenefitOptionValue.TwoDays;

                case "3 days":
                    return BenefitOptionValue.ThreeDays;

                case "4 days":
                    return BenefitOptionValue.FourDays;

                case "7 day":
                case "7 days":
                    return BenefitOptionValue.SevenDays;

                case "14 days":
                    return BenefitOptionValue.FourteenDays;

                case "15 days":
                    return BenefitOptionValue.FifteenDays;

                case "30 day":
                case "30 days":
                    return BenefitOptionValue.ThirtyDays;

                case "60 day":
                case "60 daus":
                case "60 days":
                    return BenefitOptionValue.SixtyDays;

                case "90 day":
                case "90 days":
                    return BenefitOptionValue.NinetyDays;

                case "120 days":
                    return BenefitOptionValue.OneHundredTwentyDays;

                case "180 day":
                case "180 days":
                    return BenefitOptionValue.OneHundredEightyDays;

                case "365 dady":
                case "365 day":
                case "365 days":
                    return BenefitOptionValue.ThreeHundredSixtyFiveDays;

                case "730 days":
                    return BenefitOptionValue.SevenHundredThirtyDays;

                case "5 days/5 days":
                    return BenefitOptionValue.FiveDaysAccidentFiveDaysSickness;

                case "10 days/10 days":
                    return BenefitOptionValue.TenDaysAccidentTenDaysSickness;

                case "30 days/10 days":
                    return BenefitOptionValue.ThirtyDaysAccidentTenDaysSickness;

                case "31 days/31 days":
                    return BenefitOptionValue.ThirtyOneDaysAccidentThirtyOneDaysSickness;

                case "90 days/31 days":
                    return BenefitOptionValue.NinetyDaysAccidentThirtyOneDaysSickness;

                case "365 days/30 days":
                    return BenefitOptionValue.ThreeHundredSixtyFiveDaysAccidentThirtyDaysSickness;

                case "365 days/31 days":
                    return BenefitOptionValue.ThreeHundredSixtyFiveDaysAccidentThirtyOneDaysSickness;

                case "11 weeks":
                    return BenefitOptionValue.ElevenWeeks;

                case "12 weeks":
                    return BenefitOptionValue.TwelveWeeks;

                case "13 weeks":
                    return BenefitOptionValue.ThirteenWeeks;

                case "24 weeks":
                    return BenefitOptionValue.TwentyFourWeeks;

                case "25 weeks":
                    return BenefitOptionValue.TwentyFiveWeeks;

                case "26 weeks":
                    return BenefitOptionValue.TwentySixWeeks;

                case "52 weeks":
                    return BenefitOptionValue.FiftyTwoWeeks;

                case "104 weeks":
                    return BenefitOptionValue.OneHundredFourWeeks;

                case "3 months":
                    return BenefitOptionValue.ThreeMonths;

                case "6 month":
                case "6 months":
                    return BenefitOptionValue.SixMonths;

                case "12 month":
                case "12 months":
                    return BenefitOptionValue.TwelveMonths;

                case "15 months":
                    return BenefitOptionValue.FifteenMonths;

                case "18 months":
                    return BenefitOptionValue.EighteenMonths;

                case "24 month":
                case "24 months":
                    return BenefitOptionValue.TwentyFourMonths;

                case "30 months":
                    return BenefitOptionValue.ThirtyMonths;

                case "0 years":
                    return BenefitOptionValue.ZeroYears;

                case "1 year":
                case "1 years":
                    return BenefitOptionValue.OneYear;

                case "2 year":
                case "2 years":
                    return BenefitOptionValue.TwoYears;

                case "3 years":
                    return BenefitOptionValue.ThreeYears;

                case "5-year":
                case "5 year":
                case "5 years":
                    return BenefitOptionValue.FiveYears;

                case "8 years":
                    return BenefitOptionValue.EightYears;

                case "10-year":
                case "10 year":
                case "10 years":
                    return BenefitOptionValue.TenYears;

                case "15 years":
                    return BenefitOptionValue.FifteenYears;

                case "20 years":
                    return BenefitOptionValue.TwentyYears;

                case "25 years":
                    return BenefitOptionValue.TwentyFiveYears;

                case "30 years":
                    return BenefitOptionValue.ThirtyYears;

                case "to age 65":
                    return BenefitOptionValue.ToAge65;

                case "to age 67":
                    return BenefitOptionValue.ToAge67;

                case "lifetime":
                    return BenefitOptionValue.Lifetime;

                case "5 year own occ":
                    return BenefitOptionValue.FiveYearsOwnOccupation;

                case "10 year own occ":
                    return BenefitOptionValue.TenYearsOwnOccupation;

                case "to age 65 own occ":
                    return BenefitOptionValue.ToAge65OwnOccupation;

                case "to age 67 own occ":
                    return BenefitOptionValue.ToAge67OwnOccupation;

                case "1 year (1 year base)":
                    return BenefitOptionValue.OneYear_OneYearBase;

                case "3 year (1 year base)":
                    return BenefitOptionValue.ThreeYears_OneYearBase;

                case "3 year (2 year base)":
                    return BenefitOptionValue.ThreeYears_TwoYearsBase;

                case "4 year (1 year base)":
                    return BenefitOptionValue.FourYears_OneYearBase;

                case "5 yr (1Yr base)":
                case "5 yr (1 Yr base)":
                case "5 years (1 yr base)":
                    return BenefitOptionValue.FiveYears_OneYearBase;

                case "5 yr (2yr base)":
                case "5 yr (2 yr base)":
                case "5 years (2 yr base)":
                    return BenefitOptionValue.FiveYears_TwoYearsBase;

                case "5 year (5 year base)":
                    return BenefitOptionValue.FiveYears_FiveYearsBase;

                case "8 year (2 year base)":
                    return BenefitOptionValue.EightYears_TwoYearsBase;

                case "9 year (1 year base)":
                    return BenefitOptionValue.NineYears_OneYearBase;

                case "10 yr (1yr base)":
                case "10 yr (1 yr base)":
                case "10 years (1 yr base)":
                    return BenefitOptionValue.TenYears_OneYearBase;

                case "10 yr (2yr base)":
                case "10 yr (2 yr base)":
                case "10 years (2 yr base)":
                    return BenefitOptionValue.TenYears_TwoYearsBase;

                case "10 yr (5yr base)":
                case "10 yr (5 yr base)":
                case "10 years (5 yr base)":
                    return BenefitOptionValue.TenYears_FiveYearsBase;

                case "to age 65 (2yr bas)":
                case "to age 65 (2yr base)":
                    return BenefitOptionValue.ToAge65_TwoYearsBase;

                case "to age 65 (5yr bas)":
                case "to age 65 (5yr base)":
                    return BenefitOptionValue.ToAge65_FiveYearsBase;

                case "to age 65 (10yr bas)":
                case "to age 65(10yr base)":
                    return BenefitOptionValue.ToAge65_TenYearsBase;

                case "employee":
                case "emplyee":
                    return BenefitOptionValue.Employee;

                case "employee / spouse":
                case "employee & spouse":
                case "employee/spouse":
                    return BenefitOptionValue.EmployeeSpouse;

                case "employee & child":
                case "employee/child":
                case "employee / child":
                    return BenefitOptionValue.EmployeeChild;

                case "emilyee & family":
                case "employee & family":
                    return BenefitOptionValue.EmployeeFamily;

                case "child no charge":
                    return BenefitOptionValue.ChildNoCharge;

                case "insured":
                    return BenefitOptionValue.Insured;

                case "spouse":
                    return BenefitOptionValue.Spouse;

                case "child":
                    return BenefitOptionValue.Child;

                case "family":
                    return BenefitOptionValue.Family;

                case "cancer rider":
                    return BenefitOptionValue.Cancer;

                case "critical illness":
                case "citical illness":
                case "critical illiness":
                case "critcal illness":
                    return BenefitOptionValue.CriticalIllness;

                case "diassoc":
                    return BenefitOptionValue.DisabilityIncomeAssociation;

                case "dipeo":
                    return BenefitOptionValue.DisabilityIncomeProfessionalEmployerOrganization;

                case "dizerocomp":
                    return BenefitOptionValue.DisabilityIncomeZeroCompensation;

                case "hihalfcomp":
                    return BenefitOptionValue.HospitalIndemnityHalfCompensation;

                case "hipeo":
                    return BenefitOptionValue.HospitalIndemnityProfessionalEmployerOrganization;

                case "hizerocomp":
                    return BenefitOptionValue.HospitalIndemnityZeroCompensation;

                case "hospital confinement":
                    return BenefitOptionValue.HospitalConfinement;

                case "loss ratio rider":
                    return BenefitOptionValue.LossRatio;

                case "return of premium":
                    return BenefitOptionValue.ReturnOfPremium;

                case "supersupp hi":

                    return BenefitOptionValue.SupplementalHospitalIndemnity;

                case "unionassociation":
                    return BenefitOptionValue.UnionAssociation;

                case "waive prex":
                    return BenefitOptionValue.WaivePreexistingCondition;

                case "waiver of premium":
                    return BenefitOptionValue.WaiverOfPremium;

                case "acc dth & dism rider":
                case "acc death and dism":
                case "acc death and disb":
                    return BenefitOptionValue.AccidentalDeathAndDismemberment;

                case "accident medical exp":
                    return BenefitOptionValue.AccidentMedicalExpense;

                case "aehalfcomp":
                    return BenefitOptionValue.AccidentExpenseHalfCompensation;

                case "aenonemp":
                    return BenefitOptionValue.AccidentExpenseNonEmployer;

                case "aepeo":
                    return BenefitOptionValue.AccidentExpenseProfessionalEmployerOrganization;

                case "aezerocomp":
                    return BenefitOptionValue.AccidentExpenseZeroCompensation;

                case "aviation cov rider":
                    return BenefitOptionValue.Aviation;

                case "cihalfcomp":
                    return BenefitOptionValue.CriticalIllnessHalfCompensation;

                case "cinonemp":
                    return BenefitOptionValue.CriticalIllnessNonEmployer;

                case "cipeo":
                    return BenefitOptionValue.CriticalIllnessProfessionalEmployerOrganization;

                case "cizerocomp":
                    return BenefitOptionValue.CriticalIllnessZeroCompensation;

                case "fio option":
                    return BenefitOptionValue.FutureIncreaseOption;

                case "flat amount":
                    return BenefitOptionValue.FlatAmount;

                case "hinonemp":
                    return BenefitOptionValue.HospitalIndemnityNonEmployer;

                case "hosp benefit rider":
                case "hospital benefit":
                    return BenefitOptionValue.HospitalBenefit;

                case "hosp confinement ben":
                    return BenefitOptionValue.HospitalConfinement;

                case "lifetime accident":
                    return BenefitOptionValue.LifetimeAccident;

                case "living benefit":
                    return BenefitOptionValue.LivingBenefit;

                case "long term prem ref":
                    return BenefitOptionValue.LongTermPremiumRefund;

                case "major medical":
                    return BenefitOptionValue.MajorMedical;

                case "partial disb rider":
                    return BenefitOptionValue.PartialDisbursement;

                case "pregnancy amendment":
                    return BenefitOptionValue.PregnancyAmendment;

                case "prem refund rider":
                    return BenefitOptionValue.PremiumRefund;

                case "reduced ben at 70":
                case "reduced benefit at 70":
                case "rop & reduce ben at 70":
                    return BenefitOptionValue.ReducedBenefitAtSeventy;

                case "rop":
                    return BenefitOptionValue.ReturnOfPremium;

                case "short term prem ref":
                    return BenefitOptionValue.ShortTermPremiumRefund;

                case "six month pre exist":
                    return BenefitOptionValue.SixMonthPreexist;

                case "polfee2000":
                    return BenefitOptionValue.PolicyFee2000;

                case "polfee400":
                    return BenefitOptionValue.PolicyFee400;

                case "polfee560":
                    return BenefitOptionValue.PolicyFee560;

                case "polfee600":
                    return BenefitOptionValue.PolicyFee600;

                case "polfee700":
                    return BenefitOptionValue.PolicyFee700;

                case "polfee800":
                    return BenefitOptionValue.PolicyFee800;

                case "policyfee":
                    return BenefitOptionValue.PolicyFee;

                case "1 unit":
                    return BenefitOptionValue.OneUnit;

                case "2 unit":
                case "2 units":
                    return BenefitOptionValue.TwoUnits;

                case "3 units":
                    return BenefitOptionValue.ThreeUnits;

                case "4 units":
                    return BenefitOptionValue.FourUnits;

                case "1 unit 25k/10k/5k":
                    return BenefitOptionValue.OneUnit25K10K5K;

                case "1 unit 50k/20k/10k":
                    return BenefitOptionValue.OneUnit50K20K10K;

                case "2 unit 50k/25k/10k":
                    return BenefitOptionValue.TwoUnits50K25K10K;

                case "2 unit 100k/40k/10k":
                    return BenefitOptionValue.TwoUnits100K40K10K;

                case "a":
                    return BenefitOptionValue.OccupationClassA;

                case "b":
                    return BenefitOptionValue.OccupationClassB;

                case "c":
                    return BenefitOptionValue.OccupationClassC;

                case "d":
                    return BenefitOptionValue.OccupationClassD;

                case "1a":
                    return BenefitOptionValue.OccupationClass1A;

                case "2a":
                    return BenefitOptionValue.OccupationClass2A;

                case "3a":
                    return BenefitOptionValue.OccupationClass3A;

                case "4a":
                    return BenefitOptionValue.OccupationClass4A;
                case "5a":
                    return BenefitOptionValue.OccupationClass5A;
                case "6a":
                    return BenefitOptionValue.OccupationClass6A;

                case "occ class 1":
                case "occ class I":
                    return BenefitOptionValue.OccupationClass1;

                case "occ class 2":
                case "occ class II":
                    return BenefitOptionValue.OccupationClass2;

                case "occ class 1 & 2":
                    return BenefitOptionValue.OccupationClass1And2;

                case "occ class 3":
                case "occ class III":
                    return BenefitOptionValue.OccupationClass3;

                case "occ class 4":
                    return BenefitOptionValue.OccupationClass4;

                case "superior":
                    return BenefitOptionValue.OccupationClassSuperior;

                case "excellent":
                    return BenefitOptionValue.OccupationClassExcellent;

                case "regular":
                    return BenefitOptionValue.OccupationClassRegular;

                case "normal":
                    return BenefitOptionValue.OccupationClassNormal;

                case "cancer & s/d":
                    return BenefitOptionValue.CancerAndSpecifiedDisease;

                case "cancer only":
                    return BenefitOptionValue.CancerOnly;

                case "24-hour":
                    return BenefitOptionValue.TwentyFourHour;

                case "hi admission":
                    return BenefitOptionValue.HospitalIndemnityAdmission;

                case "hi admission plus":
                    return BenefitOptionValue.HospitalIndemnityAdmissionPlus;

                case "off-the-job":
                    return BenefitOptionValue.OffTheJob;

                case "2":
                    return BenefitOptionValue.ChiropracticVisits2;

                case "3":
                    return BenefitOptionValue.ChiropracticVisits3;

                case "6":
                    return BenefitOptionValue.ChiropracticVisits6;

                case "10":
                    return BenefitOptionValue.ChiropracticVisits10;

                case "class 1":
                    return BenefitOptionValue.IndustryClass1;

                case "class 2":
                    return BenefitOptionValue.IndustryClass2;

                case "class 3":
                    return BenefitOptionValue.IndustryClass3;

                case "class 4":
                    return BenefitOptionValue.IndustryClass4;

                case "excludes dental":
                    return BenefitOptionValue.ExcludesDental;

                case "includes dental":
                    return BenefitOptionValue.IncludesDental;

                case "without cancer":
                    return BenefitOptionValue.WithoutCancer;

                case "without heart/stroke":
                    return BenefitOptionValue.WithoutHeartOrStroke;

                case "ee only/ee=sp=ch":
                    return BenefitOptionValue.CoinsurancePercentageOneHundred;

                case "ee=100/sp=50%/ch=25%":
                    return BenefitOptionValue.CoveragePercentageOneHundredFiftyTwentyFive;

                case "50-50":
                    return BenefitOptionValue.CoinsurancePercentageFiftyFifty;

                case "75-25":
                    return BenefitOptionValue.CoinsurancePercentageSeventyFiveTwentyFive;

                case "80-20":
                    return BenefitOptionValue.CoinsurancePercentageEightyTwenty;

                case "100/50/25":
                    return BenefitOptionValue.CoveragePercentageOneHundredFiftyTwentyFive;

                case "100/50/25 or 20":
                    return BenefitOptionValue.CoveragePercentageOneHundredFiftyTwentyFiveOrTwenty;

                case "100/50/50":
                    return BenefitOptionValue.CoveragePercentageOneHundredFiftyFifty;

                case "100/100/25":
                    return BenefitOptionValue.CoveragePercentageOneHundredOneHundredTwentyFive;

                case "100/100/25 or 20":
                    return BenefitOptionValue.CoveragePercentageOneHundredOneHundredTwentyFiveOrTwenty;

                case "100/100/50":
                    return BenefitOptionValue.CoveragePercentageOneHundredOneHundredFifty;

                case "100/100/100":
                    return BenefitOptionValue.CoveragePercentageOneHundredOneHundredOneHundred;

                case "combined":
                    return BenefitOptionValue.Combined;

                case "female-n, female-n":
                    return BenefitOptionValue.FemaleNonTobaccoFemaleNonTobacco;

                case "female-n, female-t":
                    return BenefitOptionValue.FemaleNonTobaccoFemaleTobacco;

                case "female-t, female-t":
                    return BenefitOptionValue.FemaleTobaccoFemaleTobacco;

                case "home office":
                    return BenefitOptionValue.HomeOffice;

                case "home office discount":
                    return BenefitOptionValue.HomeOfficeDiscount;

                case "home office discount -nt":
                case "home office discount - nt":
                case "home office discount- nt":
                case "home office discount-nt":
                case "home office non tobacco":
                    return BenefitOptionValue.HomeOfficeDiscountNonTobacco;

                case "home office discount - t":
                case "home office discount- t":
                case "home office discount-t":
                    return BenefitOptionValue.HomeOfficeDiscountTobacco;

                case "home office - nt":
                    return BenefitOptionValue.HomeOfficeNonTobacco;

                case "home office - t":
                    return BenefitOptionValue.HomeOfficeTobacco;

                case "individual":
                    return BenefitOptionValue.Individual;

                case "individual/spouse":
                    return BenefitOptionValue.IndividualSpouse;

                case "individual/child":
                    return BenefitOptionValue.IndividualChild;

                case "joint":
                    return BenefitOptionValue.Joint;

                case "joint - n/s":
                case "joint -s & n":
                case "joint n & s":
                case "joint n/s":
                case "joint s/n":
                case "joint- s & n":
                    return BenefitOptionValue.JointStandardNonTobacco;

                case "juvenile":
                    return BenefitOptionValue.Juvenile;

                case "juvenile-composite":
                    return BenefitOptionValue.JuvenileComposite;

                case "male-n, female-n":
                    return BenefitOptionValue.MaleNonTobaccoFemaleNonTobacco;

                case "male-n, female-t":
                    return BenefitOptionValue.MaleNonTobaccoFemaleTobacco;

                case "male-n, male-n":
                    return BenefitOptionValue.MaleNonTobaccoMaleNonTobacco;

                case "male-n, male-t":
                    return BenefitOptionValue.MaleNonTobaccoMaleTobacco;

                case "male-t, female-n":
                    return BenefitOptionValue.MaleTobaccoFemaleNonTobacco;

                case "male-t, female-t":
                    return BenefitOptionValue.MaleTobaccoFemaleTobacco;

                case "male-t, male-t":
                    return BenefitOptionValue.MaleTobaccoMaleTobacco;

                case "non-med non-tobacco":
                    return BenefitOptionValue.NonMedNonTobacco;

                case "nonmed conv nt":
                    return BenefitOptionValue.NonMedNonTobacco;

                case "non-med tobacco":
                case "nonmed conv t":
                    return BenefitOptionValue.NonMedTobacco;

                case "non - tobacco":
                case "non tobacco":
                case "non- tobacco":
                case "non-smoker":
                case "non-tobacco":
                case "non-tobacco.":
                case "nontobacco":
                    return BenefitOptionValue.NonTobacco;

                case "other insured":
                    return BenefitOptionValue.OtherInsured;

                case "preferred":
                case "prefferred":
                    return BenefitOptionValue.Preferred;

                case "hiq preferred non-tobacco":
                case "preferred non-tobacco":
                case "preferred nontobacco":
                    return BenefitOptionValue.PreferredNonTobacco;

                case "hiq preferred plus":
                case "preferred plus":
                    return BenefitOptionValue.PreferredPlus;

                case "preferred plus nontobacco":
                    return BenefitOptionValue.PreferredPlusNonTobacco;

                case "hiq preferred tobacco":
                case "preferred smoker":
                case "preferred tobacco ":
                    return BenefitOptionValue.PreferredTobacco;

                case "rated":
                    return BenefitOptionValue.Rated;

                case "rpu non-tobacco":
                    return BenefitOptionValue.ReducedPaidUpNonTobacco;

                case "rpu tobacco":
                    return BenefitOptionValue.ReducedPaidUpTobacco;

                case "hiq select non-tob":
                case "hiq select+ non-tob":
                case "select non tobacco":
                case "select non-nontobacco":
                case "select non-tobacc":
                case "select non-tobacc0":
                case "select non-tonbacco":
                case "select non-tobaco":
                case "select nontobacco":
                case "selection non-tobacco":
                    return BenefitOptionValue.SelectNonTobacco;

                case "hiq select non-tob vege":
                case "hiq select+ non-tob vege":
                    return BenefitOptionValue.SelectNonTobaccoVegetarian;

                case "select plus non-tobacco":
                    return BenefitOptionValue.SelectPlusNonTobacco;

                case "select plus tobacco":
                case "select pluss tobacco":
                    return BenefitOptionValue.SelectPlusTobacco;

                case "hiq select tob":
                case "hiq select+ tob":
                case "select tobacco":
                    return BenefitOptionValue.SelectTobacco;

                case "standard":
                    return BenefitOptionValue.Standard;

                case "hiq standard non-tob":
                case "hiq standard non-tobacco":
                case "hiq standared non-tob":
                case "s / nt":
                case "sstandard non-tobacco":
                case "standard / non-tobacco":
                case "standard non-tobacco":
                case "standard nontobacco":
                case "standard+ non tobacco":
                case "standard+ non-tobacco":
                    return BenefitOptionValue.StandardNonTobacco;

                case "hiq standard non-tob vege":
                    return BenefitOptionValue.StandardNonTobaccoVegetarian;

                case "standard plus non-tobacco":
                    return BenefitOptionValue.StandardPlusNonTobacco;

                case " hiq standard tobacco":
                case "hiq standard tob":
                case "hiq standard tobacco":
                case "hiq standard tobacco.":
                case "hiq standard tobacoo":
                case "standard tobacco":
                    return BenefitOptionValue.StandardTobacco;

                case "substandard":
                    return BenefitOptionValue.Substandard;

                case "super preferred":
                    return BenefitOptionValue.SuperPreferred;

                case "smoker":
                case "tobacco":
                case "t0bacco":
                    return BenefitOptionValue.Tobacco;

                case "uni-tobacco":
                    return BenefitOptionValue.UniversalTobacco;

                default:
                    return BenefitOptionValue.Unknown;
            }
        }

        public static DividendOption? ToDividendOption(this string? dividend)
        {
            switch (dividend)
            {
                case "0":
                    return DividendOption.NoDividend;
                case "1":
                    return DividendOption.CashDividend;
                case "2":
                    return DividendOption.ReducePremium;
                case "3":
                    return DividendOption.AccumulateAtInterestOrAddToCashValue;
                case "4":
                    return DividendOption.PaidupAdditions;
                case "5":
                    return DividendOption.OneYearTerm;
                case "6":
                    return DividendOption.ReduceLoan;
                case "7":
                    return DividendOption.OneYearTermConditional;
                case "8":
                    return DividendOption.TargetIsPercentageOfBase;
                case "9":
                    return DividendOption.SpecialCode;
                case "":
                    return DividendOption.None;
                default:
                    return DividendOption.Unknown;
            }
        }

        public static DeathBenefitOption? ToDeathBenefitOption(this string? dividend)
        {
            switch (dividend)
            {
                case "1":
                    return DeathBenefitOption.FaceAmountOption;
                case "2":
                    return DeathBenefitOption.FaceAmountPlusCashValueOption;
                case "":
                    return DeathBenefitOption.None;
                default:
                    return DeathBenefitOption.Unknown;
            }
        }

        public static RequirementStatus ToRequirementStatus(this string? status)
        {
            switch (status)
            {
                case "Y":
                    return RequirementStatus.Met;
                case "N":
                    return RequirementStatus.Unmet;
                case "W":
                    return RequirementStatus.Waived;
                default:
                    return RequirementStatus.Unknown;
            }
        }

        public static RequirementFulfillingParty ToRequirementFulfillingParty(this string? fulfillingParty)
        {
            switch (fulfillingParty)
            {
                case "Agent":
                    return RequirementFulfillingParty.Agent;
                case "HomeOffice":
                    return RequirementFulfillingParty.HomeOffice;
                default:
                    return RequirementFulfillingParty.Agent;
            }
        }

        public static RequirementActionType ToRequirementActionType(this string? actionType)
        {
            switch (actionType)
            {
                case "UploadFile":
                    return RequirementActionType.UploadFile;
                case "SendMessage":
                    return RequirementActionType.SendMessage;
                case "UploadFileOrSendMessage":
                    return RequirementActionType.UploadFileOrSendMessage;
                default:
                    return RequirementActionType.UploadFile;
            }
        }

        [Trace]
        public static void TrimStringProperties(this object obj)
        {
            if (obj != null)
            {
                var objProperties = obj.GetType().GetProperties();

                TrimStringPropertiesInt(obj, objProperties);
            }
        }

        [Trace]
        public static void TrimStringProperties<T>(this List<T> objects)
        {
            if (objects != null)
            {
                var objProperties = typeof(T).GetProperties();

                objects.Where(obj => obj != null)
                    .ToList()
                    .ForEach(obj => TrimStringPropertiesInt(obj, objProperties));
            }
        }

        public static string ToPhoneNumber(this string phoneNumber)
        {
            var validPhoneNumber = ValidPhoneNumber(phoneNumber);

            return validPhoneNumber ? phoneNumber : null;
        }

        public static bool ValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            var isNumeric = long.TryParse(phoneNumber, out _);
            var validLength = phoneNumber.Length == 10;

            var allZeros = false;
            if (isNumeric)
            {
                allZeros = long.Parse(phoneNumber) == 0;
            }

            if (isNumeric && validLength && !allZeros)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Trace]
        private static void TrimStringPropertiesInt(object obj, PropertyInfo[] objProperties)
        {
            objProperties.Where(property => property.PropertyType == typeof(string))
                .ToList()
                .ForEach(property =>
                {
                    var currentValue = (string?)property.GetValue(obj);
                    if (!string.IsNullOrEmpty(currentValue))
                    {
                        property.SetValue(obj, currentValue.Trim());
                    }
                });
        }
    }
}