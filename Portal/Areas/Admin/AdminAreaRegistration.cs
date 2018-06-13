using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                       "Portal_BE_Pannel",
                       "admin",
                       new { controller = "Main", action = "Index", id = UrlParameter.Optional },
                       new[] { "portal.areas.admin.controllers" }
                   );
            context.MapRoute(
                            "Portal_BE_Admins",
                            "admin/Admins/{action}/{id}",
                            new { controller = "Admins", action = "Index", id = UrlParameter.Optional },
                            new[] { "portal.areas.admin.controllers" }
                        );
            context.MapRoute(
                            "Portal_BE_Main",
                            "admin/Main/{action}/{id}",
                            new { controller = "Main", action = "Index", id = UrlParameter.Optional },
                            new[] { "portal.areas.admin.controllers" }
                        );
            context.MapRoute(
                            "Portal_BE_Menu",
                            "admin/Menu/{action}/{id}",
                            new { controller = "Menu", action = "Index", id = UrlParameter.Optional },
                            new[] { "portal.areas.admin.controllers" }
                        );
            context.MapRoute(
                           "Portal_BE_Modules",
                           "admin/Modules/{action}/{id}",
                           new { controller = "Modules", action = "Index", id = UrlParameter.Optional },
                           new[] { "portal.areas.admin.controllers" }
                       );
            context.MapRoute(
                           "Portal_BE_News",
                           "admin/News/{action}/{id}",
                           new { controller = "News", action = "Index", id = UrlParameter.Optional },
                           new[] { "portal.areas.admin.controllers" }
                       );
            context.MapRoute(
                          "Portal_BE_Pages",
                          "admin/Pages/{action}/{id}",
                          new { controller = "Pages", action = "Index", id = UrlParameter.Optional },
                          new[] { "portal.areas.admin.controllers" }
                      );
            context.MapRoute(
                          "Portal_BE_Photo",
                          "admin/Photo/{action}/{id}",
                          new { controller = "Photo", action = "Index", id = UrlParameter.Optional },
                          new[] { "portal.areas.admin.controllers" }
                      );
            context.MapRoute(
                          "Portal_BE_PhotoAlbums",
                          "admin/PhotoAlbums/{action}/{id}",
                          new { controller = "PhotoAlbums", action = "Index", id = UrlParameter.Optional },
                          new[] { "portal.areas.admin.controllers" }
                      );
            context.MapRoute(
                          "Portal_BE_Roles",
                          "admin/Roles/{action}/{id}",
                          new { controller = "Roles", action = "Index", id = UrlParameter.Optional },
                          new[] { "portal.areas.admin.controllers" }
                      );
            context.MapRoute(
                         "Portal_BE_SiteAdmins",
                         "admin/SiteAdmins/{action}/{id}",
                         new { controller = "SiteAdmins", action = "Index", id = UrlParameter.Optional },
                         new[] { "portal.areas.admin.controllers" }
                     );
            context.MapRoute(
                        "Portal_BE_SiteModules",
                        "admin/SiteModules/{action}/{id}",
                        new { controller = "SiteModules", action = "Index", id = UrlParameter.Optional },
                        new[] { "portal.areas.admin.controllers" }
                    );
            context.MapRoute(
                        "Portal_BE_Sites",
                        "admin/Sites/{action}/{id}",
                        new { controller = "Sites", action = "Index", id = UrlParameter.Optional },
                        new[] { "portal.areas.admin.controllers" }
                    );
            context.MapRoute(
                        "Portal_BE_SiteUsers",
                        "admin/SiteUsers/{action}/{id}",
                        new { controller = "SiteUsers", action = "Index", id = UrlParameter.Optional },
                        new[] { "portal.areas.admin.controllers" }
                    );
            context.MapRoute(
                        "Portal_BE_Templates",
                        "admin/Templates/{action}/{id}",
                        new { controller = "Templates", action = "Index", id = UrlParameter.Optional },
                        new[] { "portal.areas.admin.controllers" }
                    );
            //context.MapRoute(
            //                "Portal_BE",
            //                "admin/{controller}/{action}/{id}",
            //                new { controller = "main", action = "Index", id = UrlParameter.Optional },
            //                new[] { "portal.areas.admin.controllers" }
            //            );
        }
    }
}