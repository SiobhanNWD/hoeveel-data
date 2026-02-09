using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Mappers
{
    public static class CensusProvMapper
    {
        // Map converts ONE CSV row (string[]) into a CensusProvRow object
        public static CensusProvRow Map(string[] columns)
        {
            return new CensusProvRow
            {
                ProvinceCode = columns[0],             // columns[0] → prov_code
                Name = columns[1],                     // columns[1] → name
                Population2022 = int.Parse(columns[10]) // columns[2] → total_pop_2022
            };
        }
    }
}
