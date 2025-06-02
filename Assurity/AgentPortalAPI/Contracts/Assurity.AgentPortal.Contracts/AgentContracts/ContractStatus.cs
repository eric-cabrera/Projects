namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    using System.ComponentModel.DataAnnotations;

    public enum ContractStatus
    {
        // Summary:
        //     Agent that is licensed and selling products with/for Assurity.
        Active,

        // Summary:
        //     Agents for whom the review process has begun but have not yet been licensed.
        Pending,

        // Summary:
        //     "Just in Time." Agents which have been submitted to us a potential agent but
        //     have not yet been reviewed.
        [Display(Name = "Just-In-Time")]
        JIT,

        // Summary:
        //     Not expected to ever be returned by the service. Exists to ensure that we always
        //     have a value to return just in case.
        Unknown
    }
}
