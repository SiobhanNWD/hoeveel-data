namespace Hoeveel.Aggregator.Models.Stored;

public class Province_dp
{
    public string Code { get; set; } = "";     // e.g. "WC", "GP"
    public string Name { get; set; } = "";     // Filled later

    public List<Municipality> Municipalities { get; set; } = new();
}