namespace Assurity.Kafka.Accessors
{
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;

    public interface IGlobalDataAccessor
    {
        Task<List<string>> GetNewBusinessFolderIds(string policyNumber);

        Task<List<AgentDTO>> GetJustInTimeAgentIds(List<string> nbFolderIds);

        Task<List<string>> GetAgentFolderIds(string agentId);

        Task<List<string>> GetAgentFolderIdsFromAttributes(string agentId);

        Task<JustInTimeAgentNameDTO> GetAgentName(string agentId);

        Task<string?> GetQueueFromFolderId(string folderId);

        Task<JustInTimeAgentDTO> GetJitAgentInfoFromFolderId(string agentId, string folderId, string marketCode, string level);

        Task<string> GetRequirementComment(string policyNumber, int reqSeq, int reqIndex, string reqType);

        List<GlobalRequirementLookupResult> GetRequirementComments(GlobalRequirementCommentsLookupDTO dto);

        Task<string?> GetPolicyNumber(string folderId);
    }
}
