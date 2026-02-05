using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Mappers;

public static class UifwSourceMapper
{
    public static UifwSourceRow Map(string[] columns)
    {
        return new UifwSourceRow
        {
            MunicipalityCode = columns[0],
            FinancialYear = int.Parse(columns[1]),
            ItemCode = columns[2],
            ItemLabel = columns[3],
            Amount = ParseDecimal(columns[4]),
            SourceId = int.Parse(columns[5])
        };
    }

    private static decimal ParseDecimal(string value)   //in the case that a value is missing
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0m;

        return decimal.Parse(value);
    }

}
