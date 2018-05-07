using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;
using System.Web.Script.Serialization;

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
        /// <param name="ip"></param>
        /// <param name="domainUrl"></param>
        public CmsRepository(string connectionString, Guid userId, string ip, Guid siteId)
        {
            _context = connectionString;
            //_domain = (!string.IsNullOrEmpty(DomainUrl)) ? getSiteId(DomainUrl) : "";
            _ip = ip;
            _currentUserId = userId;
            _siteId = siteId;


            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


        /// <summary>
        /// Логирование
        /// </summary>
        /// <param name="log"></param>
        private void InsertLog(LogModel log, object obj = null)
        {
            using (var db = new CMSdb(_context))
            {
                db.core_logs.Insert(() => new core_logs
                {
                    d_date = DateTime.Now,
                    f_page = log.PageId,
                    c_page_name = log.PageName,
                    f_logsections = log.Section.ToString(),
                    c_comment = log.Comment,

                    f_site = _siteId,
                    f_user = _currentUserId,
                    c_ip = _ip,
                    f_action = log.Action.ToString(),
                    c_json = obj != null ? new JavaScriptSerializer().Serialize(obj) : null
                });
            }
        }

        /// <summary>
        /// Возвращает логи для страницы
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LogModel[] GetPageLogs(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_logs
                    .Where(w => w.f_page == id)
                    .Select(s => new LogModel
                    {
                        Date = s.d_date,
                        Action = (LogAction)Enum.Parse(typeof(LogAction), s.f_action),
                        Section = (LogModule)Enum.Parse(typeof(LogModule), s.f_logsections),
                        User = new UserModel
                        {
                            Id = s.fklogusers.id,
                            Surname = s.fklogusers.c_surname,
                            Name = s.fklogusers.c_name
                        }
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает логи пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LogModel[] GetUserLogs(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_logs
                    .Where(w => w.f_user == id)
                    .Select(s => new LogModel
                    {
                        Date = s.d_date,
                        Action = (LogAction)Enum.Parse(typeof(LogAction), s.f_action),
                        Section = (LogModule)Enum.Parse(typeof(LogModule), s.f_logsections),
                        PageName = s.c_page_name
                    }).ToArray();
            }
        }
        /// <summary>
        /// Возвращает пункты меню из структуры CMS га которые есит права
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public CmsMenuModel[] GetCmsMenu(Guid UserId)
        {
            using (var db = new CMSdb(_context))
            {
                #warning Тут после разработки раздела будем показывать меню в соответстиии с правами пользователя на текущем сайт

                var data = db.core_menu
                            .Where(s => s.f_parent == null)
                            .OrderBy(s => s.n_sort)
                            .Select(s => new CmsMenuModel()
                            {
                                Alias = s.c_alias,
                                GroupName = s.c_title,
                                GroupItems = s.fk_menu_parent_BackReferences
                                                .OrderBy(m => m.n_sort)
                                                .Select(m => new CmsMenuItem()
                                                {
                                                    Alias = m.c_alias,
                                                    Title = m.c_title,
                                                    Class = m.c_class
                                                }).ToArray()
                            });

                return data.ToArray();
            }
        }

        public CmsMenuItem[] GetModulMenu(Guid UserId)
        {
            using (var db = new CMSdb(_context))
            {
                //var query = db.core_user_resolutions
                //              .Where(w => w.)
                var data = db.core_controllers
                           .Where(w => w.f_parent == null)
                           .OrderBy(o => o.c_name)
                           .Select(s => new CmsMenuItem() {
                               Alias = s.c_controller_name,
                               Title = s.c_name
                           });
                if (data.Any())
                {
                    return data.ToArray();
                }
                return null;
            }
        }
    }
}
