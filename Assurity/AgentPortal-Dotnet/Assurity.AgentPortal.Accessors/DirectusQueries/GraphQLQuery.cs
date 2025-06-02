namespace Assurity.AgentPortal.Accessors.DirectusQueries;

using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public abstract class GraphQLQuery
{
    private Dictionary<string, object> arguments = new Dictionary<string, object>();

    public abstract string Query { get; }

    public void AddArgument(string name, object argument)
    {
        arguments.Add(name, argument);
    }

    public async Task<string?> PostQuery(HttpClient httpClient, ILogger logger)
    {
        var queryObject = new
        {
            query = Query,
            variables = arguments
        };

        var convertedJson = JsonConvert.SerializeObject(queryObject);

        var response = await httpClient.PostAsync(
            "/graphql",
            new StringContent(
                convertedJson,
                Encoding.UTF8,
                MediaTypeNames.Application.Json));

        var responseString = response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            logger.LogError(
                $"Directus call to get page failed. {{Status}} | {{Response}}",
                response.StatusCode,
                await response.Content.ReadAsStringAsync());
        }

        return default;
    }

    public async Task<T?> PostQuery<T>(HttpClient httpClient, ILogger logger)
    {
        var queryObject = new
        {
            query = Query,
            variables = arguments
        };

        var convertedJson = JsonConvert.SerializeObject(queryObject);

        var response = await httpClient.PostAsync(
            "/graphql",
            new StringContent(
                convertedJson,
                Encoding.UTF8,
                MediaTypeNames.Application.Json));

        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseString))
        {
            try
            {
                var strResponse = JsonConvert.DeserializeObject<T>(responseString);
                return strResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        else
        {
            logger.LogError(
                $"Directus call to get {typeof(T)} failed. {{Status}} | {{Response}}",
                response.StatusCode,
                responseString);
        }

        return default;
    }
}
