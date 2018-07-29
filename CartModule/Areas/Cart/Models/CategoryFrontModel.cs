using PgDbase.entity;
using Portal.Models;
using System;

namespace CartModule.Areas.Cart.Models
{
    /// <summary>
    /// Модель представления категории товаров
    /// </summary>
    public class ProductCategoryFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public Paged<ProductCategoryModel> List { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ProductCategoryModel Item { get; set; }

        /// <summary>
        /// Ид товаров в корзине
        /// </summary>
        public Guid[] InCart { get; set; }

    }

}