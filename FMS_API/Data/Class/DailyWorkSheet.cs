namespace FMS_API.Data.Class
{
    public class DailyWorkSheet
    {
        public string WorkId { get; set; }
        public string? TotalWorkPercentage { get; set; }
        public string? TotalWorkHours { get; set; }
        public string? TotalWorkMinutes { get; set; }
        public string? WorkDate { get; set; }
        //public string CreateUser { get; set; }
        public string? StatusId { get; set; }
        public string? ProgressNote { get; set; }
        public string? WorkDtlsId { get; set; }

        public long? EmpId { get; set; } = 0;
    }
}
