using LinqToDB;
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
                    query = query.Where(w => w.c_name.ToLower().Contains(text)
                                                      || w.n_subscr.ToString().Contains(text));
                }

                query = query.OrderBy(o => new { o.c_name});

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Subscr = s.n_subscr,
                        Name = s.c_name
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
                    .Where(w => w.f_site == _siteId)
                    .Where(w => w.b_disabled == false)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Subscr = s.n_subscr,
                        Link = s.link,
                        Inn = s.c_inn,
                        Kpp = s.c_kpp,
                        Ee = s.b_ee,
                        //Default = s.fkusersubscrs.Where(p => p.b_default == true).Any() ? true : false,
                        Name = s.c_name,
                        NameShort = s.c_name_short,
                        Department = s.f_department,
                        Address = s.c_address,
                        PostAddress = s.c_post_address,
                        Phone = s.c_phone,
                        Email = s.c_email,
                        Fax = s.c_fax,
                        Site = s.c_site,

                        Begin = s.d_contract_begin,
                        End = s.d_contract_end,

                        ContractLink = s.n_contract,
                        Contract = s.c_contract,
                        ContractDate = s.d_contract_date,
                        ContractBegin = s.d_contract_begin,
                        ContractEnd = s.d_contract_end,

                        Bank = new BankModel()
                        {
                            Name = s.c_bank_name,
                            Bik = s.c_bank_bik,
                            Inn = s.c_bank_inn,
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
                    .OrderBy(o => o.link)
                    .Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Link = s.link,
                        Name = s.c_name,
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
                               .Where(w => w.fkusersubscrssubscr.f_site == _siteId && w.fkusersubscrssubscr.b_disabled == false)
                               .Join(db.lk_subscrs, u => u.f_subscr, s => s.id, (u, s) => s);
                if (query.Any())
                {
                    return query.Select(s => new SubscrModel
                    {
                        Id = s.id,
                        Subscr = s.n_subscr,
                        Link = s.link,
                        Inn = s.c_inn,
                        Kpp = s.c_kpp,
                        Ee = s.b_ee,
                        Default = true,
                        Name = s.c_name,
                        NameShort = s.c_name_short,
                        Department = s.f_department,
                        Address = s.c_address,
                        PostAddress = s.c_post_address,
                        Phone = s.c_phone,
                        Email = s.c_email,
                        Fax = s.c_fax,
                        Site = s.c_site,

                        Begin = s.d_contract_begin,
                        End = s.d_contract_end,

                        ContractLink = s.n_contract,
                        Contract = s.c_contract,
                        ContractDate = s.d_contract_date,
                        ContractBegin = s.d_contract_begin,
                        ContractEnd = s.d_contract_end,

                        Bank = new BankModel()
                        {
                            Name = s.c_bank_name,
                            Bik = s.c_bank_bik,
                            Inn = s.c_bank_inn,
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
        public List<SubscrShortModel> GetSubscrSaldoInfo(Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_cv_subscr_saldo
                        .Where(s => s.userid == userId && s.siteid == _siteId);
                query = query.OrderBy(o => o.subscrname);
                if (query.Any())
                {
                    return query.Select(s => new SubscrShortModel
                    {
                        Id = s.subscruid,
                        SubscrId = (long)s.subscrid,
                        Name = s.subscrname,
                        Default = s.subscrdefault,
                        Disabled = s.subscrdisabled,
                        Date = s.d_date,
                        Saldo = s.n_amount,
                        Peni = s.n_peny,
                        Percent = s.n_persent,
                        LastPaymentDate = s.d_lastpayment_date,
                        LastPaymentAmount = s.n_lastpayment_amount,
                        LastPaymentLink = s.c_lastpayment_link

                    }).ToList();
                }
                return null;
                
            }
        }



        /// <summary>
        /// Смена лицевого счета
        /// </summary>
        /// <param name="subscrId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool SetUserSubscrDefault(Guid subscrId, Guid userId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var query = db.lk_user_subscrs
                         .Where(w => w.f_user == userId);

                    if (query.Any())
                    {
                        var result = query
                           .Set(s => s.b_default, false)
                           .Update();

                        var res = query
                            .Where(w => w.f_subscr == subscrId)
                            .Set(s => s.b_default, true)
                            .Update();

                        tran.Commit();
                        return true;
                    }
                    return false;
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
        public Paged<InvoiceModel> GetAccruals(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<InvoiceModel> result = new Paged<InvoiceModel>();
                var query = db.lk_invoices
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
                    .Select(s => new InvoiceModel
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
                        //DebtType = s.c_debt,
                        //DocType = s.c_doctype,
                        //PaymentId = s.n_payment
                    })
                    .ToArray();

                return new Paged<InvoiceModel>
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
        /// Возвращает ЛС по выставленному счёту
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Guid GetSubscrByAccrual(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_invoices
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

              
                query = query.OrderByDescending(o => o.d_setup);
                int itemsCount = query.Count();

                var list = query
                    .Select(s => new PuModel
                    {
                        Id = s.id,
                        IsPu = (s.link != null) ? true : false,
                        Number = s.c_number,
                        Name = s.c_name,
                        InstallPlace = s.c_install_place,
                        InstallDate = s.d_setup,
                        CheckDate = s.d_check,
                        //Tariff = s.n_tariff,
                        Multiplier = s.n_rate,
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
                    .Where(w => w.n_status == 0)
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
                        EnergyType = s.c_energytype,
                        //EnergyTypeName = s.c_energytype_name,
                        TimeZone = s.c_timezone,


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
                //    query = query.Where(w => w.f_status == status);
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
                        Period = s.n_period,
                        Amount = s.n_amount,
                    })
                    .ToArray();

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

    }
}