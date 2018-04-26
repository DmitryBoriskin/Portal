using LinqToDB;
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

        /// <summary>
        /// Логирование
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ip"></param>
        /// <param name="action"></param>
        /// <param name="pageId"></param>
        /// <param name="site"></param>
        /// <param name="section"></param>
        /// <param name="pageName"></param>
        public void InsertLog(LogModel log)
        {
            using (var db = new CMSdb(_context))
            {
                db.core_log.Insert(() => new core_log
                {
                    d_date = DateTime.Now,
                    f_page = log.Page,
                    c_page_name = log.PageName,
                    f_logsections = log.Section,
                    f_site = log.Site,
                    f_user = log.User,
                    c_ip = log.Ip,
                    f_action = log.Action
                });
            }
        }
    }
}
