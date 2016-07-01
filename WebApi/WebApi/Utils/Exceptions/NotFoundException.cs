
using DAW.Models;
namespace DAW.Utils.Exceptions
{
    class NotFoundException : MyException
    {
        private ErrorModel error;   
        public NotFoundException(string msg) : base()
        {
            this.error = new ErrorModel
            {
                type = "api/prob/not-found",
                title = "Resource not found",
                detail = msg,
                status = System.Net.HttpStatusCode.NotFound
            };
        }

        public override ErrorModel GetError()
        {
            return this.error;
        }
    }
}
