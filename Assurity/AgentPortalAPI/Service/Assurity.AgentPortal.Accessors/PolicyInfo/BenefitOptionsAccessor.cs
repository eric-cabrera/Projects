namespace Assurity.AgentPortal.Accessors.PolicyInfo;

using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.MongoDB;
using global::MongoDB.Driver;
using global::MongoDB.Driver.Linq;

public class BenefitOptionsAccessor : IBenefitOptionsAccessor
{
    public BenefitOptionsAccessor(MongoDBConnection mongoDbConnection)
    {
        MongoDbConnection = mongoDbConnection;
    }

    private MongoDBConnection MongoDbConnection { get; }

    public async Task<List<BenefitOptionsMapping>> GetHiddenBenefitOptionsMappings()
    {
        var benefitOptions = await MongoDbConnection.BenefitOptionsCollection
            .AsQueryable()
            .Where(benefitOption => benefitOption.HideBenefitOption)
            .ToListAsync();

        return benefitOptions;
    }
}
