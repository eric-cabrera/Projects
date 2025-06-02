namespace Assurity.AgentPortal.Managers.PolicyInfo.Mapping
{
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Managers.MappingExtensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoMapper;

    public class PolicyMappingProfile : Profile
    {
        public PolicyMappingProfile()
        {
            CreateMap<Policy, PolicyResponse>()
                .ForMember(
                    dest => dest.Payee,
                    opt =>
                    {
                        opt.Condition(src => src.LineOfBusiness == LineOfBusiness.ImmediateAnnuity);
                        opt.MapFrom(src => src.Payee);
                    })
                .ForMember(
                    policy => policy.Benefits,
                    opt => opt.MapFrom(policy => FilterTerminatedBenefits(policy.PolicyStatus, policy.Benefits)))
                .ForMember(
                    dest => dest.PaidToDate,
                    opt =>
                    {
                        opt.Condition(src => src.PolicyStatus != Status.Pending);
                        opt.MapFrom(src => src.PaidToDate);
                    })
                .ForMember(
                    policy => policy.BillingDay,
                    opt => opt.MapFrom(policy => FilterBillingDay(policy.BillingDay, policy.BillingForm)))
                .ForMember(
                    dest => dest.PolicyStatusReason,
                    opt =>
                    {
                        opt.Condition(src => src.PolicyStatusReason != StatusReason.None);
                        opt.MapFrom(src => src.PolicyStatusReason);
                    })
                .ForMember(
                    dest => dest.BillingForm,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.BillingForm)))
                .ForMember(
                    dest => dest.BillingMode,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.BillingMode)))
                .ForMember(
                    dest => dest.BillingStatus,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.BillingStatus)))
                .ForMember(
                    dest => dest.BillingReason,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.BillingReason)))
                .ForMember(
                    dest => dest.LineOfBusiness,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.LineOfBusiness)))
                .ForMember(
                    dest => dest.PolicyStatus,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.PolicyStatus)))
                .ForMember(
                    dest => dest.PolicyStatusDetail,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.PolicyStatusDetail)))
                .ForMember(
                    dest => dest.PolicyStatusReason,
                    opt => opt.MapFrom(
                        src => src.PolicyStatusReason != null
                        ? MappingExtensions.GetEnumDisplayName(src.PolicyStatusReason)
                        : string.Empty))
                .ForMember(
                    dest => dest.ReturnPaymentType,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.ReturnPaymentType)))
                .ForMember(
                    dest => dest.TaxQualificationStatus,
                    opt => opt.MapFrom(src => MappingExtensions.GetEnumDisplayName(src.TaxQualificationStatus)));
        }

        private static List<Benefit>? FilterTerminatedBenefits(Status policyStatus, List<Benefit> benefits)
        {
            if (benefits == null)
            {
                return null;
            }

            var filteredBenefits = new List<Benefit>();

            if (policyStatus != Status.Terminated)
            {
                filteredBenefits = benefits.Where(benefit => benefit.BenefitStatus != Status.Terminated).ToList();

                FilterPastAndFutureBenefitOptions(filteredBenefits);
            }

            if (filteredBenefits.Any())
            {
                return filteredBenefits;
            }

            return benefits;
        }

        private static short? FilterBillingDay(short? billingDay, BillingForm billingForm)
        {
            if (billingDay == null)
            {
                return null;
            }

            return billingForm == BillingForm.ListBill || billingForm == BillingForm.Direct ? null : billingDay;
        }

        private static void FilterPastAndFutureBenefitOptions(List<Benefit> benefits)
        {
            var today = DateTime.Today;
            foreach (var benefit in benefits)
            {
                if (benefit.BenefitOptions == null)
                {
                    continue;
                }

                benefit.BenefitOptions = benefit.BenefitOptions
                    .Where(option => option.StartDate.ToNullableDateOnly() <= DateOnly.FromDateTime(today) && option.StopDate.ToNullableDateOnly() > DateOnly.FromDateTime(today))
                    .ToList();
            }
        }
    }
}