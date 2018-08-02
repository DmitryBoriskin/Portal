using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CartModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления заказов интернет-магазина
    /// </summary>
    public class OrderViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<OrderModel> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public OrderModel Item { get; set; }


        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterModel Filter { get; set; }
    }
}