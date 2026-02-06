using Hoeveel.Aggregator.Models.Raw;     // TreasuryFactsResponse, UifwRow, CensusMuniRow
using Hoeveel.Aggregator.Models.Stored;  // Municipality

namespace Hoeveel.Aggregator.Builders;

public static class MunicipalityBuilder
{
    // ================== MUNICIPALITY PIPELINE ENTRY ==================
    // Creates municipality shells, then applies UIFW and Census data
    public static List<Municipality> BuildMunicipalities(TreasuryFactsResponse<UifwRow> uifwFacts, List<CensusMuniRow> censusRows)
    {
        var municipalities = CreateMunicipalities(uifwFacts);   // Step 1: establish identity (Code only)

        ApplyUifwData(municipalities, uifwFacts);                // Step 2: apply financial aggregation
        ApplyCensusData(municipalities, censusRows);             // Step 3: apply population + province code

        return municipalities;                                   // Fully enriched municipalities
    }


    // ================== STEP 1: CREATE MUNICIPALITY SHELLS ==================
    // Builds one Municipality per unique demarcation code found in UIFW data
    private static List<Municipality> CreateMunicipalities(TreasuryFactsResponse<UifwRow> facts)
    {
        return facts.Data
            .Where(r => !string.IsNullOrWhiteSpace(r.DemarcationCode))  // Ignore rows without a municipality code
            .Select(r => r.DemarcationCode)                             // Extract municipality codes
            .Distinct()                                                 // Ensure one Municipality per code
            .Select(code => new Municipality
            {
                Code = code                                             // Municipality identity
            })
            .ToList();                                                   // Materialise into a list
    }


    // ================== STEP 2: APPLY UIFW DATA ==================
    // Aggregates unauthorised, irregular, and fruitless expenditure per municipality
    public static void ApplyUifwData(List<Municipality> municipalities, TreasuryFactsResponse<UifwRow> facts)
    {
        var municipalityByCode = municipalities
            .ToDictionary(m => m.Code);                                 // Index municipalities for fast lookup

        foreach (var row in facts.Data)
        {
            if (string.IsNullOrWhiteSpace(row.DemarcationCode))          // Skip invalid rows
                continue;

            if (!municipalityByCode.TryGetValue(row.DemarcationCode, out var municipality))
                continue;                                                // UIFW row without a matching municipality

            switch (row.ItemCode?.ToLowerInvariant())                    // Aggregate by UIFW item type
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
    }


    // ================== STEP 3: APPLY CENSUS DATA ==================
    // Incorporates population, province code, and municipality name from Census CSV
    public static void ApplyCensusData(List<Municipality> municipalities, List<CensusMuniRow> censusRows)
    {
        var censusByMunicipalityCode = censusRows
            .Where(r => !string.IsNullOrWhiteSpace(r.MunicipalityCode))
            .GroupBy(r => r.MunicipalityCode)                            // Guard against duplicate CSV rows
            .Select(g => g.First())
            .ToDictionary(r => r.MunicipalityCode);                      // Index census data by municipality code

        foreach (var municipality in municipalities)
        {
            if (censusByMunicipalityCode.TryGetValue(municipality.Code, out var census))
            {
                municipality.Name = census.Name;                         // Municipality name
                municipality.Population = census.Population2022;         // Population (2022 census)
                municipality.ProvinceCode = census.ProvinceCode;         // Province code (critical for ProvinceBuilder)
            }
        }
    }
}