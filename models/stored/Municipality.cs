using System.Collections.Generic;

namespace Hoeveel.Aggregator.Models.Stored;

public class Municipality
{
    public string Code { get; set; } = "";     // Demarcation code (e.g. "JHB")
    public string Name { get; set; } = "";     // Filled later
    public string ProvinceCode { get; set; } = ""; // Filled later

    // Key = FinancialYear (2022 only for now)
    public Dictionary<int, UifwYearTotals> UifwByYear { get; set; }
        = new();

    // Temporary placeholder until population source is implemented
    public int Population { get; set; }
}
