using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Категория продукта - магазин/корзина
    /// </summary>
    public class ProductCategoryModel
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
        /// 
        /// </summary>
        public Paged<ProductModel> Products { get; set; }

    }
}
