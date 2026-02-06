namespace Hoeveel.Aggregator.Models.Config;

public class SourceConfig
{
    public UifwSourceConfig Uifw { get; set; } = new();

    public CsvSourceConfig CensusProv { get; set; } = new();
    public CsvSourceConfig CensusMuni { get; set; } = new();
}
