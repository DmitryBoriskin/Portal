using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Категория продукта - магазин/корзина
    /// </summary>
    public class CartCategoryModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Картинка
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Выключено
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Количество товаров в категории
        /// </summary>
        public int TotalProducts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Paged<CartProductModel> Products { get; set; }

    }
}
