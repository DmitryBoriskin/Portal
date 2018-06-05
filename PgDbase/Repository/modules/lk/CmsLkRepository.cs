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
    /// Репозиторий для работы с личным кабинетом
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает список лицевых счетов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<Subscr> GetSubscrs(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<Subscr> result = new Paged<Subscr>();
                var query = db.lk_subscrs
                    .Where(w => w.fkdepartments.f_site == _siteId);

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
                }
                if (!String.IsNullOrWhiteSpace(filter.Category))
                {
                    query = query.Where(w => w.f_department == Guid.Parse(filter.Category));
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
                                                      || w.c_link.Contains(p));
                            }
                        }
                    }
                }

                query = query.OrderBy(o => new { o.c_surname, o.c_name, o.c_patronymic });
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new Subscr
                    {
                        Id = s.id,
                        Link = s.c_link,
                        Created = s.d_created,
                        Disabled = s.b_disabled,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic
                    }).ToArray();

                return new Paged<Subscr>
                {
                    Items = list,
                    Pager = new PagerModel
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }

        /// <summary>
        /// Возвращает список ЛС для привязки к пользователю
        /// </summary>
        /// <returns></returns>
        public Subscr[] GetSubscrs()
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscrs
                    .Where(w => w.fkdepartments.f_site == _siteId)
                    .OrderBy(o => o.c_link)
                    .Select(s => new Subscr
                    {
                        Id = s.id,
                        Link = s.c_link,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает личный кабинет
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Subscr GetSubscr(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscrs
                    .Where(w => w.id == id)
                    .Select(s => new Subscr
                    {
                        Id = s.id,
                        Link = s.c_link,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Address = s.c_address,
                        Phone = s.c_phone,
                        Email = s.c_email,
                        Disabled = s.b_disabled,
                        Created = s.d_created, 
                        Department = s.f_department
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Создаёт лицевой счёт
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertSubscr(Subscr item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = item.Id,
                        PageName = $"{item.Surname} {item.Name} {item.Patronymic}",
                        Section = LogModule.Subscrs,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    bool result = db.lk_subscrs.Insert(() => new lk_subscrs
                    {
                        id = item.Id,
                        c_link = item.Link,
                        c_surname = item.Surname,
                        c_name = item.Name,
                        c_patronymic = item.Patronymic,
                        c_address = item.Address,
                        c_phone = item.Phone,
                        c_email = item.Email,
                        b_disabled = item.Disabled,
                        d_created = item.Created,
                        f_department = item.Department
                    }) > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновляет лицевой счёт
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateSubscr(Subscr item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = item.Id,
                        PageName = $"{item.Surname} {item.Name} {item.Patronymic}",
                        Section = LogModule.Subscrs,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.lk_subscrs
                        .Where(w => w.id == item.Id)
                        .Set(s => s.c_surname, item.Surname)
                        .Set(s => s.c_name, item.Name)
                        .Set(s => s.c_patronymic, item.Patronymic)
                        .Set(s => s.c_address, item.Address)
                        .Set(s => s.c_phone, item.Phone)
                        .Set(s => s.c_email, item.Email)
                        .Set(s => s.b_disabled, item.Disabled)
                        .Set(s => s.f_department, item.Department)
                        .Update() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Прикрепляет лицевые счета к пользователю
        /// </summary>
        /// <param name="user"></param>
        /// <param name="subscrs"></param>
        /// <returns></returns>
        public void UpdateUserSubscrs(Guid user, Guid[] subscrs)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var listExistSubscrs = db.lk_user_subscrs
                        .Where(w => w.f_user == user)
                        .Select(s => s.f_subscr)
                        .ToArray();

                    List<lk_user_subscrs> list = new List<lk_user_subscrs>();
                    foreach (var subscr in subscrs)
                    {
                        if (!listExistSubscrs.Contains(subscr))
                        {
                            list.Add(new lk_user_subscrs
                            {
                                f_user = user,
                                f_subscr = subscr
                            });
                        }
                    }

                    db.BulkCopy(list);
                    tr.Commit();
                }
            }
        }

        /// <summary>
        /// Проверяет существование лицевого счёта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckSubscrExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscrs
                    .Where(w => w.id == id).Any();
            }
        }

        /// <summary>
        /// Удаляет лицевой счёт
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteSubscr(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = false;
                    var item = db.lk_subscrs
                        .Where(w => w.id == id)
                        .SingleOrDefault();

                    if (item != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = $"{item.c_surname} {item.c_name} {item.c_patronymic}",
                            Section = LogModule.Subscrs,
                            Action = LogAction.delete
                        };
                        InsertLog(log, item);
                        result = db.Delete(item) > 0;
                    }

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Возвращает список подразделений
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<Department> GetDepartments(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<Department> result = new Paged<Department>();
                var query = db.lk_departments
                    .Where(w => w.f_site == _siteId);

                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            if (!String.IsNullOrWhiteSpace(p))
                            {
                                query = query.Where(w => w.c_title.Contains(p));
                            }
                        }
                    }
                }
                query.OrderByDescending(o => o.c_title);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new Department
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Address = s.c_address,
                        WorkTime = s.c_work_time,
                        Longitude = s.n_longitude,
                        Latitude = s.n_latitude,
                        Disabled = s.b_disabled
                    }).ToArray();

                return new Paged<Department>
                {
                    Items = list,
                    Pager = new PagerModel
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };

            }
        }

        /// <summary>
        /// Возвращает список подразделений для выпадающего списка
        /// </summary>
        /// <returns></returns>
        public GroupsModel[] GetDepartments()
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_departments
                    .Where(w => w.f_site == _siteId)
                    .Select(s => new GroupsModel
                    {
                        Id = s.id,
                        Title = s.c_title
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает подразделение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Department GetDepartment(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_departments
                    .Where(w => w.id == id)
                    .Select(s => new Department
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Address = s.c_address,
                        WorkTime = s.c_work_time,
                        Longitude = s.n_longitude,
                        Latitude = s.n_latitude,
                        Disabled = s.b_disabled
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет подразделение
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertDepartment(Department item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = item.Id,
                        PageName = item.Title,
                        Section = LogModule.Departments,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    bool result = db.lk_departments.Insert(() => new lk_departments
                    {
                        id = item.Id,
                        c_title = item.Title,
                        c_address = item.Address,
                        c_work_time = item.WorkTime,
                        n_longitude = (decimal)item.Longitude,
                        n_latitude = (decimal)item.Latitude,
                        b_disabled = item.Disabled, 
                        f_site = _siteId
                    }) > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновляет подразделение
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateDepartment(Department item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = item.Id,
                        PageName = item.Title,
                        Section = LogModule.Departments,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.lk_departments
                        .Where(w => w.id == item.Id)
                        .Set(s => s.c_title, item.Title)
                        .Set(s => s.c_address, item.Address)
                        .Set(s => s.c_work_time, item.WorkTime)
                        .Set(s => s.n_longitude, (decimal)item.Longitude)
                        .Set(s => s.n_latitude, (decimal)item.Latitude)
                        .Set(s => s.b_disabled, item.Disabled)
                        .Update() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Проверяет существование подразделения
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckDepartmentExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_departments
                    .Where(w => w.id == id).Any();
            }
        }

        /// <summary>
        /// Удаляет подразделение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteDepartment(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = false;
                    var item = db.lk_departments.Where(w => w.id == id).SingleOrDefault();
                    if (item != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = item.c_title,
                            Section = LogModule.Departments,
                            Action = LogAction.delete
                        };
                        InsertLog(log, item);

                        result = db.Delete(item) > 0;
                    }

                    tr.Commit();
                    return result;
                }
            }
        }
    }
}
