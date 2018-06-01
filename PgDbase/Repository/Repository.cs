using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Repository
{
    public class Repository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

    
        public Repository(string connectionString)
        {
            _context = connectionString;
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
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
