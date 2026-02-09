namespace Hoeveel.Aggregator.Models.Config
{
    // This class contains the configuration for CSV sources
    public class CsvSourceConfig
    {
        public string Type { get; set; } = "";      // e.g. "csv-url"
        public string FilePath { get; set; } = "";  // Path where the file should be saved
        public string Url { get; set; } = "";            // URL to download the CSV
        public string Format { get; set; } = "";    // Format (usually "csv")
    }
}
