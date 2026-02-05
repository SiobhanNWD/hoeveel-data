using Hoeveel.Aggregator.Models.Raw; 
namespace Hoeveel.Aggregator.Mappers;

public static class UifwSourceMapper
{
    // Map converts ONE CSV row (string[]) into a UifwSourceRow object which is passed into CsvLoader.Load(...)
    public static UifwSourceRow Map(string[] columns)
    {
        return new UifwSourceRow
        {
            DemarcationCode = columns[0],           // columns[0] → demarcation_code   e.g. "JHB", "NMA", "BUF"
            FinancialYear = int.Parse(columns[1]),  // columns[1] → financial_year     e.g. "2023", int.Parse is safe here because the year column is always present
            ItemCode = columns[2],                  // columns[2] → item_code          e.g. "unauthorised", "irregular", "fruitless"
            ItemLabel = columns[3],                 // columns[3] → item_label         e.g. "Unauthorised Expenditure"
            Amount = ParseDecimal(columns[4]),      // columns[4] → amount             e.g. 934000, sometimes empty in source CSV so we use ParseDecimal to safely handle missing values
            SourceId = int.Parse(columns[5])        // columns[5] → id                  e.g. 1, (source row identifier)
        };
    }

    private static decimal ParseDecimal(string value)   // Parses a decimal value safely, missing vales treased as 0, because csv contains missing fields for numeric values
    {
        if (string.IsNullOrWhiteSpace(value))   // If the value is null, empty, or whitespace, e.g. "" or "   "
            return 0m;

        return decimal.Parse(value);    // Otherwise parse the decimal normally
    }
}