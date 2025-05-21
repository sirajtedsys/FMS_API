namespace FMS_API.Data.Class
{
    public class EmpExpence
    {
        public string accMasterId {  get; set; }
        public string TotalAmt { get; set; }
        public string ExpDate { get; set; }
        public string ProjectId { get; set; }

        public List<EmpExpenceDetails> EmpExpenceDetails { get; set; }
    }
}
