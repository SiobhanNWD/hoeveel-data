namespace Hoeveel.Aggregator.Models.Raw
{
    // This class represents a single row from the province census CSV
    public class CensusProvRow
    {
        public string ProvinceCode { get; set; } = "";    // The province code (e.g., "GP")
        public string Name { get; set; } = "";            // Name of the province (e.g., "Gauteng")
        public int Population { get; set; } = 0;            // Population for the year 2022 or 2011
    }
}