using Hoeveel.Aggregator.Models.Raw;
using Hoeveel.Aggregator.Models.Stored;

namespace Hoeveel.Aggregator.Builders;

public static class MunicipalityBuilder
{
    // Builds municipalities from UIFW facts rows (2022 only)
    public static List<Municipality> BuildFromUifwFacts(
        List<UifwFactsRow> facts)
    {
        // Step 1: Group all UIFW facts by municipality code
        var groupedByMunicipality = facts
            .GroupBy(f => f.DemarcationCode);

        var municipalities = new List<Municipality>();

        // Step 2: Build one Municipality per demarcation code
        foreach (var municipalityGroup in groupedByMunicipality)
        {
            var municipality = new Municipality
            {
                Code = municipalityGroup.Key
            };

            // Step 3: Create UIFW totals for 2022
            var yearTotals = new UifwYearTotals
            {
                FinancialYear = 2022
            };

            // Step 4: Sum UIFW amounts by item type
            foreach (var row in municipalityGroup)
            {
                switch (row.ItemCode)
                {
                    case "unauthorised":
                        yearTotals.Unauthorised += row.Amount;
                        break;

                    case "irregular":
                        yearTotals.Irregular += row.Amount;
                        break;

                    case "fruitless":
                        yearTotals.Fruitless += row.Amount;
                        break;

                    default:
                        Console.WriteLine(
                            $"[WARN] MunicipalityBuilder: Unknown ItemCode '{row.ItemCode}'");
                        break;
                }
            }

            // Step 5: Attach totals to municipality
            municipality.UifwByYear[2022] = yearTotals;

            municipalities.Add(municipality);
        }

        return municipalities;
    }
}
