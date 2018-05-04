using LinqToDB;
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
        /// Возвращает постраничный список администраторо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Paged<UserModel> GetAllUsers(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<UserModel> result = new Paged<UserModel>();

                var query = db.core_users.OrderBy(o => o.fkusersitelinks).AsQueryable();

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
                }
                if (!String.IsNullOrWhiteSpace(filter.Group))
                {
                    query = query.Where(w => w.fkusersitelinks.Any(a => a.f_user_group == Guid.Parse(filter.Group)));
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

                int itemsCount = query.Count();

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
                        TryLogin = s.d_try_login,
                        Group = new GroupsModel
                        {
                            Title = s.fkusersitelinks
                                .Select(g => g.fkusersitelinkusergroup.c_title)
                                .SingleOrDefault()
                        }
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
        /// Возвращает группу пользователей для редактирования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserGroupResolution[] GetGroupResolutions(Guid groupId)
        {
            using (var db = new CMSdb(_context))
            {
                //return db.core_user_group_resolutions
                //    .Where(w => w.f_usergroup == groupId)
                //    .Select(s => new UserGroupResolution
                //    {
                //        Id = s.id,
                //        UserGroup = s.f_usergroup,
                //        IsRead = s.b_read,
                //        IsWrite = s.b_write,
                //        IsChange = s.b_change,
                //        IsDelete = s.b_delete,
                //        Menu = new GroupsModel
                //        {
                //            Id = s.fkusergroupresolutionsmenu.id,
                //            Title = s.fkusergroupresolutionsmenu.c_title
                //        }
                //    }).ToArray();


                var query = db.core_menu
                    .OrderBy(o => o.f_parent)
                    .ThenBy(o => o.n_sort)
                    .ToArray();

                UserGroupResolution[] result = new UserGroupResolution[query.Count()];

                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new UserGroupResolution
                    {
                        Id = Guid.NewGuid(),
                        UserGroup = groupId,
                        IsRead = query[i].fkusergroupresolutionss != null 
                            ? query[i].fkusergroupresolutionss.Where(a => a.f_usergroup == groupId).Any(a => a.b_read) : false,
                        IsWrite = query[i].fkusergroupresolutionss != null 
                            ? query[i].fkusergroupresolutionss.Where(a => a.f_usergroup == groupId).Any(a => a.b_write) : false,
                        IsChange = query[i].fkusergroupresolutionss != null 
                            ? query[i].fkusergroupresolutionss.Where(a => a.f_usergroup == groupId).Any(a => a.b_change) : false,
                        IsDelete = query[i].fkusergroupresolutionss != null 
                            ? query[i].fkusergroupresolutionss.Where(a => a.f_usergroup == groupId).Any(a => a.b_delete) : false,
                        Menu = new GroupsModel
                        {
                            Id = query[i].id,
                            Title = query[i].c_title
                        }
                    };
                }

                return result;
            }
        }
    }
}
