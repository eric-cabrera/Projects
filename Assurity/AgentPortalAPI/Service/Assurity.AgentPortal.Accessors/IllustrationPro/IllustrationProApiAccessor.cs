namespace Assurity.AgentPortal.Accessors.IllustrationPro;

using System.Net.Http;
using System.Xml;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.Extensions.Logging;

public class IllustrationProApiAccessor : IIllustrationProApiAccessor
{
    public IllustrationProApiAccessor(
        HttpClient httpClient,
        IConfigurationManager configurationManager,
        ILogger<IIllustrationProApiAccessor> logger)
    {
        Logger = logger;
        ConfigurationManager = configurationManager;
    }

    private IConfigurationManager ConfigurationManager { get; }

    private ILogger<IIllustrationProApiAccessor> Logger { get; }

    public async Task<string?> GetAccountId(string credentials)
    {
        using (var client = new HttpClient())
        {
            var messageBody = new StringContent(credentials, System.Text.Encoding.UTF8, "text/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "text/xml; charset=utf-8");
            client.DefaultRequestHeaders.Add("SOAPAction", ConfigurationManager.IllustrationProAgentAccountUrl);
            var response = client.PostAsync(ConfigurationManager.IllustrationProUrl + "services/agentaccountservice.svc", messageBody).Result;

            if (response.IsSuccessStatusCode)
            {
                return await GetAccountIDFromResponse(response);
            }

            Logger.LogError(
                "{Endpoint} returned response: {StatusCode} - {ResponseBody}",
                response.RequestMessage?.RequestUri,
                response.StatusCode,
                response);

            return null;
        }
    }

    private static async Task<string> GetAccountIDFromResponse(HttpResponseMessage response)
    {
        var xml = new XmlDocument();
        xml.LoadXml(await response.Content.ReadAsStringAsync());

        if (xml != null && xml.DocumentElement != null)
        {
            var status = xml.GetElementsByTagName("ErrorCode")[0];
            var tempAccountId = xml.GetElementsByTagName("TempAccountID")[0];
            if (status != null && status.InnerXml == "0" && tempAccountId != null)
            {
                return tempAccountId.InnerXml;
            }
        }

        return string.Empty;
    }
}