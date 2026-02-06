namespace Hoeveel.Aggregator.Models.Stored;

public class UifwYearTotalsr_dp
{
    public int FinancialYear { get; set; }     // e.g. 2011, 2022, 2023

    public decimal Unauthorised { get; set; }  // Unauthorised expenditure (British spelling)
    public decimal Irregular { get; set; }     // Irregular expenditure
    public decimal Fruitless { get; set; }     // Fruitless & wasteful expenditure

    public decimal Total =>
        Unauthorised + Irregular + Fruitless;  // Convenience total
}