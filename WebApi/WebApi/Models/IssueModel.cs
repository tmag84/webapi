using System;
using System.Collections.Generic;
using WebApi.Hal;

namespace DAW.Models
{
    public class IssueModel : Representation
    {
        public string proj_name { get; set; }
        public int issue_id { get; set; }
        public string issue_title { get; set; }
        public string issue_description { get; set; }
        public DateTime issue_creationDate { get; set; }
        public string issue_state { get; set; }
        public List<string> tags { get; set; }
        public List<CommentModel> comments { get; set; }
    }
}