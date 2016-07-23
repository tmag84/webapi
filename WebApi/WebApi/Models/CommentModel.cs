using System;
using WebApi.Hal;

namespace DAW.Models
{   
    public class CommentModel : Representation
    {
        public string proj_name { get; set; }
        public int issue_id { get; set; }
        public int comment_id { get; set; }        
        public string comment_text { get; set; }
        public DateTime comment_creationDate { get; set; }
    }
}