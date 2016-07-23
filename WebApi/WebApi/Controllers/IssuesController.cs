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
        private const int DEFAULT_PAGESIZE = 5;
   
        [HttpGet, Route("{id:int}")]
        public HttpResponseMessage GetIssueById(string name, int id)
        {
            HttpResponseMessage resp;
            var projectUriMaker = Request.TryGetUriMakerFor<ProjectsController>();
            var issueUriMaker = Request.TryGetUriMakerFor<IssuesController>();
            try
            {
                IssueModel issue = DB_Gets.GetProjectIssueById(name, id);

                foreach (CommentModel comment in issue.comments)
                {
                    comment.Links.Add(new Link("edit_comment", issueUriMaker.UriFor(p => p.PutComment(name, id, comment.comment_id, null)).AbsolutePath));
                    comment.Rel = "comments";
                }

                issue.Links.Add(new Link("self", issueUriMaker.UriFor(c => c.GetIssueById(name, id)).AbsolutePath));
                issue.Links.Add(new Link(name, projectUriMaker.UriFor(c => c.GetProjectByName(name, DEFAULT_PAGESIZE, 1)).AbsolutePath));
                issue.Links.Add(new Link("all-projects", projectUriMaker.UriFor(c => c.GetAllProjects()).AbsolutePath));
                issue.Links.Add(new Link("add_issue_tag", issueUriMaker.UriFor(c => c.PostTagInIssue(name,id,null)).AbsolutePath));
                issue.Links.Add(new Link("delete_issue_tag", issueUriMaker.UriFor(c => c.DeleteTagFromIssue(name, id, null)).AbsolutePath));
                issue.Links.Add(new Link("set_state", issueUriMaker.UriFor(c => c.PutStateIntoIssue(name, id, null)).AbsolutePath));
                issue.Links.Add(new Link("post_comment", issueUriMaker.UriFor(p => p.PostComment(name, id, null)).AbsolutePath));
                issue.Rel = "comments";


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
        
        [HttpPost, Route("post-issue")]
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
            return resp;
        }

        [HttpPost, Route("{id:int}/post-tag")]
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

        [HttpPut, Route("{id:int}/set-state")]
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

        [HttpPut, Route("{id:int}/edit-comment")]
        public HttpResponseMessage PutComment(string name, int id, int cid, CommentModel comment)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<IssuesController>();
            try
            {
                comment.comment_id = cid;
                DB_Puts.PutComment(name, id, comment);
                resp = new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PutComment(name, id, cid, comment)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }
        
        [HttpPost, Route("{id:int}/post-comment")]
        public HttpResponseMessage PostComment(string name, int id, CommentModel comment)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<IssuesController>();
            try
            {
                DB_Posts.PostComment(name, id, comment);
                resp = new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PostComment(name, id, comment)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }
    }
}