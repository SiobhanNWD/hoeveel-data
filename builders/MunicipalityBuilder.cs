using Hoeveel.Aggregator.Models.Raw;     // TreasuryFactsResponse, UifwRow, CensusMuniRow
using Hoeveel.Aggregator.Models.Stored;  // Municipality
using Hoeveel.Aggregator.Mappers;       // ElectionsMapper

namespace Hoeveel.Aggregator.Builders;

public static class MunicipalityBuilder
{
    // ================== MUNICIPALITY PIPELINE ENTRY ==================
    // Creates municipality shells from UIFW data, then applies UIFW and Census data
    public static List<Municipality> BuildMunicipalities(TreasuryFactsResponse<UifwRow> uifwFacts, List<CensusMuniRow> censusRows, List<ElectionsRow>? electionsRows = null)
    {
        var municipalities = CreateMunicipalities(uifwFacts);   // Step 1: establish identity (Code only)

        ApplyUifwData(municipalities, uifwFacts);                // Step 2: apply financial aggregation
        ApplyCensusData(municipalities, censusRows);             // Step 3: apply population + province code
        if (electionsRows != null && electionsRows.Count > 0)
        {
            ApplyElectionsData(municipalities, electionsRows);   // Step 4: apply elections data (governing party)
        }

        CompareMunicipalityCodes(uifwFacts, censusRows);        // Optional: compare municipality codes between UIFW and Census for sanity check

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

            if (!municipalityByCode.TryGetValue(row.DemarcationCode, out var municipality))     // Get the current row's code aka. var municipality = municipalityByCode[row.DemarcationCode]; but safer
                continue;                                                // UIFW row without a matching municipality

            switch (row.ItemCode?.ToLowerInvariant())                    // Aggregate by UIFW item type (ToLowerInvariant so that the casing in the source data doesn't affect the mapping)
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
            .Where(r => !string.IsNullOrWhiteSpace(r.MunicipalityCode))  // Ignore rows without a municipality code
            .GroupBy(r => r.MunicipalityCode)                            // Guard against duplicate CSV rows
            .Select(g => g.First())                                      // Select a single representative row per municipality
            .ToDictionary(r => r.MunicipalityCode);                      // Index census data by municipality code

        foreach (var municipality in municipalities)
        {
            if (censusByMunicipalityCode.TryGetValue(municipality.Code, out var census))    // Gets the census row for the current municipality code, if it exists
            {
                municipality.Name = census.Name;                         // Municipality name
                municipality.Population = census.Population2022;         // Population (2022 census)
                municipality.ProvinceCode = census.ProvinceCode;         // Province code (critical for ProvinceBuilder)
            }
        }
    }

    // ================== STEP 4: APPLY ELECTIONS DATA ==================
    // Aggregates election results to determine the governing party (highest vote count) for each municipality
    public static void ApplyElectionsData(List<Municipality> municipalities, List<ElectionsRow> electionsRows)
    {
        if (electionsRows == null || electionsRows.Count == 0)
            return;

        // Aggregate votes by municipality and party
        var electionsByMuniAndParty = electionsRows
            .GroupBy(e => new { e.MunicipalityCode, e.PartyName })
            .Select(g => new
            {
                MunicipalityCode = g.Key.MunicipalityCode,
                PartyName = g.Key.PartyName,
                TotalVotes = g.Sum(e => e.Votes)
            })
            .ToList();

        // Determine the winning party per municipality
        var winnersByMunicipality = electionsByMuniAndParty
            .GroupBy(e => e.MunicipalityCode)
            .Select(g => new
            {
                MunicipalityCode = g.Key,
                WinningParty = g.OrderByDescending(e => e.TotalVotes).First().PartyName
            })
            .ToDictionary(w => w.MunicipalityCode, w => w.WinningParty);

        // Apply governing party to each municipality
        foreach (var municipality in municipalities)
        {
            if (winnersByMunicipality.TryGetValue(municipality.Code, out var partyName))
            {
                municipality.GoverningParty = partyName;
            }
        }
    }


    // TODO: Figure out what to do with municipality codes that exist in UIFW but not Census, and vice versa. Log them for now and investigate later.
    public static void CompareMunicipalityCodes(
    TreasuryFactsResponse<UifwRow> uifwFacts,
    List<CensusMuniRow> censusRows)
    {
        // --- Extract distinct codes from UIFW ---
        var uifwCodes = uifwFacts.Data
            .Where(r => !string.IsNullOrWhiteSpace(r.DemarcationCode))
            .Select(r => r.DemarcationCode.Trim().ToUpper())
            .Distinct()
            .ToHashSet();

        // --- Extract distinct codes from Census ---
        var censusCodes = censusRows
            .Where(r => !string.IsNullOrWhiteSpace(r.MunicipalityCode))
            .Select(r => r.MunicipalityCode.Trim().ToUpper())
            .Distinct()
            .ToHashSet();

        // --- Calculate differences ---
        var onlyInUifw = uifwCodes.Except(censusCodes).ToList();
        var onlyInCensus = censusCodes.Except(uifwCodes).ToList();

        // --- Output summary ---
        Console.WriteLine("===== MUNICIPALITY DATA COMPARISON =====");
        Console.WriteLine($"UIFW municipalities:   {uifwCodes.Count}");
        Console.WriteLine($"Census municipalities: {censusCodes.Count}");
        Console.WriteLine();

        Console.WriteLine($"Only in UIFW:   {onlyInUifw.Count}");
        Console.WriteLine($"Only in Census: {onlyInCensus.Count}");
        Console.WriteLine();

        if (onlyInUifw.Any())
        {
            Console.WriteLine("Codes only in UIFW:");
            Console.WriteLine(string.Join(", ", onlyInUifw.OrderBy(x => x)));
            Console.WriteLine();
        }

        if (onlyInCensus.Any())
        {
            Console.WriteLine("Codes only in Census:");
            Console.WriteLine(string.Join(", ", onlyInCensus.OrderBy(x => x)));
            Console.WriteLine();
        }

        Console.WriteLine("========================================");
    }

}

