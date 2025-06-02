using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Assurity.AgentPortal.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter<MFAType>))]
public enum MFAType
{
    SMS,
    EMAIL
}
