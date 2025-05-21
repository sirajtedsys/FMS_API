namespace FMS_API.Data.Class
{
    public class DefaultMessage
    {
        public class Message1
        {
            public int Status { get; set; }
            public string Message { get; set; }
        }


        public class Message2
        {
            public int Id { get; set; }
            public int Error { get; set; }
        }

        public class Message3
        {
            public int Status { get; set; }
            public object Data { get; set; }
        }
    }
}
