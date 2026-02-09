namespace Hoeveel.Aggregator.Models.Config;

// This class represents the overall source configuration loaded from config/sources.json
// It contains properties for each individual source configuration (UIFW, Census Province, Census Municipality)
    public class SourceConfig
    {
        public UifwSourceConfig Uifw { get; set; } = new();
        public CensusSourceConfig CensusProv { get; set; } = new();
        public CensusSourceConfig CensusMuni { get; set; } = new();
    }

