using System.Collections.Generic;

namespace RemsNG.Common.Models
{
    public class Response
    {
        public string code { get; set; }
        public bool status { get; set; }
        public string description { get; set; }
        public object data { get; set; }
        public List<string> errors { get; set; }
        public int? totalPages { get; set; }
        public int? noOfRecords { get; set; }
    }
}
