

using DAW.Models;
namespace DAW.Utils.Exceptions
{
    class PaginationException : MyException
    {
        private ErrorModel error;
        public PaginationException(string msg) : base()
        {
            this.error = new ErrorModel
            {
                type = "api/prob/pagination-out-of-bounds",
                title = "Pagination out of bounds",
                detail = msg,
                status = System.Net.HttpStatusCode.NoContent
            };
        }
        public override ErrorModel GetError()
        {
            return this.error;
        }
    }
}
