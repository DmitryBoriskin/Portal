﻿using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Repository.front
{
    public partial class FrontRepository
    {

        public List<MessagesModel> GetNewMessage()
        {
            using (var db = new CMSdb(_context))
            {
                //var q=db.msg_messages
            }
            return null;
        }

        public Paged<MessagesTheme> GetHistoryMessageTheme(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.msg_messages.Where(w =>(w.f_user == _currentUserId || w.f_user_destination == _currentUserId) && w.f_parent==null && w.f_site==_siteId);
                if (query.Any())
                {
                    int itemsCount = query.Count();
                    var q = query.Select(s => new MessagesTheme
                    {
                        Id = s.id,
                        Theme = s.c_theme,
                        User=s.f_user,
                        Admin=s.b_admin,
                        View = (!s.b_admin) ? s.b_views : true,
                        Date = (from d in db.msg_messages where d.f_parent == s.id || d.id == s.id orderby d.d_date select d.d_date).FirstOrDefault(),
                        //,
                        //AllCount = (from a in db.msg_messages where a.f_parent == s.id || a.id == s.id select a.id).Count(),
                        NewMsgCount = (from a in db.msg_messages where (a.f_parent == s.id || a.id == s.id) && (a.b_views == false && a.b_admin == true) select a.id).Count()
                        
                    });
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
            return null;
        }



        public MessagesTheme GetItemMessageTheme(Guid id)
        {
            MessagesTheme model = new MessagesTheme();
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query = db.msg_messages.Where(w => w.f_site == _siteId && w.id == id && w.f_parent == null);

                    if (query.Any())
                    {
                        model.Theme = query.Single().c_theme;
                        model.Id= query.Single().id;

                        //все сообщения от администратора сделаем прочитанными
                        db.msg_messages.Where(w => w.b_admin == true && (w.f_parent==id || w.id==id)).Set(s => s.b_views, true).Update();

                        model.MessageList= db.msg_messages.Where(w => w.f_site == _siteId && (w.id == id || w.f_parent== id))
                                                   .OrderByDescending(o=>o.d_date)
                                                   .Select(s=>new MessagesModel() {
                                                             Text=s.c_text,                                    
                                                             Date=s.d_date,
                                                             Admin=s.b_admin,
                                                             User=s.f_user,
                                                             MsgUser=new UserModel
                                                             {
                                                                 Surname=s.fkuserid.AspNetUserProfilesUserId.Surname,
                                                                 Name=s.fkuserid.AspNetUserProfilesUserId.Name,
                                                                 Id=s.f_user
                                                             }
                                                   }).ToArray();
                        tr.Commit();
                        return model;
                    }
                    
                }
            }
            return null;

        }


        public bool SendMessage(MessagesModel msg)
        {
            using (var db = new CMSdb(_context))
            {
                return db.msg_messages
                         .Value(v => v.f_user, _currentUserId)
                         .Value(v => v.c_text, msg.Text)
                         .Value(v => v.f_parent, msg.Id)
                         .Value(v => v.f_site, _siteId)
                         .Value(v => v.d_date, DateTime.Now)
                         .Insert()>0;
            }            
        }
        public bool CreateNewTheme(MessagesModel msg)
        {
            using (var db = new CMSdb(_context))
            {
                return db.msg_messages
                         .Value(v => v.f_user, _currentUserId)
                         .Value(v => v.c_theme, msg.Theme)
                         .Value(v => v.c_text, msg.Text)                         
                         .Value(v => v.f_site, _siteId)
                         .Value(v => v.d_date, DateTime.Now)
                         .Insert() > 0;
            }
        }

    }
}
