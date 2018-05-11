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
        #region категории
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
                            .Select(s => new NewsCategoryModel()
                            {
                                Id = s.id,
                                Alias = s.c_alias,
                                Name = s.c_name
                            });
                if (query.Any())
                {
                    return query.ToArray();
                }
                return null;
            }
        }
        /// <summary>
        /// отдельная категория
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NewsCategoryModel GetNewsCategoryItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_material_categories.Where(w => w.id == id && w.f_site == _siteId);
                if (q.Any())
                {
                    return q.Select(s => new NewsCategoryModel
                    {
                        Alias = s.c_alias,
                        Name = s.c_name,
                        Id = s.id
                    }).Single();
                }
                return null;
            }
        }
        public bool ExistNewsCategory(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_material_categories.Where(w => w.f_site == _siteId && w.id==id).Any();
            }
        }
        public bool InsertNewsCaetegory(NewsCategoryModel category)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = category.Id,
                        PageName = category.Name,
                        Section = LogModule.NewsCategory,
                        Action = LogAction.update
                    });
                    int sort = 1;
                    var q = db.core_material_categories.Where(w => w.f_site == _siteId);
                    if (q.Any())
                    {
                        sort = q.Select(s => s.n_sort).Max() + 1;
                    }
                    bool result = db.core_material_categories
                        .Insert(() => new core_material_categories
                        {
                            c_alias = category.Alias,
                            c_name = category.Name,
                            n_sort = sort,
                            f_site = _siteId
                        }) > 0;
                    tr.Commit();
                    return result;
                }
            }
        } 

        public bool UpdateNewsCategory(NewsCategoryModel category)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = category.Id,
                        PageName = category.Name,
                        Section = LogModule.NewsCategory,
                        Action = LogAction.update
                    });
                    var q = db.core_material_categories.Where(w => w.id == category.Id && w.f_site==_siteId);
                    if (q.Any())
                    {
                        q
                         .Set(s => s.c_name, category.Name)
                         .Set(s => s.c_alias, category.Alias)
                         .Update();
                        tr.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }
        #endregion

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
