using Hoeveel.Aggregator.Models.Raw;
using Hoeveel.Aggregator.Models.Stored;

namespace Hoeveel.Aggregator.Builders;

public static class MunicipalityBuilder
{
    // Builds one Municipality per demarcation code from raw UIFW fact rows
    public static List<Municipality> BuildFromUifw(TreasuryFactsResponse<UifwRow> facts)
    {
        return facts.Data
            .Where(r => !string.IsNullOrWhiteSpace(r.DemarcationCode))      // Ignore rows without a municipality code
            .GroupBy(r => r.DemarcationCode)                                // Group all rows by municipality
            .Select(group =>                                                 
            {
                var municipality = new Municipality                         // Create one aggregated Municipality per group
                {
                    Code = group.Key                                        // Here group.Key == group.DemarcationCode, because they were grouped according to DemaractionCode
                };

                foreach (var row in group)                                  // Aggregate UIFW amounts by item type
                {
                    switch (row.ItemCode?.ToLowerInvariant())    
                    {           
                        case "unauthorised":
                            municipality.Unauthorised += row.Amount;
                            break;
                        case "irregular":
                            municipality.Irregular += row.Amount;
                            break;
                        case "fruitless":
                            municipality.Fruitless += row.Amount;
                            break;
                    }
                }

                return municipality;                                        // Return the municipality object
            })
            .ToList();                                                      // Materialise the grouped municipalities into a list
    }
}