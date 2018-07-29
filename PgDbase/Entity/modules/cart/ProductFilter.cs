using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Продукт/Сервис из магазина/корзины
    /// </summary>
    public class CartFilter: FilterModel
    {
        /// <summary>
        /// Клиент
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Цена по убыванию
        /// </summary>
        public bool? PriceDesc { get; set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public int? Status { get; set; }

    }
}
