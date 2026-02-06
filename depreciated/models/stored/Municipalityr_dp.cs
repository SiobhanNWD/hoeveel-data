namespace Hoeveel.Aggregator.Models.Stored;

public class Municipalityr_dp
{
    public string Code { get; set; } = "";     // Demarcation code (e.g. "JHB")
    public string Name { get; set; } = "";     // Empty for now
    public int Population { get; set; } = 0;   // Temporary single population value

    //TODO: public Dictionary<int, int> PopulationAnchors { get; set; } = new();     // Key = census year, Value = population e.g. { 2011 => 4434827, 2022 => 5635127 }
    public Dictionary<int, UifwYearTotals> UifwByYear { get; set; } = new();    // Key = FinancialYear

    public string ProvinceCode { get; set; } = ""; // Filled later
}
