using LinqToDB;
using PgDbase.entity;
using PgDbase.entity.cms;
using PgDbase.models;
using PgDbase.Services;
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
        public SitesList GetSitesList(FilterParams filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_site.AsQueryable();
                if (filter.Disabled != null)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled);
                }
                if (filter.SearchText != null)
                {
                    query = query.Where(w => w.c_name.Contains(filter.SearchText));
                }
                query = query.OrderBy(o => new { o.c_name });
                if (query.Any())
                {
                    var List = query
                          .Select(s => new SitesModel
                          {
                              Id = s.id,
                              Title = s.c_name                              
                          })
                          .Skip(filter.Size * (filter.Page - 1))
                          .Take(filter.Size);

                    SitesModel[] sitesInfo = List.ToArray();

                }
            }
            return null;
        }
    }
}
