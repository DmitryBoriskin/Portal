using System.Web.Mvc;

namespace CartModule.Areas.Cart
{
    public class CartAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Cart";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Products_FE",
                "Cart/Products/{action}/{id}",
                new { controller = "Products", action = "Index", id = UrlParameter.Optional },
                new[] { "CartModule.Areas.Cart.Controllers" }
            );

            context.MapRoute(
                "Orders_FE",
                "Cart/Orders/{action}/{id}",
                new { controller = "Orders", action = "Index", id = UrlParameter.Optional },
                new[] { "CartModule.Areas.Cart.Controllers" }
            );
        }
    }
}