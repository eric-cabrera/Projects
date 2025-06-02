using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PolicyMigration
{
    /// <summary>
    /// Taken from NBFRestAPI
    /// </summary>
    public enum InsuredRelationshipToPrimary
    {
        Spouse,
        Child,
        Self
    }

    /// <summary>
    /// Taken from NBFRestAPI
    /// </summary>
    public enum OwnerType
    {
        Primary,
        Contingent
    }

    /// <summary>
    /// Taken from NBFRestAPI
    /// </summary>
    public enum PayorType
    {
        Primary,
        Additional,
        SecondaryAddressee
    }

    /// <summary>
    /// Based on CaseManagement V2 from EDA
    /// </summary>
    public enum ApplicationStatus
    {
        ApplicationCreated,
        InterviewStarted,
        InterviewCompleted,
        AwaitingSignature,
        ApplicationSubmitted
    }

    /// <summary>
    /// Derived from EDA research.Based on Marketing wireframe buckets.
    /// </summary>
    public enum PolicyStatus
    {
        Pending,
        Active,
        Lapsed,
        Terminated
    }

    /// <summary>
    /// Derived from EDA research.Based on Production Credit and only represents Terminated. 
    /// PolicyInfo is probably a better place to source from since it has more reasons and covers more statuses.
    /// </summary>
    public enum PolicyStatusReason
    {
        None,
        Reinstated,
        ReadyToIssue,
        Reissued,
        Restored,
        Reserved,
        AnniversaryProcessingError,
        BillingError,
        ConversionPending,
        DeathPending,
        HomeOfficeSuspended,
        LapsePending,
        Lapse,
        ModeaversaryProcessingError,
        MonthaversaryProcessingError,
        PolicyChange,
        RenewalPending,
        Replaced,
        SurrenderPending,
        WaiverPending,
        NotIssued,
        NotTaken,
        Postponed,
        Surrendered,
        Withdrawn,
        Incomplete,
        Submitted,
        Canceled,
        ETIConversion,
        RPIConversion,
        Conversion,
        DeathClaim,
        Declined,
        ExchangeBenefit,
        Expire,
        Matured,
        MPRtoMDVConversion,
        UnappliedCash,
        ReturnedEft,
        IncompleteMIB,
        Ineligible,
        DeclinedFullUnd,
        Error,
        WithdrawnPayor,
        DeclinedReApply,
        IneligibleAge,
        WithdrawnOwner,
        IneligibleMaxCV,
        NotTakenOwner,
        IneligibleHospital,
        ReturnedCheck,
        DeclinedCOCPositive,
        DeclinedNonreapply,
        IneligibleDrugQuestion,
        NotTakenPayor,
        IncompleteRewrite,
        NotTakenRating,
        Unknown
    }

    public enum BenefitStatus
    {
        Pending,
        Active,
        Lapsed,
        Terminated
    }

    public enum BenefitStatusReason
    {
        // Statuses listed are placeholders. Not needed for AssureLink at this time, but probably useful for the future.
        Declined,
        Incomplete,
        Ineligible,
        NotTaken,
        Postponed,
        Withdrawn,
        Lapsed,
        NotIssued
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum CoverageTypes
    {
        Policy,
        Family
    }


    /// <summary>
    /// Taken from Assurity_Common_Library/UnitedStatesodel
    /// </summary>
    public enum State
    {
        [Display(Name = "Alaska")]
        AK = 45,
        [Display(Name = "Alabama")]
        AL,
        [Display(Name = "Arkansas")]
        AR,
        [Display(Name = "Arizona")]
        AZ,
        [Display(Name = "California")]
        CA,
        [Display(Name = "Colorado")]
        CO,
        [Display(Name = "Connecticut")]
        CT,
        [Display(Name = "Dist. of Columbia")]
        DC,
        [Display(Name = "Delaware")]
        DE,
        [Display(Name = "Florida")]
        FL,
        [Display(Name = "Georgia")]
        GA,
        [Display(Name = "Guam")]
        GU,
        [Display(Name = "Hawaii")]
        HI,
        [Display(Name = "Iowa")]
        IA,
        [Display(Name = "Idaho")]
        ID,
        [Display(Name = "Illinois")]
        IL,
        [Display(Name = "Indiana")]
        IN,
        [Display(Name = "Kansas")]
        KS,
        [Display(Name = "Kentucky")]
        KY,
        [Display(Name = "Louisiana")]
        LA,
        [Display(Name = "Massachusetts")]
        MA,
        [Display(Name = "Maryland")]
        MD,
        [Display(Name = "Maine")]
        ME,
        [Display(Name = "Michigan")]
        MI,
        [Display(Name = "Minnesota")]
        MN,
        [Display(Name = "Missouri")]
        MO,
        [Display(Name = "Mississippi")]
        MS,
        [Display(Name = "Montana")]
        MT,
        [Display(Name = "North Carolina")]
        NC,
        [Display(Name = "North Dakota")]
        ND,
        [Display(Name = "Nebraska")]
        NE,
        [Display(Name = "New Hampshire")]
        NH,
        [Display(Name = "New Jersey")]
        NJ,
        [Display(Name = "New Mexico")]
        NM,
        [Display(Name = "Nevada")]
        NV,
        [Display(Name = "New York")]
        NY,
        [Display(Name = "Ohio")]
        OH,
        [Display(Name = "Oklahoma")]
        OK,
        [Display(Name = "Oregon")]
        OR,
        [Display(Name = "Pennsylvania")]
        PA,
        [Display(Name = "Puerto Rico")]
        PR,
        [Display(Name = "Rhode Island")]
        RI,
        [Display(Name = "South Carolina")]
        SC,
        [Display(Name = "South Dakota")]
        SD,
        [Display(Name = "Tennessee")]
        TN,
        [Display(Name = "Texas")]
        TX,
        [Display(Name = "Utah")]
        UT,
        [Display(Name = "Virginia")]
        VA,
        [Display(Name = "Virgin Islands")]
        VI,
        [Display(Name = "Vermont")]
        VT,
        [Display(Name = "Washington")]
        WA,
        [Display(Name = "Wisconsin")]
        WI,
        [Display(Name = "West Virginia")]
        WV,
        [Display(Name = "Wyoming")]
        WY
    }

    /// <summary>
    /// Taken from NBFRestAPI
    /// </summary>
    public enum Country
    {
        UNKNOWN,
        AFGHANISTAN,
        ALBANIA,
        ALGERIA,
        AMERICAN_SAMOA,
        ANDORRA,
        ANGOLA,
        ANGUILLA,
        ANTARCTICA,
        ANTIGUA_BARBUDA,
        ARGENTINA,
        ARMENIA,
        ARUBA,
        ASCENSION_ISLANDS,
        AUSTRALIA,
        AUSTRIA,
        AZERBAIJAN,
        BAHAMAS,
        BAHRAIN,
        BANGLADESH,
        BARBADOS,
        BELARUS,
        BELGIUM,
        BELIZE,
        BENIN,
        BERMUDA,
        BHUTAN,
        BOLIVIA,
        BOSNIA_HERZEGOVINA,
        BOTSWANA,
        BOUVET_ISLAND,
        BRAZIL,
        BRITISH_INDIAN_OCNTERR,
        BRUNEI,
        BULGARIA,
        BURKINA_FASO,
        BURMA,
        BURUNDI,
        CAMBODIA,
        CAMEROON,
        CANADA,
        CAPEVERDE_ISLAND,
        CAYMAN_IS,
        CENTRAL_AFRICAN_REP,
        CHAD,
        CHILE,
        CHINA,
        CHRISTMAS_ISLANDS,
        COCOA_ISLANDS,
        COCOS,
        COCOS_KEELING,
        COLUMBIA,
        COMOROS,
        CONGO,
        CONGO_DEM_REP,
        COOK_ISLANDS,
        COSTA_RICA,
        COTE_DIVORIE,
        CROATIA,
        CURACAO,
        CUBA,
        CYPRUS,
        CZECH_REPUBLIC,
        DENMARK,
        DJIBOUTI,
        DOMINICA,
        DOMINICAN_REPUBLIC,
        ECUADOR,
        EGYPT,
        EL_SALVADOR,
        EQUATORIAL_GUINEA,
        ERITREA,
        ESTONIA,
        ETHIOPIA,
        FALKLAND_ISLANDS,
        FAEROE_ISLANDS,
        FIJI,
        FINLAND,
        FRANCE,
        FRENCH_GUIANA,
        FRENCH_POLYNESIA,
        FRENCH_SOUTHERN_TERRITORIES,
        GABON,
        GAMBIA,
        GEORGIA,
        GERMANY,
        GHANA,
        GIBRALTAR,
        GREECE,
        GREENLAND,
        GRENADA,
        GUADALOUPE,
        GUAM,
        GUATEMALA,
        GUERNSEY,
        GUINEA,
        GUINEA_BISSAU,
        GUYANA,
        HAITI,
        HEARD_ISLAND,
        HOLY_SEE,
        HONDURAS,
        HONG_KONG,
        HUNGARY,
        ICELAND,
        INDIA,
        INDONESIA,
        IRAN,
        IRAQ,
        IRELAND,
        ISLE_OF_MAN,
        ISRAEL,
        ITALY,
        IVORY_COAST,
        JAMAICA,
        JAPAN,
        JERSEY,
        JORDAN,
        KAZAKHSTAN,
        KENYA,
        KIRIBATI,
        KOREA_DEM_PEOPLE_REP,
        KOREA_REPUBLIC,
        KOSOVO,
        KUWAIT,
        KYRGYZSTAN,
        LAOS,
        LATVIA,
        LEBANON,
        LESOTHO,
        LIBERIA,
        LIBYA,
        LIECHTENSTEIN,
        LITHUANIA,
        LUXEMBOURG,
        MACAO,
        MACEDONIA,
        MADAGASCAR,
        MALAWI,
        MALAYSIA,
        MALDIVES,
        MALI,
        MALTA,
        MARSHALL_ISLANDS,
        MARTINIQUE,
        MAURITANIA,
        MAURITIUS,
        MAYOTTE_ISLAND,
        MEXICO,
        MICRONESIA,
        MOLDOVA,
        MONACO,
        MONGOLIA,
        MONTENEGRO,
        MONTSERRAT,
        MOROCCO,
        MOZAMBIQUE,
        MYANMAR,
        NAMIBIA,
        NAURU,
        NEPAL,
        NETHERLANDS,
        NETHERLANDS_ANTILLES,
        NEW_CALEDONIA,
        NEW_ZEALAND,
        NICARAGUA,
        NIGER,
        NIGERIA,
        NIUE,
        NORFOLK_ISLAND,
        NORTH_YEMEN,
        NORTH_MARIANA_ISLANDS,
        NORWAY,
        OMAN,
        PAKISTAN,
        PALAU,
        PALESTINE,
        PANAMA,
        PAPUA_NEW_GUINEA,
        PARAGUAY,
        PERU,
        PHILIPPINES,
        PITCARIN_ISLANDS,
        POLAND,
        PORTUGAL,
        PUERTO_RICO,
        QATAR,
        REUNION_ISLAND,
        ROMANIA,
        RUSSIA,
        RWANDA,
        SAIPAN,
        SAMOA,
        SAN_MARINO,
        SAO_TOME,
        SAUDI_ARABIA,
        SENEGAL,
        SERBIA,
        SEYCHELLES,
        SIERRA_LEONE,
        SINGAPORE,
        SLOVAKIA,
        SLOVENIA,
        SOLOMON_ISLANDS,
        SOMALIA,
        SOUTH_AFRICA,
        SOUTH_GEORGIA_SANDWICH,
        SOUTH_SUDAN, // Already has an _
        SOUTH_YEMEN,
        SPAIN,
        SRI_LANKA,
        ST_PIERRE,
        ST_HELENA,
        ST_KITTS_NEVIS,
        ST_LUCIA,
        ST_VINCENT,
        SUDAN,
        SURINAME,
        SVALBARD_ISLAND,
        SWAZILAND,
        SWEDEN,
        SWITZERLAND,
        SYRIA,
        TAIWAN,
        TAJIKISTAN,
        TANZANIA,
        THAILAND,
        EAST_TIMOR,
        TOGO,
        TOKELAU,
        TONGA_ISLANDS,
        TRINIDAD_TOBAGO,
        TUNISIA,
        TURKEY,
        TURKMENISTAN,
        TURKS_CAICOS_IS,
        TUVALU,
        UGANDA,
        UKRAINE,
        USSR,
        UNITED_ARAB_EMIRATES,
        UK,
        US_MINOR_OUTLYING_ISLANDS,
        USA,
        URUGUAY,
        VIRGIN_ISLANDS_US,
        UZBEKISTAN,
        VANUATU,
        VENEZUELA,
        VIETNAM,
        BRITISH_VIRGINIS,
        WALLIS_ISLANDS,
        WESTERN_SARAHA,
        WESTERN_SAMOA,
        YEMEN,
        YUGOSLAVIA,
        ZAIRE,
        ZAMBIA,
        ZIMBABWE,
        OTHER
    }

    public enum CoverageType
    {
        Base,
        Rider
    }
}
