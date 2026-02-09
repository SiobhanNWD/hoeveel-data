namespace Hoeveel.Aggregator.Models.Stored;

public class Municipality
{
    public string Code { get; set; } = "";         // Demarcation code (e.g. "BUF")
    public string Name { get; set; } = "";         
    public string ProvinceCode { get; set; } = ""; 

    public int Population { get; set; }            // Population

    public decimal Unauthorised { get; set; }       // Unauthorised Expenditure 
    public decimal Irregular { get; set; }          // Irregular Expenditure
    public decimal Fruitless { get; set; }          // Fruitless and Wasteful Expenditure
    public decimal Uifw                             // Total UIFW for the year
        => Unauthorised + Irregular + Fruitless;

    // TODO: public governingParty { get; set; } = "";          // Governing party (e.g. "ANC")
}