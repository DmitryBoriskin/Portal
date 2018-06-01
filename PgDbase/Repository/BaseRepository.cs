using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase
{
    public class BaseRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        public BaseRepository()
        {
            _context = "dbConnection";
        }
        public BaseRepository(string ConnectionString)
        {
            _context = ConnectionString;
        }


        public Guid GetSiteId(string domainUrl)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    var query = db.core_site_domains
                        .Where(w => w.c_domain.ToLower() == domainUrl.ToLower())
                        .SingleOrDefault();

                    if (query != null)
                    {
                        return query.f_site;
                    }

                    return Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("cmsRepository > getSiteId: It is not possible to determine the site by url (" + domainUrl + ") " + ex);
            }
        }
    }
}
