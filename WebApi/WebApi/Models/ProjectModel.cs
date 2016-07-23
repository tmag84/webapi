using System;
using System.Collections.Generic;
using WebApi.Hal;

namespace DAW.Models
{ 
    public class ProjectModel : Representation
    {
        public string proj_name { get; set; }
        public DateTime proj_creationDate { get; set; }
        public List<IssueModel> issues { get; set; }
        public List<string> tags { get; set; }
        public int totalIssues  { get; set; }
    }
}