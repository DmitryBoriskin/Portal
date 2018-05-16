using PgDbase.entity;
using PgDbase.models;
using System.Linq;
using LinqToDB;
using System;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с изображениями
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает постраничный список фотоальбомов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<PhotoAlbumModel> GetPhotoAlbums(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<PhotoAlbumModel> result = new Paged<PhotoAlbumModel>();
                var query = db.core_photo_albums
                    .Where(w => w.f_site == _siteId);

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
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
                                query = query.Where(w => w.c_title.Contains(p)
                                                      || w.c_text.Contains(p));
                            }
                        }
                    }
                }

                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new PhotoAlbumModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Preview = s.c_preview,
                        Text = s.c_text,
                        Date = s.d_date,
                        Disabled = s.b_disabled
                    });

                return new Paged<PhotoAlbumModel>
                {
                    Items = list.ToArray(),
                    Pager = new PagerModel
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }

        /// <summary>
        /// Возвращает фотоальбом
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PhotoAlbumModel GetPhotoAlbum(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_photo_albums
                    .Where(w => w.id == id)
                    .Select(s => new PhotoAlbumModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Preview = s.c_preview,
                        Text = s.c_text,
                        Date = s.d_date,
                        Disabled = s.b_disabled,
                        Photos = s.fkphotosphotoalbumss
                            .OrderBy(o => o.n_sort)
                            .Select(p => new PhotoModel
                            {
                                Id = p.id,
                                Preview = p.c_preview,
                                Sort = p.n_sort
                            }).ToArray()
                    }).SingleOrDefault();
            }
        }
    }
}
