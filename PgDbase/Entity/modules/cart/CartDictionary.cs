using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Репозиторий вакансий для работы во внешней части
    /// </summary>
    public class CartDictionary
    {
        public static Dictionary<OrderStatus, string> OrderStatusDic
        {
            get
            {
                return new Dictionary<OrderStatus, string>()
                        {
                            { OrderStatus.Pending, "Ожидается"},
                            { OrderStatus.Processing, "Обрабатывается"},
                            { OrderStatus.Shipped, "Отправлен"},
                            { OrderStatus.Complete, "Выполнен"},
                            { OrderStatus.Сanceled, "Аннулирован"},
                            { OrderStatus.Error, "Ошибка"},
                        };
            }
        }

        public static Dictionary<AcquiringMethod, string> OrderAcquiringDic
        {
            get
            {
                return new Dictionary<AcquiringMethod, string>()
                        {
                            { AcquiringMethod.Cash, "Наличные"},
                            { AcquiringMethod.Card, "Карта"},
                            { AcquiringMethod.Transfer, "Банковский перевод"},
                        };
            }

        }


        public static Dictionary<DeliveryMethod, string> OrderDeliveryDic
        {
            get
            {
                return new Dictionary<DeliveryMethod, string>()
                        {
                            { DeliveryMethod.No, "Не указано"},
                            { DeliveryMethod.Self, "Самовывоз"},
                            { DeliveryMethod.Post, "Почта"},
                            { DeliveryMethod.Сourier, "Курьер"},
                        };
            }

        }
    }
}
