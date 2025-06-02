namespace Assurity.AgentPortal.Managers;

using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Accessors.DirectusDTOs;
using Assurity.AgentPortal.Accessors.DirectusQueries;
using Assurity.AgentPortal.Accessors.DirectusQueries.Contracting;
using Assurity.AgentPortal.Contracts;
using Assurity.AgentPortal.Contracts.Directus;
using Assurity.AgentPortal.Contracts.Directus.PageProtectionResponse;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class DirectusManager : IDirectusManager
{
    private readonly ILogger<DirectusManager> logger;

    public DirectusManager(
        ILogger<DirectusManager> log,
        IDirectusAccessor directusAccessor,
        IUserDataAccessor userDataAccessor)
    {
        logger = log;
        DirectusAccessor = directusAccessor;
        UserDataAccessor = userDataAccessor;
    }

    private IDirectusAccessor DirectusAccessor { get; set; }

    private IUserDataAccessor UserDataAccessor { get; set; }

    public async Task<CommissionDatesSummary?> GetAgentCenterCommissionDates()
    {
        var response = await DirectusAccessor.GetAgentCenterCommissionDates();

        if (response == null)
        {
            return null;
        }

        return ProcessCommissionDates(response.Data.AgentCenterCommissionDates);
    }

    // There are three types of market codes;
    // - worksite (codes starting with ws)
    //   when the user has selected a worksite code, they should see;
    //   - contracts marked as wsall
    //   - depending on if their code starts with wsr or wsrn they should see
    //     contracts marked with the matching code along with the wsall contracts
    // - non-worksite
    //   when the user has selected a non-worksite code, they should see;
    //   - contracts marked as all
    //   - depeding on if the market code is reverse hierarchy or not; (indicated by COMPANY_CODE == 02 in PHIER_AGENT_HIERARCHY)
    //     - reverse hierarchy: contracts with levels lexicographically lower than them
    //     - regular hierarchy: contracts with levels lexicographically higher than them
    // - NY market codes (indicated by comparison of agent_level to upline agent_level in PHIER_AGENT_HIERARCHY)
    //   - contracts marked as that market code
    //   - contaracts marked as all
    // NOTE: a user should always be able to see contracts that are a match for their selected Market Code and level
    //
    // We use the contracts query twice to get a mix of results; once for non-hierarchy and once for forward/reverse hirerarchy
    // (except for in the case of a NY market code)
    // - non-hierarchy query: used for market codes that don't require retreival of downline contracts
    //   (WSALL, ALL)
    // - ws codes: we make a separate call for wsr and wsrn codes since they are generalized from the market codes that appear
    //   like WSR12 or WSRN15
    // - forward hierarchy: used for market codes that require retreival of downline contracts where
    //   the level of a downline contract is higher than the level of the selected market code
    // - reverse hierarchy: used for market codes that require retreival of downline contracts where
    //   the level of a downline contract is lower than the level of the selected market code
    //
    // There is a special rule for agent level 2B: 2B should only be seen by agents level 01, 02, and 2B
    //
    // We sort the returned contracts by type:
    // - first, contracting kits, sorted alphabetically
    // - second, commission schedules, sorted by level according to hierarchy (reverse or normal)
    //
    // More information on contracting logic:
    // https://assuritylife.sharepoint.com/:u:/r/sites/AssurityWebProperties/SitePages/Contracting-Kits-%26-Commission-Schedule-Business-Logic.aspx
    public async Task<List<Contracts.Directus.Contract>?> GetContracts(
        string marketCode,
        List<string> agentLevels,
        string accessToken,
        string agentId)
    {
        var marketCodeInformation = await UserDataAccessor.GetMarketCodeInformation(accessToken, agentId, marketCode);
        if (marketCodeInformation == null)
        {
            return null;
        }

        var contracts = new List<Contracts.Directus.Contract>();
        var nonHierarchyMarketCodes = new List<string>();
        if (marketCode.StartsWith("ws", StringComparison.OrdinalIgnoreCase))
        {
            var worksiteContracts = await GetWorksiteMarketCodeContracts(marketCode, agentLevels);
            if (worksiteContracts == null)
            {
                return null;
            }

            contracts.AddRange(worksiteContracts);
            nonHierarchyMarketCodes.Add("WSALL");
        }
        else
        {
            var individualContracts = await GetIndividualMarketCodeContractsByHierarchy(marketCode, agentLevels, marketCodeInformation.IsReverseHierarchyMarketCode);
            if (individualContracts != null)
            {
                contracts.AddRange(individualContracts);
            }

            if (!marketCodeInformation.IsNewYorkMarketCode)
            {
                nonHierarchyMarketCodes.Add("ALL");
            }
        }

        if (nonHierarchyMarketCodes.Count > 0)
        {
            var nonHierarchyContracts = await GetNonHierarchyMarketCodeContracts(nonHierarchyMarketCodes);
            if (nonHierarchyContracts != null)
            {
                contracts.AddRange(nonHierarchyContracts);
            }
        }

        contracts = Filter2BContracts(contracts, agentLevels);
        return SortContracts(contracts, marketCodeInformation.IsReverseHierarchyMarketCode);
    }

    public async Task<byte[]?> GetDirectusFile(string id)
    {
        var response = await DirectusAccessor.GetDirectusFile(id);

        if (response == null)
        {
            return null;
        }

        return response;
    }

    public async Task<List<Contracts.Directus.TemporaryMessage>?> GetTemporaryMessages()
    {
        var response = await DirectusAccessor.GetTemporaryMessages();

        if (response == null)
        {
            return null;
        }

        return response.Data.TemporaryMessages.Select(message => new Contracts.Directus.TemporaryMessage
        {
            Id = message.Id,
            Heading = message.Heading,
            Message = message.Message,
            PublishDate = message.PublishDate,
            CtaLabel = message.CtaLabel,
            CtaLink = message.CtaLink,
        }).ToList();
    }

    public async Task<DirectusPageResponse?> GetDirectusPageContent(string slug, bool loggedIn)
    {
        var query = new PageQuery();

        query.AddArgument("slug", slug);

        var response = await DirectusAccessor.GetDirectusPage(query);

        if (response == null)
        {
            return null;
        }

        var protectionParse = JsonConvert.DeserializeObject<PageProtectionResponse>(response);
        var protectedPage = protectionParse?.Data.AgentCenterPages.FirstOrDefault()?.Protected ?? false;
        var hasAccess = protectedPage ? loggedIn ? true : false : true;

        return new DirectusPageResponse
        {
            HasAccess = hasAccess,
            PageDataJson = hasAccess ? response : string.Empty
        };
    }

    public async Task<ContactsResponse> GetContacts()
    {
        var response = await DirectusAccessor.GetContacts();

        return new ContactsResponse
        {
            Contacts = response
                ?.Select(c => new Contact
                {
                    Id = c.Id,
                    Name = $"{c.FirstName} {c.LastName}",
                    Title = c.Title,
                    Phone = c.Phone,
                    Email = c.Email,
                    States = c.Region?.States ?? [],
                    Image = c.Image.Id,
                    ContactTypes = c.ContractTypes.Select(c => c.ContractType.Name).ToList(),
                }).ToList() ?? throw new Exception(),
        };
    }

    private CommissionDatesSummary ProcessCommissionDates(List<CommissionDates> records)
    {
        var commissionsProcessed = new List<DateOnly>();
        var statementsAvailable = new List<DateOnly>();
        var directDeposit = new List<DateOnly>();

        foreach (var record in records)
        {
            if (record.CommissionsProcessed != null)
            {
                var dates = ParseDates(record.CommissionsProcessed);
                commissionsProcessed.AddRange(dates);
            }

            if (record.StatementsAvailable != null)
            {
                var dates = ParseDates(record.StatementsAvailable);
                statementsAvailable.AddRange(dates);
            }

            if (record.DirectDeposit != null)
            {
                var dates = ParseDates(record.DirectDeposit);
                directDeposit.AddRange(dates);
            }
        }

        return new CommissionDatesSummary
        {
            CommissionsProcessed = commissionsProcessed.OrderBy(x => x).Select(x => x.ToString("MMMM dd, yyyy")).ToList(),
            StatementsAvailable = statementsAvailable.OrderBy(x => x).Select(x => x.ToString("MMMM dd, yyyy")).ToList(),
            DirectDeposit = directDeposit.OrderBy(x => x).Select(x => x.ToString("MMMM dd, yyyy")).ToList(),
        };
    }

    private static List<DateOnly> ParseDates(List<CommissionDate> dates)
    {
        var parsedDates = new List<DateOnly>();

        foreach (var date in dates)
        {
            if (date == null || string.IsNullOrWhiteSpace(date.Date))
            {
                continue;
            }

            var parsedDate = DateOnly.Parse(date.Date);
            parsedDates.Add(parsedDate);
        }

        return parsedDates;
    }

    private static string? GetLowestAgentLevel(List<string> agentLevels)
    {
        return agentLevels.Aggregate((acc, value) =>
        {
            if (string.Compare(value, acc, StringComparison.OrdinalIgnoreCase) == -1 || string.IsNullOrEmpty(value))
            {
                return value;
            }
            else
            {
                return acc;
            }
        });
    }

    private static string? GetHighestAgentLevel(List<string> agentLevels)
    {
        return agentLevels.Aggregate((acc, value) =>
        {
            if (string.Compare(value, acc, StringComparison.OrdinalIgnoreCase) == 1 || string.IsNullOrEmpty(acc))
            {
                return value;
            }
            else
            {
                return acc;
            }
        });
    }

    // Contracting kits should be displayed above commission schedules
    // Contracting kits are displayed in alphanumeric order
    // Commission schedules are displayed by agent level, sorted depending on hierarchy structure
    private static List<Contracts.Directus.Contract> SortContracts(List<Contracts.Directus.Contract> contracts, bool isReverseHierarchy)
    {
        var sortedContracts = new List<Contracts.Directus.Contract>();
        var distinctContracts = contracts.DistinctBy(contract => contract.Id);

        var contractingKits = distinctContracts
            .Where(contract => contract.DisplayName.Contains("contracting", StringComparison.OrdinalIgnoreCase))
            .OrderBy(contract => contract.DisplayName);

        sortedContracts.AddRange(contractingKits);

        var commissionSchedules = distinctContracts
                .Where(contract => !contract.DisplayName.Contains("contracting", StringComparison.OrdinalIgnoreCase));

        if (isReverseHierarchy)
        {
            commissionSchedules = commissionSchedules.OrderBy(contract => contract.ConvertedAgentLevel);
        }
        else
        {
            commissionSchedules = commissionSchedules.OrderByDescending(contract => contract.ConvertedAgentLevel);
        }

        sortedContracts.AddRange(commissionSchedules);
        return sortedContracts;
    }

    // Only agents with level 01, 02, and 2B should be able to see contracts with agent level 2B
    private static List<Contracts.Directus.Contract> Filter2BContracts(List<Contracts.Directus.Contract> contracts, List<string> agentLevels)
    {
        if (agentLevels.Contains("01") || agentLevels.Contains("02") || agentLevels.Contains("2B"))
        {
            return contracts;
        }
        else
        {
            return contracts
                .Where(contract => contract.AgentLevel != "2B")
                .ToList();
        }
    }

    private async Task<List<Contracts.Directus.Contract>?> GetWorksiteMarketCodeContracts(string marketCode, List<string> agentLevels)
    {
        var contracts = new List<Contracts.Directus.Contract>();
        if (marketCode.StartsWith("wsrn", StringComparison.OrdinalIgnoreCase))
        {
            var wsrnContracts = await GetMarketCodeContractsByAgentLevel(["WSRN"], agentLevels);
            if (wsrnContracts == null)
            {
                return null;
            }

            contracts.AddRange(wsrnContracts);
        }
        else if (marketCode.StartsWith("wsr", StringComparison.OrdinalIgnoreCase))
        {
            var wsrContracts = await GetMarketCodeContractsByAgentLevel(["WSR"], agentLevels);
            if (wsrContracts == null)
            {
                return null;
            }

            contracts.AddRange(wsrContracts);
        }

        var wsContracts = await GetMarketCodeContractsByAgentLevel([marketCode], agentLevels);
        if (wsContracts == null)
        {
            return null;
        }

        contracts.AddRange(wsContracts);
        return contracts;
    }

    private async Task<List<Contracts.Directus.Contract>?> GetMarketCodeContractsByAgentLevel(string[] marketCodes, List<string> agentLevels)
    {
        var query = new ContractsQuery();
        query.AddArgument("marketCodes", marketCodes);

        var contracts = await DirectusAccessor.GetContracts(query);
        if (contracts == null)
        {
            return null;
        }

        return contracts.Data.Contracts
            .Where(contract => agentLevels.Contains(contract.AgentLevel))
            .Select(contract => new Contracts.Directus.Contract(contract.DisplayName, contract.File.Filename, contract.File.Id, contract.AgentLevel))
            .ToList();
    }

    private async Task<List<Contracts.Directus.Contract>?> GetIndividualMarketCodeContractsByHierarchy(
        string marketCode,
        List<string> agentLevels,
        bool isReverseHierarchy)
    {
        var query = new ContractsQuery();
        query.AddArgument("marketCodes", new string[] { marketCode });

        var normalizedAgentLevels = agentLevels.Select(al => al.ToLowerInvariant() == "2b" ? "02b" : al).ToList();

        var response = await DirectusAccessor.GetContracts(query);
        if (response == null)
        {
            return null;
        }

        if (isReverseHierarchy)
        {
            return response.Data.Contracts
                .Where(contract => string.Compare(contract.AgentLevel, GetLowestAgentLevel(normalizedAgentLevels), StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(contract => new Contracts.Directus.Contract(contract.DisplayName, contract.File.Filename, contract.File.Id, contract.AgentLevel))
                .ToList();
        }
        else
        {
            return response.Data.Contracts
                .Where(contract => string.Compare(contract.AgentLevel, GetHighestAgentLevel(normalizedAgentLevels), StringComparison.OrdinalIgnoreCase) <= 0)
                .Select(contract => new Contracts.Directus.Contract(contract.DisplayName, contract.File.Filename, contract.File.Id, contract.AgentLevel))
                .ToList();
        }
    }

    private async Task<List<Contracts.Directus.Contract>?> GetNonHierarchyMarketCodeContracts(List<string> nonHierarchyMarketCodes)
    {
        var nonHierarchyQuery = new ContractsQuery();
        nonHierarchyQuery.AddArgument("marketCodes", nonHierarchyMarketCodes);

        var nonHierarchyResults = await DirectusAccessor.GetContracts(nonHierarchyQuery);

        if (nonHierarchyResults?.Data?.Contracts != null)
        {
            return nonHierarchyResults.Data.Contracts
                .Select(contract => new Contracts.Directus.Contract(contract.DisplayName, contract.File.Filename, contract.File.Id, contract.AgentLevel))
                .ToList();
        }

        return null;
    }
}
