using LinqToDB;
using PgDbase.entity;
using PgDbase.Entity.common;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.front
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        /// <summary>
        /// Идентифкатор сайта
        /// </summary>
        private Guid _siteId;

        /// <summary>
        /// ip-адрес
        /// </summary>
        private string _ip = string.Empty;

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private Guid _currentUserId = Guid.Empty;


        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

        public FrontRepository(string connectionString, Guid siteId, string ip, Guid userId)
        {
            _context = connectionString;
            _siteId = siteId;
            _ip = ip;
            _currentUserId = userId;

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


        /// <summary>
        /// Проверка прикреплен ли данный модуль к сайту
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool ModuleAllowed(string controllerName)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_site_controllers
                    .Where(t => t.fksitecontrollerscontrollers.c_controller_name.ToLower() == controllerName.ToLower())
                    .Where(t => t.f_site == _siteId);

                if (data.Any())
                    return true;

                return false;
            }
        }


        public LayoutModel GetLayoutInfo(Guid UserId)
        {
            LayoutModel model = new LayoutModel();
            using (var db = new CMSdb(_context))
            {
                #region главное меню
                //var q = db.core_page_groups.Where(w => w.f_site == _siteId && w.c_alias == "main")
                //          .Join(db.core_page_group_links, n => n.id, m => m.f_page_group, (n, m) => m)
                //          .Join(db.core_pages, e => e.f_page, o => o.gid, (e, o) => new { e, o })
                //          .Where(w=>w.o.b_disabled==false);

                //if (q.Any())
                //{
                //    model.MainMenu= q.OrderBy(o => o.e.n_sort)
                //            .Select(s => new PageModel
                //            {
                //                Name = s.o.c_name,                                
                //                Url = (String.IsNullOrEmpty(s.o.c_url))? "/page"+s.o.c_path+ s.o.c_alias: s.o.c_url,
                //                FaIcon=s.o.c_fa_icon,
                //                Childrens= GetChildMenu(s.o.gid)
                //            }).ToArray();
                //}
                #endregion


#warning !!!!Это функционал модуля LkModule  - данный функционал должен реализовываться как виджет
                #region  лицевые счета                
                var ls_q = db.lk_user_subscrs.Where(w => w.f_user == UserId)
                             .Join(db.lk_subscrs, u => u.f_subscr, s => s.id, (u, s) => new { u, s });
                if (ls_q.Any())
                {
                    //подключенные ЛС
                    model.ConnectionSubscrList = ls_q
                                                    .OrderByDescending(o => o.u.d_attached)
                                                    .Select(s => new SubscrModel()
                                                    {
                                                        Subscr = s.s.c_subscr,
                                                        Name = s.s.c_surname,
                                                        Default = s.u.b_default,
                                                        Id = s.s.id
                                                    }).ToArray();
                    //выбранный ЛС(по умолчанию)
                    model.DefaultSubscr = ls_q.Where(w => w.u.b_default == true)
                                              .Select(s => new SubscrModel
                                              {
                                                  Address = s.s.c_address,
                                                  Subscr = s.s.c_subscr,
                                                  Name = s.s.c_surname
                                              }).Single();
                }
                #endregion
            }
            return model;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Breadcrumbs> GetBreadCrumbCollection(string alias, string path)
        {
            List<Breadcrumbs> data = new List<Breadcrumbs>();
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages.Where(w => w.c_url == path + alias && w.f_site == _siteId);
                if (!q.Any())
                    q = db.core_pages.Where(w => w.c_path == path && w.c_alias == alias && w.f_site == _siteId);

                while (q.Any())
                {
                    var d = q.Single();

                    var bread = new Breadcrumbs
                    {
                        Title = d.c_name,
                        Url = (d.c_url != null) ? d.c_url : d.c_path + d.c_alias
                    };
                    data.Add(bread);
                    q = db.core_pages.Where(w => w.gid == d.pgid);
                }



                data.Add(new Breadcrumbs
                {
                    Title = "Главная",
                    Url = "/"
                });
                data.Reverse();
                return data;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public PageModel[] GetChildMenu(Guid parentId)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                                      .Where(w => w.pgid == parentId && w.f_site == _siteId && w.b_disabled == false);
                if (q.Any())
                {
                    return q.OrderBy(o => o.n_sort)
                            .Select(s => new PageModel
                            {
                                Name = s.c_name,
                                Url = (String.IsNullOrEmpty(s.c_url)) ? "/page" + s.c_path + s.c_alias : s.c_url,
                            }).ToArray();
                }
                return null;
            }
        }


        #region Page

        public PageModel GetPage(string path, string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                              .Where(w => w.c_path == path && w.c_alias == alias && w.f_site == _siteId && w.b_disabled == false);
                if (q.Any())
                {
                    return q.Select(s => new PageModel
                    {
                        Name = s.c_name,
                        Text = s.c_text,
                        Url = s.c_url,
                        Childrens = (db.core_pages.Where(w => w.pgid == s.gid).Select(e => new PageModel()
                        {
                            Name = e.c_name,
                            Path = e.c_path,
                            Alias = e.c_alias,
                            Url = e.c_url
                        }).ToArray())
                    }).Single();
                }
                return null;
            }
        }

        /// <summary>
        /// сестренские элементы по пути
        /// </summary>
        /// <param name="path"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public PageModel[] GetPageChild(string path, string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                              .Where(w => w.c_path == path && w.c_alias == alias && w.f_site == _siteId)
                              .Select(s => s.gid)
                              .Join(db.core_pages, n => n, m => m.pgid, (n, m) => m);

                if (q.Any())
                {
                    return q.OrderBy(o => o.n_sort)
                            .Select(s => new PageModel
                            {
                                Name = s.c_name,
                                Text = s.c_text,
                                Url = s.c_url,
                                Alias = s.c_alias,
                                Path = s.c_path
                            }).ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// сестренские элементы
        /// </summary>
        /// <param name="ParentId">идентификатор родителя</param>
        /// <returns></returns>
        public PageModel[] GetPageSister(Guid ParentId)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                              .Where(w => w.pgid == ParentId && w.f_site == _siteId && w.b_disabled == false);
                if (q.Any())
                {
                    return q.OrderBy(o => o.n_sort)
                            .Select(s => new PageModel
                            {
                                Name = s.c_name,
                                Text = s.c_text,
                                Url = s.c_url
                            }).ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// Возвращает элементы карты сайта по группе
        /// </summary>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public PageModel[] GetPageGroup(string Alias)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_page_groups.Where(w => w.f_site == _siteId && w.c_alias == Alias)
                          .Join(db.core_page_group_links, n => n.id, m => m.f_page_group, (n, m) => m)
                          .Join(db.core_pages, e => e.f_page, o => o.gid, (e, o) => new { e, o });
                if (q.Any())
                {
                    return q.OrderBy(o => o.e.n_sort)
                            .Select(s => new PageModel
                            {
                                Name = s.o.c_name,
                                Url = (String.IsNullOrEmpty(s.o.c_url)) ? "/page" + s.o.c_path + s.o.c_alias : s.o.c_url,
                                FaIcon = s.o.c_fa_icon,
                                Childrens = GetChildMenu(s.o.gid)
                            }).ToArray();
                }
                return null;
            }
        }

        #endregion


        #region News
        /// <summary>
        /// список новостей
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<NewsModel> GetNewsList(FilterModel filter)
        {
            Paged<NewsModel> result = new Paged<NewsModel>();
            using (var db = new CMSdb(_context))
            {
                var query = db.core_materials.Where(w => w.f_site == _siteId && w.b_disabled == false);

                if (!string.IsNullOrEmpty(filter.Category))
                {
                    var qcat = db.core_material_categories.Where(w => w.c_alias == filter.Category)
                             .Join(db.core_material_category_link, n => n.id, m => m.f_materials_category, (n, m) => m.f_materials);

                    query = query.Join(qcat, n => n.gid, m => m, (n, m) => n);
                }
                if (filter.Date.HasValue)
                    query = query.Where(s => s.d_date > filter.Date.Value);
                if (filter.DateEnd.HasValue)
                    query = query.Where(s => s.d_date < filter.DateEnd.Value.AddDays(1));


                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            if (!String.IsNullOrWhiteSpace(p))
                            {
                                query = query.Where(w => w.c_title.Contains(p));
                            }
                        }
                    }
                }
                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                              .Take(filter.Size)
                              .Select(s => new NewsModel
                              {
                                  //Id = s.id,
                                  //Guid = s.gid,
                                  LinkNews = (s.c_alias != null) ? "/news/" + s.id + "-" + s.c_alias : "/news/" + s.id,
                                  Title = s.c_title,
                                  Date = s.d_date,
                                  Photo = s.c_photo,
                                  ViewCount = s.c_view_count,
                                  Category = s.fkcategorieslinks
                                                    .Join(
                                                            db.core_material_categories,
                                                            e => e.f_materials_category,
                                                            o => o.id,
                                                            (e, o) => o
                                                            ).Select(sc => new NewsCategoryModel()
                                                            {
                                                                Alias = sc.c_alias,
                                                                Name = sc.c_name
                                                            }).ToArray()
                              });
                return new Paged<NewsModel>()
                {
                    Items = list.ToArray(),
                    Pager = new PagerModel()
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }


        public NewsModel GetNewsItem(int id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_materials.Where(w => w.id == id && w.f_site == _siteId && w.b_disabled == false);
                if (data.Any())
                {
                    return data.Select(s => new NewsModel
                    {
                        Title = s.c_title,
                        Text = s.c_text,
                        Date = s.d_date,
                        Category = s.fkcategorieslinks
                                                                .Join(
                                                                        db.core_material_categories,
                                                                        e => e.f_materials_category,
                                                                        o => o.id,
                                                                        (e, o) => o
                                                                        ).Select(sc => new NewsCategoryModel()
                                                                        {
                                                                            Alias = sc.c_alias,
                                                                            Name = sc.c_name
                                                                        }).ToArray()

                    }).Single();
                }
                return null;
            }
        }
        #endregion


        /// <summary>
        /// фотогаллерея
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<core_photos> GetGallery(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_photos.Where(w => w.f_album == id);
                if (data.Any())
                    return data.OrderBy(o => o.n_sort).ToList<core_photos>();
                else
                    return null;
            }
        }

    }
}
