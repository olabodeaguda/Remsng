using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class Response
    {
        public string code { get; set; }
        public bool status { get; set; }
        public string description { get; set; }
        public object data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> errors { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? totalPages { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? noOfRecords { get; set; }
    }
}
