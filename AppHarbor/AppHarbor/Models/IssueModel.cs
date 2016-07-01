using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebApi.Hal;

namespace DAW.Models
{
    public class IssueModel : Representation
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime creationDate { get; set; }
        public string state { get; set; }
        public List<string> tags { get; set; }
        public List<CommentModel> comments { get; set; }
    }

}