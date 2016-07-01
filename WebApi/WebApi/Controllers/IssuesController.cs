using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAW.Utils;
using DAW.Utils.Exceptions;
using DAW.Utils.DB;
using Drum;
using WebApi.Hal;
using DAW.Models;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;


namespace DAW.Controllers
{
    [RoutePrefix(Const_Strings.ISSUE_ROUTE_PREFIX)]
    public class IssuesController : ApiController
    {
        private const int DEFAULT_PAGESIZE = 1;
   
        [HttpGet, Route("{id:int}")]
        public HttpResponseMessage GetIssueById(string name, int id)
        {
            HttpResponseMessage resp;
            var issueUriMaker = Request.TryGetUriMakerFor<IssuesController>();

            try
            {
                IssueModel issue = DB_Gets.GetProjectIssueById(name, id);

                issue.Href = issueUriMaker.UriFor(c => c.GetIssueById(name,id)).AbsoluteUri;
                issue.Rel = "issues";

                var projectUriMaker = Request.TryGetUriMakerFor<ProjectsController>();
                var project_link = new Link("project "+name, projectUriMaker.UriFor(c => c.GetProjectByName(name, DEFAULT_PAGESIZE, 1)).AbsoluteUri);
                issue.Links.Add(project_link);

                var all_projects_link = new Link("all-projects", projectUriMaker.UriFor(c => c.GetAllProjects()).AbsoluteUri);
                issue.Links.Add(all_projects_link);

                resp = Request.CreateResponse<IssueModel>(HttpStatusCode.OK, issue);

            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = issueUriMaker.UriFor(c => c.GetIssueById(name, id)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }


        [HttpPost, Route("create")]
        public HttpResponseMessage PostIssue(string name, IssueModel model)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<IssuesController>();
            try
            {
                DB_Posts.PostIssue(name, model);
                resp = new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PostIssue(name, model)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return null;
        }

        [HttpPost, Route("{id:int}/add-tag")]
        public HttpResponseMessage PostTagInIssue(string name, int id, string tag)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<IssuesController>();
            try
            {
                DB_Posts.PostTagInIssue(name, id, tag);
                resp = new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PostTagInIssue(name, id, tag)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

        [HttpDelete, Route("{id:int}/delete-tag")]
        public HttpResponseMessage DeleteTagFromIssue(string name, int id, string tag)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<IssuesController>();
            try
            {
                DB_Deletes.DeleteTagFromIssue(name, id, tag);
                resp = new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.DeleteTagFromIssue(name, id, tag)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }

        [HttpPut, Route("{id:int}/re-state")]
        public HttpResponseMessage PutStateIntoIssue(string name, int id, string new_state)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<IssuesController>();
            try
            {
                DB_Puts.PutStateInIssue(name, id, new_state);
                resp = new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PutStateIntoIssue(name, id, new_state)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }
    }
}
