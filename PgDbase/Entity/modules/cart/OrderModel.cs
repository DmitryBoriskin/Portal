using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Продукт/Сервис из магазина/корзины
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Дата отправки заказа
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        public string UserAddress { get; set; }

        /// <summary>
        /// Статус оплаты
        /// </summary>
        public bool Payed { get; set; }

        /// <summary>
        /// Способ доставки
        /// </summary>
        public DeliveryMethod DeliveryType { get; set; }

        /// <summary>
        /// Способ оплаты
        /// </summary>
        public AcquiringMethod AcquiringType { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Количество позиций в заказе
        /// </summary>
        public int Total
        {
            get
            {
                if (Products != null && Products.Count() > 0)
                    return Products.Count();

                return 0;
            }
        }

        /// <summary>
        /// Сумма к оплате
        /// </summary>
        public decimal TotalSum {
            get
            {
                if (Products != null && Products.Count() > 0)
                    return Products.Sum(p => p.AmountSum);

                return 0.00m;
            }
        }

        /// <summary>
        /// Позиции (список заказанных товаров)
        /// </summary>
        public OrderedItemModel[] Products { get; set; }

    }

    public class OrderedItemModel: ProductModel
    {
        /// <summary>
        /// Сылка на продукт
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Цена за n единиц товара
        /// </summary>
        public decimal AmountSum { get; set; }
    }

    public enum OrderStatus
    {

        /// <summary>
        /// В ожидании
        /// </summary>
        Pending = 1,

        /// <summary>
        /// В обработке
        /// </summary>
        Processing = 2,

        /// <summary>
        /// Отправлен
        /// </summary>
        Shipped = 3,

        /// <summary>
        ///Выполнен
        /// </summary>
        Complete = 4,

        /// <summary>
        /// Отменен
        /// </summary>
        Сanceled = 5,

        /// <summary>
        /// Ошибка
        /// </summary>
        Error = 6
    }

    public enum DeliveryMethod
    {
        /// <summary>
        /// Нет
        /// </summary>
        No = 1,

        /// <summary>
        /// Самовывоз
        /// </summary>
        Self = 2,

        /// <summary>
        /// Курьром
        /// </summary>
        Сourier = 3,

        /// <summary>
        /// Почтой
        /// </summary>
        Post = 4,
    }

    public enum AcquiringMethod
    {
        
        /// <summary>
        ///Наличными
        /// </summary>
        Cash = 1,

        /// <summary>
        /// Банковской картой
        /// </summary>
        Card = 2,

        /// <summary>
        /// Банковский перевод
        /// </summary>
        Transfer = 3,
    }
}
