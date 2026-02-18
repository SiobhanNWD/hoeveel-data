namespace Hoeveel.Aggregator.Models.Stored;

public class JsonExportInfo
{
    public int _year { get; set; }               // Target year for the data (used to pick the nearest available year from the sources config when building URLs and file paths for each source)
    public int _uifwYear { get; set; }           // Actual year of the UIFW data used (picked from config based on the target year)
    public int _censusMuniYear { get; set; }     // Actual year of the Census Municipality data used (picked from config based on the target year)
    public int _censusProvYear { get; set; }     // Actual year of the Census Province data used (picked from config based on the target year)
    public int _electionsYear { get; set; }      // Actual year of the Elections data used (picked from config based on the target year)
}