namespace Assurity.AgentPortal.Accessors.LifePortraits;

using System.Net.Http;
using System.Xml;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.Extensions.Logging;

public class LifePortraitsApiAccessor : ILifePortraitsApiAccessor
{
    public LifePortraitsApiAccessor(
        HttpClient httpClient,
        IConfigurationManager configurationManager,
        ILogger<ILifePortraitsApiAccessor> logger)
    {
        Logger = logger;
        ConfigurationManager = configurationManager;
        HttpClient = httpClient;
    }

    private HttpClient HttpClient { get; set; }

    private IConfigurationManager ConfigurationManager { get; }

    private ILogger<ILifePortraitsApiAccessor> Logger { get; }

    public async Task<string?> GetURL(string credentials, CancellationToken cancellationToken)
    {
        var response = await HttpClient.PostAsync(ConfigurationManager.LifePortraitsUri + "Fipsco.ASP?PageName=LoginRequest", new StringContent(credentials), cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await GetUrlFromResponse(response);
        }

        Logger.LogError(
            "{Endpoint} returned response: {StatusCode} - {ResponseBody}",
            response.RequestMessage?.RequestUri,
            response.StatusCode,
            response);

        return null;
    }

    private async Task<string?> GetUrlFromResponse(HttpResponseMessage response)
    {
        var xml = new XmlDocument();
        xml.LoadXml(await response.Content.ReadAsStringAsync());

        if (xml != null && xml.DocumentElement != null)
        {
            return GetUrlFromXml(xml);
        }

        Logger.LogError(
            "{Endpoint} cannot parse XML response: {StatusCode} - {ResponseBody}",
            response.RequestMessage?.RequestUri,
            response.StatusCode,
            response);

        return null;
    }

    private string GetUrlFromXml(XmlDocument xml)
    {
        var status = xml.DocumentElement?.SelectSingleNode("STATUS");
        var tempAgentId = xml.DocumentElement?.SelectSingleNode("TEMPAGENTID")?.InnerXml ?? string.Empty;

        if (status != null && status.InnerXml == "0" && tempAgentId != null)
        {
            return $"{ConfigurationManager.LifePortraitsUri}Fipsco.asp?PageName=AutoLogin&ErrorURL=&LinkKey={tempAgentId}";
        }
        else
        {
            Logger.LogError("LifePortraits Error: StatusCode: {statuscode} and StatusInfo: {statusinfo}", status?.InnerXml, status?.NextSibling?.LastChild?.OuterXml);
            return null;
        }
    }
}