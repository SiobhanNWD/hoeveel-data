using Hoeveel.Aggregator.Models.Raw;
using Hoeveel.Aggregator.Models.Aggregated;

namespace Hoeveel.Aggregator.Aggregators;

public static class UifwAggregator
{
    // Aggregates raw UIFW rows by municipality and financial year
    // Input  : List<UifwSourceRow> (one row per CSV record)
    // Output : List<UifwMunicipalityTotals> (summed unauthorised, irregular, fruitless amounts)
    public static List<UifwMunicipalityTotals> AggregateByMunicipalityAndYear(List<UifwSourceRow> rows)
    {
        //Step 1: Group raw rows r by municipality and year (refered to as g later)
        return rows.GroupBy(r => new           // return rows grouped by municipality code and financial year
            {
                r.DemarcationCode,    // e.g. "JHB", "CPT", "NMA"
                r.FinancialYear       // e.g. 2023
            })
            //Step 2: Convert each group of rows g into one UifwMunicipalityTotals object result
            .Select(g =>            // return the grouped rows g as a aggregated totals object result
            {
                var result = new UifwMunicipalityTotals         // Setting the municipality and year, as each group of rows g will have the same one
                {
                    MunicipalityCode = g.Key.DemarcationCode,   // Group key → municipality code
                    FinancialYear = g.Key.FinancialYear         // Group key → financial year
                };
                
                foreach (var row in g)      // Loop through all UIFW rows in the group g for this municipality & year and set the object's unauthorised, fruitless and irregular amounts
                {
                    switch (row.ItemCode)   // Use item_code from the row to decide which UIFW bucket the amount belongs to
                    {
                        case "unauthorised":
                            result.UnauthorisedAmount += row.Amount; // Sum unauthorised expenditure
                            break;
                        case "irregular":
                            result.IrregularAmount += row.Amount;    // Sum irregular expenditure
                            break;
                        case "fruitless":
                            result.FruitlessAmount += row.Amount;    // Sum fruitless & wasteful expenditure
                            break;
                        default:         // Unknown item_code – remove below to ignore
                            Console.ForegroundColor = ConsoleColor.Yellow; // warnings = yellow
                            Console.WriteLine($"[WARN] UifwAggregator: Unknown ItemCode '{row.ItemCode}'");
                            Console.ResetColor();
                        break;
                    }
                }

                return result;  // return result = One aggregated result per municipality + year
            })
            .ToList();          // Materialise results/ objects into a List
    }
}
