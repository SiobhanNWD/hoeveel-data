namespace Hoeveel.Aggregator.Models.Config;

    public class UifwSourceConfig
    {
        public string Type { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public string Cube { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public string Format { get; set; } = "";
        public int PageSize { get; set; } = 10000;

        public int[] Years {get; set;} = new[] {2021};    //available years to download (defaults to 2011 and 2022 if not specified in config)

        public string BuildPath(int year)
            => $"{FilePath}_{year}.{Format}";       // file path

        // Builds the UIFW URL from config/sources.json:
        // https://municipaldata.treasury.gov.za/api/cubes/uifwexp/facts?cut=financial_year_end.year:2022&pagesize=10000&format=json
        public string BuildUrl(int year)
            => $"{BaseUrl}/cubes/{Cube}/{Endpoint}?cut=financial_year_end.year:{year}&pagesize={PageSize}&format={Format}";
    }

