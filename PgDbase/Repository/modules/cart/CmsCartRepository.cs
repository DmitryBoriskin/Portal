using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий вакансий для работы во внешней части
    /// </summary>
    public partial class CmsRepository
    {
        #region Заказы
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

                if (!string.IsNullOrEmpty(filter.SearchText))
                    query = query
                        .Where(c => c.n_order.ToString().Contains(filter.SearchText)
                        || c.c_client_name.ToLower().Contains(filter.SearchText.ToLower())
                        || c.c_client_email.ToLower().Contains(filter.SearchText.ToLower())
                        || c.c_client_phone.Contains(filter.SearchText));


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
                        Status = (OrderStatus)s.f_status,
                        DeliveryType = (DeliveryMethod)s.f_delivery,
                        AcquiringType = (AcquiringMethod)s.f_acquiring,
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
        #endregion

        #region Категории товаров

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

                if (filter.Disabled.HasValue)
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);

                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                    query = query.Where(w => w.c_name.ToLower().Contains(filter.SearchText.ToLower()));

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
                        Disabled = s.b_disabled,
                        TotalProducts = db.cart_products.Where(p => p.fkcategoriess.Any(t => t.f_category == s.id)).Count()

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
        /// Возвращает список категорий товаров из магазина
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
        /// Возвращает категорий товара из магазина со списком товаров
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
        /// Проверка наличия в базе записи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckCartCategoryExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cart_categories
                    .Where(w => w.id == id);

                if (data.Any())
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Новая запись
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertCartCategory(CartCategoryModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cart_categories
                        .Where(s => s.id == item.Id);
                    if (!data.Any())
                    {
                        var newCategory = new cart_categories()
                        {
                            id = item.Id,
                            f_site = _siteId,
                            c_name = item.Title,
                            c_desc = item.Desc,
                            b_disabled = item.Disabled,
                            c_icon = item.Icon,
                            d_date_create = DateTime.Now,
                            c_user_create = _currentUserId.ToString()
                        };

                        db.Insert(newCategory);

                        var log = new LogModel()
                        {
                            PageId = item.Id,
                            PageName = item.Title,
                            Section = LogModule.Cart,
                            Action = LogAction.insert,
                            Comment = "Добавлена новая категория"
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Новая запись
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateCartCategory(CartCategoryModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cart_categories
                        .Where(s => s.id == item.Id);
                    if (data.Any())
                    {
                        var category = data.Single();
                        category.c_name = item.Title;
                        category.c_desc = item.Desc;
                        category.b_disabled = item.Disabled;
                        category.c_icon = item.Icon;
                        category.d_date_create = DateTime.Now;
                        category.c_user_create = _currentUserId.ToString();

                        db.Update(category);

                        var log = new LogModel()
                        {
                            PageId = item.Id,
                            PageName = item.Title,
                            Section = LogModule.Cart,
                            Action = LogAction.update,
                            Comment = "Изменена категория"
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Удаление записи
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool DeleteCartCategory(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cart_categories
                        .Where(s => s.id == id);

                    if (data.Any())
                    {
                        var category = data.Single();

                        //Логирование
                        var log = new LogModel()
                        {
                            PageId = category.id,
                            PageName = category.c_name,
                            Section = LogModule.Cart,
                            Action = LogAction.update,
                            Comment = "Изменена категория"
                        };
                        InsertLog(log);

                        db.Delete(category);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        #endregion

        #region Товары

        /// <summary>
        /// Возвращает постраничный список категорий товаров из магазина
        /// </summary>
        /// <returns></returns>
        public Paged<CartProductModel> GetProducts(CartFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<CartProductModel>();
                var query = db.cart_products
                    .Where(s => !s.b_disabled);

                if (filter.CategoryId.HasValue)
                    query = query.Where(s => s.fkcategoriess.Any(c => c.f_category == filter.CategoryId.Value));

                if (!string.IsNullOrEmpty(filter.Category) && Guid.TryParse(filter.Category, out Guid catId))
                    query = query.Where(s => s.fkcategoriess.Any(c => c.f_category == catId));

                if (filter.Disabled.HasValue)
                    query = query.Where(s => s.b_disabled == filter.Disabled.Value);

                if (!string.IsNullOrEmpty(filter.SearchText))
                    query = query.Where(s => s.c_title.ToLower().Contains(filter.SearchText.ToLower())
                                    || s.c_desc.Contains(filter.SearchText.ToLower()));

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new CartProductModel
                    {
                        Id = s.id,
                        Number = s.n_product,
                        Title = s.c_title,
                        Desc = s.c_desc,
                        Price = s.n_price,
                        PricePrev = s.n_price_old,
                        PriceInfoPrev = s.c_price_old,
                        PriceInfo = s.c_price,
                        Disabled = s.b_disabled,

                        Categories = GetCartCategoriesList(new CartFilter() { ProductId = s.id }),
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
        /// Возвращает список(массив) категорий товаров из магазина
        /// </summary>
        /// <returns></returns>
        public CartProductModel[] GetProductsList(CartFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<CartProductModel>();
                var query = db.cart_products_categories
                    .Where(c => !c.fkproductscategoriesproducts.b_disabled);

                if (filter.CategoryId.HasValue)
                    query = query
                        .Where(c => c.f_category == filter.CategoryId.Value);

                var list = query
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

                return list;
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
        /// Проверка наличия в базе записи
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckCartProductExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.cart_products
                    .Where(w => w.id == id);

                if (data.Any())
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Новая запись
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertCartProduct(CartProductModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cart_products
                        .Where(s => s.id == item.Id);

                    if (!data.Any())
                    {
                        var newProduct = new cart_products()
                        {
                            id = item.Id,
                            c_title = item.Title,
                            c_desc = item.Desc,
                            b_disabled = item.Disabled,
                            c_price = item.PriceInfo,
                            n_price = item.Price,
                            c_price_old = item.PriceInfoPrev,
                            n_price_old = item.PricePrev,
                            f_site = _siteId,
                            //n_product
                            d_date_create = DateTime.Now,
                            c_user_create = _currentUserId.ToString()
                        };

                        var log = new LogModel()
                        {
                            PageId = item.Id,
                            PageName = item.Title,
                            Section = LogModule.Cart,
                            Action = LogAction.insert,
                            Comment = "Добавлен новый товар/услуга"
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Новая запись
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateCartProduct(CartProductModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cart_products
                        .Where(s => s.id == item.Id);

                    if (data.Any())
                    {
                        var product = data.Single();

                        product.c_title = item.Title;
                        product.c_desc = item.Desc;
                        product.b_disabled = item.Disabled;
                        product.c_price = item.PriceInfo;
                        product.n_price = item.Price;
                        product.c_price_old = item.PriceInfoPrev;
                        product.n_price_old = item.PricePrev;

                        product.d_date_update = DateTime.Now;
                        product.c_user_update = _currentUserId.ToString();

                        var log = new LogModel()
                        {
                            PageId = item.Id,
                            PageName = item.Title,
                            Section = LogModule.Cart,
                            Action = LogAction.update,
                            Comment = "Изменен товар/услуга"
                        };
                        InsertLog(log);


                        tran.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаление записи
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool DeleteCartProduct(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.cart_products
                        .Where(s => s.id == id);

                    if (data.Any())
                    {
                        var product = data.Single();

                        //Логирование
                        var log = new LogModel()
                        {
                            PageId = product.id,
                            PageName = product.c_title,
                            Section = LogModule.Cart,
                            Action = LogAction.update,
                            Comment = "Изменена категория"
                        };
                        InsertLog(log);

                        db.Delete(product);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }
        #endregion
    }
}
