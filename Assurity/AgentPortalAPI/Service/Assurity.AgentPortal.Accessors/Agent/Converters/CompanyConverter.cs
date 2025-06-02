namespace Assurity.AgentPortal.Contracts.Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Assurity.Agent.Contracts;

    public class CompanyConverter : JsonConverter<Company>
    {
        public override Company Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            try
            {
                return (Company)Enum.Parse(typeof(Company), value);
            }
            catch (ArgumentException)
            {
                throw new JsonException($"Unknown company code: {value}");
            }
        }

        public override void Write(Utf8JsonWriter writer, Company value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
