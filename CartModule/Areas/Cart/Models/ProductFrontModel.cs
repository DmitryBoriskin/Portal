using PgDbase.entity;
using Portal.Models;
using System;

namespace CartModule.Areas.Cart.Models
{
    /// <summary>
    /// Модель представления товары
    /// </summary>
    public class ProductFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Paged<CartCategoryModel> Categories { get; set; }

        /// <summary>
        /// Список 
        /// </summary>
        public Paged<CartProductModel> List { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CartProductModel Item { get; set; }


        /// <summary>
        /// Ид товаров в корзине
        /// </summary>
        public Guid[] InCart { get; set; }


    }

}