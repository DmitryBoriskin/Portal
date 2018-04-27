using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase
{
    public class AccountRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        
        /// <summary>
        /// Идентифкатор сайта
        /// </summary>
        private Guid _siteid = Guid.Empty;
        /// <summary>
        /// ip-адрес
        /// </summary>
        private string _ip = string.Empty;

        /// <summary>
        /// Конструктор
        /// </summary>
        public AccountRepository()
        {
            _context = "dbConnection";
        }
        public AccountRepository(string ConnectionString, string Ip, Guid siteId)
        {
            _context = ConnectionString;
            _ip = Ip;
            _siteid = siteId;
        }


        /// <summary>
        /// Получаем данные об пользователе по email или id
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public AccountModel getCmsAccount(string Email)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_user.
                    Where(w => w.c_email == Email).
                    Select(s => new AccountModel
                    {
                        Id = s.id,
                        Mail = s.c_email,
                        Salt = s.c_salt,
                        Hash = s.c_hash,                        
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        CountError = (s.n_error_count >= 5),
                        LockDate = s.d_try_login,
                        Disabled = s.b_disabled
                    });
                if (data.Any()) return data.First();
                return null;
            }
        }        
        public AccountModel getCmsAccount(Guid Id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_user.
                   Where(w => w.id == Id).
                   Select(s => new AccountModel {
                       Id=s.id,
                       Mail=s.c_email,
                       Salt=s.c_salt,
                       Hash=s.c_hash,
                       Name=s.c_name,
                       Surname=s.c_surname
                   });
                if (data.Any()) return data.First();
                return null;
            }
        }
        public DomainList[] GetSiteLinkUser(Guid UserId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_user_site_link
                            .Where(w => w.f_user == UserId)
                            .Select(s => s.fkusersitelinksite.fkdomains.Where(ww => ww.b_default));
                //if (query.Any()) return query.ToArray();
                return null;
            }
        }



        /// <summary>
        /// Проверка существования пользователя
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public bool getCmsAccountCode(Guid Code)
        {
            using (var db = new CMSdb(_context))
            {
                bool result = false;

                int count = db.core_user.Where(w => w.c_change_pass_code == Code).Count();
                if (count > 0) result = true;

                return result;
            }
        }
        
        public void SuccessLogin(Guid id, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                Guid? change_pass_code = null;

                var data = db.core_user.Where(w => w.id == id)
                        .Set(u => u.n_error_count, 0)
                        .Set(u => u.d_try_login, DateTime.Now)
                        .Set(u => u.c_change_pass_code, change_pass_code)
                        .Update();

                // Логирование
                var log = new LogModel{
                    PageId=id,
                    PageName= "Авторизация в CMS",
                    Section=LogSection.Account,
                    Action= LogAction.login
                };
                InsertLog(log);
            }
        }

        /// <summary>
        /// Записываем неудачную попытку входа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IP"></param>
        public int FailedLogin(Guid id, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                int Num = db.core_user.Where(w => w.id == id).ToArray().First().n_error_count + 1;

                var data = db.core_user.Where(w => w.id == id)
                        .Set(u => u.n_error_count, Num)
                        .Set(u => u.d_try_login, DateTime.Now)
                        .Update();

                // Логирование
                //insertLog(id, IP, "failed_login", id, String.Empty, "Users", "Неудачная попытка входа");

                if (Num == 5)
                {
                    // Логирование
                    //insertLog(id, IP, "account_lockout", id, String.Empty, "Users", "Блокировка аккаунта");
                }

                return Num;
            }
        }

        /// <summary>
        /// записываем код востановления пароля
        /// </summary>
        /// <param name="id">id аккаунта</param>
        /// <param name="Code">код восстановления</param>
        /// <param name="IP"></param>
        public void SetRestorePassCode(Guid id, Guid Code, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_user.Where(w => w.id == id)
                    .Set(u => u.c_change_pass_code, Code)
                    .Update();

                // Логирование
                var log = new LogModel
                {
                    PageId = id,
                    UserId = id,
                    Action = LogAction.reqest_change_pass,
                    Section = LogSection.Account
                };
                InsertLog(log);
            }
        }
                
        /// <summary>
        /// Сбросс пароля по ссылке пришедшей на почту
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Salt"></param>
        /// <param name="Hash"></param>
        /// <param name="IP"></param>
        /// <returns></returns>
        public bool ChangePasByCode(Guid Code, string Salt, string Hash, string IP)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query = db.core_user.Where(w => w.c_change_pass_code == Code);
                    if (query.Any())
                    {
                        string LogTitle = query.First().c_surname + " " + query.First().c_name;
                        Guid accountId = query.First().id;
                        Guid? change_pass_code = null;
                        query
                            .Set(p => p.c_salt, Salt)
                            .Set(p => p.c_hash, Hash)
                            .Set(p => p.n_error_count, 0)
                            .Set(p => p.c_change_pass_code, change_pass_code)
                            .Update();
                        tr.Commit();

                        var log = new LogModel
                        {
                            PageId = accountId,
                            PageName = "Авторизация в CMS",
                            Section = LogSection.Account,
                            Action = LogAction.change_pass
                        };
                        InsertLog(log);
                        return true;
                    }
                    return false;
                }
            }
        }


        /// <summary>
        /// логировнаие
        /// </summary>
        /// <param name="log"></param>
        public void InsertLog(LogModel log)
        {
            using (var db = new CMSdb(_context))
            {
                db.core_log.Insert(() => new core_log
                {
                    d_date = DateTime.Now,
                    f_page = log.PageId,
                    c_page_name = log.PageName,
                    f_logsections = log.Section.ToString(),
                    f_site = _siteid,
                    f_user = log.PageId,
                    c_ip = _ip,
                    f_action = log.Action.ToString()
                });
            }
        }

    }
}
