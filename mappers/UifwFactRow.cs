namespace Hoeveel.Aggregator.Models.Raw;

public class UifwFactRow
{
    public string DemarcationCode { get; set; } = "";
    public string DemarcationLabel { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public int FinancialYear { get; set; }
    public decimal Amount { get; set; }
}
