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
                query = query.OrderBy(o => o.d_date);
                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                              .Take(filter.Size)
                              .Select(s => new NewsModel {
                                  Id = s.id,
                                  Title = s.c_title,
                                  Date = s.d_date,
                                  Category = s.fkcategorieslinks
                                                    .Join(
                                                            db.core_material_categories,
                                                            e => e.f_materials_category,
                                                            o => o.id,
                                                            (e, o) => o
                                                            ).Select(sc => new NewsCategoryModel() {
                                                                Alias=sc.c_alias,
                                                                Name=sc.c_name
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
    }
}
