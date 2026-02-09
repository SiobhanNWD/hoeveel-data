namespace Hoeveel.Aggregator.Models.Raw;

/// <summary>
/// Represents a single row of election data from the IEC (Independent Electoral Commission).
/// This raw model captures results at the party level for each municipality.
/// 
/// CHANGE LOG:
/// - Added: 2025-02-09 To support 2021 municipal election results (used as 2022 data)
/// </summary>
public class ElectionsRow
{
    /// <summary>
    /// The municipality code (demarcation code)
    /// Example: "BUF" for Buffalo City Metropolitan Municipality
    /// </summary>
    public string MunicipalityCode { get; set; } = "";
    
    /// <summary>
    /// The full name of the municipality
    /// Example: "Buffalo City Metropolitan Municipality"
    /// </summary>
    public string MunicipalityName { get; set; } = "";
    
    /// <summary>
    /// The province code where the municipality is located
    /// Example: "EC" for Eastern Cape
    /// </summary>
    public string ProvinceCode { get; set; } = "";
    
    /// <summary>
    /// The name of the political party
    /// Example: "ANC", "DA", "EFF", etc.
    /// </summary>
    public string PartyName { get; set; } = "";
    
    /// <summary>
    /// The number of votes received by this party in this municipality
    /// </summary>
    public int Votes { get; set; }
    
    /// <summary>
    /// The percentage of votes received by this party in this municipality
    /// </summary>
    public decimal VotePercentage { get; set; }
}
