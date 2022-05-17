using System.Collections.Generic;

namespace MobileChat.Models.Data
{
    public class Firebase
    {
        public class Data
        {
            public string message { get; set; }
            public string body { get; set; }
        }

        public class Message
        {
            public string to { get; set; }
            public Data data { get; set; }
        }

        public class Result
        {
            public string message_id { get; set; }
        }

        public class Response
        {
            public long multicast_id { get; set; }
            public int success { get; set; }
            public int failure { get; set; }
            public int canonical_ids { get; set; }
            public List<Result> results { get; set; }
        }
    }
}
