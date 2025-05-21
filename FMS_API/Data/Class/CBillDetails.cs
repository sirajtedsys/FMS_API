namespace FMS_API.Data.Class
{
    public class CBillDetails
    {
        public int CrbcId { get; set; }
        public string Section { get; set; }
        public string TransId { get; set; }
        public string TransNo { get; set; }
        public string TransDate { get; set; }
        public decimal SetAmt { get; set; }
        public decimal PrevColAmt { get; set; }
        public int CustId { get; set; }
        public decimal DiscAmt { get; set; }
        public decimal TransAmt { get; set; }
    }
}
