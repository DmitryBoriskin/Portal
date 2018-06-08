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
        #region Лицевые счета

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
        public Subscr[] GetSubscrs(Guid user)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscrs
                    .Where(w => w.fkdepartments.f_site == _siteId)
                    .Where(w => !w.fkusersubscrs.Any(a => a.f_user == user))
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
        public bool AddUserSubscr(Guid user, Guid[] subscrs)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    try
                    {
                        var listExistsSubscrs = db.lk_user_subscrs
                            .Where(w => w.f_user == user)
                            .Select(s => s.f_subscr)
                            .ToArray();

                        List<lk_user_subscrs> list = new List<lk_user_subscrs>();
                        foreach (var subscr in subscrs)
                        {
                            if (!listExistsSubscrs.Contains(subscr))
                            {
                                list.Add(new lk_user_subscrs
                                {
                                    f_user = user,
                                    f_subscr = subscr,
                                    d_attached = DateTime.Now
                                });
                            }
                        }
                        db.BulkCopy(list);

                        tr.Commit();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Удаляет связь пользователя с ЛС
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool DropUserSubscr(Guid id, Guid user)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = db.lk_user_subscrs
                        .Where(w => w.f_subscr == id)
                        .Where(w => w.f_user == user)
                        .Delete() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Возвращает список прикреплённых ЛС
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Subscr[] GetSelectedSubscrs(Guid user)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_user_subscrs
                    .Where(w => w.f_user == user)
                    .Select(s => new Subscr
                    {
                        Id = s.f_subscr,
                        Link = s.fkusersubscrssubscr.c_link,
                        Surname = s.fkusersubscrssubscr.c_surname,
                        Name = s.fkusersubscrssubscr.c_name,
                        Patronymic = s.fkusersubscrssubscr.c_patronymic
                    }).ToArray();
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

        #endregion

        #region Подразделения

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

        #endregion

        #region Выставленные счета

        /// <summary>
        /// Возвращает список выставленных счетов для пользователя
        /// </summary>
        /// <param name="subscr"></param>
        /// <returns></returns>
        public Paged<Charge> GetCharges(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<Charge> result = new Paged<Charge>();
                var query = db.lk_charges
                    .Where(w => w.f_subscr == subscr);

                if (filter.Payed.HasValue)
                {
                    query = query.Where(w => w.b_payed == filter.Payed.Value);
                }

                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new Charge
                    {
                        Id = s.id,
                        Date = s.d_date,
                        Debt = (decimal)s.n_debt,
                        Payed = s.b_payed
                    }).ToArray();

                return new Paged<Charge>
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

        #endregion

        #region Приборы учёта

        /// <summary>
        /// Возвращает список приборов учёта
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<MeterDevice> GetMeterDevices(Guid subscr, FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<MeterDevice> result = new Paged<MeterDevice>();
                var query = db.lk_meter_devices
                    .Where(w => w.f_subscr == subscr);

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
                }

                query = query.OrderByDescending(o => o.d_install);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new MeterDevice
                    {
                        Number = s.c_number,
                        Mark = s.c_mark,
                        InstallDate = s.d_install,
                        Disabled = s.b_disabled
                    }).ToArray();

                return new Paged<MeterDevice>
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

        #endregion
    }
}
