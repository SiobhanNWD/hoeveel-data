namespace Hoeveel.Aggregator.Models.Stored;

// Stores UIFW totals for ONE municipality in ONE year
public class UifwYearTotals
{
    public int FinancialYear { get; set; }

    public decimal Unauthorised { get; set; }
    public decimal Irregular { get; set; }
    public decimal Fruitless { get; set; }

    // Convenience total
    public decimal Total =>
        Unauthorised + Irregular + Fruitless;
}
