using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Mappers
{
    public static class CensusMuniMapper
    {
        // Map converts ONE CSV row (string[]) into a CensusMuniRow object
        public static CensusMuniRow Map(string[] columns, int year)
        {
            if (year == 2011)
            {
                return new CensusMuniRow
                {
                    ProvinceCode = columns[0],              // columns[0] → prov_code
                    MunicipalityCode = columns[2],          // columns[2] → muni_code
                    Name = columns[3],                      // columns[3] → name
                    Population = int.Parse(columns[9])      // columns[9] → total_pop_2011
                };
            }
            else                        // defaul to year 2022 if not 2011 (since we only have 2011 and 2022 data for census, we can assume it's 2022 if not 2011)
            {
                return new CensusMuniRow
                {
                    ProvinceCode = columns[0],              // columns[0] → prov_code
                    MunicipalityCode = columns[2],          // columns[2] → muni_code
                    Name = columns[3],                      // columns[3] → name
                    Population = int.Parse(columns[14])     // columns[14] → total_pop_2022
                };
            }
        }
    }
}
