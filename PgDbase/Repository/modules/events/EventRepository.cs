using LinqToDB;
using PgDbase.entity;
using PgDbase.Event.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с событиями/мероприятиями
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// список событий с постраничной разбивкой
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<EventsModel> GetEventsList(EventFilterModel filter)
        {
            Paged<NewsModel> result = new Paged<NewsModel>();
            using (var db = new CMSdb(_context))
            {
                var query = db.event_events.Where(w => w.f_site == _siteId);

                if (filter.Disabled != null)
                {
                    query = query.Where(w=>w.b_disabled== filter.Disabled);
                }
                if (filter.Annual!= null)
                {
                    query = query.Where(w => w.b_annual !=filter.Annual);
                }
                if (filter.Date.HasValue)
                    query = query.Where(s => s.d_date > filter.Date.Value);
                if (filter.DateEnd.HasValue)
                    query = query.Where(s => s.d_date < filter.DateEnd.Value.AddDays(1));


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

                query = query.OrderBy(o => o.d_date);
                int itemsCount = query.Count();
                var list = query.Skip(filter.Size * (filter.Page - 1))
                                .Take(filter.Size)
                                .Select(s => new EventsModel {
                                    Id = s.id,
                                    Guid = s.gid,
                                    Title = s.c_title,
                                    Disabled=s.b_disabled,
                                    Annual=s.b_annual,
                                    Date = s.d_date,
                                    DateEnd=s.d_date_end,
                                    Photo = s.c_photo,
                                    ViewCount = s.c_view_count
                                });
                return new Paged<EventsModel>()
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
        /// единичная запись события
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventsModel GetEventItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.event_events.Where(w => w.gid == id);
                if (data.Any())
                {
                    var d= data.Select(s => new EventsModel {
                        Alias=s.c_alias,
                        Annual=s.b_annual,
                        Date=s.d_date,
                        DateEnd=s.d_date_end,
                        Desc=s.c_desc,
                        Disabled=s.b_disabled,
                        Guid=s.gid,
                        Id=s.id,
                        Keyw=s.c_keyw,
                        Photo=s.c_photo,
                        SourceName=s.c_source_name,
                        SourceUrl=s.c_source_url,
                        Text=s.c_text,
                        Title=s.c_title,
                        ViewCount=s.c_view_count                        
                    }).Single();

                    var q = db.event_events_material_link.Where(w => w.f_events == id);
                    if (q.Any())
                    {
                        d.NewsInclude = q.Select(s => new NewsModel()
                        {
                            Guid = s.f_news,
                            Title = s.fkeventsmaterialsnews.c_title
                        }).ToArray();
                    }

                    return d;
                }
                return null;
            }

        }

        /// <summary>
        /// проверка существования события по идентифкатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckEvets(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.event_events.Where(w => w.gid == id && w.f_site == _siteId).Any();
            }
        }

        /// <summary>
        /// добавление события
        /// </summary>
        /// <param name="insert"></param>
        /// <returns></returns>
        public bool InsertEvent(EventsModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = insert.Guid,
                        PageName = insert.Title,
                        Section = LogModule.Events,
                        Action = LogAction.insert
                    });

                    db.event_events
                        .Insert(
                        ()=>new event_events {                            
                            b_annual= insert.Annual,
                            b_disabled= insert.Disabled,
                            c_alias= insert.Alias,
                            c_desc= insert.Desc,
                            c_keyw= insert.Keyw,
                            c_photo= insert.Photo,
                            c_source_name= insert.SourceName,
                            c_source_url= insert.SourceUrl,
                            c_text= insert.Text,
                            c_title= insert.Title,
                            c_view_count= insert.ViewCount,
                            d_date= insert.Date,
                            d_date_end= insert.DateEnd,                            
                            f_site=_siteId,
                            gid= insert.Guid
                        });
                    tr.Commit();
                    return true;
                }                    
            }
        }
        /// <summary>
        /// обновлдение события
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public bool UpdateEvent(EventsModel update)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var q = db.event_events.Where(w => w.gid == update.Guid && w.f_site == _siteId);
                    if (q.Any())
                    {
                        InsertLog(new LogModel
                        {
                            PageId = update.Guid,
                            PageName = update.Title,
                            Section = LogModule.Events,
                            Action = LogAction.insert
                        });

                        q.Set(s => s.b_annual, update.Annual)                           
                            .Set(s => s.b_disabled, update.Disabled)
                            .Set(s => s.c_alias, update.Alias)
                            .Set(s => s.c_desc, update.Desc)
                            .Set(s => s.c_keyw, update.Keyw)
                            .Set(s => s.c_photo, update.Photo)
                            .Set(s => s.c_source_name, update.SourceName)
                            .Set(s => s.c_source_url, update.SourceUrl)
                            .Set(s => s.c_text, update.Text)
                            .Set(s => s.c_title, update.Title)
                            .Set(s => s.d_date, update.Date)
                            .Set(s => s.d_date_end, update.DateEnd)                            
                            .Update();
                    }
                    tr.Commit();
                }
                return true;
            }
        }
        /// <summary>
        /// удаление события
        /// </summary>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public bool DeleteEvent(Guid Guid)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var q = db.event_events.Where(w => w.gid==Guid && w.f_site == _siteId);
                    if (q.Any())
                    {
                        var events = q.Single();
                        InsertLog(new LogModel
                        {
                            PageId = Guid,
                            PageName = events.c_title,
                            Comment = "Удалено событие" + String.Format("{0}/", events.c_title),
                            Section = LogModule.News,
                            Action = LogAction.delete,
                        }, events);
                        q.Delete();
                        tr.Commit();
                        return true;
                    }
                    return false;                    
                }   
            }
        }


        /// <summary>
        /// Возвращает прицепленные к новости события
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EventsModel[] GetAttachEventsForNews(Guid id)
        {
           using (var db = new CMSdb(_context))
           {
               var q = db.event_events_material_link.Where(w => w.f_news == id);
               if (q.Any())
               {
                   return q.Select(s => new EventsModel() {
                       Guid=s.f_events,
                       AttachEventNewsId=s.id,
                       Title =s.fkeventsmaterials.c_title                        
                   }).ToArray();
               }                          
           }
           return null;
        }
        /// <summary>
        /// список событий которые можно подключить к новости
        /// </summary>
        /// <returns></returns>
        public EventsModel[] GetLastEvents(Guid NewsId)
        {
            using (var db = new CMSdb(_context))
            {   
                var q = db.event_events
                          .Where(w => w.f_site == _siteId);                

                var AllEvents = db.event_events
                             .Where(w => w.f_site == _siteId).Select(s=>s.gid);

                var SelectedEvents = db.event_events_material_link.Where(w => w.f_news == NewsId).Select(s=>s.f_events).ToArray();
                var list = new List<Guid>();
                foreach (var item in AllEvents)
                {
                    if(SelectedEvents!=null && !SelectedEvents.Contains(item)){
                        list.Add(item);
                    }
                }
                var EventsDropdownList = list.Join(db.event_events, m => m, n => n.gid, (m, n) => n).OrderByDescending(o=>o.d_date).Take(500);
                if (EventsDropdownList.Any())
                {
                    return EventsDropdownList.Select(s => new EventsModel() {
                        Title=s.c_title,
                        Guid=s.gid
                    }).ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Прикрепляет к новости событие
        /// </summary>
        /// <param name="NewsId"></param>
        /// <param name="EventId"></param>
        /// <returns></returns>
        public bool AttachEventsForNews(Guid NewsId, Guid EventId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                   var result= db.event_events_material_link.Insert(
                        ()=> new event_events_material_link {
                            f_news=NewsId,
                            f_events=EventId
                        })>0;                        

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// удаляет связь новости и события
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteAttachEventsForNews(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.event_events_material_link.Where(w => w.id == id);
                if (q.Any())
                {
                    q.Delete();
                    return true;
                }
                return false;
            }
        }

    }
}
