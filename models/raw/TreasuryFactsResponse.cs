using System.Text.Json.Serialization;

namespace Hoeveel.Aggregator.Models.Raw;

// Generic wrapper for Treasury /facts API responses
// The Treasury API always returns an envelope object with a "data" array inside it
// This class allows us to deserialize that envelope while keeping the row type flexible
public class TreasuryFactsResponse<T>
{
    // The "data" property in the JSON response
    // Holds the list of fact rows (e.g. UIFW facts, income/expense facts, etc.)
    // T will be a concrete row type like UifwFactsRow
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();
}
