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
                                      DateEnd = s.d_date_end,
                                      Disabled=s.b_disabled
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

        public bool UpdateVote(VoteModel vote)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var q = db.vote_vote.Where(w => w.id == vote.Id && w.f_site == _siteId);
                    if (q.Any())
                    {
                        InsertLog(new LogModel
                        {
                            PageId = vote.Id,
                            PageName = vote.Title,
                            Section = LogModule.Vote,
                            Action = LogAction.update
                        });

                        var thisnews = q.Single();


                        bool result = q.Set(s => s.c_title, vote.Title)
                                       .Set(s => s.c_text, vote.Text)                                       
                                       .Set(s => s.d_date_start, vote.DateStart)
                                       .Set(s => s.d_date_end, vote.DateEnd)
                                       .Set(s => s.b_disabled, vote.Disabled)
                                       .Set(s => s.b_important, vote.Important)
                                       .Set(s => s.b_type_multi, vote.TypeMulti)
                                       .Update() > 0;
                        
                        tr.Commit();
                        return result;
                    }
                    return false;


                }
            }
        }
        
        public bool DeleteVote(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query = db.vote_vote.Where(w => w.id == id && w.f_site == _siteId);
                    if (query.Any())
                    {
                        InsertLog(new LogModel
                        {
                            PageId = query.Single().id,
                            PageName = query.Single().c_title,
                            Section = LogModule.Vote,
                            Action = LogAction.delete
                        },query.Single());

                        query.Delete();

                        tr.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool AddAnswer(AnswerModel answer)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    int sort = 0;
                    var q = db.vote_answers.Where(w => w.f_vote == answer.ParentId).Select(s => s.n_sort);
                    if (q.Any())
                    {
                        sort=q.Max();
                    }
                    sort++;

                    db.vote_answers.Insert(() => new vote_answers
                    {
                        c_variant = answer.Variant,
                        f_vote =answer.ParentId,
                        n_sort=sort
                    });
                    tr.Commit();
                }
                

                return true;
            }
        }

        public bool DeleteAnswer(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query = db.vote_answers.Where(w => w.id == id);
                    if (query.Any())
                    {
                        var data = query.Single();
                        db.vote_answers
                          .Where(w => w.f_vote == data.f_vote)
                          .Where(w=>w.n_sort>data.n_sort)
                          .Set(s => s.n_sort, s => s.n_sort - 1)
                          .Update();

                        query.Delete();
                        tr.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// изьменение порядка вариантов ответов
        /// </summary>
        /// <param name="id"></param>
        /// <param name="new_num"></param>
        /// <returns></returns>
        public bool ChangePositionAnswer(Guid id, int new_num)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query_answer = db.vote_answers.Where(w => w.id == id);

                    Guid Parent = query_answer.Single().f_vote;
                    int actual_num = query_answer.Single().n_sort;

                    if (new_num != actual_num)
                    {
                        if (new_num > actual_num)
                        {
                            db.vote_answers.Where(w => w.f_vote == Parent && w.n_sort > actual_num && w.n_sort <= new_num)
                              .Set(p => p.n_sort, p => p.n_sort - 1)
                              .Update();
                        }
                        else
                        {
                            var q = db.vote_answers.Where(w => w.f_vote == Parent && w.n_sort < actual_num && w.n_sort >= new_num);
                            q.Set(p => p.n_sort, p => p.n_sort + 1)
                            .Update();
                        }
                        db.vote_answers.Where(w => w.id == id).Set(s => s.n_sort, new_num).Update();
                    }
                    tr.Commit();
                }
            }
            return true;
        }

        


    }
}
