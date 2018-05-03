using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.cms
{
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает постраничный список эл-тов карты сайта
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PageModel[] GetPages(PageFilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_pages
                    .Where(w => w.fkpagesites.id == _siteId)
                    .Where(w => w.pgid == filter.Parent)
                    .Select(s => new PageModel
                    {
                        Id = s.gid,
                        Name = s.c_name,
                        ParentId = s.pgid,
                        Sort = s.n_sort,
                        IsDisabled = s.b_disabled,
                        CountChilds = db.core_pages
                                        .Where(w => w.pgid == s.gid)
                                        .Count()
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает эл-т карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageModel GetPage(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_pages
                    .Where(w => w.gid == id)
                    .Select(s => new PageModel
                    {
                        Id = s.gid,
                        Name = s.c_name,
                        ParentId = s.pgid,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        Text = s.c_text,
                        Url = s.c_url,
                        IsDisabled = s.b_disabled,
                        Keywords = s.c_keyw,
                        Desc = s.c_desc,
                        SiteController = s.f_sites_controller,
                        Childrens = GetPages(new PageFilterModel { Parent = s.gid })
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет эл-т карты сайта
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool InsertPage(PageModel page)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = page.Id,
                        PageName = page.Name,
                        Section = LogModule.Pages,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    int sort = db.core_pages
                        .Where(w => w.f_site == _siteId)
                        .Where(w => w.pgid == page.ParentId)
                        .Select(s => s.n_sort)
                        .Max() + 1;

                    bool result = db.core_pages.Insert(() => new core_pages
                    {
                        gid = page.Id,
                        c_name = page.Name,
                        pgid = page.ParentId,
                        c_path = page.Path,
                        c_alias = page.Alias,
                        c_text = page.Text,
                        c_url = page.Url,
                        n_sort = sort,
                        f_site = _siteId,
                        b_disabled = page.IsDisabled,
                        c_keyw = page.Keywords,
                        c_desc = page.Desc,
                        f_sites_controller = page.SiteController
                    }) > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновление эл-та карты сайта
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool UpdatePage(PageModel page)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = page.Id,
                        PageName = page.Name,
                        Section = LogModule.Pages,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.core_pages
                        .Where(w => w.gid == page.Id)
                        .Set(s => s.c_name, page.Name)
                        .Set(s => s.pgid, page.ParentId)
                        .Set(s => s.c_path, page.Path)
                        .Set(s => s.c_alias, page.Alias)
                        .Set(s => s.c_text, page.Text)
                        .Set(s => s.c_url, page.Url)
                        .Set(s => s.b_disabled, page.IsDisabled)
                        .Set(s => s.c_keyw, page.Keywords)
                        .Set(s => s.c_desc, page.Desc)
                        .Set(s => s.f_sites_controller, page.SiteController)
                        .Update() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Удаляет эл-т карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePage(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = false;

                    var page = db.core_pages.Where(w => w.gid == id).SingleOrDefault();
                    if (page != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = page.c_name,
                            Section = LogModule.Pages,
                            Action = LogAction.delete
                        };
                        InsertLog(log, page);

                        result = db.Delete(page) > 0;

                        tr.Commit();
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Возвращает хлебные крошки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GroupsModel[] GetBreadCrumbs(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                List<GroupsModel> list = new List<GroupsModel>();
                if (id != Guid.Empty)
                {
                    var item = GetBreadCrumb(id, db);
                    while (item != null)
                    {
                        list.Add(item);
                        item = GetBreadCrumb(item.Id, db);
                    }

                    list.Reverse();
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Возвращает эл-т хлебной крошки
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private GroupsModel GetBreadCrumb(Guid parent, CMSdb db)
        {
            return db.core_pages
                .Where(w => w.pgid == parent)
                .Select(s => new GroupsModel
                {
                    Id = s.gid,
                    Title = s.c_name
                }).SingleOrDefault();
        }

        /// <summary>
        /// Проверяет существование эл-ты карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckPageExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_pages
                    .Where(w => w.gid == id).Any();
            }
        }
    }
}
