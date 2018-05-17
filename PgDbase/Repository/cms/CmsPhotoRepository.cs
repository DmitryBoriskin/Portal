using PgDbase.entity;
using PgDbase.models;
using System.Linq;
using LinqToDB;
using System;
using System.Collections.Generic;
using LinqToDB.Data;

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

        /// <summary>
        /// Добавляет фотоальбом
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public bool InsertPhotoAlbum(PhotoAlbumModel album)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = album.Id,
                        PageName = album.Title,
                        Section = LogModule.PhotoAlbums,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    bool result = db.core_photo_albums.Insert(() => new core_photo_albums
                    {
                        id = album.Id,
                        c_title = album.Title,
                        c_preview = album.Preview,
                        c_text = album.Text,
                        d_date = album.Date,
                        b_disabled = album.Disabled,
                        f_site = _siteId
                    }) > 0;
                    
                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновляет фотоальбом
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public bool UpdatePhotoAlbum(PhotoAlbumModel album)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = album.Id,
                        PageName = album.Title,
                        Section = LogModule.PhotoAlbums,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.core_photo_albums
                        .Where(w => w.id == album.Id)
                        .Set(s => s.c_title, album.Title)
                        .Set(s => s.c_preview, album.Preview)
                        .Set(s => s.c_text, album.Text)
                        .Set(s => s.d_date, album.Date)
                        .Set(s => s.b_disabled, album.Disabled)
                        .Update() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновляет превью для альбома
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prev"></param>
        /// <returns></returns>
        public bool UpdatePhotoAlbumPreview(Guid id, string prev)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_photo_albums
                    .Where(w => w.id == id)
                    .Set(s => s.c_preview, prev)
                    .Update() > 0;
            }
        }

        /// <summary>
        /// Удаляет фотоальбом
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePhotoAlbum(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = false;
                    var album = db.core_photo_albums.Where(w => w.id == id).SingleOrDefault();
                    if (album != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = album.c_title,
                            Section = LogModule.PhotoAlbums,
                            Action = LogAction.delete
                        };
                        InsertLog(log, album);

                        result = db.Delete(album) > 0;
                    }

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Проверяет существование альбома
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckPhotoAlbumExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_photo_albums
                    .Where(w => w.id == id).Any();
            }
        }

        /// <summary>
        /// Прикрепляет изображения к альбому
        /// </summary>
        /// <returns></returns>
        public void InsertPhotos(Guid album, IEnumerable<PhotoModel> photos)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    List<core_photos> list = new List<core_photos>();

                    foreach (var p in photos)
                    {
                        list.Add(new core_photos
                        {
                            id = Guid.NewGuid(),
                            f_album = album,
                            c_title = p.Title,
                            d_date = p.Date,
                            c_url = p.Url,
                            c_preview = p.Preview,
                            n_sort = p.Sort
                        });
                    }
                    db.BulkCopy(list);

                    string albumTitle = db.core_photo_albums
                                    .Where(w => w.id == album)
                                    .Select(s => s.c_title)
                                    .SingleOrDefault();

                    var log = new LogModel
                    {
                        PageId = album,
                        PageName = albumTitle,
                        Section = LogModule.PhotoAlbums,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    tr.Commit();
                }
            }
        }

        /// <summary>
        /// Удаляет изображение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PhotoCoreModel DeletePhoto(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var photo = db.core_photos.Where(w => w.id == id).SingleOrDefault();
                    var log = new LogModel
                    {
                        PageId = id,
                        PageName = photo.c_title,
                        Section = LogModule.Photos,
                        Action = LogAction.delete
                    };
                    InsertLog(log, photo);

                    bool success = db.Delete(photo) > 0;

                    tr.Commit();
                    
                    if (success)
                    {
                        return new PhotoCoreModel
                        {
                            Id = id,
                            Album = photo.f_album,
                            Title = photo.c_title 
                        };
                    }
                    return null;
                }
            }
        }
    }
}
