using Hoeveel.Aggregator.Mappers;

namespace Hoeveel.Aggregator.Models.Config;

// This class represents the overall source configuration loaded from config/sources.json
// It contains properties for each individual source configuration (UIFW, Census Province, Census Municipality)
    public class SourceConfig
    {
        public UifwSourceConfig Uifw { get; set; } = new();
        public CsvSourceConfig CensusProv { get; set; } = new();
        public CsvSourceConfig CensusMuni { get; set; } = new();

        // Elections source configuration (config/sources.json -> "elections")
        public ElectionsSourceConfig Elections { get; set; } = new();
        public ExportOptionsConfig ExportOptions { get; set; } = new();   // Export options configuration (config/sources.json -> "exportOptions")s
        
    }

