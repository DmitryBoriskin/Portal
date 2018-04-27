using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с сайттами
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает список сайтов с постраничной разбивкой
        /// </summary>
        /// <param name="filter">параметры для фильтрации</param>
        /// <returns></returns>
        public Paged<SitesModel> GetSitesList(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_site.AsQueryable();
                if (filter.Disabled != null)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled);
                }
                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            if (!String.IsNullOrWhiteSpace(p))
                            {
                                query = query.Where(w => w.c_name.Contains(p));
                            }
                        }
                    }
                }
                query = query.OrderBy(o => new { o.c_name });
                if (query.Any())
                {
                    int ItemCount = query.Count();
                    var List = query
                                .Skip(filter.Size * (filter.Page - 1))
                                .Take(filter.Size)
                                .Select(s => new SitesModel
                                {
                                    Id = s.id,
                                    Title = s.c_name
                                });
                    return new Paged<SitesModel>(List.ToArray(), filter.Size, filter.Page, ItemCount);
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает еденичный элемент сайта или пустое значение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SitesModel GetSites(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_site
                         .Where(w => w.id == id)
                         .Select(s => new SitesModel
                         {
                             Id = s.id,
                             Title = s.c_name
                         })
                         .SingleOrDefault();
            }
        }

        /// <summary>
        /// true если существует сайт с таким идентифктаором
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckSiteExist(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_site.Where(w => w.id == id).Any();
            }
        }


        public bool UpdateSite(SitesModel site)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = site.Id,
                        PageName = site.Title,
                        Section = LogSection.Sites,
                        Action = LogAction.update
                    });

                    bool result = db.core_site
                                  .Where(w => w.id == site.Id)
                                  .Set(s => s.c_name, site.Title)
                                  .Update() > 0;
                    tr.Commit();
                    return result;
                }
            }
        }

        public bool InsertSites(SitesModel site)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = site.Id,
                        PageName = site.Title,
                        Section = LogSection.Sites,
                        Action = LogAction.insert
                    });
                    bool result = db.core_site.Insert(() => new core_site
                    {
                        id = site.Id,
                        c_name = site.Title
                    }) > 0;
                    tr.Commit();
                    return result;
                }
            }
        }



        public bool DeleteSite(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var site = db.core_site.Where(w => w.id == id).SingleOrDefault();
                    if (site != null)
                    {
                        InsertLog(new LogModel
                        {
                            PageId = site.id,
                            PageName = site.c_name,
                            Section = LogSection.Sites,
                            Action = LogAction.update
                        });
                        db.Delete(site);
                        tr.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}
