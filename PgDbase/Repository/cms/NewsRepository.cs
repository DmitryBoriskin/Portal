using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class CmsRepository
    {
        public Paged<NewsModel> GetNewsList(FilterModel filter)
        {
            Paged<NewsModel> result = new Paged<NewsModel>();
            using (var db = new CMSdb(_context))
            {
                var query = db.core_materials.Where(w => w.f_site == _siteId);
                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled);
                }
            }                
            return null;
        }
    }
}
