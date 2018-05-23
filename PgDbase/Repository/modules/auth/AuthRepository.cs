using LinqToDB;
using PgDbase.entity;
using PgDbase.Auth.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с внешними пользователями
    /// </summary>
    public partial class CmsRepository
    {
        #region Пользователи админка

        /// <summary>
        /// Проверка существует ли пользователь с id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool FrontUserExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                //var data = db.auth_users
                //    .Where(t => t.id == id);

                //if (data.Any())
                //    return true;

                return false;
            }
        }
        
        /// <summary>
        /// Проверка существует ли пользователь с email
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool FrontUserExists(string email)
        {
            using (var db = new CMSdb(_context))
            {
                //var data = db.auth_users
                //    .Where(t => t.email == email);

                //if (data.Any())
                //    return true;

                return false;
            }
        }

        /// <summary>
        /// Список пользователей как массив (возможно для рассылок и тп)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public FrontUserModel[] GetFrontUserList(FrontUserFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                //var query = db.auth_users.AsQueryable();

                //// Filter

                //query = query.OrderBy(t => t.c_surname)
                //            .OrderBy(t => t.c_name)
                //            .OrderBy(t => t.c_surname);

                //var list = query
                //    .Select(s => new FrontUserModel()
                //    {
                //        Id = s.id,
                //        //Mail = s.email,
                //        Surname = s.c_surname,
                //        Name = s.c_surname,
                //        Patronymic = s.c_patronymic,
                //        Disabled = s.b_disabled,
                //        //Hash = s.c_hash,
                //        //Salt = s.c_salt,
                //        //LockDate = s.d_locked_date,
                //        //ResetPwdCode = s.c_reset_password_code,
                //        //VerificationCode = s.c_verification_code
                //    });

                //return list.ToArray();
                return null;
            }
        }

        /// <summary>
        /// Возвращает постраничный список пользователей
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<FrontUserModel> GetFrontUsers(FrontUserFilter filter)
        {
            //using (var db = new CMSdb(_context))
            //{
            //    var query = db.auth_users.AsQueryable();


            //    if (!string.IsNullOrEmpty(filter.SearchText))
            //        query = query.Where(t => (t.c_surname + " " + t.c_name + " " + t.c_patronymic).ToLower().Contains(filter.SearchText.ToLower()));

            //    //if (filter.Site.HasValue)
            //    //    query = query.Where(t => t.site == filter.Site.Value);

            //    query = query.OrderBy(t => t.c_surname)
            //                .OrderBy(t => t.c_name)
            //                .OrderBy(t => t.c_surname);

            //    int itemsCount = query.Count();

            //    var list = query
            //        .Skip(filter.Size * (filter.Page - 1))
            //        .Take(filter.Size).Select(s => new FrontUserModel()
            //        {
            //            Id = s.id,
            //            //Mail = s.email,
            //            Surname = s.c_surname,
            //            Name = s.c_surname,
            //            Patronymic = s.c_patronymic,
            //            Disabled = s.b_disabled,
            //            //Hash = s.c_hash,
            //            //Salt = s.c_salt,
            //            //LockDate = s.d_locked_date,
            //            //ResetPwdCode = s.c_reset_password_code,
            //            //VerificationCode = s.c_verification_code

            //        });

            //    return new Paged<FrontUserModel>()
            //    {
            //        Items = list.ToArray(),
            //        Pager = new PagerModel()
            //        {
            //            PageNum = filter.Page,
            //            PageSize = filter.Size,
            //            TotalCount = itemsCount
            //        }
            //    };
            //}
            return null;
        }

        /// <summary>
        /// Возвращает пользователя
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public FrontUserModel GetFrontUser(Guid id)
        {
            //using (var db = new CMSdb(_context))
            //{
            //    var query = db.auth_users
            //        .Where(t => t.id == id);

            //    var data = query
            //        .Select(s => new FrontUserModel()
            //        {

            //            Id = s.id,
            //            //Mail = s.email,
            //            Surname = s.c_surname,
            //            Name = s.c_surname,
            //            Patronymic = s.c_patronymic,
            //            Disabled = s.b_disabled,
            //            //Hash = s.c_hash,
            //            //Salt = s.c_salt,
            //            //LockDate = s.d_locked_date,
            //            //ResetPwdCode = s.c_reset_password_code,
            //            //VerificationCode = s.c_verification_code

            //        });

            //    return data.SingleOrDefault();
            //}
            return null;
        }

        /// <summary>
        /// Добавляет запись о пользователе
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool InsertFrontUser(FrontUserModel user)
        {
            //using (var db = new CMSdb(_context))
            //{
            //    using (var tran = db.BeginTransaction())
            //    {
            //        var cdTemplate = new auth_users()
            //        {
            //            id = user.Id,

            //        };
            //        db.Insert(cdTemplate);

            //        var log = new LogModel()
            //        {
            //            PageId = Guid.NewGuid(),
            //            PageName = $"{user.Surname} {user.Name} {user.Patronymic}",
            //            Section = LogModule.Users,
            //            Action = LogAction.insert
            //        };
            //        InsertLog(log);

            //        tran.Commit();
            //        return true;
            //    }
            //}
            return false;
        }

        /// <summary>
        /// Изменения данных пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateFrontUser(FrontUserModel user)
        {
            //using (var db = new CMSdb(_context))
            //{
            //    using (var tran = db.BeginTransaction())
            //    {
            //        var data = db.auth_users
            //            .Where(s => s.id == user.Id);

            //        if (data.Any())
            //        {
            //            var cdUsers = data.SingleOrDefault();
            //            cdUsers.c_surname = user.Surname;
            //            cdUsers.c_name = user.Name;
            //            cdUsers.c_patronymic = user.Patronymic;


            //            db.Update(cdUsers);

            //            var log = new LogModel()
            //            {
            //                PageId = user.Id,
            //                PageName = $"{user.Surname} {user.Name} {user.Patronymic}",
            //                Section = LogModule.Templates,
            //                Action = LogAction.update
            //            };
            //            InsertLog(log);

            //            tran.Commit();
            //            return true;
            //        };
            //        return false;
            //    }
            //}
            return false;
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteFrontUser(Guid id)
        {
            //using (var db = new CMSdb(_context))
            //{
            //    using (var tran = db.BeginTransaction())
            //    {
            //        var data = db.auth_users
            //            .Where(s => s.id == id);

            //        if (data.Any())
            //        {
            //            var cdUser = data.Single();
            //            db.Delete(cdUser);

            //            var log = new LogModel()
            //            {
            //                PageId = id,
            //                PageName = $"{cdUser.c_surname} {cdUser.c_name} {cdUser.c_patronymic}",
            //                Section = LogModule.Modules,
            //                Action = LogAction.delete
            //            };
            //            InsertLog(log);

            //            tran.Commit();
            //            return true;
            //        }

            //        return false;
            //    }
            //}
            return false;
        }
        
        #endregion

    }
}
