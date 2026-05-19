using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CatalogoAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de Web API

            // Remueve soporte XML y deja JSON
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Rutas de Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
