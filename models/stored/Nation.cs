namespace Hoeveel.Aggregator.Models.Stored;

public class Nation
{
    public string Name { get; } = "South Africa";

    public decimal Population => Provinces.Sum(p => p.Population);            // Calculates Sum of all the Provinces' Populations
    
    public decimal Unauthorised => Provinces.Sum(p => p.Unauthorised);      // Calculates Sum of all the Provinces' Unauthorized Amounts
    public decimal Irregular    => Provinces.Sum(p => p.Irregular);         // Calculates Sum of all the Provinces' Irregular Amounts
    public decimal Fruitless    => Provinces.Sum(p => p.Fruitless);         // Calculates Sum of all the Provinces' Fruitless Amounts
    public decimal Uifw         => Provinces.Sum(p => p.Uifw);              // Calculates Sum of all the Provinces' Uifw Amounts (which should equal Sum(unauthorized + irregular + fruitless))

    public string GoverningParty { get; set; } = "";          // Governing party (e.g. "ANC")

    public List<Province> Provinces { get; set; } = new();                  // List of all provinces belonging to this nation
}
