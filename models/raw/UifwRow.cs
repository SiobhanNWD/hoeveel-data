using System.Text.Json.Serialization;

namespace Hoeveel.Aggregator.Models.Raw;

public class UifwRow
{
    [JsonPropertyName("demarcation.code")]
    public string DemarcationCode { get; set; } = "";

    [JsonPropertyName("financial_year_end.year")]
    public int FinancialYear { get; set; }

    [JsonPropertyName("item.code")]
    public string ItemCode { get; set; } = "";

    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }
}


