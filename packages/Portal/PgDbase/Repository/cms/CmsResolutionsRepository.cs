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
                var query = db.core_menu
                    .OrderBy(o => o.f_parent)
                    .ThenBy(o => o.n_sort);

                var data = query.ToArray();

                UserGroupResolution[] result = new UserGroupResolution[data.Length];

                var resolutions = db.core_user_group_resolutions
                    .Where(w => w.f_usergroup == groupId);

                for (int i = 0; i < result.Length; i++)
                {
                    resolutions = resolutions.Where(w => w.f_menu == data[i].id);
                    result[i] = new UserGroupResolution
                    {
                        Id = Guid.NewGuid(),
                        UserGroup = groupId,
                        IsRead = resolutions
                            .Select(a => a.b_read).SingleOrDefault(),
                        IsWrite = resolutions
                            .Select(a => a.b_write).SingleOrDefault(),
                        IsChange = resolutions
                            .Select(a => a.b_change).SingleOrDefault(),
                        IsDelete = resolutions
                            .Select(a => a.b_delete).SingleOrDefault(),
                        Menu = new GroupsModel
                        {
                            Id = data[i].id,
                            Title = data[i].c_title
                        }
                    };
                }

                return result;
            }
        }

        /// <summary>
        /// Обновляет права на раздел
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateGroupResolution(ClaimParams data)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_user_group_resolutions
                    .Where(w => w.f_usergroup == data.GroupId)
                    .Where(w => w.f_menu == data.MenuId);

                if (query.Any())
                {
                    switch (data.Claim)
                    {
                        case "read":
                            return query.Set(u => u.b_read, data.IsChecked).Update() > 0;
                        case "write":
                            return query.Set(u => u.b_write, data.IsChecked).Update() > 0;
                        case "change":
                            return query.Set(u => u.b_change, data.IsChecked).Update() > 0;
                        case "delete":
                            return query.Set(u => u.b_delete, data.IsChecked).Update() > 0;
                        default: return false;
                    }
                }
                else
                {
                    core_user_group_resolutions item = new core_user_group_resolutions
                    {
                        id = Guid.NewGuid(),
                        f_usergroup = data.GroupId,
                        f_menu = data.MenuId,
                        b_read = false,
                        b_write = false,
                        b_change = false,
                        b_delete = false
                    };
                    switch (data.Claim)
                    {
                        case "read":
                            item.b_read = data.IsChecked;
                            break;
                        case "write":
                            item.b_write = data.IsChecked;
                            break;
                        case "change":
                            item.b_change = data.IsChecked;
                            break;
                        case "delete":
                            item.b_delete = data.IsChecked;
                            break;
                    }
                    return db.core_user_group_resolutions.Insert(() => new core_user_group_resolutions
                    {
                        id = item.id,
                        f_usergroup = item.f_usergroup,
                        f_menu = item.f_menu,
                        b_read = item.b_read,
                        b_write = item.b_write,
                        b_change = item.b_change,
                        b_delete = item.b_delete
                    }) > 0;
                }
            }
        }

        /// <summary>
        /// Возвращает права на раздел относительно группы
        /// </summary>
        /// <param name="user"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public ResolutionModel GetUserResolutionGroup(Guid user, string controller)
        {
            using (var db = new CMSdb(_context))
            {
                Guid group = db.core_user_site_link
                    .Where(w => w.f_user == user)
                    .Where(w => w.f_site == _siteId)
                    .Select(s => s.f_user_group)
                    .SingleOrDefault();

                var resolutions = db.core_user_group_resolutions
                    .Where(w => w.f_usergroup == group)
                    .Where(w => w.fkusergroupresolutionsmenu
                                    .c_alias.ToLower() == controller.ToLower())
                    .Select(s => new ResolutionModel
                    {
                        IsRead = s.b_read,
                        IsWrite = s.b_write,
                        IsChange = s.b_change,
                        IsDelete = s.b_delete
                    });

                return resolutions.SingleOrDefault();
            }
        }
    }
}
