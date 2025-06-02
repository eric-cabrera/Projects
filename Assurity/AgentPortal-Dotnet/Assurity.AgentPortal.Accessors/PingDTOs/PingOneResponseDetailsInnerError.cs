namespace Assurity.AgentPortal.Accessors.ProfilePingDTOs;

using System.Text.Json.Serialization;

public class PingOneResponseDetailsInnerError
{
    [JsonPropertyName("unsatisfiedRequirements")]
    public string[] UnsatisfiedRequirements { get; set; }

    [JsonPropertyName("length")]
    public string Length { get; set; }

    [JsonPropertyName("history")]
    public string History { get; set; }

    [JsonPropertyName("minCharacters")]
    public string MinCharacters { get; set; }

    [JsonPropertyName("minComplexity")]
    public string MinComplexity { get; set; }

    [JsonPropertyName("excludesCommonlyUsed")]
    public string ExcludesCommonlyUsed { get; set; }

    [JsonPropertyName("maxRepeatedCharacters")]
    public string MaxRepeatedCharacters { get; set; }

    [JsonPropertyName("notSimilarToCurrent")]
    public string NotSimilarToCurrent { get; set; }

    [JsonPropertyName("attemptsRemaining")]
    public int AttemptsRemaining { get; set; }

}