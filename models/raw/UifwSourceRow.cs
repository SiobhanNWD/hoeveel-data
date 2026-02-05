namespace Hoeveel.Aggregator.Models.Raw;

//This class maps ONE UIFW CSV -> ONE UifwSourceRow Object
//Csv Format:   demarcation_code,financial_year,item_code,item_label,amount,id"

public class UifwSourceRow
{
    public string DemarcationCode { get; set; } = "";
    public int FinancialYear { get; set; }

    public string ItemCode { get; set; } = "";
    public string ItemLabel { get; set; } = "";

    public decimal Amount { get; set; }

    public int SourceId { get; set; }
}
