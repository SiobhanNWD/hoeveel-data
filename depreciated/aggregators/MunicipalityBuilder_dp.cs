using Hoeveel.Aggregator.Models.Aggregated;
using Hoeveel.Aggregator.Models.Stored;

namespace Hoeveel.Aggregator.Aggregators;

public static class MunicipalityBuilderr_dp
{
    // Builds Municipality domain entities from aggregated UIFW totals
    // Input  : List<UifwMunicipalityTotals> (one row per municipality + year)
    // Output : Dictionary<string, Municipality> keyed by municipality code
    public static Dictionary<string, Municipality> BuildFromUifw(List<UifwMunicipalityTotals> totals)
    {
        // Create a dictionary to store municipalities by their demarcation code
        // Key : Municipality code (e.g. "JHB"), Value : Municipality domain entity
        var municipalities = new Dictionary<string, Municipality>();

        foreach (var t in totals)       // Loop through each aggregated UIFW record (municipality + year)
        {
            if (!municipalities.TryGetValue(t.MunicipalityCode, out var municipality))  // Try to get the municipality from the dictionary, If it does not exist yet, we will create it
            {
                municipality = new Municipality         // Create a new municipality domain object
                {
                    Code = t.MunicipalityCode           // Demarcation code (e.g. "CPT", "BUF", "NMA")
                };

                municipalities.Add(t.MunicipalityCode, municipality);       // Store the newly created municipality in the dictionary
            }

            municipality.UifwByYear[t.FinancialYear] = new UifwYearTotals   // Attach UIFW totals for this specific financial year, as Each municipality can have multiple years of UIFW data
            {
                FinancialYear = t.FinancialYear,        // Store the financial year (e.g. 2011, 2022, 2023)

                // Map aggregated UIFW values into the domain model
                Unauthorised = t.UnauthorisedAmount, // Unauthorised expenditure (British spelling)
                Irregular = t.IrregularAmount,       // Irregular expenditure
                Fruitless = t.FruitlessAmount        // Fruitless & wasteful expenditure
            };
        }

        return municipalities;      // Return all municipalities with UIFW data attached
    }
}
