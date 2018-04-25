using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase
{
    public class AccountRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        private Guid _siteid = Guid.Empty;
        /// <summary>
        /// Конструктор
        /// </summary>
        public AccountRepository()
        {
            _context = "dbConnection";
        }
        public AccountRepository(string ConnectionString)
        {
            _context = ConnectionString;
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
                        Group = s.f_group.ToLower(),
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        CountError = (s.n_error_count >= 5),
                        LockDate = s.d_try_login,
                        Disabled = s.b_disabled
                    });
                if (!data.Any()) { return null; }
                else { return data.First(); }
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

                int count = db.core_user.Where(w => w.с_change_pass_code == Code).Count();
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
                        .Set(u => u.с_change_pass_code, change_pass_code)
                        .Update();

                // Логирование
                InsertLog(id, IP, "login", id, _siteid, "Users", "Авторизация в CMS");
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
                    .Set(u => u.с_change_pass_code, Code)
                    .Update();

                // Логирование
                InsertLog(id, IP, "reqest_change_pass", id, _siteid, "Users", "Восстановление пароля");
            }
        }

        public void InsertLog(Guid UserId, string IP, string Action, Guid PageId, Guid Site, string Section, string PageName)
        {
            using (var db = new CMSdb(_context))
            {
                db.core_log.Insert(() => new core_log
                {
                    d_date = DateTime.Now,
                    f_page = PageId,
                    c_page_name = PageName,
                    f_logsections = Section,
                    f_site = Site,
                    f_user = UserId,
                    c_ip = IP,
                    f_action = Action
                });
            }
        }

    }
}
