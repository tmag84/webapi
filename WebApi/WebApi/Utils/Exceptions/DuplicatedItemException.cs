

using DAW.Models;
namespace DAW.Utils.Exceptions
{
    class DuplicatedItemException : MyException
    {
        private ErrorModel error;
        public DuplicatedItemException(string msg) : base()
        {
            this.error = new ErrorModel
            {
                type = "api/prob/duplicated-item",
                title = "Attempted to create duplicated item",
                detail = msg,
                status = System.Net.HttpStatusCode.Conflict
            };            
        }

        public override ErrorModel GetError()
        {
            return this.error;
        }
    }
}
