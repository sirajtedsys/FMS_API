namespace FMS_API.Data.Class
{
    public class LeaveRequest
    {

       
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public string StatusId { get; set; }
            public string? CreateUser { get; set; }
            public string LeaveReason { get; set; }
            public string LeaveRequestDate { get; set; }
        

    }
}
