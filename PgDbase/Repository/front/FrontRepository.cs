using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.front
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        /// <summary>
        /// Идентифкатор сайта
        /// </summary>
        private Guid _siteId;

        /// <summary>
        /// ip-адрес
        /// </summary>
        private string _ip = string.Empty;

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private Guid _currentUserId = Guid.Empty;


        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


        public FrontRepository(string connectionString, Guid siteId, string ip, Guid userId)
        {
            _context = connectionString;
            _siteId = siteId;
            _ip = ip;
            _currentUserId = userId;

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


        public PageModel GetPage(string path,string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                              .Where(w => w.c_path == path && w.c_alias == alias && w.f_site == _siteId && w.b_disabled == false);
                if (q.Any())
                {
                    return q.Select(s => new PageModel {
                        Name = s.c_name,
                        Text = s.c_text,
                        Url = s.c_url,
                        Childrens =(db.core_pages.Where(w=>w.pgid==s.gid).Select(e => new PageModel()
                                        {
                                            Name = e.c_name,
                                            Path = e.c_path,
                                            Alias = e.c_alias,
                                            Url = e.c_url
                                        }).ToArray())
                    }).Single();
                }
                return null;                              
            }
        }        
         
        /// <summary>
        /// сестренские элементы по пути
        /// </summary>
        /// <param name="path"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        public PageModel[] GetPageChild(string path, string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                              .Where(w => w.c_path == path && w.c_alias == alias && w.f_site == _siteId)
                              .Select(s=>s.gid)
                              .Join(db.core_pages,n=>n,m=>m.pgid,(n,m)=>m);

                if (q.Any())
                {
                    return q.OrderBy(o=>o.n_sort)
                            .Select(s => new PageModel
                            {
                                Name = s.c_name,
                                Text = s.c_text,
                                Url = s.c_url,
                                Alias=s.c_alias,
                                Path=s.c_path
                            }).ToArray();
                }
                return null;
            }
        }
        /// <summary>
        /// сестренские элементы
        /// </summary>
        /// <param name="ParentId">идентификатор родителя</param>
        /// <returns></returns>
        public PageModel[] GetPageSister(Guid ParentId)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                              .Where(w => w.pgid==ParentId && w.f_site == _siteId && w.b_disabled == false);
                if (q.Any())
                {
                    return q.OrderBy(o => o.n_sort)
                            .Select(s => new PageModel
                            {
                                Name = s.c_name,
                                Text = s.c_text,
                                Url = s.c_url
                            }).ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public PageModel[] GetPageGroup(string Alias)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_page_groups.Where(w => w.f_site == _siteId && w.c_alias == Alias)
                          .Join(db.core_page_group_links, n => n.id, m => m.f_page_group, (n, m) => m)
                          .Join(db.core_pages, e => e.f_page, o => o.gid, (e, o) =>new {e,o});
                if (q.Any())
                {
                    return q.OrderBy(o => o.e.n_sort)
                            .Select(s => new PageModel
                            {
                                Name = s.o.c_name,
                                Alias=s.o.c_alias,
                                Path=s.o.c_path,
                                Url = s.o.c_url
                            }).ToArray();
                }              
                return null;
            }
        }



        /// <summary>
        /// фотогаллерея
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<core_photos> GetGallery(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_photos.Where(w => w.f_album == id);
                if (data.Any())
                    return data.OrderBy(o => o.n_sort).ToList<core_photos>();
                else
                    return null;
            }
        }


    }
}
