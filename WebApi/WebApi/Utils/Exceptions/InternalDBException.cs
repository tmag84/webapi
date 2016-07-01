
using DAW.Models;
namespace DAW.Utils.Exceptions
{
    class InternalDBException : MyException
    {
        private ErrorModel error;
        public InternalDBException(string msg) : base()
        {
            this.error = new ErrorModel
            {
                type = "api/prob/duplicated-item",
                title = "Attempted to create duplicated item",
                detail = msg,
                status = System.Net.HttpStatusCode.Forbidden
            };
        }

        public override ErrorModel GetError()
        {
            return this.error;
        }
    }
}
