using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с модулями системы
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает постраничный список модулей
        /// </summary>
        /// <returns></returns>
        public Paged<ModuleModel> GetModules(ModuleFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_controllers.AsQueryable();

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new ModuleModel
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Name = s.c_name,
                        Controller = s.c_controller_name,
                        Action = s.c_action_name,
                        View = s.c_default_view
                    });

                return new Paged<ModuleModel>()
                {
                    Items = list.ToArray(),
                    Pager = new PagerModel()
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }

        /// <summary>
        /// Возвращает модуль
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ModuleModel GetModule(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_controllers
                   .Select(s => new ModuleModel
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Name = s.c_name,
                        Controller = s.c_controller_name,
                        Action = s.c_action_name,
                        View = s.c_default_view
                    });
                return data;
            }
        }

        /// <summary>
        /// Добавляет пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool InsertModule(ModuleModel module)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var newController = new core_controllers
                    {
                        id = module.Id,
                        pid = module.ParentId,
                        c_name = module.Name,
                        c_controller_name = module.Controller,
                        c_action_name = module.Action,
                        c_default_view = module.View
                    };
                    db.Insert(newController);

                    var log = new LogModel
                    {
                        PageId = Guid.NewGuid(),
                        PageName = module.Name,
                        Section = LogSection.Module,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateModule(ModuleModel module)
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

                    bool result = db.core_users
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

                        log = new LogModel
                        {
                            PageId = currentLink.id,
                            PageName = GetLogTitleForUserSiteLink(user.Id, user.Group, db),
                            Section = LogSection.UserSiteLinks,
                            Action = LogAction.update
                        };
                        InsertLog(log);
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
                    var user = db.core_users.Where(w => w.id == id).SingleOrDefault();

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

                        db.core_users
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
                return db.core_users
                    .Where(w => w.id == id).Any();
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
                return db.core_users
                    .Where(w => w.c_email == email.ToLower()).Any();
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

                    var user = db.core_users.Where(w => w.id == id).SingleOrDefault();
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
                return db.core_user_groups
                    .Select(s => new GroupsModel
                    {
                        Id = s.id,
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
                        Section = LogSection.UserSiteLinks,
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
        /// Возвращает заголовок при логировании
        /// добавления связи пользователя и сайта
        /// </summary>
        /// <returns></returns>
        private string GetLogTitleForUserSiteLink(Guid userId, Guid groupId, CMSdb db)
        {
            string user = db.core_users
                .Where(w => w.id == userId)
                .Select(s => $"{s.c_surname} {s.c_name}")
                .SingleOrDefault();

            string domain = db.core_sites
                .Where(w => w.id == _siteId)
                .Select(s => s.c_name)
                .SingleOrDefault();

            string group = db.core_user_groups
                .Where(w => w.id == groupId)
                .Select(s => s.c_title)
                .SingleOrDefault();

            return $"{user} {group} {domain}";
        }
    }
}
