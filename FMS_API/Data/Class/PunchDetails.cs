namespace FMS_API.Data.Class
{
    public class PunchDetails
    {
       
        public int P_OPR_ID { get; set; }
            public long PunchId { get; set; } = 0;
            public DateTime? PunchDate { get; set; }
            public int PunchType { get; set; } = 0;
            public long EmpId { get; set; } = 0;
            public string? Where1 { get; set; }
            public string? Where { get; set; }
            public int Hdr { get; set; } = 0;
            public string? StrSDate { get; set; }
            public string? StrFDate { get; set; }
            public int? PunchFrom { get; set; }
            public long? ProjectId { get; set; } = 0;
        public string? PunchRemarks { get; set; } 
			


	}
}
