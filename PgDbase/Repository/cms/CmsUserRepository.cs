using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает постраничный список пользователей
        /// </summary>
        /// <returns></returns>
        public Paged<UserModel> GetUsers(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_user.AsQueryable();

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
                }
                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            if (!String.IsNullOrWhiteSpace(p))
                            {
                                query = query.Where(w => w.c_surname.Contains(p) 
                                                    || w.c_name.Contains(p)
                                                    || w.c_patronymic.Contains(p)
                                                    || w.c_email.Contains(p));
                            }
                        }
                    }
                }
                query = query.OrderBy(o => new { o.c_surname, o.c_name });

                if (query.Any())
                {
                    int itemCount = query.Count();

                    var list = query.Skip(filter.Size * (filter.Page - 1))
                        .Take(filter.Size).Select(s => new UserModel
                        {
                            Id = s.id,
                            Email = s.c_email,
                            Surname = s.c_surname,
                            Name = s.c_name,
                            Patronimyc = s.c_patronymic,
                            Disabled = s.b_disabled,
                            ErrorCount = s.n_error_count,
                            TryLogin = s.d_try_login
                        });

                    return new Paged<UserModel>(list.ToArray(), filter.Size, filter.Page, itemCount);
                }
                return null;
            }
        }

        /// <summary>
        /// Возвращает пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUser(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_user
                    .Where(w => w.id == id)
                    .Select(s => new UserModel
                    {
                        Id = s.id,
                        Email = s.c_email,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronimyc = s.c_patronymic,
                        Disabled = s.b_disabled,
                        TryLogin = s.d_try_login,
                        Group = s.fkusersitelinks
                            .Where(w => w.f_site == _siteId)
                            .Select(g => g.f_user_group).SingleOrDefault()
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool InsertUser(UserModel user)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = user.Id,
                        PageName = $"{user.Surname} {user.Name} {user.Patronimyc}",
                        Section = LogSection.Users,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    bool result = db.core_user.Insert(() => new core_user
                    {
                        id = user.Id,
                        c_email = user.Email,
                        c_salt = user.Salt,
                        c_hash = user.Hash,
                        c_surname = user.Surname,
                        c_name = user.Name,
                        c_patronymic = user.Patronimyc,
                        b_disabled = user.Disabled
                    }) > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUser(UserModel user)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = user.Id,
                        PageName = $"{user.Surname} {user.Name} {user.Patronimyc}",
                        Section = LogSection.Users,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.core_user
                        .Where(w => w.id == user.Id)
                        .Set(s => s.c_email, user.Email)
                        .Set(s => s.c_name, user.Name)
                        .Set(s => s.c_surname, user.Surname)
                        .Set(s => s.c_patronymic, user.Patronimyc)
                        .Set(s => s.b_disabled, user.Disabled)
                        .Update() > 0;

                    // группа
                    var currentLink = db.core_user_site_link
                        .Where(w => w.f_user == user.Id)
                        .Where(w => w.f_site == _siteId)
                        .Where(w => w.f_user_group == user.Group)
                        .SingleOrDefault();

                    bool isExistsGroupOnThisSite = currentLink != null;

                    if (isExistsGroupOnThisSite)
                    {
                        currentLink.f_user_group = user.Group;
                        db.Update(currentLink);
                    }
                    else
                    {
                        InsertUserSiteLink(user.Id, user.Group);
                    }

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Меняет пароль
        /// </summary>
        /// <param name="id"></param>
        /// <param name="salt"></param>
        /// <param name="hash"></param>
        public void ChangePassword(Guid id, string salt, string hash)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var user = db.core_user.Where(w => w.id == id).SingleOrDefault();

                    if (user != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = $"{user.c_surname} {user.c_name} {user.c_patronymic}",
                            Section = LogSection.Users,
                            Action = LogAction.change_pass
                        };
                        InsertLog(log);

                        db.core_user
                            .Where(w => w.id == id)
                            .Set(s => s.c_salt, salt)
                            .Set(s => s.c_hash, hash)
                            .Update();

                        tr.Commit(); 
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет существование пользователя по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckUserExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_user.Where(w => w.id == id).Any();
            }
        }

        /// <summary>
        /// Проверяет существование пользователя по email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckUserExists(string email)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_user.Where(w => w.c_email == email.ToLower()).Any();
            }
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = false;

                    var user = db.core_user.Where(w => w.id == id).SingleOrDefault();
                    if (user != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = $"{user.c_surname} {user.c_name} {user.c_patronymic}",
                            Section = LogSection.Users,
                            Action = LogAction.delete
                        };
                        InsertLog(log);

                        result = db.Delete(user) > 0;

                        tr.Commit();
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Возвращает список групп
        /// </summary>
        /// <returns></returns>
        public GroupsModel[] GetGroups()
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_user_group
                    .Select(s => new GroupsModel
                    {
                        Title = s.c_title,
                        Alias = s.c_alias
                    }).ToArray();  
            }
        }
        
        /// <summary>
        /// Добавляет связь пользователя с сайтом
        /// </summary>
        /// <returns></returns>
        public bool InsertUserSiteLink(Guid userId, Guid groupId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    Guid id = Guid.NewGuid();
                    var log = new LogModel
                    {
                        PageId = id,
                        PageName = GetLogTitleForUserSiteLink(userId, groupId, db),
                        Section = LogSection.UserSiteLink,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    bool result = db.core_user_site_link
                        .Insert(() => new core_user_site_link
                        {
                            id = id,
                            f_site = _siteId,
                            f_user = userId,
                            f_user_group = groupId
                        }) > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновляет группу для пользователя на сайте
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool UpdateUserSiteLink(Guid userId, Guid groupId)
        {
            return false;
        }

        /// <summary>
        /// Возвращает заголовок при логировании
        /// добавления связи пользователя и сайта
        /// </summary>
        /// <returns></returns>
        private string GetLogTitleForUserSiteLink(Guid userId, Guid groupId, CMSdb db)
        {
            string user = db.core_user
                .Where(w => w.id == userId)
                .Select(s => $"{s.c_surname} {s.c_name}")
                .SingleOrDefault();

            string domain = db.core_site
                .Where(w => w.id == _siteId)
                .Select(s => s.c_name)
                .SingleOrDefault();

            string group = db.core_user_group
                .Where(w => w.id == groupId)
                .Select(s => s.c_title)
                .SingleOrDefault();

            return $"{user} {group} {domain}";
        }
    }
}
