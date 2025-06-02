namespace Assurity.Kafka.IntegrationTests.Config
{
    using Microsoft.Extensions.Configuration;

    public static class TestConfigurationManager
    {
        public static IConfigurationRoot GetConfigurationRoot(string file)
        {
            return new ConfigurationBuilder()
                .AddJsonFile(file, true, true)
                .Build();
        }
    }
}
