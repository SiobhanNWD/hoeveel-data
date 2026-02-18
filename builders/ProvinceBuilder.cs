using Hoeveel.Aggregator.Models.Stored;
using Hoeveel.Aggregator.Models.Raw;   // CensusProvRow, ElectionsRow

namespace Hoeveel.Aggregator.Builders;

public static class ProvinceBuilder
{
    // Takes a list of Municipality objects and groups them into Province objects
    public static List<Province> BuildFromMunicipalities(List<Municipality> municipalities, List<CensusProvRow> censusProvRows, List<ElectionsRow>? electionsRows = null)
    {
        var provinces = municipalities
            .Where(m => !string.IsNullOrWhiteSpace(m.ProvinceCode))   // Ignore municipalities without a province (for now)
            .GroupBy(m => m.ProvinceCode)                             // Group municipalities by ProvinceCode
            .Select(group =>
            {
                var province = new Province                           // Create one Province per group
                {
                    Code = group.Key,                                 // group.Key == ProvinceCode
                    Municipalities = group.ToList(),                   // Assign all municipalities belonging to this province

                    // Census Data
                    Name = censusProvRows.FirstOrDefault(p => p.ProvinceCode == group.Key)?.Name ?? "Unknown",  // Look up province name from Census data, fallback to "Unknown"
                    Population = censusProvRows.FirstOrDefault(p => p.ProvinceCode == group.Key)?.Population ?? group.Sum(m => m.Population), // Look up province population from Census data, fallback to sum of municipality populations   
                };
                // Console.WriteLine($"Census Province Population is {province.Population} and municipalities total population is {group.Sum(m => m.Population)}."); REMOVED as this has been verified to be correct
                return province;                                      // UIFW totals are calculated via Province properties
            })
            .ToList();                                                 // Convert all provinces to a List

        // Apply elections data if available
        if (electionsRows != null && electionsRows.Count > 0)
        {
            ApplyElectionsData(provinces, electionsRows);
        }

        return provinces;
    }

    // DONE Elections: GoverningParty = electionsProvRows.FirstOrDefault(p => p.ProvinceCode == group.Key)?.GoverningParty ?? "Unknown" Or something like that
    // ================== APPLY ELECTIONS DATA ==================
    // Aggregates election results to determine the governing party (highest vote count) for each province
    private static void ApplyElectionsData(List<Province> provinces, List<ElectionsRow> electionsRows)
    {
        if (electionsRows == null || electionsRows.Count == 0)
            return;

        // Aggregate votes by province and party
        var electionsByProvAndParty = electionsRows
            .GroupBy(e => new { e.ProvinceCode, e.PartyName })
            .Select(g => new
            {
                ProvinceCode = g.Key.ProvinceCode,
                PartyName = g.Key.PartyName,
                TotalVotes = g.Sum(e => e.Votes)
            })
            .ToList();

        // Determine the winning party per province
        var winnersByProvince = electionsByProvAndParty
            .GroupBy(e => e.ProvinceCode)
            .Select(g => new
            {
                ProvinceCode = g.Key,
                WinningParty = g.OrderByDescending(e => e.TotalVotes).First().PartyName
            })
            .ToDictionary(w => w.ProvinceCode, w => w.WinningParty);

        // Apply governing party to each province
        foreach (var province in provinces)
        {
            if (winnersByProvince.TryGetValue(province.Code, out var partyName))
            {
                province.GoverningParty = partyName;
            }
        }
    }
}