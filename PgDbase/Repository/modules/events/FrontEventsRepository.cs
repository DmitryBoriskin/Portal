using PgDbase.Event.models;
using PgDbase.entity;
using System;
using System.Linq;

namespace PgDbase.Repository.front
{
    public partial class Repository
    {
        /// <summary>
        /// список событий
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<EventsModel> GetEvents(FilterModel filter)
        {
            Paged<EventsModel> result = new Paged<EventsModel>();
            using (var db = new CMSdb(_context))
            {
                var query = db.event_events.Where(w => !w.b_disabled);

                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            if (!String.IsNullOrWhiteSpace(p))
                            {
                                query = query.Where(w => w.c_title.Contains(p) || w.c_text.Contains(p));
                            }
                        }
                    }
                }
                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();
                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new EventsModel
                    {
                        Id=s.id,
                        Title = s.c_title,
                        Date = s.d_date,                        
                        Text = s.c_text
                    }).ToArray();

                return new Paged<EventsModel>
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


        public EventsModel GetEventItem(int id)
        {
            using (var db = new CMSdb(_context))
            {



                //return db.event_events.Where(w => w.id == id)
                //       .Join(db.event_events_material_link, m => m.gid, n => n.f_events, (n, m) => new { m, n })
                //        .Select(s => new EventsModel
                //        {
                //            Alias = s.n.c_alias,
                //            Title = s.n.c_title,
                //            Text = s.n.c_text,
                //            Date = s.n.d_date,
                //            DateEnd = s.n.d_date_end,
                //            Keyw = s.n.c_keyw,
                //            Desc = s.n.c_desc,
                //            Annual = s.n.b_annual,
                //            NewsInclude = s.m.fkeventsmaterialsnews.S

                //        }).SingleOrDefault();
                return db.event_events.Where(w => w.id == id)
                          .Select(s => new EventsModel {
                              Alias=s.c_alias,
                              Title=s.c_title,
                              Text=s.c_text,
                              Date=s.d_date,
                              DateEnd=s.d_date_end,
                              Keyw=s.c_keyw,
                              Desc=s.c_desc,
                              Annual=s.b_annual,
                              //NewsInclude= GetEventIncludeNews(s.gid,db)
                          }).SingleOrDefault();

            }
        }

        //public NewsModel[] GetEventIncludeNews(Guid IdEvent, CMSdb db)
        //{
        //    return db.event_events_material_link.Where(w => w.f_events == IdEvent)
        //                .Select(n => new NewsModel()
        //                {
        //                    Title = n.fkeventsmaterialsnews.c_title,
        //                    Id = n.fkeventsmaterialsnews.id
        //                }).ToArray();
        //}
    }
}
