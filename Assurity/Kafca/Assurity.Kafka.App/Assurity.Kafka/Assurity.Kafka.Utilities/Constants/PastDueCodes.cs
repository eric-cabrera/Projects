namespace Assurity.Kafka.Utilities.Constants
{
    public static class PastDueCodes
    {
        public static readonly List<string> IncludedForPastDueContractCodes = new List<string>
        {
            "A",
            "S"
        };

        public static readonly List<string> ExcludedFromPastDueLinesOfBusiness = new List<string>
        {
            "A",
            "I",
            "U"
        };

        public static readonly List<string> ExcludedFromPastDueBillingReasons = new List<string>
        {
            "PU",
            "RU",
            "ET",
            "SP"
        };
    }
}