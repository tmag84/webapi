using Owin;
using Microsoft.Owin;
using Drum;
using WebApi.Hal;
using System.Web.Http;
using System.Web.Http.Cors;
using DAW.Utils;
using Newtonsoft.Json;

[assembly: OwinStartup(typeof(DAWApi.Startup))]

namespace DAWApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            Register(config);
            app.UseWebApi(config);
        }

        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("http://localhost:8000", "*", "*");
            config.EnableCors(cors);

            config.MapHttpAttributeRoutesAndUseUriMaker();

            //project route            
            config.Routes.MapHttpRoute(
                "projects",
                Const_Strings.PROJECT_ROUTE_PREFIX
                );


            //issues route            
            config.Routes.MapHttpRoute(
                "issues",
                Const_Strings.ISSUE_ROUTE_PREFIX
                );


            //comments route            
            config.Routes.MapHttpRoute(
                "comments",
                Const_Strings.COMMENT_ROUTE_PREFIX
                );


            config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var jsonhal = new JsonHalMediaTypeFormatter();
            jsonhal.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonhal.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsonhal.SerializerSettings.Formatting = Formatting.Indented;
            config.Formatters.Add(jsonhal);
        }
    }
}
