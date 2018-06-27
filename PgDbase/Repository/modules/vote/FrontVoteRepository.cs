using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.Entity.modules.vote;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.front
{
    public partial class FrontRepository
    {

        public Paged<VoteModel> GetVoteList(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.vote_vote.Where(w => w.f_site == _siteId);
                query = query.Where(w => w.b_disabled == false);




                #warning тут надо ограничить по дате завершения и по дате начала публикации

                query = query.OrderByDescending(o => o.d_date_start);
                if (query.Any())
                {
                    int itemsCount = query.Count();
                    var list = query.Skip(filter.Size * (filter.Page - 1))
                                  .Take(filter.Size)
                                  .Select(s => new VoteModel
                                  {
                                      Id = s.id,
                                      Title = s.c_title,
                                      DateStart = s.d_date_start,
                                      DateEnd = s.d_date_end,
                                      Disabled = s.b_disabled
                                  });
                    return new Paged<VoteModel>()
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
                else return null;
            }
        }

        /// <summary>
        /// результат выдаст данные для голосования или данные статистики голосования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VoteModel GetVoteItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.vote_vote.Where(w => w.id == id && w.f_site == _siteId);
                if (query.Any())
                {
                    var q = db.vote_stat_answer
                              .Where(w => w.f_user == _currentUserId)
                              .Join(query,n=>n.f_vote,m=>m.id,(n,m)=>new {n,m});
                    if (q.Any())
                    {
                        //данные с результатами голосования
                        return query.Select(s => new VoteModel
                        {
                            ShowStat=true,//признак по которому понимаем что будем показывать статистику с результатом голосования - используем в view
                            Id = s.id,
                            Title = s.c_title,
                            Text = s.c_text,
                            DateEnd = s.d_date_end,
                            DateStart = s.d_date_start,                            
                            Important = s.b_important,
                            Disabled = s.b_disabled,
                            TypeMulti = s.b_type_multi,
                            ListStat = s.fkids.OrderBy(o => o.n_sort).Select(a => new AnswerAndStat()
                            {
                                Id = a.id,
                                Variant = a.c_variant,
                                AllCount=s.fkstats.Count(),
                                CurrentCount=s.fkstats.Where(e=>e.f_answer==a.id).Count()
                            }).ToList(),
                        }).Single();
                    }
                    else
                    {
                        //данные для формы голосования
                        return query.Select(s => new VoteModel
                        {
                            ShowStat = false,
                            Id = s.id,
                            Title = s.c_title,
                            Text = s.c_text,
                            DateEnd = s.d_date_end,
                            DateStart = s.d_date_start,
                            Important = s.b_important,
                            Disabled = s.b_disabled,
                            TypeMulti = s.b_type_multi,
                            List = s.fkids.OrderBy(o => o.n_sort).Select(a => new AnswerModel()
                            {
                                Id = a.id,
                                Variant = a.c_variant
                            }).ToList()
                        }).Single();
                    }
                    
                }
                return null;
            }
        }

        /// <summary>
        /// процесс голосования
        /// </summary>
        /// <param name="VoteId"></param>
        /// <param name="Answer"> массив идентификаторов вариантов ответом</param>
        /// <returns></returns>
        public bool ActionVote(Guid VoteId, string[] Answer)
        {
            using (var db = new CMSdb(_context))
            {
                List<vote_stat_answer> list = new List<vote_stat_answer>();
                foreach (var item in Answer)
                {
                    list.Add(new vote_stat_answer {                        
                        f_answer= Guid.Parse(item),
                        f_vote=VoteId,
                        d_date=DateTime.Now,
                        f_user=_currentUserId
                    });
                }

                using (var tr = db.BeginTransaction())
                {
                    db.BulkCopy(list);
                    tr.Commit();
                    return true;
                }                    
            }
        }

        public VoteModel GetVoteForIndexPage()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.vote_vote
                              .Where(w => w.f_site == _siteId && w.b_disabled == false)
                              .Where(w => w.d_date_start <= DateTime.Now && (w.d_date_end >= DateTime.Now || w.d_date_end ==null))
                              .OrderByDescending(o => o.d_date_start)
                              .OrderByDescending(i => i.b_important);
                if (query.Any())
                {
                    return query.Select(s => new VoteModel
                    {
                        Title = s.c_title,
                        Id = s.id,
                        Text=s.c_text
                    }).First();
                }
                return null;
            }
        }

    }
}
