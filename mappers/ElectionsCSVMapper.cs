using Hoeveel.Aggregator.Models.Raw;

namespace Hoeveel.Aggregator.Mappers;

/// <summary>
/// Maps CSV rows from the consolidated elections CSV file into ElectionsRow objects.
/// Used by CsvLoader to deserialize the elections data.
/// </summary>
public static class ElectionsCSVMapper
{
    public static ElectionsRow Map(string[] columns)
    {
        return new ElectionsRow
        {
            MunicipalityCode = columns[0],
            MunicipalityName = columns[1],
            ProvinceCode = columns[2],
            PartyName = columns[3],
            Votes = int.TryParse(columns[4], out var votes) ? votes : 0,
            VotePercentage = decimal.TryParse(columns[5], out var pct) ? pct : 0m
        };
    }
}
