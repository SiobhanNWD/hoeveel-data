namespace Hoeveel.Aggregator.Models.Aggregated;

public class UifwMunicipalityTotalsr_dp
{
    public string MunicipalityCode { get; set; } = "";   // Municipality demarcation code, e.g. "JHB", "CPT", "NMA"
    public int FinancialYear { get; set; }               // Financial year the totals apply to, e.g. 2023

    public decimal UnauthorisedAmount { get; set; }      // Sum of all "unauthorised" UIFW amounts for this municipality + year
    public decimal IrregularAmount { get; set; }         // Sum of all "irregular" UIFW amounts for this municipality + year
    public decimal FruitlessAmount { get; set; }         // Sum of all "fruitless" UIFW amounts for this municipality + year

    public decimal UifwAmount =>                         // Computed total UIFW amount (no setter to prevent inconsistencies)
        UnauthorisedAmount + IrregularAmount + FruitlessAmount;
}

