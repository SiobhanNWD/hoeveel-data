namespace Hoeveel.Aggregator.Models.Config;

public class UifwSourceConfig
{
    public string Type { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public string Cube { get; set; } = "";
    public int Year { get; set; }
    public string FilePath { get; set; } = "";
    public string Endpoint { get; set; } = "";
    public string Format { get; set; } = "";
    public int PageSize { get; set; } = 10000;

    // Builds the UIFW facts URL from config:
    // https://municipaldata.treasury.gov.za/api/cubes/uifwexp/facts?cut=financial_year_end.year:2022&pagesize=10000&format=json
    public string BuildFactsUrl()
        => $"{BaseUrl}/cubes/{Cube}/{Endpoint}?cut=financial_year_end.year:{Year}&pagesize={PageSize}&format={Format}";
}
