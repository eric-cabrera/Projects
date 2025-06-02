namespace Assurity.Kafka.Accessors
{
    using System.Linq;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Microsoft.EntityFrameworkCore;
    using NewRelic.Api.Agent;

    public class GlobalDataAccessor : IGlobalDataAccessor
    {
        public GlobalDataAccessor(IDbContextFactory<GlobalDataContext> globalDataContextFactory)
        {
            GlobalDataContextFactory = globalDataContextFactory;
        }

        private IDbContextFactory<GlobalDataContext> GlobalDataContextFactory { get; }

        [Trace]
        public async Task<List<string>> GetNewBusinessFolderIds(string policyNumber)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext.VAttributesunionArcs
                .Where(attributesunionArc =>
                    attributesunionArc.PolicyNumber == policyNumber &&
                    attributesunionArc.ObjectType == 3)
                .Select(attributesunionArc => attributesunionArc.ObjectId)
                .Distinct()
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public async Task<List<AgentDTO>> GetJustInTimeAgentIds(List<string> nbFolderIds)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext.SysZ9processes
                .Where(z9Process => !string.IsNullOrWhiteSpace(z9Process.NBFOLDEROBJID) &&
                    nbFolderIds.Contains(z9Process.NBFOLDEROBJID) &&
                    !string.IsNullOrWhiteSpace(z9Process.AGENTID) &&
                    !string.IsNullOrWhiteSpace(z9Process.AGENTMARKETCODE) &&
                    !string.IsNullOrWhiteSpace(z9Process.AGENTLEVEL) &&
                    z9Process.AGENTID.Trim().Length > 0 &&
                    z9Process.AGENTID.Trim().Length <= 6)
                .Select(z9Process => new AgentDTO
                {
                    AGENT = z9Process.AGENTID,
                    MARKET_CODE = z9Process.AGENTMARKETCODE,
                    AGENT_LEVEL = z9Process.AGENTLEVEL
                })
                .Distinct()
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public async Task<List<string?>> GetAgentFolderIds(string agentId)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext.SysACAgentData
                .Where(acagentData => acagentData.AGENTID == agentId)
                .Select(acagentData => acagentData.FOLDERID)
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public async Task<List<string?>> GetAgentFolderIdsFromAttributes(string agentId)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext.Attributes
                .Where(attributes => attributes.AgentID == agentId &&
                attributes.OBJECT_TYPE == 3)
                .Select(attributes => attributes.OBJECT_ID)
                .ToListAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public async Task<JustInTimeAgentNameDTO> GetAgentName(string agentId)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext.SysACAgentData
                .Where(acagentData => acagentData.AGENTID == agentId)
                .Select(acagentData => new JustInTimeAgentNameDTO
                {
                    AgentId = agentId,
                    FirstName = acagentData.FIRSTNAME,
                    MiddleName = acagentData.MIDDLENAME,
                    LastName = acagentData.LASTNAME,
                    BusinessName = acagentData.BUSINESSNAME
                })
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public async Task<string?> GetQueueFromFolderId(string folderId)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext.Queues
                .Where(queue => queue.ID == folderId)
                .Select(queue => queue.QUEUE)
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public async Task<JustInTimeAgentDTO> GetJitAgentInfoFromFolderId(string agentId, string folderId, string marketCode, string level)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext.SysACAgentMarketCodes
                .Where(acagentMarketCode =>
                    acagentMarketCode.FOLDERID == folderId &&
                    acagentMarketCode.PENDINGRPTDISABLED == PendingRpt.PendingRptDisabled &&
                    acagentMarketCode.MARKETCODE == marketCode && acagentMarketCode.CONTRACTLEVEL == level)
                .Select(acagentMarketCode => new
                {
                    acagentMarketCode.MARKETCODE,
                    acagentMarketCode.CONTRACTLEVEL,
                    acagentMarketCode.UPLINEAGENTID,
                    acagentMarketCode.UPLINEMARKETCODE,
                    acagentMarketCode.UPLINECONTRACTLEVEL,
                })
                .ToListAsync();

            if (result == null || result.Count == 0)
            {
                return new JustInTimeAgentDTO();
            }

            var justInTimeAgent = new JustInTimeAgentDTO
            {
                AgentId = agentId,
                MarketCode = result.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.MARKETCODE))?.MARKETCODE,
                Level = result.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.CONTRACTLEVEL))?.CONTRACTLEVEL,
                UplineAgentId = result.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.UPLINEAGENTID))?.UPLINEAGENTID,
                UplineMarketCode = result.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.UPLINEMARKETCODE))?.UPLINEMARKETCODE,
                UplineLevel = result.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.UPLINECONTRACTLEVEL))?.UPLINECONTRACTLEVEL
            };
            PolicyInfoExtensions.TrimStringProperties(justInTimeAgent);
            return justInTimeAgent;
        }

        [Trace]
        public async Task<string> GetRequirementComment(string policyNumber, int reqSeq, int reqIndex, string reqType)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();
            var result = await globalDataContext.SysNBRequirements
                .Where(requirement => requirement.POLICYNUMBER == policyNumber
                && requirement.REQSEQ == reqSeq
                && requirement.IX == reqIndex
                && requirement.REQTYPE == reqType)
                .Select(requirement => requirement.REQNOTE)
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }

        [Trace]
        public List<GlobalRequirementLookupResult> GetRequirementComments(GlobalRequirementCommentsLookupDTO dto)
        {
            using var globalDataContext = GlobalDataContextFactory.CreateDbContext();

            var result =
                dto.Lookups.Join(
                    globalDataContext.SysNBRequirements.Where(req => req.POLICYNUMBER == dto.PolicyNumber),
                    lookup => new { lookup.REQSEQ, lookup.IX, lookup.REQTYPE },
                    sysNbReq => new { sysNbReq.REQSEQ, sysNbReq.IX, sysNbReq.REQTYPE },
                    (lookup, globalData) => new GlobalRequirementLookupResult
                    {
                        Ix = lookup.IX,
                        Sequence = lookup.REQSEQ,
                        Type = lookup.REQTYPE,
                        Note = globalData.REQNOTE.Trim()
                    })
                .ToList();

            return result;
        }

        [Trace]
        public async Task<string?> GetPolicyNumber(string folderId)
        {
            using var globalDataContext = await GlobalDataContextFactory.CreateDbContextAsync();

            var result = await globalDataContext
                .VAttributesunionArcs
                .Where(vAttributesUnionArc => vAttributesUnionArc.ObjectId == folderId)
                .Select(vAttributesUnionArc => vAttributesUnionArc.PolicyNumber)
                .FirstOrDefaultAsync();

            PolicyInfoExtensions.TrimStringProperties(result);
            return result;
        }
    }
}
