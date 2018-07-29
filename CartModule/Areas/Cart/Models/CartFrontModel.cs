using PgDbase.entity;
using Portal.Models;

namespace CartModule.Areas.Cart.Models
{
    /// <summary>
    /// Модель представления корзины
    /// </summary>
    public class CartFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public OrderedItemModel[] List { get; set; }

    }

}