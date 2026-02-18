namespace Hoeveel.Aggregator.Models.Config
{
    // This class contains the configuration for the Elections sources
    public class ElectionsSourceConfig
    {
        public string Type { get; set; } = "";      // e.g. "csv-url"
        public string FilePath { get; set; } = "";  // Path where the file should be saved
        // Original single-url property (kept for backward compatibility)
        public string Url { get; set; } = "";       // URL to download the CSV

        // New properties to allow building province download URLs similarly to UifwSourceConfig
        public string BaseUrl { get; set; } = "";   // e.g. https://results.elections.org.za/home/LGEPublicReports
        public int ReportId { get; set; } = 1091;    // Report id used in the public reports path
        public string ReportPath { get; set; } = "Downloadable%20Party%20Results";
        public int ElectionYear { get; set; } = 2021;

        // List of province codes to iterate when downloading per-province reports (defaults to populated list if nothing overides it from sources.config)
        public string[] Provinces { get; set; } = new[]
        {
            "FS", // Free State
            "EC", // Eastern Cape
            "GP", // Gauteng
            "KN", // KwaZulu-Natal
            "NP", // Limpopo (NP)
            "MP", // Mpumalanga
            "NW", // North West
            "NC", // Northern Cape
            "WP"  // Western Cape
        };

        public string Format { get; set; } = "";    // Format (usually "csv")


        // Builds the province-specific download URL using the configured base and report mapping
        public string BuildProvinceUrl(string provinceCode, int? electionYear = null)
        {
            int year = electionYear ?? ElectionYear;
            int reportId = year switch
            {
                2021 => 1091,
                2016 => 1091,
                2011 => 1091,
                _ => ReportId
            };

            var basePart = !string.IsNullOrEmpty(BaseUrl) ? BaseUrl.TrimEnd('/') : Url.TrimEnd('/');
            return $"{basePart}/{reportId}/{ReportPath}/{provinceCode}.csv";
        }
    }
}