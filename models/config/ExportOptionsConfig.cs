using Hoeveel.Aggregator.Mappers;

namespace Hoeveel.Aggregator.Models.Config;

// This class represents the overall source configuration loaded from config/sources.json
// It contains properties for each individual source configuration (UIFW, Census Province, Census Municipality)
    public class ExportOptionsConfig
{
    public string FilePath { get; set; } = string.Empty;   // Output file path

    public string Format { get; set; } = "json";           // Export format (future-proofing)

    public JsonSettingsConfig JsonSettings { get; set; } = new();  // Nested JSON settings
}

public class JsonSettingsConfig
{
    public bool WriteIndented { get; set; }      // Controls indentation
    public bool UseCamelCase { get; set; }       // Controls camelCase conversion
    public bool IgnoreNulls { get; set; }        // Controls null omission
}