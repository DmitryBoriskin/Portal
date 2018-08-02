using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CartModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления категории товаров интернет-магазина
    /// </summary>
    public class CartCategoryViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<CartCategoryModel> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public CartCategoryModel Item { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CartProductModel[] ProductsList { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterModel Filter { get; set; }
    }
}