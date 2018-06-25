using LinqToDB;
using PgDbase.entity;
using PgDbase.Entity.modules.messages;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.cms
{
    public partial class CmsRepository
    {
        /// <summary>
        /// возвращает список сообщений с разбивкой по темам
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<MessagesTheme> GetMessages(MessagesFilter filter)
        {
            Paged<MessagesTheme> result = new Paged<MessagesTheme>();
            using (var db = new CMSdb(_context))
            {
                var query = db.msg_messages.Where(w => w.f_parent == null && w.f_site==_siteId);                
                int itemsCount = query.Count();
                var q = query.Select(s => new MessagesTheme
                                {
                                    Id = s.id,
                                    Theme = s.c_theme,
                                    //View = (s.b_admin) ? s.b_views : true,
                                    Date = (from d in db.msg_messages where d.f_parent == s.id || d.id==s.id orderby d.d_date descending select d.d_date).FirstOrDefault(),
                                    AllCount=(from a in db.msg_messages where a.f_parent == s.id || a.id == s.id select a.id).Count(),
                                    NewMsgCount= (from a in db.msg_messages where (a.f_parent == s.id || a.id == s.id) && (a.b_views == false && a.b_admin==false) select a.id).Count(),
                });

                


                if (filter.ViewMessages != null && filter.ViewMessages==true)
                    q = q.Where(w => w.NewMsgCount>0);

                if (filter.Date.HasValue)
                    q = q.Where(s => s.Date > filter.Date.Value);
                if (filter.DateEnd.HasValue)
                    q = q.Where(s => s.Date < filter.DateEnd.Value.AddDays(1));


                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            if (!String.IsNullOrWhiteSpace(p))
                            {
                                q= q.Where(w => w.Theme.Contains(p));
                            }
                        }
                    }
                }
                q = q.OrderByDescending(d => d.Date);
                var list = q.Skip(filter.Size * (filter.Page - 1))
                            .Take(filter.Size)
                            .ToArray();

                return new Paged<MessagesTheme>
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
        /// возвращает ветку  темы
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<MessagesModel> GetMessagesThemeItem(Guid id)
        {
            List<MessagesModel> model = new List<MessagesModel>();
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var q = db.msg_messages.Where(w => w.id == id && w.f_site == _siteId && w.f_parent == null);
                    if (q.Any())
                    {
                        //делаем сообщения пользователя прочитанными
                        var e=db.msg_messages.Where(w => (w.id == id || w.f_parent==id) && w.b_admin==false)
                            .Set(s => s.b_views, true)
                            .Update();



                        var TopMessage = q.Single();
                        MessagesModel topelement = new MessagesModel()
                        {
                            Date = TopMessage.d_date,
                            Text = TopMessage.c_text,
                            Theme = TopMessage.c_theme,
                            Admin = TopMessage.b_admin,
                            MsgUser = db.core_AspNetUserProfiles.Where(w => w.UserId == TopMessage.f_user.ToString())
                                                            .Select(s => new UserModel
                                                            {
                                                                Surname = s.Surname,
                                                                Name = s.Name,
                                                                Id = TopMessage.f_user
                                                            }).Single()
                        };
                        model.Add(topelement);
                        var MsgList = db.msg_messages.Where(w => w.f_parent == TopMessage.id)
                                                     .OrderBy(o => o.d_date)
                                                     .Join(db.core_AspNetUserProfiles, n => n.fkuserid.Id, m => m.UserId, (n, m) => new { n, m })
                                                     .Select(s => new MessagesModel()
                                                     {
                                                         Date = s.n.d_date,
                                                         Text = s.n.c_text,
                                                         Admin = s.n.b_admin,
                                                         MsgUser = new UserModel
                                                         {
                                                             Surname = s.m.Surname,
                                                             Name = s.m.Name,
                                                             Id = s.n.f_user
                                                         }
                                                     }).ToList();
                        model.AddRange(MsgList);
                        model.Reverse();
                        tr.Commit();
                        return model;
                    }
                }
                
                return null;                
            }
        }
        /// <summary>
        /// Определяет есть ли
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckMessages(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.msg_messages.Where(w => w.id == id && w.f_site == _siteId).Any();
            }
        }
        public bool InsertMessages(MessagesModel insert)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = insert.Id,
                        PageName = insert.Theme,
                        Section = LogModule.Events,
                        Action = LogAction.insert
                    });
                    Guid? _parent = null;
                    if((insert.ParentId != Guid.Empty))
                    {
                        _parent = insert.ParentId;
                    }                    

                    db.msg_messages
                        .Insert(
                        () => new msg_messages
                        {
                            b_admin=insert.Admin,
                            c_text=insert.Text,
                            c_theme=insert.Theme,
                            f_user= _currentUserId,
                            f_user_destination=insert.UserDestination,
                            d_date=insert.Date,
                            f_parent= _parent,
                            f_site=_siteId,
                            id=insert.Id
                        });
                    tr.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// удалят ветку сообщений из одной темы
        /// </summary>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public bool DeleteMessages(Guid Guid)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var q = db.msg_messages.Where(w => w.id == Guid && w.f_site == _siteId);

                    if (q.Any())
                    {
                        var msg = q.Single();
                        db.msg_messages.Where(w => w.f_parent == Guid).Delete();
                        InsertLog(new LogModel
                        {
                            PageId = Guid,
                            PageName = msg.c_theme,
                            Comment = "Удалено сообщение" + String.Format("{0}/", msg.c_theme),
                            Section = LogModule.Messages,
                            Action = LogAction.delete,
                        }, msg);
                        q.Delete();
                        tr.Commit();
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Список пользователей сайта
        /// </summary>
        /// <returns></returns>
        public List<UserModel> GetUserList()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_AspNetUsers.Where(w => w.SiteId == _siteId);
                if (query.Any())
                {
                    return query
                        .OrderBy(o=>new { o.AspNetUserProfilesUserId.Surname, o.AspNetUserProfilesUserId.Name, o.AspNetUserProfilesUserId.Patronymic })
                        .Select(s => new UserModel() {
                                            Id=s.UserId,
                                            Surname=s.AspNetUserProfilesUserId.Surname,
                                            Name= s.AspNetUserProfilesUserId.Name,
                                            Patronimyc= s.AspNetUserProfilesUserId.Patronymic
                    }).ToList();
                }
                return null;
            }


        }
    }
}
