using Hoeveel.Aggregator.Mappers;

namespace Hoeveel.Aggregator.Models.Config;

// This class represents the overall source configuration loaded from config/sources.json
// It contains properties for each individual source configuration (UIFW, Census Province, Census Municipality)
    public class ExportOptionsConfig
{
    public string FilePath { get; set; } = string.Empty;   // Output file path

    public string Format { get; set; } = "json";           // Export format (future-proofing)

    public JsonSettingsConfig JsonSettings { get; set; } = new();  // Nested JSON settings

    public int[] Years {get; set;} = new[] {2011, 2022};    //available years to download (defaults to 2011 and 2022 if not specified in config)

        public string BuildPath(int year)
            => $"{FilePath}_{year}.{Format}";       // file path
}

public class JsonSettingsConfig
{
    public bool WriteIndented { get; set; }      // Controls indentation
    public bool UseCamelCase { get; set; }       // Controls camelCase conversion
    public bool IgnoreNulls { get; set; }        // Controls null omission
}