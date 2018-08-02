using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Продукт/Сервис из магазина/корзины
    /// </summary>
    public class CartProductModel
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
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Изображения
        /// </summary>
        public ProductImage[] Images { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Описание, цена за единицу товара и тп 
        /// </summary>
        public string PriceInfo { get; set; }

        /// <summary>
        /// Описание, цена за единицу товара и тп 
        /// </summary>
        public string PriceInfoPrev { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal? PricePrev { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal? Discount {
            get {
                if (PricePrev == null || PricePrev == 0.00m || PricePrev == Price)
                    return null;

                return (1.00m - Price / PricePrev) * 100 * (-1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Категории
        /// </summary>
        public CartCategoryModel[] Categories { get; set; }

    }

    public class ProductImage
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMain { get; set; }
    }

}
