using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Mappers
{
    public static class CensusProvMapper
    {
        // Map converts ONE CSV row (string[]) into a CensusProvRow object
        public static CensusProvRow Map(string[] columns, int year)
        {
            if (year == 2011)
            {
                return new CensusProvRow
                {
                    ProvinceCode = columns[0],             // columns[0] → prov_code
                    Name = columns[1],                     // columns[1] → name
                    Population = int.Parse(columns[5])     // columns[2] → total_pop_2011
                };
            }
            else                        // defaul to year 2022 if not 2011 (since we only have 2011 and 2022 data for census, we can assume it's 2022 if not 2011)
            {
                return new CensusProvRow
                {
                    ProvinceCode = columns[0],             // columns[0] → prov_code
                    Name = columns[1],                     // columns[1] → name
                    Population = int.Parse(columns[10])     // columns[2] → total_pop_2022
                };
            }

        }
    }
}
