using Microsoft.VisualBasic;

namespace Hoeveel.Aggregator.Models.Config
{
    // This class contains the configuration for the Census sources
    public class CensusSourceConfig
    {
        public string Type { get; set; } = "";      // e.g. "csv-url"
        public string FilePath { get; set; } = "";  // Path where the file should be saved
        public string Url { get; set; } = "";       // URL to download the CSV
        public string Format { get; set; } = "";    // Format (usually "csv")
        public int[] Years {get; set;} = new[] {2011, 2022};    //available years to download (defaults to 2011 and 2022 if not specified in config)

        public string BuildPath(int year)
            => $"{FilePath}_{year}.{Format}";       // file path
    }
}
