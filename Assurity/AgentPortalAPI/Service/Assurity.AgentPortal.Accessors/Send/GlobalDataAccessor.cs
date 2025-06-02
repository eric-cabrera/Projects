namespace Assurity.AgentPortal.Accessors.Send;

using System.Linq;
using Assurity.AgentPortal.Accessors.DTOs;
using Assurity.AgentPortal.Accessors.GlobalData.Context;
using Microsoft.EntityFrameworkCore;

public class GlobalDataAccessor : IGlobalDataAccessor
{
    public GlobalDataAccessor(IDbContextFactory<GlobalDataContext> globalDataContextFactory)
    {
        GlobalDataContextFactory = globalDataContextFactory;
    }

    private IDbContextFactory<GlobalDataContext> GlobalDataContextFactory { get; }

    public async Task<string> GetObjectIdForNewBusinessTransaction(string policyNumber)
    {
        using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

        var objectId = await globalDataContext.Attributes
            .Where(attributes =>
                attributes.ObjectType == 3
                && attributes.PolicyNumber == policyNumber)
            .Select(attributes => attributes.ObjectId)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrEmpty(objectId))
        {
            objectId = objectId.Trim();
        }

        return objectId;
    }

    public async Task<List<AttributeObject>?> GetApplicationData(string policyNumber)
    {
        using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

        var attributeObjects = new List<AttributeObject>();
        attributeObjects = await globalDataContext.Attributes
            .Where(attributes =>
                attributes.DocType == "APPLICATION" && attributes.PrintInclude == "Y"
                && attributes.PolicyNumber == policyNumber)
            .OrderByDescending(attributes => attributes.ObjectId.Trim())
            .Select(attributes => new AttributeObject
            {
                ObjectId = attributes.ObjectId.Trim(),
                ObjectClass = attributes.ObjectClass
            }).ToListAsync();

        if (attributeObjects?.Any() ?? false)
        {
            return attributeObjects;
        }

        return null;
    }
}