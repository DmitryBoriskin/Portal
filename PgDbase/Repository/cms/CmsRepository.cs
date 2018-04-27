using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с сущностями бд
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private Guid _currentUserId = Guid.Empty;

        /// <summary>
        /// ip-адрес
        /// </summary>
        private string _ip = string.Empty;

        /// <summary>
        /// Идентифкатор сайта
        /// </summary>
        private Guid _siteId;

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
        public CmsRepository(string connectionString, Guid userId, string Ip, Guid siteId)
        {
            _context = connectionString;
            //_domain = (!string.IsNullOrEmpty(DomainUrl)) ? getSiteId(DomainUrl) : "";
            _ip = Ip;
            _currentUserId = userId;
            _siteId = siteId;


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
        /// <param name="log"></param>
        private void InsertLog(LogModel log)
        {
            using (var db = new CMSdb(_context))
            {
                db.core_log.Insert(() => new core_log
                {
                    d_date = DateTime.Now,
                    f_page = log.PageId,
                    c_page_name = log.PageName,
                    f_logsections = log.Section.ToString(),
                    f_site = _siteId,
                    f_user = _currentUserId,
                    c_ip = _ip,
                    f_action = log.Action.ToString()
                });
            }
        }


        public CmsMenuModel[] GetCmsMenu(Guid UserId)
        {
            using (var db = new CMSdb(_context))
            {
#warning Тут после разработки раздела будем показывать меню в соответстиии с правами пользователя на текущем сайт
                var data = db.core_cms_menu_group
                            .Select(s => new CmsMenuModel() {
                                Alias = s.c_alias,
                                GroupName = s.c_title,
                                GroupItems = s.fkcmsmenucmsmenugroups.Select(m => new CmsMenuItem() {
                                    Alias = m.c_alias,
                                    Title = m.c_title,
                                    Class = m.c_class
                                }).ToArray()
                            });
                if (data.Any()) return data.ToArray();
                return null;
            }
        }

    }
}
