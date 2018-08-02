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
    public class CartProductViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<CartProductModel> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public CartProductModel Item { get; set; }

        /// <summary>
        /// Список всех категорий
        /// </summary>
        public CartCategoryModel[] Categories { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterModel Filter { get; set; }
    }
}