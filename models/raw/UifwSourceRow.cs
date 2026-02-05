namespace Hoeveel.Aggregator.Models.Raw;

//This class maps ONE UIFW CSV -> ONE UifwSourceRow Object
//Csv Format:   demarcation_code,financial_year,item_code,item_label,amount,id"

    public class UifwSourceRow
    {
        public string MunicipalityCode { get; set; } = string.Empty;  // demarcation_code
        public int FinancialYear { get; set; }                         // financial_year
        public string ItemCode { get; set; } = string.Empty;           // item_code
        public string ItemLabel { get; set; } = string.Empty;          // item_label
        public decimal Amount { get; set; }                            // amount
        public int SourceId { get; set; }                               // id
    }