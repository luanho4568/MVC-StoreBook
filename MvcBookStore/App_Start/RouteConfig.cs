using System.Web.Mvc;
using System.Web.Routing;

namespace MvcBookStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }

            );
            routes.MapRoute(
    name: "AdminSuaSach",
    url: "Admin/SuaSach/{id}",
    defaults: new { controller = "Admin", action = "SuaSach", id = UrlParameter.Optional }
);

        }
    }
}
