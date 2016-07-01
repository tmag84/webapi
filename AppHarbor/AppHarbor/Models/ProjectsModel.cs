using System.Collections.Generic;
using WebApi.Hal;

namespace DAW.Models
{
    
    public class ProjectsModel : SimpleListRepresentation<ProjectModel>
    {
        public ProjectsModel(IList<ProjectModel> projects)
            : base(projects)
        {
        }
    }
}
