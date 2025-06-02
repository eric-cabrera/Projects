namespace Assurity.AgentPortal.Engines.Tests;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class GenerateTestData
{
    internal static List<string> GenerateHeaders(int numberOfColumns)
    {
        var headers = new List<string>();
        for (int i = 0; i < numberOfColumns; i++)
        {
            headers.Add($"Header {i}");
        }

        return headers;
    }

    [ExcludeFromCodeCoverage]
    internal static List<Data> GenerateData(int numberOfRows)
    {
        var data = new List<Data>();
        for (int i = 0; i < numberOfRows; i++)
        {
            data.Add(new Data()
            {
                Name = $"Name {i}",
                Description = $"Description {i}",
                Value = $"Value {i}"
            });
        }

        return data;
    }
}
