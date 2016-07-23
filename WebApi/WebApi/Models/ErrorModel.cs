using WebApi.Hal;
namespace DAW.Models
{
    class ErrorModel
    {
        public string type { get; set; }
        public string title { get; set; }
        public string detail { get; set; }
        public string instance { get; set; }
        public System.Net.HttpStatusCode status { get; set; }
    }
}
