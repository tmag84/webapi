using DAW.Utils;
using Drum;
using Newtonsoft.Json;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi.Hal;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ConfigureCors(config);
            ConfigureHttpRoutes(config);
            ConfigureFormatters(config);
        }

        private static void ConfigureCors(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("http://localhost:8000", "*", "*");
            config.EnableCors(cors);
        }

        private static void ConfigureHttpRoutes(HttpConfiguration config)
        {
            // Web API routes
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
        }

        private static void ConfigureFormatters(HttpConfiguration config)
        {
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