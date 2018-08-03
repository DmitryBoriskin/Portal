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
        public Paged<SubscrModel> GetSubscrs(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<SubscrModel> result = new Paged<SubscrModel>();
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
                    var text = filter.SearchText.ToLower();
                    query = query.Where(w => w.c_name.ToLower().Contains(text)
                                                      || w.c_subscr.Contains(text));
                }

                query = query.OrderBy(o => new { o.c_name });

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Subscr = s.c_subscr,
                        Name = s.c_name,
                    }).ToArray();

                return new Paged<SubscrModel>
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
        public SubscrModel[] GetSubscrs(Guid user)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscrs
                    .Where(w => w.fkdepartments.f_site == _siteId)
                    .Where(w => !w.fkusersubscrs.Any(a => a.f_user == user))
                    .OrderBy(o => o.link)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Link = s.link,
                        Name = s.c_name,
                        Subscr = s.c_subscr
                    })
                    .ToArray();
            }
        }

        /// <summary>
        /// Возвращает лицевой счет
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SubscrModel GetSubscr(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscrs
                    .Where(w => w.id == id)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Subscr = s.c_subscr,
                        Link = s.link,
                        Ee = s.b_ee,
                        Inn = s.c_inn,
                        Kpp = s.c_kpp,
                        Name = s.c_name,
                        Address = s.c_address,
                        PostAddress = s.c_post_address,
                        Phone = s.c_phone,
                        Email = s.c_email,
                        Department = s.f_department,
                        Contract = s.c_contract,
                        ContractDate = s.d_contract_date,
                        Begin = s.d_contract_begin,
                        End = s.d_contract_end,
                        Bank = new BankModel()
                        {
                            Name = s.c_bank_name,
                            Dep = s.c_bank_dep,
                            Bik = s.c_bank_bik,
                            Inn = s.c_bank_inn,
                            Ks = s.c_bank_ks,
                            Rs = s.c_bank_rs
                        }
                    })
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Возвращает личный кабинет
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SubscrConfigs GetSubscrConfigs(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscr_configs
                    .Where(w => w.f_subscr == id)
                    .Select(s => new SubscrConfigs
                    {
                        EDO = s.c_edo_link,
                        Manager = new SubscrManager()
                        {
                            Id = Guid.NewGuid(),
                            FIO = "",
                            Phone = ""
                        }
                    })
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Создаёт лицевой счёт
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool InsertSubscr(SubscrModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var dbSubscr = db.lk_subscrs
                      .Where(s => s.id == item.Id || s.c_subscr == item.Subscr);

                    if (!dbSubscr.Any())
                    {
                        var subscr = new lk_subscrs()
                        {
                            id = item.Id,
                            c_subscr = item.Subscr,
                            b_ee = item.Ee,

                            c_address = item.Address,
                            c_post_address = item.PostAddress,

                            c_contract = item.Contract,
                            d_contract_date = item.ContractDate,
                            d_contract_begin = item.Begin,
                            d_contract_end = item.End,
                            link = item.Link
                        };


                        subscr.c_name = item.Name;
                        if (item.Bank != null)
                        {
                            subscr.c_bank_name = item.Bank.Name;
                            subscr.c_bank_dep = item.Bank.Dep;
                            subscr.c_bank_bik = item.Bank.Bik;
                            subscr.c_bank_inn = item.Bank.Inn;
                            subscr.c_bank_ks = item.Bank.Ks;
                            subscr.c_bank_rs = item.Bank.Rs;
                        };

                        db.Insert(subscr);

                        if (item.Configs != null)
                        {
                            var configs = new lk_subscr_configs()
                            {
                                id = Guid.NewGuid(),
                                f_subscr = item.Id,
                                c_subscr = item.Subscr,
                                c_edo_link = item.Configs.EDO
                            };

                            if (item.Configs.Manager != null)
                                configs.f_manager = item.Configs.Manager.Id;

                            db.Insert(configs);
                        }


                        var log = new LogModel
                        {
                            PageId = item.Id,
                            PageName = $"{item.Name}",
                            Section = LogModule.Subscrs,
                            Action = LogAction.insert
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;

                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Обновляет лицевой счёт
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool UpdateSubscr(SubscrModel item)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var dbSubscr = db.lk_subscrs
                        .Where(s => s.id == item.Id);

                    if (dbSubscr.Any())
                    {
                        var subscr = dbSubscr.Single();

                        subscr.c_subscr = item.Subscr;

                        subscr.b_ee = item.Ee;
                        subscr.c_name = item.Name;
                        subscr.c_inn = item.Inn;
                        subscr.c_kpp = item.Kpp;

                        subscr.c_address = item.Address;
                        subscr.c_post_address = item.PostAddress;

                        subscr.c_contract = item.Contract;
                        subscr.d_contract_date = item.ContractDate;
                        subscr.d_contract_begin = item.Begin;
                        subscr.d_contract_end = item.End;
                        subscr.link = item.Link;

                        if (item.Ee && item.Bank != null)
                        {
                            subscr.c_bank_name = item.Bank.Name;
                            subscr.c_bank_dep = item.Bank.Dep;
                            subscr.c_bank_bik = item.Bank.Bik;
                            subscr.c_bank_inn = item.Bank.Inn;
                            subscr.c_bank_ks = item.Bank.Ks;
                            subscr.c_bank_rs = item.Bank.Rs;
                        }
                        db.Update(subscr);

                        if (item.Configs != null)
                        {
                            var configs = new lk_subscr_configs();

                            var configsData = db.lk_subscr_configs
                                .Where(p => p.f_subscr == item.Id);

                            if (configsData.Any())
                            {
                                configs = configsData.Single();
                                configs.c_edo_link = item.Configs.EDO;

                                if (item.Configs.Manager != null)
                                    configs.f_manager = item.Configs.Manager.Id;

                                db.Update(configs);
                            }
                            else
                            {
                                configs = new lk_subscr_configs()
                                {
                                    id = Guid.NewGuid(),
                                    f_subscr = item.Id,
                                    c_subscr = item.Subscr,
                                    c_edo_link = item.Configs.EDO
                                };

                                if (item.Configs.Manager != null)
                                    configs.f_manager = item.Configs.Manager.Id;

                                db.Insert(configs);
                            }
                        }

                        var log = new LogModel
                        {
                            PageId = item.Id,
                            PageName = $"{item.Name}",
                            Section = LogModule.Subscrs,
                            Action = LogAction.update
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
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
                            .Select(s => new { s.f_subscr, s.b_default })
                            .ToArray();

                        List<lk_user_subscrs> list = new List<lk_user_subscrs>();
                        int count = 0;
                        foreach (var subscr in subscrs)
                        {
                            if (!listExistsSubscrs.Select(s => s.f_subscr).Contains(subscr))
                            {
                                list.Add(new lk_user_subscrs
                                {
                                    f_user = user,
                                    f_subscr = subscr,
                                    d_attached = DateTime.Now,
                                    b_default = !listExistsSubscrs.Any(a => a.b_default) && count == 0
                                });
                                count++;
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
                    var query = db.lk_user_subscrs
                        .Where(w => w.f_subscr == id)
                        .Where(w => w.f_user == user);

                    var link = query.SingleOrDefault();

                    bool result = query.Delete() > 0;

                    if (link.b_default)
                    {
                        var newDefault = db.lk_user_subscrs
                            .Where(w => w.f_user == user)
                            .FirstOrDefault();

                        if (newDefault != null)
                        {
                            result = db.lk_user_subscrs
                                .Where(w => w.f_user == user)
                                .Where(w => w.f_subscr == newDefault.f_subscr)
                                .Set(s => s.b_default, true)
                                .Update() > 0;
                        }
                    }
                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Выставляет дефолтный ЛС для пользователя
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SetDefaultUserSubscrLink(Guid subscr, Guid user)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query = db.lk_user_subscrs
                        .Where(w => w.f_user == user);

                    if (query.Any())
                    {
                        var result = query
                           .Set(s => s.b_default, false)
                           .Update();

                        var res = query
                            .Where(w => w.f_subscr == subscr)
                            .Set(s => s.b_default, true)
                            .Update();

                        tr.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Возвращает список прикреплённых ЛС
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SubscrModel[] GetSelectedSubscrs(Guid user)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_user_subscrs
                    .Where(w => w.f_user == user)
                    .OrderBy(o => o.d_attached)
                    .Select(s => new SubscrModel
                    {
                        Id = s.f_subscr,
                        Link = s.fkusersubscrssubscr.link,
                        Name = s.fkusersubscrssubscr.c_name,
                        Default = s.b_default
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
                    var item = db.lk_subscrs
                        .Where(w => w.id == id)
                        .SingleOrDefault();

                    if (item != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = $"{item.c_name}",
                            Section = LogModule.Subscrs,
                            Action = LogAction.delete
                        };

                        InsertLog(log, item);
                        db.Delete(item);
                        tr.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        #endregion

        #region Персональный менеджер

        /// <summary>
        /// Возвращает список подразделений для выпадающего списка
        /// </summary>
        /// <returns></returns>
        public GroupsModel[] GetManagers()
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_managers
                    .Where(w => w.f_site == _siteId)
                    .Select(s => new GroupsModel
                    {
                        Id = s.id,
                        Title = s.c_name
                    }).ToArray();
            }
        }
        #endregion


        #region Подразделения

        /// <summary>
        /// Возвращает список подразделений
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<DepartmentModel> GetDepartments(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<DepartmentModel> result = new Paged<DepartmentModel>();
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
                    .Select(s => new DepartmentModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Address = s.c_address,
                        WorkTime = s.c_work_time,
                        Longitude = s.n_longitude,
                        Latitude = s.n_latitude,
                        Disabled = s.b_disabled
                    }).ToArray();

                return new Paged<DepartmentModel>
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
        public DepartmentModel GetDepartment(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_departments
                    .Where(w => w.id == id)
                    .Select(s => new DepartmentModel
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
        public bool InsertDepartment(DepartmentModel item)
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
        public bool UpdateDepartment(DepartmentModel item)
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
        public Paged<AccrualModel> GetAccruals(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<AccrualModel> result = new Paged<AccrualModel>();
                var query = db.lk_accruals
                    .Where(w => w.f_subscr == subscr);

                if (filter.Payed.HasValue)
                {
                    query = query.Where(w => w.b_closed == filter.Payed.Value);
                }

                if (filter.Date.HasValue)
                {
                    query = query.Where(w => w.d_date >= filter.Date.Value);
                }
                if (filter.DateEnd.HasValue)
                {
                    query = query.Where(w => w.d_date <= filter.DateEnd.Value.AddDays(1));
                }

                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new AccrualModel
                    {

                        Id = s.id,
                        Date = s.d_date,
                        Period = s.n_period,
                        Payed = s.b_closed,
                        Status = s.c_status,
                        Number = s.c_number,
                        Amount = s.n_amount,
                        Tax = s.n_tax,
                        Cons = s.n_cons,
                        Quantity = s.n_quantity,
                        Quantity2 = s.n_quantity2,
                        DebtType = s.c_debt,
                        DocType = s.c_doctype,
                        PaymentId = s.n_payment
                    })
                    .ToArray();

                return new Paged<AccrualModel>
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
        /// Возвращает выставленный счёт
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AccrualModel GetAccrual(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_accruals
                    .Where(w => w.id == id)
                    .Select(s => new AccrualModel
                    {

                        Id = s.id,
                        Date = s.d_date,
                        Period = s.n_period,
                        Payed = s.b_closed,
                        Status = s.c_status,
                        Number = s.c_number,
                        Amount = s.n_amount,
                        Tax = s.n_tax,
                        Cons = s.n_cons,
                        Quantity = s.n_quantity,
                        Quantity2 = s.n_quantity2,
                        DebtType = s.c_debt,
                        DocType = s.c_doctype,
                        PaymentId = s.n_payment
                    })
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Возвращает ЛС по выставленному счёту
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Guid GetSubscrByAccrual(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_accruals
                    .Where(w => w.id == id)
                    .Select(s => s.f_subscr)
                    .SingleOrDefault();
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
        public Paged<PuModel> GetSubscrDevices(Guid subscr, FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<PuModel> result = new Paged<PuModel>();
                var query = db.lk_subscr_devices
                    .Where(w => w.f_subscr == subscr);

                query = query.OrderByDescending(o => o.d_install);
                int itemsCount = query.Count();

                var list = query
                    .Select(s => new PuModel
                    {
                        Id = s.id,
                        Number = s.c_number,
                        Name = s.c_name,
                        InstallPlace = s.c_install_place,
                        InstallDate = s.d_install,
                        CheckDate = s.d_check,
                        Multiplier = s.n_factor,
                        Tariff = s.n_tariff,
                        DeviceInfo = (s.f_device != null) ?
                                new DeviceModel()
                                {
                                    Name = s.fksubscrdevicesdevices.c_name,
                                    Tariff = s.fksubscrdevicesdevices.n_tariff,
                                    Modification = s.fksubscrdevicesdevices.c_modification,
                                    Manufactirer = s.fksubscrdevicesdevices.c_manufacturer,
                                    Phase3 = s.fksubscrdevicesdevices.b_phase3,
                                    DeviceCategory = s.fksubscrdevicesdevices.c_device_category,
                                    EnergyCategory = s.fksubscrdevicesdevices.c_energy_category,
                                    PrecissionClass = s.fksubscrdevicesdevices.c_precission_class,
                                    VoltageNominal = s.fksubscrdevicesdevices.c_voltage_nominal
                                }
                                : null

                    }).ToArray();

                return new Paged<PuModel>
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

        #region Показания ПУ

        /// <summary>
        /// Возвращает показание по ПУ
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public MeterModel[] GetMeters(Guid device)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_meters
                    .Where(w => w.f_device == device)
                    .OrderByDescending(o => o.d_date)
                    .Take(100)
                    .Select(s => new MeterModel
                    {
                        Id = s.id,
                        Date = s.d_date,
                        DatePrev = s.d_date_prev,
                        Value = s.n_value,
                        Const = s.n_cons,
                        Quantity = s.n_quantity,
                        Year = s.n_year,
                        Month = s.n_month,
                        Days = s.n_days,
                        DeliveryMethod = s.c_delivery_method,

                    }).ToArray();
            }
        }

        #endregion

        #region Платежи

        /// <summary>
        /// Возвращает список платежей
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<PaymentModel> GetPayments(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<PaymentModel> result = new Paged<PaymentModel>();
                var query = db.lk_payments
                    .Where(w => w.f_subscr == subscr);

                //if (!String.IsNullOrWhiteSpace(filter.Status))
                //{
                //    Guid status = Guid.Parse(filter.Status);
                //    query = query.Where(w => w.c_status == status);
                //}
                //if (!String.IsNullOrWhiteSpace(filter.Type))
                //{
                //    Guid type = Guid.Parse(filter.Type);
                //    query = query.Where(w => w.f_type == type);
                //}

                if (filter.Date.HasValue)
                {
                    query = query.Where(w => w.d_date >= filter.Date.Value);
                }
                if (filter.DateEnd.HasValue)
                {
                    query = query.Where(w => w.d_date <= filter.DateEnd.Value.AddDays(1));
                }

                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new PaymentModel
                    {
                        Id = s.id,
                        Date = s.d_date,
                        Amount = s.n_amount,
                        Period = s.n_period,
                    }).ToArray();

                return new Paged<PaymentModel>
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

        #region Тарифы

        /// <summary>
        /// Возвращает список тарифов ПУ
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        //public TariffModel[] GetTariffes(Guid device)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        return db.lk_tariffes
        //            .Where(w => w.fkmeterdevices.Any(a => a.f_device == device))
        //            .Select(s => new TariffModel
        //            {
        //                Id = s.id,
        //                Title = s.c_title,
        //                Begin = s.d_begin,
        //                End = s.d_end,
        //                Disabled = s.b_disabled,
        //                Values = s.fktariffvaluess
        //                    .Select(t => new TariffValueModel
        //                    {
        //                        Id = t.id,
        //                        Title = t.c_title,
        //                        Price = t.n_price
        //                    }).ToArray()
        //            }).ToArray();
        //    }
        //}

        #endregion
    }
}