using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с правами
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Список ролей
        /// </summary>
        /// <returns></returns>
        public RoleModel[] GetRoles(string[] excludeRoles = null)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_AspNetRoles
                    .Where(s => s.Discriminator == "ApplicationRole");

                if (excludeRoles != null)
                    query = query.Where(s => !excludeRoles.Contains(s.Name));

                var data = query.Select(s => new RoleModel()
                {
                    Id = Guid.Parse(s.Id),
                    Name = s.Name,
                    Desc = s.Desc,
                    Claims = GetRoleClaims(s.Id)
                });

                return data.ToArray();
            }
        }

        /// <summary>
        /// Список сайтов из ролей
        /// </summary>
        /// <returns></returns>
        public RoleModel[] GetSites()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_AspNetRoles
                    .Where(s => s.Discriminator == "IdentityRole")
                    .Where(s => s.Name != Guid.Empty.ToString());

                var data = query.Select(s => new RoleModel()
                {
                    Id = Guid.Parse(s.Id),
                    Name = s.Name,
                    Desc = GetSiteName(s.Name),
                });

                return data.ToArray();
            }
        }

        /// <summary>
        /// Роль
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RoleModel GetRole(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_AspNetRoles
                    .Where(s => s.Id == id.ToString());

                var data = query.Select(s => new RoleModel()
                {
                    Id = Guid.Parse(s.Id),
                    Name = s.Name,
                    Desc = s.Desc,
                    Claims = GetRoleClaims(s.Id)
                });

                return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Права роли
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RoleClaimModel[] GetRoleClaims(Guid roleId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_AspNetRoleClaims
                    .Where(s => s.RoleId == roleId.ToString());

                var data = query.Select(s => new RoleClaimModel()
                {
                    Id = s.Id,
                    RoleId = Guid.Parse(s.RoleId),
                    Type = s.ClaimType,
                    Value = s.ClaimValue,
                    Checked = true
                });

                return data.ToArray();
            }
        }

        /// <summary>
        /// права роли
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private RoleClaimModel[] GetRoleClaims(string roleId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_AspNetRoleClaims
                    .Where(s => s.RoleId == roleId);

                var data = query.Select(s => new RoleClaimModel()
                {
                    Id = s.Id,
                    RoleId = Guid.Parse(s.RoleId),
                    Type = s.ClaimType,
                    Value = s.ClaimValue,
                    Checked = true
                });

                return data.ToArray();
            }
        }

        /// <summary>
        /// Получение списка ролей для пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public RoleModel[] GetUserRoles(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var _userId = userId.ToString();
                var query = db.core_AspNetUserRoles
                    .Where(s => s.AspNetRolesRoleId.Discriminator == "ApplicationRole")
                    .Where(s => s.UserId == _userId);

                var data = query.Select(s => new RoleModel()
                {
                    Id = Guid.Parse(s.RoleId),
                    Name = s.AspNetRolesRoleId.Name,
                    Desc = s.AspNetRolesRoleId.Desc,
                    //Claims = GetRoleClaims(s.RoleId)
                });

                return data.ToArray();
            }
        }

        /// <summary>
        /// Сайты, к которым привязан пользователь
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public RoleModel[] GetUserSites(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var _userId = userId.ToString();
                var query = db.core_AspNetUserRoles
                    .Where(s => s.AspNetRolesRoleId.Discriminator == "IdentityRole")
                    .Where(s => s.AspNetRolesRoleId.Name != Guid.Empty.ToString())
                    .Where(s => s.UserId == _userId);

                var data = query.Select(s => new RoleModel()
                {
                    Id = Guid.Parse(s.RoleId),
                    Name = s.AspNetRolesRoleId.Name,
                    Desc = GetSiteName(s.AspNetRolesRoleId.Name),
                    //Claims = GetRoleClaims(s.RoleId)
                });

                return data.ToArray();
            }
        }

        /// <summary>
        /// Изменение прав у роли
        /// </summary>
        /// <param name="roleClaim"></param>
        /// <returns></returns>
        public bool UpdateRoleClaim(RoleClaimModel roleClaim)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var dbRoleclaim = db.core_AspNetRoleClaims
                       .Where(s => s.RoleId == roleClaim.RoleId.ToString())
                       .Where(s => s.ClaimType == roleClaim.Type)
                       .Where(s => s.ClaimValue == roleClaim.Value);

                    if (roleClaim.Checked)
                    {
                        if (!dbRoleclaim.Any())
                        {
                            //insert
                            var newRoleClaim = new core_AspNetRoleClaims()
                            {
                                RoleId = roleClaim.RoleId.ToString(),
                                ClaimType = roleClaim.Type,
                                ClaimValue = roleClaim.Value
                            };

                            db.Insert(newRoleClaim);

                            //log
                            //var log = new LogModel
                            //{
                            //    PageId = Guid.NewGuid,
                            //    PageName = "",
                            //    Section = LogModule.Users,
                            //    Action = LogAction.update,
                            //    Comment = "Изменена связь пользователя с сайтами"
                            //};
                            //InsertLog(log);


                            tran.Commit();
                            return true;
                        }

                    }
                    else
                    {
                        if (dbRoleclaim.Any())
                        {
                            //delete
                            var roleClaimData = dbRoleclaim.Single();
                            db.Delete(roleClaimData);

                            //log

                            tran.Commit();
                            return true;
                        }

                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Дублируем пользователя на др. сайт
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public bool DublicateUser(Guid userId, Guid siteId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var newGuid = Guid.NewGuid();

                    var dbUser = db.core_AspNetUsers
                       .Where(s => s.Id == userId.ToString())
                       .SingleOrDefault();

                    if (dbUser != null)
                    {
                        //Аккаунт
                        dbUser.Id = newGuid.ToString();
                        dbUser.UserName = newGuid.ToString();
                        dbUser.UserId = newGuid;

                        db.Insert(dbUser);

                        //Данные
                        var dbUserProfile = db.core_AspNetUserProfiles
                            .Where(s => s.UserId == userId.ToString())
                            .SingleOrDefault();

                        if (dbUserProfile == null)
                            return false;

                        dbUserProfile.UserId = newGuid.ToString();
                        dbUserProfile.IsOriginal = false;
                        db.Insert(dbUserProfile);

                        //Права
                        var dbUserRoles = db.core_AspNetUserRoles
                            .Where(s => s.UserId == userId.ToString())
                            .Select(s => s).ToArray();
                            
                        if(dbUserRoles.Count() > 0)
                        {
                            //Если не нужно создавать пользователя личного кабинета, то исключаем роль "User"
                            foreach (var dbUserRole in dbUserRoles.ToArray())
                            {
                                dbUserRole.UserId = newGuid.ToString();
                                //db.Insert(dbUserRole);
                            }
                            db.BulkCopy(dbUserRoles);
                        }

                        tran.Commit();
                        return true;
                    }

                    //log
                    //var log = new LogModel
                    //{
                    //    PageId = Guid.NewGuid,
                    //    PageName = "",
                    //    Section = LogModule.Users,
                    //    Action = LogAction.update,
                    //    Comment = "Изменена связь пользователя с сайтами"
                    //};
                    //InsertLog(log);

                    return false;
                }
            }
        }

        /// <summary>
        /// Дублируем пользователя на др. сайт
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public bool DeleteDublicateUser(Guid userId, Guid siteId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    //Оригинал записи
                    var dbUser = db.core_AspNetUsers
                       .Where(s => s.Id == userId.ToString())
                       .SingleOrDefault();

                    if(dbUser != null)
                    {
                        //Удаляемая запись (не оригинал, оригинал удалять нельзя)
                        var dbDublicateUser = db.core_AspNetUsers
                          .Where(s => s.Email == dbUser.Email)
                          .Where(s => s.SiteId == siteId)
                          .Where(s => s.Id != dbUser.Id)
                          .Where(s => s.AspNetUserProfilesUserId.IsOriginal == false)
                          .SingleOrDefault();

                        if (dbDublicateUser == null)
                            return false;

                        var dbDublicateUserId = dbDublicateUser.Id;

                        //Удаляем права
                         db.core_AspNetUserRoles
                            .Where(s => s.UserId == dbDublicateUserId)
                            .Delete();

                        //Удаляем Данные
                        db.core_AspNetUserProfiles
                            .Where(s => s.UserId == dbDublicateUserId)
                            .Delete();

                        //Аккаунт
                        db.core_AspNetUsers
                           .Where(s => s.Id == dbDublicateUserId)
                           .Delete();

                        tran.Commit();
                        return true;
                    }


                    //log
                    //var log = new LogModel
                    //{
                    //    PageId = Guid.NewGuid,
                    //    PageName = "",
                    //    Section = LogModule.Users,
                    //    Action = LogAction.update,
                    //    Comment = "Изменена связь пользователя с сайтами"
                    //};
                    //InsertLog(log);

                    return false;
                }
            }
        }

        ///// <summary>
        ///// Добавление пользователю роли
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="roleId"></param>
        ///// <returns></returns>
        //public bool AddUserRole(string userId, string roleId)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tran = db.BeginTransaction())
        //        {

        //            var dbUserRole = db.core_AspNetUserRoles
        //               .Where(s => s.RoleId == roleId)
        //               .Where(s => s.UserId == userId);


        //            if (!dbUserRole.Any())
        //            {
        //                //insert
        //                var newUserRole = new core_AspNetUserRoles()
        //                {
        //                    RoleId = roleId,
        //                    UserId = userId
        //                };

        //                db.Insert(newUserRole);

        //                //log
        //                //var log = new LogModel
        //                //{
        //                //    PageId = Guid.NewGuid,
        //                //    PageName = "",
        //                //    Section = LogModule.Users,
        //                //    Action = LogAction.update,
        //                //    Comment = "Изменена связь пользователя с сайтами"
        //                //};
        //                //InsertLog(log);


        //                tran.Commit();
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Удаление роли у пользователя
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="roleId"></param>
        ///// <returns></returns>
        //public bool DeleteUserRole(string userId, string roleId)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tran = db.BeginTransaction())
        //        {

        //            var dbUserRole = db.core_AspNetUserRoles
        //               .Where(s => s.RoleId == roleId)
        //               .Where(s => s.UserId == userId);

        //            if (dbUserRole.Any())
        //            {
        //                var _userRole = dbUserRole.Single();
        //                db.Delete(_userRole);
        //                tran.Commit();
        //                return true;
        //            }

        //            return false;
        //        }
        //    }
        //}

        /// <summary>
        /// Возвращает группу пользователей для редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public UserGroupResolution[] GetGroupResolutions(Guid groupId)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        var query = db.core_menu
        //            .OrderBy(o => o.f_parent)
        //            .ThenBy(o => o.n_sort);

        //        var data = query.ToArray();

        //        UserGroupResolution[] result = new UserGroupResolution[data.Length];

        //        var resolutions = db.core_user_group_resolutions
        //            .Where(w => w.f_usergroup == groupId);

        //        for (int i = 0; i < result.Length; i++)
        //        {
        //            resolutions = resolutions.Where(w => w.f_menu == data[i].id);
        //            result[i] = new UserGroupResolution
        //            {
        //                Id = Guid.NewGuid(),
        //                UserGroup = groupId,
        //                IsRead = resolutions
        //                    .Select(a => a.b_read).SingleOrDefault(),
        //                IsWrite = resolutions
        //                    .Select(a => a.b_write).SingleOrDefault(),
        //                IsChange = resolutions
        //                    .Select(a => a.b_change).SingleOrDefault(),
        //                IsDelete = resolutions
        //                    .Select(a => a.b_delete).SingleOrDefault(),
        //                Menu = new GroupsModel
        //                {
        //                    Id = data[i].id,
        //                    Title = data[i].c_title
        //                }
        //            };
        //        }

        //        return result;
        //    }
        //}

        /// <summary>
        /// Обновляет права на раздел
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //public bool UpdateGroupResolution(ClaimParams data)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        var query = db.core_user_group_resolutions
        //            .Where(w => w.f_usergroup == data.GroupId)
        //            .Where(w => w.f_menu == data.MenuId);

        //        if (query.Any())
        //        {
        //            switch (data.Claim)
        //            {
        //                case "read":
        //                    return query.Set(u => u.b_read, data.IsChecked).Update() > 0;
        //                case "write":
        //                    return query.Set(u => u.b_write, data.IsChecked).Update() > 0;
        //                case "change":
        //                    return query.Set(u => u.b_change, data.IsChecked).Update() > 0;
        //                case "delete":
        //                    return query.Set(u => u.b_delete, data.IsChecked).Update() > 0;
        //                default: return false;
        //            }
        //        }
        //        else
        //        {
        //            core_user_group_resolutions item = new core_user_group_resolutions
        //            {
        //                id = Guid.NewGuid(),
        //                f_usergroup = data.GroupId,
        //                f_menu = data.MenuId,
        //                b_read = false,
        //                b_write = false,
        //                b_change = false,
        //                b_delete = false
        //            };
        //            switch (data.Claim)
        //            {
        //                case "read":
        //                    item.b_read = data.IsChecked;
        //                    break;
        //                case "write":
        //                    item.b_write = data.IsChecked;
        //                    break;
        //                case "change":
        //                    item.b_change = data.IsChecked;
        //                    break;
        //                case "delete":
        //                    item.b_delete = data.IsChecked;
        //                    break;
        //            }
        //            return db.core_user_group_resolutions.Insert(() => new core_user_group_resolutions
        //            {
        //                id = item.id,
        //                f_usergroup = item.f_usergroup,
        //                f_menu = item.f_menu,
        //                b_read = item.b_read,
        //                b_write = item.b_write,
        //                b_change = item.b_change,
        //                b_delete = item.b_delete
        //            }) > 0;
        //        }
        //    }
        //}

        /// <summary>
        /// Возвращает права на раздел относительно группы
        /// </summary>
        /// <param name="user"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        //public ResolutionModel GetUserResolutionGroup(Guid user, string controller)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        Guid group = db.core_user_site_link
        //            .Where(w => w.f_user == user)
        //            .Where(w => w.f_site == _siteId)
        //            .Select(s => s.f_user_group)
        //            .SingleOrDefault();

        //        return db.core_user_group_resolutions
        //            .Where(w => w.f_usergroup == group)
        //            .Where(w => w.fkusergroupresolutionsmenu
        //                            .c_alias.ToLower() == controller.ToLower())
        //            .Select(s => new ResolutionModel
        //            {
        //                IsRead = s.b_read,
        //                IsWrite = s.b_write,
        //                IsChange = s.b_change,
        //                IsDelete = s.b_delete
        //            }).SingleOrDefault();
        //    }
        //}
    }
}
