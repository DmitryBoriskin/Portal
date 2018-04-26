using PgDbase.entity.cms;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase
{
    /// <summary>
    /// Репозиторий для работы с сущностями бд
    /// </summary>
    public class CmsRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        private Guid _currentUserId = Guid.Empty;
        private string _ip = string.Empty;
        private string _domain = string.Empty;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CmsRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="userId"></param>
        /// <param name="Ip"></param>
        /// <param name="domainUrl"></param>
        public CmsRepository(string connectionString, Guid userId, string Ip, string domainUrl)
        {
            _context = connectionString;
            //_domain = (!string.IsNullOrEmpty(DomainUrl)) ? getSiteId(DomainUrl) : "";
            _ip = Ip;
            _currentUserId = userId;

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

        /// <summary>
        /// Возвращает идентификатор сайта
        /// </summary>
        /// <param name="domainUrl"></param>
        /// <returns></returns>
        public Guid GetSiteGuid(string domainUrl)
        {
            try
            {
                using (var db = new CMSdb(_context))
                {
                    return db.core_sites_domains.Where(w => w.c_domain == domainUrl).SingleOrDefault().fksitesdomainssite.id;
                }

            }
            catch(Exception ex)
            {
                throw new Exception("cmsRepository > getSiteId: It is not possible to determine the site by url (" + domainUrl + ") " + ex);
            }
        }


        public CmsMenuModel[] GetCmsMenu(Guid UserId, Guid SiteId)
        {
            using (var db = new CMSdb(_context))
            {
              //var query=
                return null;
                    
            }
        }

    }
}
