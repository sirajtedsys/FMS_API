using System.ComponentModel.DataAnnotations;

namespace FMS_API.TedtrackerClient.TTCDbNodels
{
    public class PRMMASTER_ProjectLoginSettings
    {
        [Key]
        public int LOGIN_ID { get; set; }
        public string PROJ_ID { get; set; }
        public string TOKEN { get; set; }
        public DateTime CREATE_ON { get; set; }

    }
}
