﻿using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.front
{
    /// <summary>
    /// Репозиторий для работы с личным кабинетом
    /// </summary>
    public partial class FrontRepository
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
                    query = query.Where(w => (w.c_surname + " " + w.c_name + " " + w.c_patronymic).ToLower().Contains(text)

                                                      || w.c_org.ToLower().Contains(text)
                                                      || w.c_subscr.Contains(text));
                }

                query = query.OrderBy(o => new { o.c_surname, o.c_name, o.c_patronymic });

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Subscr = s.c_subscr,
                        Disabled = s.b_disabled,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic
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
        /// Возвращает личный кабинет
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
                        Link = s.c_link,
                        Ee = s.b_ee,
                        OrgName = s.c_org,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Address = s.c_address,
                        PostAddress = s.c_post_address,
                        Phone = s.c_phone,
                        Email = s.c_email,
                        Disabled = s.b_disabled,
                        Department = s.f_department,
                        Contract = s.c_contract,
                        ContractDate = s.d_contract_date,
                        Begin = s.d_begin,
                        End = s.d_end,
                        Bank = new BankModel()
                        {
                            Name = s.c_bank_name,
                            Dep = s.c_bank_dep,
                            Bik = s.c_bank_bik,
                            Kpp = s.c_bank_kpp,
                            Inn = s.c_bank_inn,
                            Ks = s.c_bank_ks,
                            Rs = s.c_bank_rs
                        }
                    })
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Возвращает список ЛС для привязки к пользователю
        /// </summary>
        /// <returns></returns>
        public SubscrModel[] GetSubscrs(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_subscrs
                    .Where(w => w.fkdepartments.f_site == _siteId)
                    .Where(w => !w.fkusersubscrs.Any(a => a.f_user == userId))
                    .OrderBy(o => o.c_link)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Link = s.c_link,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic
                    })
                    .ToArray();
            }
        }


        /// <summary>
        /// Информация о ЛС по умолчанию
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SubscrModel GetUserSubscrDefault(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_user_subscrs
                               .Where(w => w.f_user == userId && w.b_default == true)
                               .Join(db.lk_subscrs, u => u.f_subscr, s => s.id, (u, s) => s);
                if (query.Any())
                {
                    return query.Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Subscr = s.c_subscr,
                        Link = s.c_link,
                        Ee = s.b_ee,
                        Default = true,
                        OrgName = s.c_org,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Address = s.c_address,
                        PostAddress = s.c_post_address,
                        Phone = s.c_phone,
                        Email = s.c_email,
                        Disabled = s.b_disabled,
                        Department = s.f_department,
                        Contract = s.c_contract,
                        ContractDate = s.d_contract_date,
                        Begin = s.d_begin,
                        End = s.d_end,
                        Bank = new BankModel()
                        {
                            Name = s.c_bank_name,
                            Dep = s.c_bank_dep,
                            Bik = s.c_bank_bik,
                            Kpp = s.c_bank_kpp,
                            Inn = s.c_bank_inn,
                            Ks = s.c_bank_ks,
                            Rs = s.c_bank_rs
                        }

                    }).Single();
                }
                return null;
            }
        }

        /// <summary>
        /// Информация о ЛС по умолчанию
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SubscrShortModel> GetSubscrInfoForTopPannel(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_cv_subscr_saldo
                        .Where(s => s.userid == userId && s.siteid == _siteId);

                return query.Select(s => new SubscrShortModel
                {
                    Id = s.userid,
                    SubscrUid = s.subscruid,
                    SubscrId = s.subscrid,
                    Name = s.subscrname,
                    Default = s.subscrdefault,
                    Debt = s.subscrdebit,
                    Disabled = s.subscrdisabled

                }).ToList();
            }
        }



        /// <summary>
        /// выбираем лицевой счет
        /// </summary>
        /// <param name="subscrId">ид лицевого счета</param>
        /// <param name="userId">ид  пользователя</param>
        /// <returns></returns>
        public bool SetUserSubscrDefault(Guid subscrId, Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    //находим все подключенные лицевые счета пользователя
                    var userSubscrs = db.lk_user_subscrs
                                        .Where(w => w.f_user == userId);


                    if (userSubscrs.Any())
                    {
                        //убираем признак выбранности у выбранного ЛС
                        userSubscrs.Where(w => w.b_default == true)
                            .Set(s => s.b_default, false)
                            .Update();


                        // ставим признак выбранности на другой ЛС
                        userSubscrs.Where(w => w.f_subscr == subscrId)
                            .Set(s => s.b_default, true)
                            .Update();

                        tr.Commit();
                        return true;
                    }
                }
            }
            return false;
        }


        ///// <summary>
        ///// Создаёт лицевой счёт
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public bool InsertSubscr(SubscrModel item)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tran = db.BeginTransaction())
        //        {
        //            var dbSubscr = db.lk_subscrs
        //              .Where(s => s.id == item.Id || s.c_subscr == item.Subscr);

        //            if (!dbSubscr.Any())
        //            {
        //                var subscr = new lk_subscrs()
        //                {
        //                    id = item.Id,
        //                    c_subscr = item.Subscr,

        //                    b_disabled = item.Disabled,
        //                    b_ee = item.Ee,

        //                    c_address = item.Address,
        //                    c_post_address = item.PostAddress,

        //                    c_contract = item.Contract,
        //                    d_contract_date = item.ContractDate,
        //                    d_begin = item.Begin,
        //                    d_end = item.End,
        //                    c_link = item.Link
        //                };


        //                if (item.Ee)
        //                {
        //                    subscr.c_org = item.OrgName;
        //                    if (item.Bank != null)
        //                    {
        //                        subscr.c_bank_name = item.Bank.Name;
        //                        subscr.c_bank_dep = item.Bank.Dep;
        //                        subscr.c_bank_bik = item.Bank.Bik;
        //                        subscr.c_bank_kpp = item.Bank.Kpp;
        //                        subscr.c_bank_inn = item.Bank.Inn;
        //                        subscr.c_bank_ks = item.Bank.Ks;
        //                        subscr.c_bank_rs = item.Bank.Rs;
        //                    };
        //                }
        //                else
        //                {
        //                    subscr.c_surname = item.Surname;
        //                    subscr.c_name = item.Name;
        //                    subscr.c_patronymic = item.Patronymic;
        //                }


        //                db.Insert(subscr);

        //                var log = new LogModel
        //                {
        //                    PageId = item.Id,
        //                    PageName = $"{item.Surname} {item.Name} {item.Patronymic}",
        //                    Section = LogModule.Subscrs,
        //                    Action = LogAction.insert
        //                };
        //                InsertLog(log);

        //                tran.Commit();
        //                return true;

        //            }

        //            return false;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Обновляет лицевой счёт
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public bool UpdateSubscr(SubscrModel item)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tran = db.BeginTransaction())
        //        {
        //            var log = new LogModel
        //            {
        //                PageId = item.Id,
        //                PageName = $"{item.Surname} {item.Name} {item.Patronymic}",
        //                Section = LogModule.Subscrs,
        //                Action = LogAction.update
        //            };
        //            InsertLog(log);

        //            var dbSubscr = db.lk_subscrs
        //                .Where(s => s.id == item.Id);

        //            if (dbSubscr.Any())
        //            {
        //                var subscr = dbSubscr.Single();

        //                subscr.c_subscr = item.Subscr;

        //                subscr.b_disabled = item.Disabled;
        //                subscr.b_ee = item.Ee;

        //                if (item.Ee)
        //                {
        //                    subscr.c_org = item.OrgName;
        //                }
        //                else
        //                {
        //                    subscr.c_surname = item.Surname;
        //                    subscr.c_name = item.Name;
        //                    subscr.c_patronymic = item.Patronymic;
        //                }

        //                subscr.c_address = item.Address;
        //                subscr.c_post_address = item.PostAddress;

        //                subscr.c_contract = item.Contract;
        //                subscr.d_contract_date = item.ContractDate;
        //                subscr.d_begin = item.Begin;
        //                subscr.d_end = item.End;
        //                subscr.c_link = item.Link;

        //                if (item.Ee && item.Bank != null)
        //                {
        //                    subscr.c_bank_name = item.Bank.Name;
        //                    subscr.c_bank_dep = item.Bank.Dep;
        //                    subscr.c_bank_bik = item.Bank.Bik;
        //                    subscr.c_bank_kpp = item.Bank.Kpp;
        //                    subscr.c_bank_inn = item.Bank.Inn;
        //                    subscr.c_bank_ks = item.Bank.Ks;
        //                    subscr.c_bank_rs = item.Bank.Rs;
        //                }

        //                db.Update(subscr);
        //                tran.Commit();
        //                return true;
        //            }

        //            return false;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Прикрепляет лицевые счета к пользователю
        ///// </summary>
        ///// <param name="user"></param>
        ///// <param name="subscrs"></param>
        ///// <returns></returns>
        //public bool AddUserSubscr(Guid user, Guid[] subscrs)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            try
        //            {
        //                var listExistsSubscrs = db.lk_user_subscrs
        //                    .Where(w => w.f_user == user)
        //                    .Select(s => new { s.f_subscr, s.b_default })
        //                    .ToArray();

        //                List<lk_user_subscrs> list = new List<lk_user_subscrs>();
        //                int count = 0;
        //                foreach (var subscr in subscrs)
        //                {
        //                    if (!listExistsSubscrs.Select(s => s.f_subscr).Contains(subscr))
        //                    {
        //                        list.Add(new lk_user_subscrs
        //                        {
        //                            f_user = user,
        //                            f_subscr = subscr,
        //                            d_attached = DateTime.Now,
        //                            b_default = !listExistsSubscrs.Any(a => a.b_default) && count == 0
        //                        });
        //                        count++;
        //                    }
        //                }
        //                db.BulkCopy(list);

        //                tr.Commit();
        //                return true;
        //            }
        //            catch
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Удаляет связь пользователя с ЛС
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public bool DropUserSubscr(Guid id, Guid user)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            var query = db.lk_user_subscrs
        //                .Where(w => w.f_subscr == id)
        //                .Where(w => w.f_user == user);

        //            var link = query.SingleOrDefault();

        //            bool result = query.Delete() > 0;

        //            if (link.b_default)
        //            {
        //                var newDefault = db.lk_user_subscrs
        //                    .Where(w => w.f_user == user)
        //                    .FirstOrDefault();

        //                if (newDefault != null)
        //                {
        //                    result = db.lk_user_subscrs
        //                        .Where(w => w.f_user == user)
        //                        .Where(w => w.f_subscr == newDefault.f_subscr)
        //                        .Set(s => s.b_default, true)
        //                        .Update() > 0;
        //                }
        //            }
        //            tr.Commit();
        //            return result;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Выставляет дефолтный ЛС для пользователя
        ///// </summary>
        ///// <param name="subscr"></param>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public bool SetDefaultUserSubscrLink(Guid subscr, Guid user)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            var query = db.lk_user_subscrs
        //                .Where(w => w.f_user == user);

        //            bool result = query
        //                .Set(s => s.b_default, false)
        //                .Update() > 0;

        //            result = query
        //                .Where(w => w.f_subscr == subscr)
        //                .Set(s => s.b_default, true)
        //                .Update() > 0;

        //            tr.Commit();
        //            return result;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Возвращает список прикреплённых ЛС
        ///// </summary>
        ///// <param name="user"></param>
        ///// <returns></returns>
        //public SubscrModel[] GetSelectedSubscrs(Guid user)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        return db.lk_user_subscrs
        //            .Where(w => w.f_user == user)
        //            .OrderBy(o => o.d_attached)
        //            .Select(s => new SubscrModel
        //            {
        //                Id = s.f_subscr,
        //                Link = s.fkusersubscrssubscr.c_link,
        //                Surname = s.fkusersubscrssubscr.c_surname,
        //                Name = s.fkusersubscrssubscr.c_name,
        //                Patronymic = s.fkusersubscrssubscr.c_patronymic,
        //                Default = s.b_default
        //            }).ToArray();
        //    }
        //}

        ///// <summary>
        ///// Проверяет существование лицевого счёта
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool CheckSubscrExists(Guid id)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        return db.lk_subscrs
        //            .Where(w => w.id == id).Any();
        //    }
        //}

        ///// <summary>
        ///// Удаляет лицевой счёт
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool DeleteSubscr(Guid id)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            var item = db.lk_subscrs
        //                .Where(w => w.id == id)
        //                .SingleOrDefault();

        //            if (item != null)
        //            {
        //                var log = new LogModel
        //                {
        //                    PageId = id,
        //                    PageName = $"{item.c_surname} {item.c_name} {item.c_patronymic}",
        //                    Section = LogModule.Subscrs,
        //                    Action = LogAction.delete
        //                };

        //                InsertLog(log, item);
        //                db.Delete(item);
        //                tr.Commit();
        //                return true;
        //            }

        //            return false;
        //        }
        //    }
        //}

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

        ///// <summary>
        ///// Добавляет подразделение
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public bool InsertDepartment(DepartmentModel item)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            var log = new LogModel
        //            {
        //                PageId = item.Id,
        //                PageName = item.Title,
        //                Section = LogModule.Departments,
        //                Action = LogAction.insert
        //            };
        //            InsertLog(log);

        //            bool result = db.lk_departments.Insert(() => new lk_departments
        //            {
        //                id = item.Id,
        //                c_title = item.Title,
        //                c_address = item.Address,
        //                c_work_time = item.WorkTime,
        //                n_longitude = (decimal)item.Longitude,
        //                n_latitude = (decimal)item.Latitude,
        //                b_disabled = item.Disabled,
        //                f_site = _siteId
        //            }) > 0;

        //            tr.Commit();
        //            return result;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Обновляет подразделение
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public bool UpdateDepartment(DepartmentModel item)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            var log = new LogModel
        //            {
        //                PageId = item.Id,
        //                PageName = item.Title,
        //                Section = LogModule.Departments,
        //                Action = LogAction.update
        //            };
        //            InsertLog(log);

        //            bool result = db.lk_departments
        //                .Where(w => w.id == item.Id)
        //                .Set(s => s.c_title, item.Title)
        //                .Set(s => s.c_address, item.Address)
        //                .Set(s => s.c_work_time, item.WorkTime)
        //                .Set(s => s.n_longitude, (decimal)item.Longitude)
        //                .Set(s => s.n_latitude, (decimal)item.Latitude)
        //                .Set(s => s.b_disabled, item.Disabled)
        //                .Update() > 0;

        //            tr.Commit();
        //            return result;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Проверяет существование подразделения
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool CheckDepartmentExists(Guid id)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        return db.lk_departments
        //            .Where(w => w.id == id).Any();
        //    }
        //}

        ///// <summary>
        ///// Удаляет подразделение
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool DeleteDepartment(Guid id)
        //{
        //    using (var db = new CMSdb(_context))
        //    {
        //        using (var tr = db.BeginTransaction())
        //        {
        //            bool result = false;
        //            var item = db.lk_departments.Where(w => w.id == id).SingleOrDefault();
        //            if (item != null)
        //            {
        //                var log = new LogModel
        //                {
        //                    PageId = id,
        //                    PageName = item.c_title,
        //                    Section = LogModule.Departments,
        //                    Action = LogAction.delete
        //                };
        //                InsertLog(log, item);

        //                result = db.Delete(item) > 0;
        //            }

        //            tr.Commit();
        //            return result;
        //        }
        //    }
        //}

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
                    .Select(s => new AccrualModel
                    {
                        Id = s.id,
                        Date = s.d_date,
                        Debt = (decimal)s.n_debt,
                        Payed = s.b_payed
                    }).ToArray();

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
                return db.lk_charges
                    .Where(w => w.id == id)
                    .Select(s => new AccrualModel
                    {
                        Id = s.id,
                        Date = s.d_date,
                        Debt = (decimal)s.n_debt,
                        Payed = s.b_payed
                    }).SingleOrDefault();
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
                return db.lk_charges
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
                var query = db.lk_meter_devices
                    .Where(w => w.f_subscr == subscr);

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
                }

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
                        Disabled = s.b_disabled,
                        DeviceInfo = (s.f_device_type != null) ?
                                new DeviceModel()
                                {
                                    Name = s.fkmeterdevicedevicetypes.c_name,
                                    Tariff = s.fkmeterdevicedevicetypes.n_tariff,
                                    Modification = s.fkmeterdevicedevicetypes.c_modification,
                                    Manufactirer = s.fkmeterdevicedevicetypes.c_manufacturer,
                                    Phase3 = s.fkmeterdevicedevicetypes.b_phase3,
                                    DeviceCategory = s.fkmeterdevicedevicetypes.c_device_category,
                                    EnergyCategory = s.fkmeterdevicedevicetypes.c_energy_category,
                                    PrecissionClass = s.fkmeterdevicedevicetypes.c_precission_class,
                                    VoltageNominal = s.fkmeterdevicedevicetypes.c_voltage_nominal
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
                    .Where(w => w.f_meter_device == device)
                    //.OrderBy(o => o.d_send)
                    .Take(20)
                    .Select(s => new MeterModel
                    {
                        Id = s.id,
                        Send = s.d_send,
                        Output = s.n_output,
                        DrawlType = new GroupsModel
                        {
                            Id = s.f_drawl_type,
                            //Title = s.fkmeterdrawltypes.c_title
                        }
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

                if (!String.IsNullOrWhiteSpace(filter.Status))
                {
                    Guid status = Guid.Parse(filter.Status);
                    query = query.Where(w => w.f_status == status);
                }
                if (!String.IsNullOrWhiteSpace(filter.Type))
                {
                    Guid type = Guid.Parse(filter.Type);
                    query = query.Where(w => w.f_type == type);
                }

                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new PaymentModel
                    {
                        Date = s.d_date,
                        Amount = (decimal)s.n_amount,
                        Status = new GroupsModel
                        {
                            Id = s.fkpaymentstatuses.id,
                            Title = s.fkpaymentstatuses.c_title
                        },
                        Type = new GroupsModel
                        {
                            Id = s.fkpaymenttypes.id,
                            Title = s.fkpaymenttypes.c_title
                        }
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

        /// <summary>
        /// Возвращает список статусов по платежам
        /// </summary>
        /// <returns></returns>
        public GroupsModel[] GetPaymentStatuses()
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_payment_statuses
                    .OrderBy(o => o.c_title)
                    .Select(s => new GroupsModel
                    {
                        Id = s.id,
                        Title = s.c_title
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает список типов платежей
        /// </summary>
        /// <returns></returns>
        public GroupsModel[] GetPaymentTypes()
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_payment_types
                    .OrderBy(o => o.c_title)
                    .Select(s => new GroupsModel
                    {
                        Id = s.id,
                        Title = s.c_title
                    }).ToArray();
            }
        }

        #endregion

        #region Тарифы

        /// <summary>
        /// Возвращает список тарифов ПУ
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public TariffModel[] GetTariffes(Guid device)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_tariffes
                    .Where(w => w.fkmeterdevices.Any(a => a.f_device == device))
                    .Select(s => new TariffModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Begin = s.d_begin,
                        End = s.d_end,
                        Disabled = s.b_disabled,
                        Values = s.fktariffvaluess
                            .Select(t => new TariffValueModel
                            {
                                Id = t.id,
                                Title = t.c_title,
                                Price = t.n_price
                            }).ToArray()
                    }).ToArray();
            }
        }

        #endregion
    }
}
