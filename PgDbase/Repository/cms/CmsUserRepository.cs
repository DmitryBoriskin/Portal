using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
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
        public Paged<UserModel> GetPortalAdmins(UserFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<UserModel> result = new Paged<UserModel>();

                var siteIdStr = _siteId.ToString();

                var query = db.core_AspNetUsers
                             //Условие принадлежности к какой-либо роли, исключая User
                             .Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name != "User" && r.AspNetRolesRoleId.Discriminator == "ApplicationRole"));

                if (filter.Disabled.HasValue)
                    query = query.Where(s => s.AspNetUserProfilesUserId.Disabled == filter.Disabled.Value);

                if (filter.ExcludeRoles != null)
                    query = query.Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name == siteIdStr && r.AspNetRolesRoleId.Discriminator == "IdentityRole"));

                if (!String.IsNullOrWhiteSpace(filter.Group))
                {
                    query = query.Where(s => s.AspNetUserRolesUserId.AspNetRolesRoleId.Name == filter.Group);
                }

                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(s =>
                                            (s.AspNetUserProfilesUserId.Surname + " " + s.AspNetUserProfilesUserId.Name + " " + s.AspNetUserProfilesUserId.Patronymic)
                                            .ToLower()
                                            .Contains(filter.SearchText.ToLower())
                                            || s.Email == filter.SearchText
                                        );
                }

                query = query.OrderBy(s => new { s.AspNetUserProfilesUserId.Surname, s.AspNetUserProfilesUserId.Name, s.AspNetUserProfilesUserId.Patronymic });

                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new UserModel
                    {
                        Id = s.UserId,
                        SiteId = s.SiteId,
                        Surname = s.AspNetUserProfilesUserId.Surname,
                        Name = s.AspNetUserProfilesUserId.Name,
                        Patronimyc = s.AspNetUserProfilesUserId.Patronymic,
                        BirthDate = s.AspNetUserProfilesUserId.BirthDate,
                        Email = s.Email,
                        EmailConfirmed = s.EmailConfirmed,
                        Phone = s.PhoneNumber,
                        PhoneConfirmed = s.PhoneNumberConfirmed,
                        RegDate = s.AspNetUserProfilesUserId.RegDate,
                        Disabled = s.AspNetUserProfilesUserId.Disabled,

                        Roles = GetUserRoles(s.UserId)
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
        public Paged<UserModel> GetSiteAdmins(UserFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<UserModel> result = new Paged<UserModel>();

                var siteIdStr = _siteId.ToString();
                var query = db.core_AspNetUsers
                             //Условие принадлежности к какой-либо роли, исключая User
                             .Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name != "User" && r.AspNetRolesRoleId.Discriminator == "ApplicationRole"))
                             //Условие принадлежности к сайту
                             .Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name == siteIdStr && r.AspNetRolesRoleId.Discriminator == "IdentityRole"));


                if (filter.Disabled.HasValue)
                    query = query.Where(s => s.AspNetUserProfilesUserId.Disabled == filter.Disabled.Value);

                if (filter.ExcludeRoles != null)
                    query = query.Where(s => !db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && filter.ExcludeRoles.Contains(r.AspNetRolesRoleId.Name) && r.AspNetRolesRoleId.Discriminator == "ApplicationRole"));
                //query = query.Where(s => !filter.ExcludeRoles.Contains(s.AspNetUserRolesUserId.AspNetRolesRoleId.Name));

                if (!String.IsNullOrWhiteSpace(filter.Group))
                {
                    query = query.Where(s => s.AspNetUserRolesUserId.AspNetRolesRoleId.Name == filter.Group);
                }

                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(s =>
                                            (s.AspNetUserProfilesUserId.Surname + " " + s.AspNetUserProfilesUserId.Name + " " + s.AspNetUserProfilesUserId.Patronymic)
                                            .ToLower()
                                            .Contains(filter.SearchText.ToLower())
                                            || s.Email == filter.SearchText
                                        );
                }

                query = query.OrderBy(s => new { s.AspNetUserProfilesUserId.Surname, s.AspNetUserProfilesUserId.Name, s.AspNetUserProfilesUserId.Patronymic });

                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new UserModel
                    {
                        Id = s.UserId,
                        SiteId = s.SiteId,
                        Surname = s.AspNetUserProfilesUserId.Surname,
                        Name = s.AspNetUserProfilesUserId.Name,
                        Patronimyc = s.AspNetUserProfilesUserId.Patronymic,
                        BirthDate = s.AspNetUserProfilesUserId.BirthDate,
                        Email = s.Email,
                        EmailConfirmed = s.EmailConfirmed,
                        Phone = s.PhoneNumber,
                        PhoneConfirmed = s.PhoneNumberConfirmed,
                        RegDate = s.AspNetUserProfilesUserId.RegDate,
                        Disabled = s.AspNetUserProfilesUserId.Disabled,

                        Roles = GetUserRoles(s.UserId)
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

                var siteIdStr = _siteId.ToString();

                var query = db.core_AspNetUsers
                                //Условие принадлежности к роли User
                                .Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name == "User" && r.AspNetRolesRoleId.Discriminator == "ApplicationRole"))
                                //Условие принадлежности к сайту
                                .Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name == siteIdStr && r.AspNetRolesRoleId.Discriminator == "IdentityRole"));


                if (filter.Disabled.HasValue)
                    query = query.Where(s => s.AspNetUserProfilesUserId.Disabled == filter.Disabled.Value);


                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    query = query.Where(s =>
                                            (s.AspNetUserProfilesUserId.Surname + " " + s.AspNetUserProfilesUserId.Name + " " + s.AspNetUserProfilesUserId.Patronymic)
                                            .ToLower()
                                            .Contains(filter.SearchText.ToLower())
                                            || s.Email == filter.SearchText
                                        );
                }

                query = query.OrderBy(s => new { s.AspNetUserProfilesUserId.Surname, s.AspNetUserProfilesUserId.Name, s.AspNetUserProfilesUserId.Patronymic });

                int itemsCount = query.Count();

                var list = query.Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new UserModel
                    {
                        Id = s.UserId,
                        SiteId = s.SiteId,
                        Surname = s.AspNetUserProfilesUserId.Surname,
                        Name = s.AspNetUserProfilesUserId.Name,
                        Patronimyc = s.AspNetUserProfilesUserId.Patronymic,
                        BirthDate = s.AspNetUserProfilesUserId.BirthDate,
                        Email = s.Email,
                        EmailConfirmed = s.EmailConfirmed,
                        Phone = s.PhoneNumber,
                        PhoneConfirmed = s.PhoneNumberConfirmed,
                        RegDate = s.AspNetUserProfilesUserId.RegDate,
                        Disabled = s.AspNetUserProfilesUserId.Disabled,
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
        ///Список пользователей в виде массива. Для построения выпадающего списка
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public UserModel[] GetSiteUsersList(string searchText)
        {
            using (var db = new CMSdb(_context))
            {
                var siteIdStr = _siteId.ToString();

                var query = db.core_AspNetUsers
                                //Условие принадлежности к роли User
                                .Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name == "User" && r.AspNetRolesRoleId.Discriminator == "ApplicationRole"))
                                //Условие принадлежности к сайту
                                .Where(s => db.core_AspNetUserRoles.Any(r => r.UserId == s.Id && r.AspNetRolesRoleId.Name == siteIdStr && r.AspNetRolesRoleId.Discriminator == "IdentityRole"));


                if (!String.IsNullOrWhiteSpace(searchText))
                {
                    query = query
                            .Where(s =>
                                    (s.AspNetUserProfilesUserId.Surname + " " + s.AspNetUserProfilesUserId.Name + " " + s.AspNetUserProfilesUserId.Patronymic)
                                    .ToLower()
                                    .Contains(searchText.ToLower())
                                    || s.Email.ToLower().Contains(searchText.ToLower())
                                );
                }

                query = query.OrderBy(s => new { s.AspNetUserProfilesUserId.Surname, s.AspNetUserProfilesUserId.Name, s.AspNetUserProfilesUserId.Patronymic });

                int itemsCount = query.Count();

                var list = query
                    .Select(s => new UserModel
                    {
                        Id = s.UserId,
                        Surname = s.AspNetUserProfilesUserId.Surname,
                        Name = s.AspNetUserProfilesUserId.Name,
                        Patronimyc = s.AspNetUserProfilesUserId.Patronymic,
                        BirthDate = s.AspNetUserProfilesUserId.BirthDate,
                        Email = s.Email,
                    })
                    .ToArray();
                

                return list;
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
                    PhoneConfirmed = s.PhoneNumberConfirmed
                    //Roles = null
                    //Sites = null
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
