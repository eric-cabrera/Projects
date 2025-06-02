namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using Assurity.AgentPortal.Accessors.MongoDb.Contracts;

public interface IBenefitOptionsAccessor
{
    Task<List<BenefitOptionsMapping>> GetHiddenBenefitOptionsMappings();
}
