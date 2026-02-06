namespace Hoeveel.Aggregator.Models.Raw
{
    // This class represents a single row from the municipality census CSV
    public class CensusMuniRow
    {
        public string ProvinceCode { get; set; } = "";    // The province code (e.g., "GP")
        public string MunicipalityCode { get; set; } = ""; // The municipality code (e.g., "JHB")
        public string Name { get; set; } = "";             // Name of the municipality (e.g., "City of Johannesburg")
        public int Population2022 { get; set; } = 0;     // Population for the year 2022
    }
}
