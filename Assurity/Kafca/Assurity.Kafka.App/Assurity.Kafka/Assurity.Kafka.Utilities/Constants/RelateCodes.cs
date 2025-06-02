namespace Assurity.Kafka.Utilities.Constants
{
    public static class RelateCodes
    {
        public const string Insured = "IN";
        public const string Payor = "PA";
        public const string Owner = "PO";

        /// <summary>
        /// Contains the LifePro Annuitant relate code values that are relevant to the type of
        /// participants that the Mongo Policy object stores.
        /// </summary>
        public static readonly List<string> AnnuitantRelateCodes = new List<string>
        {
            "A1",
            "A2",
            "A3"
        };

        /// <summary>
        /// Contains the LifePro Assignee relate code values that are relevant to the type of
        /// participants that the Mongo Policy object stores.
        /// </summary>
        public static readonly List<string> AssigneeRelateCodes = new List<string>
        {
            "AS"
        };

        /// <summary>
        /// Contains the LifePro Beneficiary relate code values that are relevant to the type of
        /// participants that the Mongo Policy object stores.
        /// </summary>
        public static readonly List<string> BeneficiaryRelateCodes = new List<string>
        {
            "B1",
            "B2",
            "ZT",
            "ZU",
            "J1",
            "J2",
            "XP",
            "XC",
            "XT"
        };

        /// <summary>
        /// Contains the LifePro Owner relate code values that are relevant to the type of
        /// participants that the Mongo Policy object stores.
        /// </summary>
        public static readonly List<string> OwnerRelateCodes = new List<string>
        {
            "PO",
            "O1",
            "ZC"
        };

        /// <summary>
        /// Contains the LifePro Payee relate code value that is relevant to the type of
        /// participants that the Mongo Policy object stores.
        /// </summary>
        public static readonly List<string> PayeeRelateCodes = new List<string>
        {
            "PE"
        };

        /// <summary>
        /// Contains the LifePro Payor relate code values that are relevant to the type of
        /// participants that the Mongo Policy object stores.
        /// </summary>
        public static readonly List<string> PayorRelateCodes = new List<string>
        {
            "PA",
            "P1",
            "UL",
            "U2"
        };

        /// <summary>
        /// Contains the LifePro Insured relate code values that are relevant to the type of
        /// participants that the Mongo Policy object stores.
        /// </summary>
        public static readonly List<string> InsuredRelateCodes = new List<string>
        {
            "IN",
            "ML",
            "JE"
        };

        public static List<string> GetRelateCodesExcludingBeneficiary()
        {
            var relateCodes = new List<string>();
            relateCodes.AddRange(AnnuitantRelateCodes);
            relateCodes.AddRange(AssigneeRelateCodes);
            relateCodes.AddRange(OwnerRelateCodes);
            relateCodes.AddRange(PayeeRelateCodes);
            relateCodes.AddRange(PayorRelateCodes);
            relateCodes.AddRange(InsuredRelateCodes);

            return relateCodes;
        }
    }
}