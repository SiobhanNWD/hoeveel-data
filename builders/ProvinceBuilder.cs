using Hoeveel.Aggregator.Models.Stored;

namespace Hoeveel.Aggregator.Builders;

public static class ProvinceBuilder
{
    // Takes a list of Municipality objects and groups them into Province objects
    public static List<Province> BuildFromMunicipalities(List<Municipality> municipalities)
    {
        return municipalities
            .Where(m => !string.IsNullOrWhiteSpace(m.ProvinceCode))   // Ignore municipalities without a province (for now)
            .GroupBy(m => m.ProvinceCode)                             // Group municipalities by ProvinceCode
            .Select(group =>
            {
                var province = new Province                           // Create one Province per group
                {
                    Code = group.Key,                                 // group.Key == ProvinceCode
                    Municipalities = group.ToList()                   // Assign all municipalities belonging to this province
                };

                return province;                                      // UIFW totals are calculated via Province properties
            })
            .ToList();                                                 // Convert all provinces to a List
    }
}