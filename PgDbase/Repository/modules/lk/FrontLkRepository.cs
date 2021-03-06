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
                    query = query.Where(w => w.c_name.ToLower().Contains(text)
                                                      || w.n_subscr.ToString().Contains(text));
                }

                query = query.OrderBy(o => new { o.c_name });

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
                        Addres = s.c_address,
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


        public SubscrManager GetManager(Guid subscrId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_subscr_configs
                    .Where(w => w.f_subscr == subscrId);
                               
                if (query.Any())
                {
                    return query.Select(s => new SubscrManager
                    {
                        Id = s.fksubscrconfigsmanagers.id,
                        FIO = s.fksubscrconfigsmanagers.c_name,
                        Email = s.fksubscrconfigsmanagers.c_email,
                        Desc=s.fksubscrconfigsmanagers.c_desc,
                        Phone=s.fksubscrconfigsmanagers.c_phone,
                        Post=s.fksubscrconfigsmanagers.c_post                        
                    })
                    .FirstOrDefault();
                }
                return null;
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

        #region Счета-фактуры
        /// <summary>
        /// Возвращает список выставленных счетов для пользователя
        /// </summary>
        /// <param name="subscr"></param>
        /// <returns></returns>
        public Paged<InvoiceModel> GetInvoices(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
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
                if (!string.IsNullOrEmpty(filter.Type))
                {
                    query = query.Where(w => w.n_doc_type.ToString() == filter.Type);
                }

                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new InvoiceModel
                    {
                        Id = s.id,
                        Link = s.link,
                        SubscrId = s.f_subscr,
                        Subscr = s.n_subscr,

                        Date = s.d_date,
                        DateBegin = s.d_date_begin,
                        DateEnd = s.d_date_end,
                        DateDue = s.d_date_due,

                        Debit = s.b_debit,
                        PeriodId = s.n_period,
                        Payed = s.b_closed,

                        StatusId = s.n_status,
                        Status = s.c_status,

                        UnitId = s.n_unit,
                        Unit = s.c_unit,

                        Number = s.c_number,
                        Amount = s.n_amount,
                        Tax = s.n_tax,
                        Cons = s.n_cons,
                        Quantity = s.n_quantity,
                        Quantity2 = s.n_quantity2,

                        DebtTypeId = s.n_debts,
                        DebtType = s.c_debts,

                        DocTypeId = s.n_doc_type,
                        DocType = s.c_doc_type,

                        PaymentId = s.n_payment,
                        PaysheetId = s.n_paysheet,

                        SaleCategoryId = s.n_sale_category,
                        SaleCategory = s.c_sale_category,

                        Details = GetInvoiceDetail(s.link)

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
        /// Возвращает список выставленных счетов для пользователя
        /// </summary>
        /// <param name="subscr"></param>
        /// <returns></returns>
        public InvoiceModel[] GetInvoicesList(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
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
                if (!string.IsNullOrEmpty(filter.Type))
                {
                    query = query.Where(w => w.n_doc_type.ToString() == filter.Type);
                }

                query = query.OrderBy(o => o.d_date);

                var list = query
                    .Select(s => new InvoiceModel
                    {
                        Id = s.id,
                        Link = s.link,
                        SubscrId = s.f_subscr,
                        Subscr = s.n_subscr,

                        Date = s.d_date,
                        DateBegin = s.d_date_begin,
                        DateEnd = s.d_date_end,
                        DateDue = s.d_date_due,

                        Debit = s.b_debit,
                        PeriodId = s.n_period,
                        Payed = s.b_closed,

                        StatusId = s.n_status,
                        Status = s.c_status,

                        UnitId = s.n_unit,
                        Unit = s.c_unit,

                        Number = s.c_number,
                        Amount = s.n_amount,
                        Tax = s.n_tax,
                        Cons = s.n_cons,
                        Quantity = s.n_quantity,
                        Quantity2 = s.n_quantity2,

                        DebtTypeId = s.n_debts,
                        DebtType = s.c_debts,

                        DocTypeId = s.n_doc_type,
                        DocType = s.c_doc_type,

                        PaymentId = s.n_payment,
                        PaysheetId = s.n_paysheet,

                        SaleCategoryId = s.n_sale_category,
                        SaleCategory = s.c_sale_category,

                        Details = GetInvoiceDetail(s.link)

                    })
                    .ToArray();

                return list;
            }
        }

        /// <summary>
        /// Возвращает выставленный счёт
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public InvoiceModel GetInvoice(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_invoices
                    .Where(w => w.id == id)
                    .Select(s => new InvoiceModel
                    {
                        Id = s.id,
                        Link = s.link,
                        SubscrId = s.f_subscr,
                        Subscr = s.n_subscr,

                        Date = s.d_date,
                        DateBegin = s.d_date_begin,
                        DateEnd = s.d_date_end,
                        DateDue = s.d_date_due,

                        Debit = s.b_debit,
                        PeriodId = s.n_period,
                        Payed = s.b_closed,

                        StatusId = s.n_status,
                        Status = s.c_status,

                        UnitId = s.n_unit,
                        Unit = s.c_unit,

                        Number = s.c_number,
                        Amount = s.n_amount,
                        Tax = s.n_tax,
                        Cons = s.n_cons,
                        Quantity = s.n_quantity,
                        Quantity2 = s.n_quantity2,

                        DebtTypeId = s.n_debts,
                        DebtType = s.c_debts,

                        DocTypeId = s.n_doc_type,
                        DocType = s.c_doc_type,

                        PaymentId = s.n_payment,
                        PaysheetId = s.n_paysheet,

                        SaleCategoryId = s.n_sale_category,
                        SaleCategory = s.c_sale_category,

                        Details = GetInvoiceDetail(s.link)
                    })
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Детализация по выставленному счету
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public InvoiceDetailModel[] GetInvoiceDetail(long link)
        {
            using (var db = new CMSdb(_context))
            {
                return db.lk_invoices_details
                    .Where(w => w.n_invoice == link)
                    .Select(s => new InvoiceDetailModel
                    {
                        Id = s.id,
                        Link = s.link,
                        InvoiceId = s.f_invoice,
                        InvoiceLInk = s.n_invoice,

                        TimeZoneId = s.n_time_zone,
                        TimeZone = s.c_time_zone,

                        SuboperationTypeId = s.n_suboperation_type,
                        SuboperationType = s.c_suboperation_type,

                        UnitId = s.n_unit,
                        Unit = s.c_unit,

                        TariffId = s.n_tariff,
                        Tariff = s.c_tariff,

                        InvoceGrp = s.n_invoce_grp,
                        BillGrp = s.n_bill_grp,

                        DateBegin = s.d_date_begin,
                        DateEnd = s.d_date_end,
                        Period = s.n_period,

                        Amount = s.n_amount,
                        TaxAmount = s.n_tax,
                        Quantity = s.n_quantity,
                        TariffAmount = s.n_tariff_amount,

                        Amount0 = s.n_amount0,
                        TaxAmount0 = s.n_tax0,
                        Quantity0 = s.n_quantity0,
                        TariffAmount0 = s.n_tariff_amount0,

                        Amount1 = s.n_amount1,
                        TaxAmount1 = s.n_tax1,
                        Quantity1 = s.n_quantity1,
                        TariffAmount1 = s.n_tariff_amount1

                    })
                    .ToArray();
            }
        }

        /// <summary>
        /// Типы счет-фактур
        /// </summary>
        public HdbkModel[] GetInvoiceDocTypes()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.lk_invoices.Select(s => new HdbkModel()
                {
                    Id = s.n_doc_type.ToString(),
                    Name = s.c_doc_type
                })
                .Distinct()
                .ToArray();

                return data;
            }
        }

        #endregion

        #region
        /// <summary>
        /// Возвращает дебит - кредит, оборот
        /// </summary>
        /// <param name="subscr"></param>
        /// <returns></returns>
        public StatisticsModel[] GetDebitCreditStatisticsList(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_cv_subscr_debit_credit
                    .Where(w => w.f_subscr == subscr);

                if (filter.Date.HasValue)
                {
                    var month = (filter.Date.Value.Month < 10) ? "0" + filter.Date.Value.Month : filter.Date.Value.Month.ToString();
                    var beginPeriod = int.Parse($"{filter.Date.Value.Year}{month}");
                    query = query.Where(w => w.n_period >= beginPeriod);
                }
                if (filter.DateEnd.HasValue)
                {
                    var month = (filter.DateEnd.Value.Month < 10) ? "0" + filter.DateEnd.Value.Month : filter.DateEnd.Value.Month.ToString();
                    var endPeriod = int.Parse($"{filter.DateEnd.Value.Year}{month}");
                    query = query.Where(w => w.n_period <= endPeriod);
                }

                query = query.OrderByDescending(o => o.n_period);

                var list = query
                    .Select(s => new StatisticsModel
                    {
                        SubscrId = s.f_subscr,
                        Subscr = s.n_subscr,

                        PeriodId = s.n_period,
                        InvoiceAmount = s.n_invoice_amount,
                        PaymentAmount = s.n_payment_amount
                    })
                     .ToArray();

                return list;
            }
        }
        /// <summary>
        /// Возвращает дебит - кредит, оборот
        /// </summary>
        /// <param name="subscr"></param>
        /// <returns></returns>
        public Paged<StatisticsModel> GetDebitCreditData(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_cv_subscr_debit_credit
                    .Where(w => w.f_subscr == subscr);

                //if (filter.Date.HasValue)
                //{
                //    var month = (filter.Date.Value.Month < 10) ? "0" + filter.Date.Value.Month : filter.Date.Value.Month.ToString();
                //    var beginPeriod = int.Parse($"{filter.Date.Value.Year}{month}");
                //    query = query.Where(w => w.n_period >= beginPeriod);
                //}
                //if (filter.DateEnd.HasValue)
                //{
                //    var month = (filter.DateEnd.Value.Month < 10) ? "0" + filter.DateEnd.Value.Month : filter.DateEnd.Value.Month.ToString();
                //    var endPeriod = int.Parse($"{filter.DateEnd.Value.Year}{month}");
                //    query = query.Where(w => w.n_period <= endPeriod);
                //}

                query = query.OrderByDescending(o => o.n_period);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new StatisticsModel
                    {
                        SubscrId = s.f_subscr,
                        Subscr = s.n_subscr,

                        PeriodId = s.n_period,
                        InvoiceAmount = s.n_invoice_amount,
                        PaymentAmount = s.n_payment_amount
                    })
                     .ToArray();

                return new Paged<StatisticsModel>
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
        public Paged<PuModel> GetSubscrDevices(Guid subscr, LkFilter filter)
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
                        //Link = s.link,
                        SubscrId = s.f_subscr,
                        SubscrLink = s.n_subscr,
                        IsPu = (s.link != null) ? true : false,
                        Number = s.c_number,
                        Name = s.c_name,
                        Year = s.n_year,
                        InstallPlace = s.c_install_place,
                        InstallDate = s.d_setup,
                        CheckDate = s.d_check,
                        NextCheckDate = s.d_next_check,
                        ReplaceBeforeDate = s.d_replace_before,
                        ValidDate = s.d_valid,
                        EnergyLvl = s.c_energy_lvl,
       
                        Multiplier = s.n_rate,
                        DeviceId = s.f_device,
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
                                    VoltageNominal = s.fksubscrdevicesdevices.c_voltage_nominal,
                                    CheckCycle = s.fksubscrdevicesdevices.n_check_cycle
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

        /// <summary>
        /// информацмя о счетчика
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PuModel GetPuModel(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_subscr_devices
                              .Where(w => w.id == id);
                if (query.Any())
                {
                    return query
                   .Select(s => new PuModel
                   {
                       Id = s.id,
                       //Link = s.link,
                       SubscrId = s.f_subscr,
                       SubscrLink = s.n_subscr,
                       IsPu = (s.link != null) ? true : false,
                       Number = s.c_number,
                       Name = s.c_name,
                       Year = s.n_year,
                       InstallPlace = s.c_install_place,
                       InstallDate = s.d_setup,
                       CheckDate = s.d_check,
                       NextCheckDate = s.d_next_check,
                       ReplaceBeforeDate = s.d_replace_before,
                       ValidDate = s.d_valid,
                       EnergyLvl = s.c_energy_lvl,

                       Multiplier = s.n_rate,
                       DeviceId = s.f_device,
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
                                    VoltageNominal = s.fksubscrdevicesdevices.c_voltage_nominal,
                                    CheckCycle = s.fksubscrdevicesdevices.n_check_cycle
                                }
                                : null

                   }).Single();
                }


                return null;
            }
        }

        public Paged<MeterModel> GetMeterModel(Guid id,FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_meters.Where(w => w.f_device == id);

                if (filter.Date != null)
                {
                    query = query.Where(w => w.d_date >= filter.Date);
                }

                if (filter.DateEnd!= null)
                {
                    query = query.Where(w => w.d_date <= filter.DateEnd);
                }


                if (query.Any())
                {
                    query = query.OrderByDescending(o => o.d_date);
                    int itemsCount = query.Count();
                    var list = query
                        .Skip(filter.Size * (filter.Page - 1))
                        .Take(filter.Size)
                        .Select(s => new MeterModel {
                            Date=s.d_date,
                            Year=s.n_year,
                            Month=s.n_month,
                            DeliveryMethod=s.c_delivery_method,
                            EnergyTypeName=s.c_energytype,
                            TimeZone=s.c_timezone,
                            Value=s.n_value
                        }).ToArray();

                    return new Paged<MeterModel>
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
                return null;
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
        /// Возвращает постраничный список платежей
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<PaymentModel> GetPayments(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var result = new Paged<PaymentModel>();
                var query = db.lk_payments
                    .Where(w => w.f_subscr == subscr);

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
                        PeriodId = s.n_period,
                        Amount = s.n_amount,
                        Destination = s.c_destination,
                        Documents = GetBindInvoiceDocuments(s.link)
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

        /// <summary>
        /// Возвращает список платежей в виде массива
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PaymentModel[] GetPaymentsList(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_payments
                    .Where(w => w.f_subscr == subscr);

                if (filter.Date.HasValue)
                {
                    query = query.Where(w => w.d_date >= filter.Date.Value);
                }
                if (filter.DateEnd.HasValue)
                {
                    query = query.Where(w => w.d_date <= filter.DateEnd.Value.AddDays(1));
                }

                query = query.OrderBy(o => o.d_date);

                var list = query
                    .Select(s => new PaymentModel
                    {
                        Id = s.id,
                        Date = s.d_date,
                        PeriodId = s.n_period,
                        Amount = s.n_amount,
                        Destination = s.c_destination,
                        Documents = GetBindInvoiceDocuments(s.link)
                    })
                    .ToArray();

                return list;
            }
        }

        /// <summary>
        /// Связанные документы(счета-фактуры) по платежам
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        public DocumentModel[] GetBindInvoiceDocuments(long paymentId)
        {
            using (var db = new CMSdb(_context))
            {

                var query = db.lk_invoices
                    .Where(w => w.n_payment == paymentId);

                var data = query
                    .Select(s => new DocumentModel
                    {
                        Id = s.id,
                        Date = s.d_date,
                        Number = s.c_number,
                        Type = s.c_doc_type
                    })
                    .ToArray();

                return data;
            }
        }

        #endregion

        // <summary>
        /// Возвращает постраничный список расчетных ведомостей 
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<PaysheetModel> GetPaysheets(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_paysheets
                    .Where(w => w.f_subscr == subscr);

                if (filter.Date.HasValue)
                {
                    query = query.Where(w => w.d_date >= filter.Date.Value);
                }
                if (filter.DateEnd.HasValue)
                {
                    query = query.Where(w => w.d_date <= filter.DateEnd.Value.AddDays(1));
                }

                int itemsCount = query.Count();
                query = query.OrderBy(o => o.d_date);

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new PaysheetModel
                    {
                        Id = s.id,
                        Link = s.link,
                        Number = s.c_number,

                        SubscrId = s.f_subscr,
                        SubscrLink = s.n_subscr,

                        DocTypeId = s.n_doctype,
                        DocType = s.c_doctype,

                        StatusId = s.n_status,
                        Status = s.c_status,

                        SaleCategoryId = s.n_sale_category,
                        SaleCategory = s.c_sale_category,

                        Date = s.d_date,
                        DateBegin = s.d_date_begin,
                        DateEnd = s.d_date_end,
                        PeriodId = s.n_period,

                        Amount = s.n_amount,
                        Cons = s.n_cons,
                        Quantity = s.n_quantity,
                        Quantity2 = s.n_quantity2,
                        TaxAmount = s.n_tax,

                        //Details = null,
                        Documents = GetBindInvoiceDocuments(s.link)
                    })
                    .ToArray();

                return  new Paged<PaysheetModel>
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

        // <summary>
        /// Возвращает список расчетных ведомостей в виде массива
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public StatisticsModel[] GetConsumptionStatistics(Guid subscr, LkFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_cv_subscr_consumption
                    .Where(w => w.f_subscr == subscr);

                if (filter.Date.HasValue)
                {
                    var month = (filter.Date.Value.Month < 10) ? "0" + filter.Date.Value.Month : filter.Date.Value.Month.ToString();
                    var beginPeriod = int.Parse($"{filter.Date.Value.Year}{month}");
                    query = query.Where(w => w.n_period >= beginPeriod);
                }
                if (filter.DateEnd.HasValue)
                {
                    var month = (filter.DateEnd.Value.Month < 10) ? "0" + filter.DateEnd.Value.Month : filter.DateEnd.Value.Month.ToString();
                    var endPeriod = int.Parse($"{filter.DateEnd.Value.Year}{month}");
                    query = query.Where(w => w.n_period <= endPeriod);
                }

                query = query.OrderByDescending(o => o.n_period);

                var list = query
                    .Select(s => new StatisticsModel
                    {
                       

                        SubscrId = s.f_subscr,
                        Subscr = s.n_subscr,
                        PeriodId = s.n_period,

                        ConsumptionAmount1 = s.n_quantity_kvth,
                        ConsumptionAmount2 = s.n_quantity_kvt
                    })
                    .ToArray();

                return list;
            }
        }

        // <summary>
        /// Возвращает список расчетных ведомостей в виде массива
        /// </summary>
        /// <param name="subscr"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PaySheetDetailsModel[] GetPaysheets(Guid paysheetId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.lk_paysheets_details
                    .Where(w => w.f_paysheet == paysheetId);

                var list = query
                    .Select(s => new PaySheetDetailsModel
                    {
                        Id = s.id,
                        Link = s.link,

                        PaysheetId = s.f_paysheet,
                        PaysheetLInk = s.n_paysheet,

                        BillGrp = s.n_bill_grp,
                        InvoceGrp = s.n_invoce_grp,

                        CalcMethod = s.c_calc_method,
                        DeviceId = s.f_device,
                        DeviceLink = s.n_device,
                        DeviceName = s.c_device,

                        DateBegin = s.d_date_begin,
                        DateEnd = s.d_date_end,
                        Period = s.n_period,

                        VoltageNominal = s.c_voltage_nominal,
                        UnitId = s.n_unit,
                        Unit = s.c_unit,

                        EnergyLvlId = s.n_energy_lvl,
                        EnergyLvl = s.c_energy_lvl,
                        
                        EnergyTypeId = s.n_energy_type,
                        EnergyType = s.c_energy_type,

                        SaleItem = s.c_sale_item,
                        SaleItemName = s.c_sale_item_doc,

                        TimeZoneId = s.n_tariff_zone,
                        TimeZone = s.c_tariff,

                        TariffId = s.n_tariff,
                        Tariff = s.c_tariff,
                        TariffAmount = s.n_tariff_amount,

                        TaxPersent = s.n_tax_persent,
                        Amount = s.n_amount,
                        Quantity = s.n_quantity,
                        TaxAmount = s.n_tax,
                        Cons = s.n_cons,
                        MrCons = s.n_mr_cons,
                        Persent = s.n_persent,

                    })
                    .ToArray();

                return list;
            }
        }

    }
}