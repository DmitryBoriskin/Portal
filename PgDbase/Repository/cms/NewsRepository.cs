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
        /// <summary>
        /// категории
        /// </summary>
        /// <returns></returns>
        public NewsCategoryModel[] GetNewsCategory()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_material_categories
                            .Where(w => w.f_site == _siteId)
                            .OrderBy(o => o.n_sort)
                            .Select(s => new NewsCategoryModel() {
                                Alias=s.c_alias,
                                Name=s.c_name
                            });
                if (query.Any())
                {
                    return query.ToArray();
                }
                return null;
            }
        }

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
        /// <summary>
        /// single news
        /// </summary>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public NewsModel GetNewsItem(Guid Guid) {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_materials.Where(w => w.gid == Guid);
                if (query.Any())
                {

                    return query.Select(s=> new NewsModel {
                                Guid=s.gid,
                                Date=s.d_date,
                                Title=s.c_title,
                                Text=s.c_text,
                                Photo=s.c_photo,
                                Keyw=s.c_keyw,
                                Desc=s.c_desc,
                                SourceName=s.c_source_name,
                                SourceUrl=s.c_source_url,
                                Disabled=s.b_disabled,
                                Important=s.b_important                                
                                }).Single();
                }
                return null;
            }
        }


        public bool InsertNews(NewsModel news) {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = news.Guid,
                        PageName = news.Title,
                        Section = LogModule.News,
                        Action = LogAction.update
                    });

                    bool result = db.core_materials
                                  .Insert(
                                  () => new core_materials {
                                      gid=news.Guid,
                                      c_title=news.Title,
                                      
                                      
                                  }) > 0;

                    return true;
                }
            }
        }
    }
}
