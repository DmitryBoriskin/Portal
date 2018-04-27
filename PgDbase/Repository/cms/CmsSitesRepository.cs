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
                                .Select(s => new SitesModel {
                                    Id=s.id,
                                    Title=s.c_name
                                });
                    return new Paged<SitesModel>(List.ToArray(), filter.Size, filter.Page, ItemCount);
                }
            }
            return null;
        }
    }
}
