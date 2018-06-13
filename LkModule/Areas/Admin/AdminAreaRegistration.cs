﻿using System.Web.Mvc;

namespace LkModule.Areas.Admin
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
            //context.MapRoute(
            //    "Admin_default",
            //    "Admin/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
              "Subscrs_BE",
              "Admin/Subscrs/{action}/{id}",
              new { controller = "Subscrs", action = "Index", id = UrlParameter.Optional },
              new[] { "LkModule.Areas.Admin.Controllers" }
           );
            context.MapRoute(
            "SubscrsWidget_BE",
            "Admin/SubscrsWidget/{action}/{id}",
            new { controller = "SubscrsWidget", action = "Index", id = UrlParameter.Optional },
            new[] { "LkModule.Areas.Admin.Controllers" }
         );
            context.MapRoute(
            "Charges_BE",
            "Admin/Charges/{action}/{id}",
            new { controller = "Charges", action = "Index", id = UrlParameter.Optional },
            new[] { "LkModule.Areas.Admin.Controllers" }
         );
            context.MapRoute(
              "Departments_BE",
              "Admin/Departments/{action}/{id}",
              new { controller = "Departments", action = "Index", id = UrlParameter.Optional },
              new[] { "LkModule.Areas.Admin.Controllers" }
           );
            context.MapRoute(
              "MeterDevices_BE",
              "Admin/MeterDevices/{action}/{id}",
              new { controller = "MeterDevices", action = "Index", id = UrlParameter.Optional },
              new[] { "LkModule.Areas.Admin.Controllers" }
           );
            context.MapRoute(
             "Payments_BE",
             "Admin/Payments/{action}/{id}",
             new { controller = "Payments", action = "Index", id = UrlParameter.Optional },
             new[] { "LkModule.Areas.Admin.Controllers" }
          );
        }
    }
}