using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
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
        public PageModel[] GetPages(SiteMapFilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_pages
                    .Where(w => w.fkpagesites.id == _siteId);

                return db.core_pages
                    .Where(w => w.pgid == filter.Parent)
                    .Select(s => new PageModel
                    {
                        Id = s.gid,
                        Name = s.c_name,
                        ParentId = s.pgid,
                        Sort = s.n_sort,
                        IsDisabled = s.b_disabled
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
                        SiteController = s.f_sites_controller
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
                        Section = LogSection.Page,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    bool result = db.core_pages.Insert(() => new core_pages
                    {
                        gid = page.Id,
                        c_name = page.Name,
                        pgid = page.ParentId,
                        c_path = page.Path,
                        c_alias = page.Alias,
                        c_text = page.Text,
                        c_url = page.Url,
                        n_sort = page.Sort,
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
                        Section = LogSection.Page,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.core_pages
                        .Where(w => w.gid == page.Id)
                        .Set(s => s.c_name, page.Name)
                        .Update() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }
    }
}
