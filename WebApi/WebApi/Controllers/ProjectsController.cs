using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAW.Utils;
using DAW.Utils.Exceptions;
using DAW.Utils.DB;
using DAW.Models;
using Drum;
using WebApi.Hal;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Linq;

namespace DAW.Controllers
{
    [RoutePrefix(Const_Strings.PROJECT_ROUTE_PREFIX)]
    public class ProjectsController : ApiController
    {
        private const int DEFAULT_PAGESIZE = 1;

        [HttpGet,Route("")]
        public HttpResponseMessage GetAllProjects()
        {            
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            try
            {
                List<ProjectModel> projects = DB_Gets.GetAllProjects(); 
                foreach(ProjectModel p in projects)
                {
                    //Href e Rel for Self 
                    p.Href = uriMaker.UriFor(c => c.GetProjectByName(p.name, DEFAULT_PAGESIZE, 1)).AbsoluteUri;
                    p.Rel = "projects";
                }

                ProjectsModel projects_hal = new ProjectsModel(projects);
                projects_hal.Href = uriMaker.UriFor(c => c.GetAllProjects()).AbsoluteUri;
               
                resp = Request.CreateResponse<ProjectsModel>(HttpStatusCode.OK, projects_hal);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.GetAllProjects()).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                    error.status, error,
                    new JsonMediaTypeFormatter(),
                    new MediaTypeHeaderValue("application/problem+json"));     
            }
            return resp;
        }

        [HttpPost, Route("create")]
        public HttpResponseMessage PostProject(ProjectModel model)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            try
            {
                DB_Posts.PostProject(model);
                resp = new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PostProject(model)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

        [HttpGet, Route("search-for-project/{name}")]
        public HttpResponseMessage GetProjectsByNameSearch(string name)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            try
            {
                List<ProjectModel> projects = DB_Gets.GetAllProjects().FindAll(p => p.name.Contains(name));
                foreach (ProjectModel p in projects)
                {
                    p.Href = uriMaker.UriFor(c => c.GetProjectByName(p.name, DEFAULT_PAGESIZE, 1)).AbsoluteUri;
                    p.Rel = "projects";
                }
                ProjectsModel projects_hal = new ProjectsModel(projects);
                projects_hal.Href = uriMaker.UriFor(c => c.GetAllProjects()).AbsoluteUri;

                resp = Request.CreateResponse<ProjectsModel>(HttpStatusCode.OK, projects_hal);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.GetProjectsByNameSearch(name)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                    error.status, error,
                    new JsonMediaTypeFormatter(),
                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

        [HttpGet, Route("{name}")]
        public HttpResponseMessage GetProjectByName(string name, int pageSize = DEFAULT_PAGESIZE, int page = 1)
        {
            HttpResponseMessage resp;
            var projectUriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            try
            {
                ProjectModel project = DB_Gets.GetProjectByName(name, pageSize, page);
                project.Href = projectUriMaker.UriFor(c => c.GetProjectByName(name, pageSize, page)).AbsoluteUri;
                project.Rel = "projects";

                project.issues = project.issues
                        .Skip(pageSize * (page - 1))
                        .Take(pageSize)
                        .ToList();

                var issueUriMaker = Request.TryGetUriMakerFor<IssuesController>();
                foreach (IssueModel i in project.issues)
                {
                    i.Href = issueUriMaker.UriFor(c => c.GetIssueById(name, i.id)).AbsoluteUri;
                    i.Rel = "issues";
                }

                if (page > 1)
                {
                    project.Links.Add(new Link("prev", projectUriMaker.UriFor(c => c.GetProjectByName(name, pageSize, page - 1)).AbsoluteUri));
                }
                //checks if there's enough issues to put in next page              
                if ((page * pageSize) < project.totalIssues)
                {
                    project.Links.Add(new Link("next", projectUriMaker.UriFor(c => c.GetProjectByName(name, pageSize, page + 1)).AbsoluteUri));
                }


                var project_link = new Link("all-projects", projectUriMaker.UriFor(p => p.GetAllProjects()).AbsoluteUri);
                project.Links.Add(project_link);

                resp = Request.CreateResponse<ProjectModel>(HttpStatusCode.OK, project);

            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();

                error.instance = projectUriMaker.UriFor(c => c.GetProjectByName(name, pageSize, page)).AbsoluteUri;

                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

        [HttpGet, Route("{name}/number/{page:int}")]
        public HttpResponseMessage GetProjectByNameWithPage(string name, int pageSize = DEFAULT_PAGESIZE,int page = 1)
        {
            return GetProjectByName(name,DEFAULT_PAGESIZE,page);
        }

        [HttpGet, Route("{name}/search-for-issue")]
        public HttpResponseMessage GetProjectIssueByTitleSearch(string name, string title, int pageSize = DEFAULT_PAGESIZE, int page = 1)
        {
            HttpResponseMessage resp;
            var projectUriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            try
            {
                ProjectModel project = DB_Gets.SearchProjectIssues(name, title, pageSize, page);

                project.Href = projectUriMaker.UriFor(c => c.GetProjectByName(name, pageSize, page)).AbsoluteUri;
                project.Rel = "project";

                var issueUriMaker = Request.TryGetUriMakerFor<IssuesController>();
                foreach (IssueModel i in project.issues)
                {
                    i.Href = issueUriMaker.UriFor(c => c.GetIssueById(name, i.id)).AbsoluteUri;
                    i.Rel = "issues";
                }

                if (page > 1)
                {
                    project.Links.Add(new Link("prev", projectUriMaker.UriFor(c => c.GetProjectIssueByTitleSearch(name, title, pageSize, page - 1)).AbsoluteUri));
                }
                //checks if there's enough issues to put in next page              
                if ((page * pageSize) < project.totalIssues)
                {
                    project.Links.Add(new Link("next", projectUriMaker.UriFor(c => c.GetProjectIssueByTitleSearch(name, title, pageSize, page + 1)).AbsoluteUri));
                }

                var project_link = new Link("all-projects", projectUriMaker.UriFor(p => p.GetAllProjects()).AbsoluteUri);
                project.Links.Add(project_link);

                resp = Request.CreateResponse<ProjectModel>(HttpStatusCode.OK, project);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = projectUriMaker.UriFor(c => c.GetProjectIssueByTitleSearch(name, title,DEFAULT_PAGESIZE,page)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                    error.status, error,
                    new JsonMediaTypeFormatter(),
                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

        [HttpPost, Route("{name}/add-tag")]
        public HttpResponseMessage PostTagInProject(string name, string tag)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            try
            {
                DB_Posts.PostTagInProject(name, tag);
                resp = new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PostTagInProject(name, tag)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

        [HttpDelete, Route("{name}/delete-tag")]
        public HttpResponseMessage DeleteTagFromProject(string name, string tag)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            try
            {
                DB_Deletes.DeleteTagFromProject(name, tag);
                resp = new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.DeleteTagFromProject(name, tag)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

    }
}
