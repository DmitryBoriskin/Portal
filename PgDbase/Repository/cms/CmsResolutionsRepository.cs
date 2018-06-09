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
        public bool DublicateUser(Guid newId, Guid userId, Guid siteId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var dbUser = db.core_AspNetUsers
                       .Where(s => s.Id == userId.ToString())
                       .SingleOrDefault();

                    if (dbUser != null)
                    {
                        //Аккаунт
                        dbUser.Id = newId.ToString();
                        dbUser.UserName = newId.ToString();
                        dbUser.UserId = newId;
                        dbUser.SiteId = siteId;

                        db.Insert(dbUser);

                        //Данные
                        var dbUserProfile = db.core_AspNetUserProfiles
                            .Where(s => s.UserId == userId.ToString())
                            .SingleOrDefault();

                        if (dbUserProfile == null)
                            return false;

                        dbUserProfile.UserId = newId.ToString();
                        db.Insert(dbUserProfile);

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

                    if (dbUser != null)
                    {
                        //Удаляемая запись (не оригинал, оригинал удалять нельзя)
                        var dbDublicateUser = db.core_AspNetUsers
                          .Where(s => s.Email == dbUser.Email)
                          .Where(s => s.SiteId == siteId)
                          .Where(s => s.Id != dbUser.Id)
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

    }
}
