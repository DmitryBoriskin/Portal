using LinqToDB;
using PgDbase.entity;
using PgDbase.Lk.models;
using System;
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
                    .Where(w => w.fkusersubscrs.Any(a => a.fkusersubscrsusers.SiteId == _siteId));

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
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
                        Id=  s.id,
                        Link = s.c_link,
                        Surname = s.c_surname,
                        Name = s.c_name,
                        Patronymic = s.c_patronymic,
                        Address = s.c_address,
                        Phone = s.c_phone,
                        Email = s.c_email,
                        Disabled = s.b_disabled,
                        Created = s.d_created
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
                        d_created = item.Created
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
                        .Update() > 0;

                    tr.Commit();
                    return result;
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
    }
}
