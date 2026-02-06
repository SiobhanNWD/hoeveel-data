using System.Text.Json.Serialization;

namespace Hoeveel.Aggregator.Models.Raw;

// Matches the Treasury /facts response envelope
public class TreasuryFactsResponse<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();
}
