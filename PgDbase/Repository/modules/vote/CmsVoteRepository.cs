using LinqToDB;
using PgDbase.entity;
using PgDbase.Entity.modules.messages;
using PgDbase.Entity.modules.vote;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.cms
{
    public partial class CmsRepository
    {
        public Paged<VoteModel> GetVoteList(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.vote_vote.Where(w => w.f_site == _siteId);
                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled);
                }
                if (filter.Date.HasValue)
                    query = query.Where(s => s.d_date_start > filter.Date.Value);
                if (filter.DateEnd.HasValue)
                    query = query.Where(s => s.d_date_end < filter.DateEnd.Value.AddDays(1));
                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            query = query.Where(w => w.c_title.Contains(p));
                        }
                    }
                }
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
                                      DateEnd = s.d_date_end
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

        public bool ChechVote(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.vote_vote.Where(w => w.id == id).Any();
            }
        }


        public VoteModel GetVoteItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.vote_vote.Where(w => w.id == id && w.f_site==_siteId);
                if (query.Any())
                {
                    return query.Select(s => new VoteModel {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        DateEnd = s.d_date_end,
                        DateStart = s.d_date_start,
                        Important = s.b_important,
                        Disabled = s.b_disabled,
                        TypeMulti = s.b_type_multi,
                        List = s.fkids.OrderBy(o => o.n_sort).Select(a => new AnswerModel() {
                            Id=a.id,
                            Variant=a.c_variant
                        }).ToList()
                    }).Single();
                }
                return null;
            }
        }

        public bool InsertVote(VoteModel vote)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = vote.Id,
                        PageName = vote.Title,
                        Section = LogModule.Vote,
                        Action = LogAction.insert
                    });

                    db.vote_vote
                                  .Insert(
                                  () => new vote_vote
                                  {
                                      id = vote.Id,
                                      c_title = vote.Title,
                                      c_text=vote.Text,                                      
                                      d_date_start= vote.DateStart,
                                      d_date_end= vote.DateEnd,
                                      b_type_multi=vote.TypeMulti,                                      
                                      b_disabled = vote.Disabled,
                                      b_important = vote.Important,
                                      f_site = _siteId
                                  });
                  
                    tr.Commit();
                    return true;
                }
            }
        }

        public bool UpdateVote(VoteModel news)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var q = db.vote_vote.Where(w => w.id == news.Id && w.f_site == _siteId);
                    if (q.Any())
                    {
                        InsertLog(new LogModel
                        {
                            PageId = news.Id,
                            PageName = news.Title,
                            Section = LogModule.Vote,
                            Action = LogAction.update
                        });

                        var thisnews = q.Single();


                        bool result = q.Set(s => s.c_title, news.Title)
                                       .Set(s => s.c_text, news.Text)                                       
                                       .Set(s => s.d_date_start, news.DateStart)
                                       .Set(s => s.d_date_end, news.DateEnd)
                                       .Set(s => s.b_disabled, news.Disabled)
                                       .Set(s => s.b_important, news.Important)
                                       .Update() > 0;
                        
                        tr.Commit();
                        return result;
                    }
                    return false;


                }
            }
        }



    }
}
