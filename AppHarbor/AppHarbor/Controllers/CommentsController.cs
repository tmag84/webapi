using System.Web.Http;
using System.Net;
using System.Net.Http;
using DAW.Utils;
using DAW.Utils.Exceptions;
using DAW.Utils.DB;
using DAW.Models;
using Drum;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

namespace DAW.Controllers
{
    [RoutePrefix(Const_Strings.COMMENT_ROUTE_PREFIX)]
    public class CommentsController : ApiController
    {

        [HttpPut, Route("edit-comment/{cid}")]
        public HttpResponseMessage EditComment(string name, int id, int cid, CommentModel model)
        {
            HttpResponseMessage resp;
            model.Id = cid;
            var uriMaker = Request.TryGetUriMakerFor<CommentsController>();
            try
            {
                DB_Puts.PutComment(name, id, model);
                resp = new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.EditComment(name, id, cid, model)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));   
            }
            return resp;
        }

        [HttpPost, Route("create-comment")]
        public HttpResponseMessage PostComment(string name, int id, CommentModel model)
        {
            HttpResponseMessage resp;
            var uriMaker = Request.TryGetUriMakerFor<CommentsController>();
            try
            {
                DB_Posts.PostComment(name, id, model);
                resp = new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (MyException e)
            {
                ErrorModel error = e.GetError();
                error.instance = uriMaker.UriFor(c => c.PostComment(name, id, model)).AbsoluteUri;
                resp = Request.CreateResponse<ErrorModel>(
                                    error.status, error,
                                    new JsonMediaTypeFormatter(),
                                    new MediaTypeHeaderValue("application/problem+json"));
            }
            return resp;
        }
    }
}
