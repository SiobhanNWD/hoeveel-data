namespace Hoeveel.Aggregator.Models.Config;

//try use the same names as in the sorces.json file for consistency

public class SourceConfig
{
    public string uifwSourceUrl { get; set; } = string.Empty;
    public string uifwSourcePath { get; set; } = string.Empty;
}

//Use in SourceConfigLoader