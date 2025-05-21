using System.ComponentModel.DataAnnotations;

namespace FMS_API.Data.DbModel
{
    public class PRMMASTER_LoginSettings
    {
        [Key]
        public int LOGIN_ID { get; set; }
        public string EMP_ID { get; set; }
        public string TOKEN { get; set; }
        public DateTime CREATE_ON { get; set; }

    }
}
