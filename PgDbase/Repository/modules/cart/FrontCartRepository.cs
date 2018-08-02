using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.front
{
    /// <summary>
    /// Репозиторий вакансий для работы во внешней части
    /// </summary>
    public partial class FrontRepository
    {
        /// <summary>
        /// Возвращает постраничный список категорий товаров из магазина
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<CartCategoryModel> GetCartCategories(CartFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<CartCategoryModel>();
                var query = db.cart_categories
                    .Where(w => !w.b_disabled);

                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                    query = query.Where(w => w.c_name.Contains(filter.SearchText));

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new CartCategoryModel
                    {
                        Id = s.id,
                        Icon = s.c_icon,
                        Title = s.c_name,
                        Desc = s.c_desc,
                        Disabled = s.b_disabled

                    }).ToArray();

                return new Paged<CartCategoryModel>
                {
                    Items = list,
                    Pager = new PagerModel
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }

        /// <summary>
        /// Возвращает список категорий товаров из магазина в виде массива
        /// </summary>
        /// <returns></returns>
        public CartCategoryModel[] GetCartCategoriesList(CartFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cart_categories
                  .Where(w => !w.b_disabled);

                //Список категорий, к которым отнесен товар
                if (filter.ProductId.HasValue)
                    query = query
                        .Where(c => c.fkproductss.Any(p => p.f_product == filter.ProductId.Value));

                var data = query
                  .Select(s => new CartCategoryModel
                  {
                      Id = s.id,
                      Icon = s.c_icon,
                      Title = s.c_name,
                      Desc = s.c_desc
                  }).ToArray();

                return data;
            }
        }

        /// <summary>
        /// Возвращает категорию
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CartCategoryModel GetCartCategory(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cart_categories
                    .Where(w => w.id == id)
                    .Where(w => !w.b_disabled)
                    .Select(s => new CartCategoryModel
                    {
                        Id = s.id,
                        Icon = s.c_icon,
                        Title = s.c_name,
                        Desc = s.c_desc,
                        Disabled = s.b_disabled,
                        TotalProducts = db.cart_products.Where(p => p.fkcategoriess.Any(t => t.f_category == s.id)).Count()
                        //Products заполняем в контроллере
                    })
                    .SingleOrDefault();

                return data;
            }
        }

        /// <summary>
        /// Возвращает список категорий товаров из магазина
        /// </summary>
        /// <returns></returns>
        public Paged<CartProductModel> GetProducts(CartFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<CartProductModel>();
                var query = db.cart_products_categories
                    .Where(c => !c.fkproductscategoriesproducts.b_disabled);

                if (filter.CategoryId.HasValue)
                    query = query
                        .Where(c => c.f_category == filter.CategoryId.Value);

                /*if (!String.IsNullOrWhiteSpace(filter.SearchText))
                    query = query
                        .Where(c => c.fkproductcategoriesproducts.c_title.Contains(filter.SearchText)
                                    || c.fkproductcategoriesproducts.c_desc.Contains(filter.SearchText)
                                    || c.fkproductcategoriescategories.c_name.Contains(filter.SearchText)
                                    || c.fkproductcategoriescategories.c_desc.Contains(filter.SearchText)
                                    );*/

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new CartProductModel
                    {
                        Id = s.f_product,
                        Number = s.fkproductscategoriesproducts.n_product,
                        Title = s.fkproductscategoriesproducts.c_title,
                        Desc = s.fkproductscategoriesproducts.c_desc,
                        Price = s.fkproductscategoriesproducts.n_price,
                        PricePrev = s.fkproductscategoriesproducts.n_price_old,
                        PriceInfoPrev = s.fkproductscategoriesproducts.c_price_old,
                        PriceInfo = s.fkproductscategoriesproducts.c_price,
                        Disabled = s.fkproductscategoriesproducts.b_disabled,

                        Categories = GetCartCategoriesList(new CartFilter() { ProductId = s.f_product }),
                        //Images = 

                    }).ToArray();

                return new Paged<CartProductModel>
                {
                    Items = list,
                    Pager = new PagerModel
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }

        /// <summary>
        /// Возвращает информацию о товаре из магазина
        /// </summary>
        /// <returns></returns>
        public CartProductModel GetProduct(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<CartProductModel>();
                var query = db.cart_products
                    .Where(c => c.id == id);

                var data = query
                    .Select(s => new CartProductModel
                    {
                        Id = s.id,
                        Number = s.n_product,
                        Title = s.c_title,
                        Desc = s.c_desc,
                        Price = s.n_price,
                        PricePrev = s.n_price_old,
                        PriceInfo = s.c_price,
                        PriceInfoPrev = s.c_price_old,
                        Disabled = s.b_disabled,

                        Categories = GetCartCategoriesList(new CartFilter() { ProductId = id }),
                        // Images

                    }).SingleOrDefault();

                return data;
            }
        }

        /// <summary>
        /// Список заказов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public OrderedItemModel[] GetCart(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<OrderedItemModel>();
                var query = db.cart_cart_items
                    .Where(c => c.f_client == userId)
                    .Where(c => !c.fkitemsproduct.b_disabled);

                var list = query
                    .Select(s => new OrderedItemModel
                    {
                        Id = s.id,
                        Number = s.fkitemsproduct.n_product,
                        Title = s.fkitemsproduct.c_title,
                        Desc = s.fkitemsproduct.c_desc,
                        PriceInfo = s.fkitemsproduct.c_price,
                        Price = s.fkitemsproduct.n_price,

                        Amount = s.n_amount
                    }).ToArray();

                return list;
            }
        }

        /// <summary>
        /// Оформление заказа
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool SendOrder(OrderModel order)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    //Получаем данные о товарах в корзине и количестве
                    var query = db.cart_cart_items
                    .Where(c => c.f_client == order.Id)
                    .Where(c => !c.fkitemsproduct.b_disabled);

                    var list = query
                        .Select(s => new OrderedItemModel
                        {
                            Id = s.id,
                            Number = s.fkitemsproduct.n_product,
                            Title = s.fkitemsproduct.c_title,
                            Desc = s.fkitemsproduct.c_desc,
                            PriceInfo = s.fkitemsproduct.c_price,
                            Price = s.fkitemsproduct.n_price,
                            Amount = s.n_amount
                        }).ToArray();

                    //Записываем в таблицу заказов
                    var newOrderId = Guid.NewGuid();
                    var newOrderDate = DateTime.Now;

                    var newOrder = new cart_orders()
                    {
                        id = newOrderId,
                        d_date = newOrderDate,
                        f_site = _siteId,
                        f_client = order.UserId,
                        c_client_phone = order.UserPhone,
                        c_client_email = order.UserEmail,
                        c_client_address = order.UserAddress,
                        c_note = order.Note,
                        f_status = (int)OrderStatus.Processing,
                        f_delivery = (int)order.DeliveryType,
                        f_acquiring = (int)order.AcquiringType,
                        b_payed = order.Payed
                    };

                    db.Insert(newOrder);

                    foreach (var item in list)
                    {
                        var newOrderProduct = new cart_orders_items()
                        {
                            id = Guid.NewGuid(),
                            f_order = newOrderId,
                            f_product = item.Id,
                            c_product = $"{item.Title} {item.PriceInfo}",
                            n_price = item.Price,
                            n_amount = item.Amount,
                            n_total_sum = item.AmountSum
                        };

                        db.Insert(newOrderProduct);
                    }

                    //Очищаем корзину
                    db.cart_cart_items
                        .Where(s => s.f_client == order.UserId)
                        .Where(s => s.f_site == _siteId)
                        .Delete();

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Список заказов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<OrderModel> GetOrders(CartFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<OrderModel>();
                var query = db.cart_orders
                    .AsQueryable();

                if (filter.UserId.HasValue)
                    query = query
                        .Where(c => c.f_client == filter.UserId.Value);

                if (filter.Date.HasValue)
                    query = query
                        .Where(c => c.d_date > filter.Date.Value);

                if (filter.DateEnd.HasValue)
                    query = query
                        .Where(c => c.d_date < filter.DateEnd.Value.AddDays(1));

                if (!string.IsNullOrEmpty(filter.Type) && int.TryParse(filter.Type, out int type))
                    query = query
                        .Where(c => c.f_status == type);


                int itemsCount = query.Count();

                var list = query
                    .OrderByDescending(s => s.d_date)
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new OrderModel
                    {
                        Id = s.id,
                        Number = s.n_order,
                        Date = s.d_date,
                        UserId = s.f_client,
                        UserName = s.c_client_name,
                        UserPhone = s.c_client_phone,
                        UserEmail = s.c_client_email,
                        UserAddress = s.c_client_address,
                        Payed = s.b_payed,
                        Status = (OrderStatus) s.f_status,
                        DeliveryType = (DeliveryMethod) s.f_delivery,
                        AcquiringType = (AcquiringMethod) s.f_acquiring,
                        Note = s.c_note,

                        Total = db.cart_orders_items.Where(p => p.f_order == s.id).Count(),
                        TotalSum = db.cart_orders_items.Where(p => p.f_order == s.id).Any()
                                    ? db.cart_orders_items.Where(p => p.f_order == s.id).Sum(p => p.n_total_sum)
                                    : 0.00m
                        //Products

                    }).ToArray();

                return new Paged<OrderModel>
                {
                    Items = list,
                    Pager = new PagerModel
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }

        /// <summary>
        /// Заказ - информация
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public OrderModel GetOrder(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cart_orders
                    .Where(s => s.id == id);

                var data = query
                    .Select(s => new OrderModel
                    {
                        Id = s.id,
                        Number = s.n_order,
                        Date = s.d_date,
                        UserId = s.f_client,
                        UserName = s.c_client_name,
                        UserPhone = s.c_client_phone,
                        UserEmail = s.c_client_email,
                        UserAddress = s.c_client_address,
                        Payed = s.b_payed,
                        Status = (OrderStatus)s.f_status,
                        DeliveryType = (DeliveryMethod)s.f_delivery,
                        AcquiringType = (AcquiringMethod)s.f_acquiring,
                        Note = s.c_note,
                        //Products в контроллере

                    }).SingleOrDefault();

                return data;
            }
        }

        /// <summary>
        /// Возвращает список товаров в заказе
        /// </summary>
        /// <returns></returns>
        public OrderedItemModel[] GetOrderItems(Guid orderId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cart_orders_items
                    .AsQueryable();

                var list = query
                    .Select(s => new OrderedItemModel
                    {
                        Id = s.id,
                        ProductId = s.f_product,
                        Number = db.cart_products.Where(t => t.id == s.f_product).Select(p => p.n_product).FirstOrDefault(),
                        Title = s.c_product,
                        Price = s.n_price,
                        Amount = s.n_amount,
                        AmountSum = s.n_total_sum,
                    })
                    .ToArray();

                return list;
            }
        }


        /// <summary>
        /// Список Id товаров в корзине
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Guid[] GetCartItemsIdList(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cart_cart_items
                    .Where(s => s.f_client == userId)
                    .Where(s => s.f_site == _siteId)
                    .Where(s => !s.fkitemsproduct.b_disabled);

                var list = query
                    .Select(s => s.f_product)
                    .ToArray();

                return list;
            }
        }

        /// <summary>
        /// Возвращает список товаров из корзины
        /// </summary>
        /// <returns></returns>
        public OrderedItemModel[] GetCartItems(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.cart_cart_items
                    .Where(s => s.f_client == userId)
                    .Where(s => s.f_site ==_siteId)
                    .Where(s => !s.fkitemsproduct.b_disabled);

                var list = query
                    .Select(s => new OrderedItemModel
                    {
                        Id = s.id,
                        ProductId = s.f_product,
                        Number = s.fkitemsproduct.n_product,
                        Title = s.fkitemsproduct.c_title,
                        Desc = s.fkitemsproduct.c_desc,
                        Price = s.fkitemsproduct.n_price,
                        PricePrev = s.fkitemsproduct.n_price_old,
                        PriceInfo = s.fkitemsproduct.c_price,
                        PriceInfoPrev = s.fkitemsproduct.c_price_old,
                        Disabled = s.fkitemsproduct.b_disabled,
                        Amount = s.n_amount
                    })
                    .ToArray();

                return list;
            }
        }

        /// <summary>
        /// Добавляем товар в корзину
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        public bool OrderAddProductToCart(Guid userId, Guid productId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cart_cart_items
                    .Where(o => o.f_client == userId)
                    .Where(o => o.f_product == productId);

                if (!data.Any())
                {
                    var product = new cart_cart_items()
                    {
                        id = Guid.NewGuid(),
                        f_site = _siteId,
                        f_client = userId,
                        f_product = productId,
                        n_amount = 1,
                        date = DateTime.Now
                    };

                    db.Insert(product);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Удаляем товар из корзины
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        public bool OrderRemoveProductFromCart(Guid userId, Guid productId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cart_cart_items
                    .Where(o => o.f_client == userId)
                    .Where(o => o.f_product == productId);

                if (data.Any())
                {
                    var product = data.Single();
                    db.Delete(product);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Добавляем товар в корзину
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        public bool OrderUpdateProductInCart(Guid userId, Guid productId, int amount)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cart_cart_items
                    .Where(o => o.f_client == userId)
                    .Where(o => o.f_product == productId);

                if (data.Any())
                {
                    var product = data.Single();
                    product.n_amount = amount;
                    db.Update(product);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Очистка корзины
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        public bool OrderClearCart(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cart_cart_items
                    .Where(o => o.f_client == userId)
                    .Delete();

                return true;
            }
        }
    }
}
