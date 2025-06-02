namespace Assurity.Kafka.Utilities.Constants
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;

    public static class BenefitTypes
    {
        [StringLength(2)]
        [Unicode(false)]
        public const string AnnuityRider = "AR";

        /// <summary>
        /// Base benefit type used for non-Universal Life policies.
        /// </summary>
        [StringLength(2)]
        [Unicode(false)]
        public const string Base = "BA";

        /// <summary>
        /// Base benefit type used for Universal Life policies.
        /// It is known as Basic Flexible Premium Benefit in LifePro.
        /// </summary>
        [StringLength(2)]
        [Unicode(false)]
        public const string BaseForUniversalLife = "BF";

        [StringLength(2)]
        [Unicode(false)]
        public const string FundValue = "FV";

        [StringLength(2)]
        [Unicode(false)]
        public const string OtherRider = "OR";

        [StringLength(2)]
        [Unicode(false)]
        public const string PaidUpAddition = "PU";

        [StringLength(2)]
        [Unicode(false)]
        public const string ShadowFundBenefit = "SB";

        [StringLength(2)]
        [Unicode(false)]
        public const string SpecifiedAmountIncrease = "SP";

        [StringLength(2)]
        [Unicode(false)]
        public const string Supplemental = "SU";

        [StringLength(2)]
        [Unicode(false)]
        public const string TableRating = "SL";

        [StringLength(2)]
        [Unicode(false)]
        public const string UniversalLife = "UV";
    }
}