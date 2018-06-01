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
        /// Администраторы, разработчики и тп всего портала
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<UserModel> GetPortalAdmins(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<UserModel> result = new Paged<UserModel>();

                var query = (from u in db.core_AspNetUsers
                             join p in db.core_AspNetUserProfiles on u.Id equals p.UserId
                             //Условие принадлежности к какой-либо роли
                             where db.core_AspNetUserRoles.Any(r => r.UserId == u.Id && r.AspNetRolesRoleId.Name != "User" && r.AspNetRolesRoleId.Discriminator == "ApplicationRole") //исключить обычного пользователя
                             select new
                             {
                                 u.Id,
                                 u.UserId,
                                 //u.UserName,
                                 u.Email,
                                 u.EmailConfirmed,
                                 u.PhoneNumber,
                                 u.PhoneNumberConfirmed,
                                 //u.LockoutEndDateUtc,
                                 //u.LockoutEnabled,
                                 //u.AccessFailedCount,
                                 u.SiteId,
                                 p.Surname,
                                 p.Name,
                                 p.Patronymic,
                                 p.Disabled,
                                 p.BirthDate,
                                 p.RegDate
                             }
                            );


                if (filter.Disabled.HasValue)
                {
                    query = query.Where(s => s.Disabled == filter.Disabled.Value);
                }
                //if (!String.IsNullOrWhiteSpace(filter.Group))
                //{
                //    query = query.Where(w => w.fkusersitelinks.Any(a => a.f_user_group == Guid.Parse(filter.Group)));
                //}
                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(s =>
                                            (s.Surname + " " + s.Name + " " + s.Patronymic).ToLower().Contains(filter.SearchText.ToLower()) ||
                                            s.Email == filter.SearchText
                                        );
                }

                query = query.OrderBy(s => new { s.Surname, s.Name, s.Patronymic });

                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new UserModel
                    {
                        Id = s.UserId,
                        SiteId = s.SiteId,
                        Surname = s.Surname,
                        Name = s.Name,
                        Patronimyc = s.Patronymic,
                        BirthDate = s.BirthDate,
                        Email = s.Email,
                        EmailConfirmed = s.EmailConfirmed,
                        Phone = s.PhoneNumber,
                        PhoneConfirmed = s.PhoneNumberConfirmed,
                        RegDate = s.RegDate,
                        Disabled = s.Disabled,

                        //Roles
                        //и сайты к которым он прикреплен
                    });


                return new Paged<UserModel>()
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
        /// Администраторы, разработчики и тп сайта
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<UserModel> GetSiteAdmins(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<UserModel> result = new Paged<UserModel>();

                var query = (from u in db.core_AspNetUsers
                             join p in db.core_AspNetUserProfiles on u.Id equals p.UserId
                             //Условие принадлежности к какой-либо роли
                             where db.core_AspNetUserRoles.Any(r => r.UserId == u.Id && r.AspNetRolesRoleId.Name != "User" && r.AspNetRolesRoleId.Discriminator == "ApplicationRole") //исключить обычного пользователя
                             //Условие принадлежности к сайту
                             where db.core_AspNetRoles.Any(s => s.Name == _siteId.ToString() && s.AspNetUserRolesRoleIds.Any(t => t.UserId == u.Id) && s.Discriminator == "IdentityRole")
                             select new
                             {
                                 u.Id,
                                 u.UserId,
                                 //u.UserName,
                                 u.Email,
                                 u.EmailConfirmed,
                                 u.PhoneNumber,
                                 u.PhoneNumberConfirmed,
                                 //u.LockoutEndDateUtc,
                                 //u.LockoutEnabled,
                                 //u.AccessFailedCount,
                                 u.SiteId,
                                 p.Surname,
                                 p.Name,
                                 p.Patronymic,
                                 p.Disabled,
                                 p.BirthDate,
                                 p.RegDate
                             }
                            );

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(s => s.Disabled == filter.Disabled.Value);
                }

                //if (!String.IsNullOrWhiteSpace(filter.Group))
                //{
                //    query = query.Where(w => w.fkusersitelinks.Any(a => a.f_user_group == Guid.Parse(filter.Group)));
                //}

                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(s =>
                                           (s.Surname + " " + s.Name + " " + s.Patronymic).ToLower().Contains(filter.SearchText.ToLower()) ||
                                            s.Email == filter.SearchText
                                        );
                }

                query = query.OrderBy(s => new { s.Surname, s.Name, s.Patronymic });

                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new UserModel
                    {
                        Id = s.UserId,
                        SiteId = s.SiteId,
                        Surname = s.Surname,
                        Name = s.Name,
                        Patronimyc = s.Patronymic,
                        BirthDate = s.BirthDate,
                        Email = s.Email,
                        EmailConfirmed = s.EmailConfirmed,
                        Phone = s.PhoneNumber,
                        PhoneConfirmed =s.PhoneNumberConfirmed,
                        RegDate = s.RegDate,
                        Disabled = s.Disabled,
                        
                        //Roles = null
                    });


                return new Paged<UserModel>()
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
        /// Все пользователи сайта
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<UserModel> GetSiteUsers(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<UserModel> result = new Paged<UserModel>();

                var query = (from u in db.core_AspNetUsers
                             join p in db.core_AspNetUserProfiles on u.Id equals p.UserId
                             //Условие принадлежности к сайту
                             where db.core_AspNetRoles.Any(s => s.Name == _siteId.ToString() && s.AspNetUserRolesRoleIds.Any(t => t.UserId == u.Id) && s.Discriminator == "IdentityRole")
                             select new {
                                 u.Id,
                                 u.UserId,
                                 //u.UserName,
                                 u.Email,
                                 u.EmailConfirmed,
                                 u.PhoneNumber,
                                 u.PhoneNumberConfirmed,
                                 //u.LockoutEndDateUtc,
                                 //u.LockoutEnabled,
                                 //u.AccessFailedCount,
                                 u.SiteId,
                                 p.Surname,
                                 p.Name,
                                 p.Patronymic,
                                 p.Disabled,
                                 p.BirthDate,
                                 p.RegDate
                             }
                            );


                if (filter.Disabled.HasValue)
                {
                    query = query.Where(s => s.Disabled == filter.Disabled.Value);
                }
                //if (!String.IsNullOrWhiteSpace(filter.Group))
                //{
                //    query = query.Where(w => w.fkusersitelinks.Any(a => a.f_user_group == Guid.Parse(filter.Group)));
                //}
                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(s =>
                                            (s.Surname + " " + s.Name + " " + s.Patronymic).ToLower().Contains(filter.SearchText.ToLower()) ||
                                            s.Email == filter.SearchText
                                        );
                }

                query = query.OrderBy(s => new { s.Surname, s.Name, s.Patronymic });

                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new UserModel
                    {
                        Id = s.UserId,
                        SiteId = s.SiteId,
                        Surname = s.Surname,
                        Name = s.Name,
                        Patronimyc = s.Patronymic,
                        BirthDate = s.BirthDate,
                        Email = s.Email,
                        EmailConfirmed = s.EmailConfirmed,
                        Phone = s.PhoneNumber,
                        PhoneConfirmed = s.PhoneNumberConfirmed,
                        RegDate = s.RegDate,
                        Disabled = s.Disabled

                    });


                return new Paged<UserModel>()
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
        /// Информация о пользователе
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUser(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_AspNetUsers
                    .Where(s => s.UserId == id);

                var data = query.Select(s => new UserModel
                {
                    Id = s.UserId,
                    SiteId = s.SiteId,
                    Surname = s.AspNetUserProfilesUserId.Surname,
                    Name = s.AspNetUserProfilesUserId.Name,
                    Patronimyc = s.AspNetUserProfilesUserId.Patronymic,
                    Disabled = s.AspNetUserProfilesUserId.Disabled,
                    BirthDate = s.AspNetUserProfilesUserId.BirthDate,
                    RegDate = s.AspNetUserProfilesUserId.RegDate,
                    Email = s.Email,
                    EmailConfirmed = s.EmailConfirmed,
                    Phone = s.PhoneNumber,
                    PhoneConfirmed = s.PhoneNumberConfirmed,
                    
                    Roles = null
                    //сайты
                });

                return data.SingleOrDefault();
            }
        }



        /// <summary>
        /// Добавляет пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //public bool InsertUser(UserModel user)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tran = db.BeginTransaction())
        //        {
        //            try
        //            {
        //                var _userId = user.Id.ToString();

        //                var dbUser = db.core_AspNetUsers
        //                    .Where(s => s.Id == _userId || (s.Email == user.Email && s.SiteId == _siteId));

        //                if (!dbUser.Any())
        //                {
        //                    var newUser = new core_AspNetUsers()
        //                    {
        //                        Id = _userId,
        //                        UserName = _userId,
        //                        UserId = user.Id,
        //                        SiteId = _siteId,
        //                        Email = user.Email,
        //                        EmailConfirmed = user.EmailConfirmed,
        //                        PhoneNumber = user.Phone,
        //                        PhoneNumberConfirmed = user.PhoneConfirmed
        //                    };
        //                    db.Insert(newUser);

        //                    var newUserProfile = new core_AspNetUserProfiles()
        //                    {
        //                        UserId = _userId,
        //                        Surname = user.Surname,
        //                        Name = user.Name,
        //                        Patronymic = user.Patronimyc,
        //                        BirthDate = user.BirthDate,
        //                        RegDate = DateTime.Now,
        //                        Disabled = user.Disabled
        //                    };
        //                    db.Insert(newUserProfile);

        //                    var log = new LogModel
        //                    {
        //                        PageId = user.Id,
        //                        PageName = $"{user.Surname} {user.Name} {user.Patronimyc}",
        //                        Section = LogModule.Users,
        //                        Action = LogAction.insert
        //                    };
        //                    InsertLog(log);

        //                    tran.Commit();

        //                    return true;
        //                }
        //                return false;
        //            }
        //            catch(Exception ex)
        //            {
        //                //log ex
        //                return false;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //public bool UpdateUser(UserModel user)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tran = db.BeginTransaction())
        //        {

        //            try
        //            {
        //                var _userId = user.Id.ToString();

        //                var dbUser = db.core_AspNetUsers
        //                    .Where(s => s.Id == _userId);

        //                if (dbUser.Any())
        //                {
        //                    var updatedUser = dbUser.Single();
        //                    updatedUser.Email = user.Email;
        //                    updatedUser.EmailConfirmed = user.EmailConfirmed;
        //                    updatedUser.PhoneNumber = user.Phone;
        //                    updatedUser.PhoneNumberConfirmed = user.PhoneConfirmed;

        //                    db.Update(updatedUser);


        //                    var dbUserProfile = db.core_AspNetUserProfiles
        //                          .Where(s => s.UserId == _userId);
        //                    var updatedUserProfile = dbUserProfile.SingleOrDefault();

        //                    updatedUserProfile.Surname = user.Surname;
        //                    updatedUserProfile.Name = user.Name;
        //                    updatedUserProfile.Patronymic = user.Patronimyc;
        //                    updatedUserProfile.BirthDate = user.BirthDate;
        //                    updatedUserProfile.Disabled = user.Disabled;

        //                    db.Update(updatedUserProfile);

        //                    var log = new LogModel
        //                    {
        //                        PageId = user.Id,
        //                        PageName = $"{user.Surname} {user.Name} {user.Patronimyc}",
        //                        Section = LogModule.Users,
        //                        Action = LogAction.update
        //                    };
        //                    InsertLog(log);


        //                    tran.Commit();
        //                    return true;
        //                }

        //                return false;

        //            }
        //            catch(Exception ex)
        //            {
        //                //log ex
        //                return false;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Меняет пароль
        /// </summary>
        /// <param name="id"></param>
        /// <param name="salt"></param>
        /// <param name="hash"></param>
        //public void ChangePassword(Guid id, string salt, string hash)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            var user = db.core_users.Where(w => w.id == id).SingleOrDefault();

        //            if (user != null)
        //            {
        //                var log = new LogModel
        //                {
        //                    PageId = id,
        //                    PageName = $"{user.c_surname} {user.c_name} {user.c_patronymic}",
        //                    Section = LogModule.Users,
        //                    Action = LogAction.update
        //                };
        //                InsertLog(log);

        //                db.core_users
        //                    .Where(w => w.id == id)
        //                    .Set(s => s.c_salt, salt)
        //                    .Set(s => s.c_hash, hash)
        //                    .Update();

        //                tr.Commit();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Проверяет существование пользователя по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckUserExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_AspNetUsers
                    .Where(w => w.UserId == id)
                    .Any();
            }
        }

        /// <summary>
        /// Проверяет существование пользователя по email на данном сайте
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool CheckUserExists(string email, Guid siteId)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_AspNetUsers
                    .Where(w => w.Email == email && w.SiteId == siteId)
                    .Any();
            }
        }

        /// <summary>
        /// Удаляет пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public bool DeleteUser(Guid id)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            bool result = false;

        //            var user = db.core_users.Where(w => w.id == id).SingleOrDefault();
        //            if (user != null)
        //            {
        //                var log = new LogModel
        //                {
        //                    PageId = id,
        //                    PageName = $"{user.c_surname} {user.c_name} {user.c_patronymic}",
        //                    Section = LogModule.Users,
        //                    Action = LogAction.delete
        //                };
        //                InsertLog(log, user);

        //                result = db.Delete(user) > 0;

        //                tr.Commit();
        //            }

        //            return result;
        //        }
        //    }
        //}


        ///// <summary>
        ///// Добавляет связь пользователя с сайтом
        ///// </summary>
        ///// <returns></returns>
        //public bool InsertUserSiteLink(CMSdb db, Guid userId, Guid groupId)
        //{
        //    Guid id = Guid.NewGuid();

        //    db.core_user_site_link
        //        .Insert(() => new core_user_site_link
        //        {
        //            id = id,
        //            f_site = _siteId,
        //            f_user = userId,
        //            f_user_group = groupId
        //        });

        //    var log = new LogModel
        //    {
        //        PageId = id,
        //        PageName = GetLogTitleForUserSiteLink(userId, groupId, db),
        //        Section = LogModule.Users,
        //        Action = LogAction.insert
        //    };
        //    InsertLog(log);

        //    return true;
        //}

        ///// <summary>
        ///// Возвращает заголовок при логировании
        ///// добавления связи пользователя и сайта
        ///// </summary>
        ///// <returns></returns>
        //private string GetLogTitleForUserSiteLink(Guid userId, Guid groupId, CMSdb db)
        //{
        //    string user = db.core_users
        //        .Where(w => w.id == userId)
        //        .Select(s => $"{s.c_surname} {s.c_name}")
        //        .SingleOrDefault();

        //    string domain = db.core_sites
        //        .Where(w => w.id == _siteId)
        //        .Select(s => s.c_name)
        //        .SingleOrDefault();

        //    string group = db.core_user_groups
        //        .Where(w => w.id == groupId)
        //        .Select(s => s.c_title)
        //        .SingleOrDefault();

        //    return $"{user} {group} {domain}";
        //}
    }
}
