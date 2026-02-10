namespace Hoeveel.Aggregator.Models.Stored;

public class Province
{
    public string Code { get; set; } = "";     // e.g. "WC", "GP"
    public string Name { get; set; } = "";     

    public int Population { get; set; } = 0;   

    public decimal Unauthorised => Municipalities.Sum(m => m.Unauthorised);     // Calculates Sum of all the Municipalities' Unauthorized Amounts
    public decimal Irregular    => Municipalities.Sum(m => m.Irregular);        // Calculates Sum of all the Municipalities' Irregular Amounts
    public decimal Fruitless    => Municipalities.Sum(m => m.Fruitless);        // Calculates Sum of all the Municipalities' Fruitless Amounts
    public decimal Uifw         => Municipalities.Sum(m => m.Uifw);             // Calculates Sum of all the Municipalities' Uifw Amounts (which should equal Sum(unauthorized + irregular + fruitless))

    public string GoverningParty { get; set; } = "";          // Governing party (e.g. "ANC")

    public List<Municipality> Municipalities { get; set; } = new();             // List of all municipalities belonging to this Province
}
