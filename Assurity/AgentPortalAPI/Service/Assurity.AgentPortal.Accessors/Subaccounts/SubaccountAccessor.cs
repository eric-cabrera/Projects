namespace Assurity.AgentPortal.Accessors.Subaccounts;

using System.Threading;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.MongoDB;
using Assurity.AgentPortal.Utilities.Emails;
using Assurity.Claims.Contracts;
using global::MongoDB.Bson;
using global::MongoDB.Driver;

public class SubaccountAccessor : ISubaccountAccessor
{
    public SubaccountAccessor(
        IEmailUtility emailUtility,
        MongoDBConnection mongoDbConnection)
    {
        EmailUtility = emailUtility;
        MongoDbConnection = mongoDbConnection;
    }

    public IEmailUtility EmailUtility { get; }

    private MongoDBConnection MongoDbConnection { get; }

    public async Task<List<PendingSubaccountDTO>> GetPendingSubaccounts(string parentAgentId, CancellationToken cancellationToken)
    {
        var filter = Builders<PendingSubaccount>.Filter.Eq(x => x.ParentAgentId, parentAgentId);
        var subaccounts = await MongoDbConnection.PendingSubaccountsCollection.Find(filter).ToListAsync(cancellationToken);

        return subaccounts.Select(MapPendingSubaccountToDTO).ToList();
    }

    public async Task<PendingSubaccountDTO?> RetrieveAndActivateSubaccount(Guid linkGuid)
    {
        var filter = Builders<PendingSubaccount>.Filter.Eq(x => x.LinkGuid, linkGuid);
        var update = Builders<PendingSubaccount>.Update.Inc(x => x.ActivationAttempts, 1);

        var updateOptions = new FindOneAndUpdateOptions<PendingSubaccount>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var pendingSubaccount = await MongoDbConnection.PendingSubaccountsCollection
            .FindOneAndUpdateAsync(filter, update, updateOptions);
        if (pendingSubaccount == null)
        {
            return null;
        }

        return MapPendingSubaccountToDTO(pendingSubaccount);
    }

    public async Task DeletePendingSubaccount(string agentId, string email)
    {
        var filter = GetPendingSubaccountAndFilter(agentId, email);

        await MongoDbConnection.PendingSubaccountsCollection.DeleteOneAsync(filter);
    }

    public async Task DeletePendingSubaccount(string id)
    {
        var filter = Builders<PendingSubaccount>.Filter.Eq(x => x.Id, ObjectId.Parse(id));

        await MongoDbConnection.PendingSubaccountsCollection.DeleteOneAsync(filter);
    }

    public async Task<bool> DoesSubaccountExist(string agentId, string email)
    {
        var filter = GetPendingSubaccountAndFilter(agentId, email);

        var result = await MongoDbConnection.PendingSubaccountsCollection.Find(filter).ToListAsync();

        if (result == null)
        {
            return false;
        }

        return result.Count > 0;
    }

    public async Task<PendingSubaccountDTO> CreateNewSubaccount(
        string parentAgentId,
        string parentUsername,
        string subaccountEmail,
        IEnumerable<string> roles)
    {
        var agentDetails = new PendingSubaccount
        {
            Email = subaccountEmail,
            ParentAgentId = parentAgentId,
            ParentUsername = parentUsername,
            Roles = roles,
            LinkGuid = Guid.NewGuid(),
            ActivationAttempts = 0,
            EmailSentAt = DateTime.Now,
        };

        await MongoDbConnection.PendingSubaccountsCollection.InsertOneAsync(agentDetails);
        await SendActivationEmail(agentDetails.LinkGuid.ToString(), agentDetails.Email);

        return MapPendingSubaccountToDTO(agentDetails);
    }

    public async Task UpdateSubaccount(string agentId, string email, IEnumerable<string> roles)
    {
        var subAccountFilter = GetPendingSubaccountAndFilter(agentId, email);

        var update = Builders<PendingSubaccount>.Update
            .Set(x => x.Roles, roles);

        var updateResult = await MongoDbConnection.PendingSubaccountsCollection
            .UpdateOneAsync(subAccountFilter, update);
    }

    public async Task ResendActivationEmail(string agentId, string email)
    {
        var subAccountFilter = GetPendingSubaccountAndFilter(agentId, email);

        var subAccountUpdate = Builders<PendingSubaccount>.Update
            .Set(x => x.LinkGuid, Guid.NewGuid())
            .Set(x => x.EmailSentAt, DateTime.Now);

        var updateOptions = new FindOneAndUpdateOptions<PendingSubaccount>
        {
            ReturnDocument = ReturnDocument.After,
        };

        var updatedSubaccount = await MongoDbConnection.PendingSubaccountsCollection
            .FindOneAndUpdateAsync(subAccountFilter, subAccountUpdate, updateOptions);

        await SendActivationEmail(updatedSubaccount.LinkGuid.ToString(), updatedSubaccount.Email);
    }

    private static FilterDefinition<PendingSubaccount> GetPendingSubaccountAndFilter(string agentId, string email)
    {
        var agentIdFilter = Builders<PendingSubaccount>.Filter.Eq(x => x.ParentAgentId, agentId);
        var emailFilter = Builders<PendingSubaccount>.Filter.Eq(x => x.Email, email);

        return Builders<PendingSubaccount>.Filter.And(agentIdFilter, emailFilter);
    }

    private static PendingSubaccountDTO MapPendingSubaccountToDTO(PendingSubaccount pendingSubaccount)
    {
        return new PendingSubaccountDTO
        {
            Id = pendingSubaccount.Id.ToString(),
            Email = pendingSubaccount.Email,
            ParentAgentId = pendingSubaccount.ParentAgentId,
            ParentUsername = pendingSubaccount.ParentUsername,
            Roles = pendingSubaccount.Roles,
            LinkGuid = pendingSubaccount.LinkGuid,
            ActivationAttempts = pendingSubaccount.ActivationAttempts,
            EmailSentAt = pendingSubaccount.EmailSentAt,
        };
    }

    private async Task SendActivationEmail(string linkGuid, string email)
    {
        var message = EmailUtility.CreateSubaccountActivationEmail(linkGuid.ToString(), email);

        await EmailUtility.SendEmail(message);
    }
}
