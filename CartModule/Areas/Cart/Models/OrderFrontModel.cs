using PgDbase.entity;
using Portal.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CartModule.Areas.Cart.Models
{
    /// <summary>
    /// Модель представления заказы
    /// </summary>
    public class OrderFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public Paged<OrderModel> List { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrderModel Item { get; set; }

    }
}