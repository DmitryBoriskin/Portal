using LinqToDB;
using Newtonsoft.Json;
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


        public CmsRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


        public CmsRepository(string connectionString, Guid siteId, string ip, Guid userId)
        {
            _context = connectionString;
            _siteId = siteId;
            _ip = ip;
            _currentUserId = userId;

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
                    c_json = (obj != null)? JsonConvert.SerializeObject(obj) : null
                });
            }
        }


        public core_sites GetCoreSites(Guid SiteId)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_sites.Where(w => w.id == SiteId).Single();
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
                        //User = new UserModel
                        //{
                        //    Id = s.fklogusers.id,
                        //    Surname = s.fklogusers.c_surname,
                        //    Name = s.fklogusers.c_name
                        //}
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
        
    }
}
