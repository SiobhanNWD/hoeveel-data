using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Mappers
{
    public static class CensusMuniMapper
    {
        // Map converts ONE CSV row (string[]) into a CensusMuniRow object
        public static CensusMuniRow Map(string[] columns)
        {
            return new CensusMuniRow
            {
                ProvinceCode = columns[0],              // columns[0] → prov_code
                MunicipalityCode = columns[2],          // columns[2] → muni_code
                Name = columns[3],                      // columns[3] → name
                Population2022 = int.Parse(columns[14]) // columns[14] → total_pop_2022
            };
        }
    }
}
