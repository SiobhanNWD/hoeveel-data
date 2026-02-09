using Hoeveel.Aggregator.Models.Stored;
using Hoeveel.Aggregator.Models.Raw;   // CensusProvRow

namespace Hoeveel.Aggregator.Builders;

public static class ProvinceBuilder
{
    // Takes a list of Municipality objects and groups them into Province objects
    public static List<Province> BuildFromMunicipalities(List<Municipality> municipalities, List<CensusProvRow> censusProvRows)  //TODO Add List<ElectionsMuniRow> electionsMuniRows or something to input 
    {
        return municipalities
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
                    Population = censusProvRows.FirstOrDefault(p => p.ProvinceCode == group.Key)?.Population2022 ?? group.Sum(m => m.Population), // Look up province population from Census data, fallback to sum of municipality populations   

                    // TODO Elections: GoverningParty = electionsProvRows.FirstOrDefault(p => p.ProvinceCode == group.Key)?.GoverningParty ?? "Unknown" Or something like that
                };
                // Console.WriteLine($"Census Province Population is {province.Population} and municipalities total population is {group.Sum(m => m.Population)}."); REMOVED as this has been verified to be correct
                return province;                                      // UIFW totals are calculated via Province properties
            })
            .ToList();                                                 // Convert all provinces to a List
    }
}