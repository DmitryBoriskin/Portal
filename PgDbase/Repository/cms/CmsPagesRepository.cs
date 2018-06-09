using LinqToDB;
using LinqToDB.Data;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.cms
{
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает постраничный список эл-тов карты сайта
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public PageModel[] GetPages(PageFilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                var q = db.core_pages
                    .Where(w => w.fkpagesites.id == _siteId);

                if (filter.GroupId != null)
                {
                    q = q.Join(db.core_page_group_links.Where(w => w.f_page_group == filter.GroupId), n => n.gid, m => m.f_page, (n, m) => new { n, m })
                         .OrderBy(o => o.m.n_sort)
                         .Select(s => s.n);
                }
                else
                {
                    q=q.Where(w => w.pgid == filter.Parent).OrderBy(o => o.n_sort);
                }
                return q                    
                    .Select(s => new PageModel
                    {
                        Id = s.gid,
                        Name = s.c_name,
                        ParentId = s.pgid,
                        Sort = s.n_sort,
                        IsDisabled = s.b_disabled,
                        CountChilds = db.core_pages
                                        .Where(w => w.pgid == s.gid)
                                        .Count()
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает эл-т карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PageModel GetPage(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_pages
                    .Where(w => w.gid == id)
                    .Select(s => new PageModel
                    {
                        Id = s.gid,
                        Name = s.c_name,
                        ParentId = s.pgid,
                        Path = s.c_path,
                        Alias = s.c_alias,
                        Text = s.c_text,
                        Url = s.c_url,
                        IsDisabled = s.b_disabled,
                        IsDeleteble = s.b_deleteble,
                        Keywords = s.c_keyw,
                        Desc = s.c_desc,
                        SiteController = s.f_sites_controller,
                        Childrens = GetPages(new PageFilterModel { Parent = s.gid }),
                        MenuGroups = s.fkpagegrouplinkpages.Select(g => g.f_page_group).ToArray()
                    }).SingleOrDefault();
            }
        }

        /// <summary>
        /// Возвращает родительский идентификатор
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Guid? GetPageParentId(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_pages
                    .Where(w => w.gid == id)
                    .Select(s => s.pgid)
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет эл-т карты сайта
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool InsertPage(PageModel page)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = page.Id,
                        PageName = page.Name,
                        Section = LogModule.Pages,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    var maxSort = db.core_pages
                        .Where(w => w.f_site == _siteId)
                        .Where(w => w.pgid == page.ParentId);

                    int sort = maxSort.Any() ? maxSort.Select(s => s.n_sort).Max() + 1 : 1;

                    bool result = db.core_pages.Insert(() => new core_pages
                    {
                        gid = page.Id,
                        c_name = page.Name,
                        pgid = page.ParentId,
                        c_path = page.Path,
                        c_alias = page.Alias,
                        c_text = page.Text,
                        c_url = page.Url,
                        n_sort = sort,
                        f_site = _siteId,
                        b_disabled = page.IsDisabled,
                        c_keyw = page.Keywords,
                        c_desc = page.Desc,
                        f_sites_controller = page.SiteController
                    }) > 0;

                    // группы меню
                    if (page.MenuGroups != null)
                    {
                        List<core_page_group_links> groups = new List<core_page_group_links>();
                        foreach (var g in page.MenuGroups)
                        {                            
                            var _q = db.core_page_group_links.Where(w => w.f_page_group == g).Select(s => s.n_sort);
                            int _sort = _q.Any() ? _q.Max() : 0;
                            _sort++;

                            groups.Add(new core_page_group_links
                            {
                                id = Guid.NewGuid(),
                                f_page = page.Id,
                                f_page_group = g,
                                n_sort=_sort
                                
                            });
                        }
                        db.BulkCopy(groups);
                    }

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновление эл-та карты сайта
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool UpdatePage(PageModel page)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = page.Id,
                        PageName = page.Name,
                        Section = LogModule.Pages,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.core_pages
                        .Where(w => w.gid == page.Id)
                        .Set(s => s.c_name, page.Name)
                        .Set(s => s.pgid, page.ParentId)
                        //.Set(s => s.c_path, page.Path)
                        .Set(s => s.c_alias, page.Alias)
                        .Set(s => s.c_text, page.Text)
                        .Set(s => s.c_url, page.Url)
                        .Set(s => s.b_disabled, page.IsDisabled)
                        .Set(s => s.c_keyw, page.Keywords)
                        .Set(s => s.c_desc, page.Desc)
                        .Set(s => s.f_sites_controller, page.SiteController)
                        .Update() > 0;

                    // группы меню
                    if (page.MenuGroups != null)
                    {
                        //группы меню к которым элемент уже прицпелен
                        var GroupExist = db.core_page_group_links.Where(w => w.f_page == page.Id);

                        #region удаляем привязки к группам меню, раннее существовавших а теперь нет
                        if (GroupExist.Any())
                        {
                            var Elements = GroupExist.ToArray().Select(s => s.f_page_group).Except<Guid>(page.MenuGroups).ToArray<Guid>();

                            if (Elements.Any())
                            {
                                foreach (Guid PageGroup in Elements)
                                {
                                    var delelement = db.core_page_group_links.Where(w => w.f_page_group == PageGroup && w.f_page == page.Id);
                                    //смещаем sort
                                    db.core_page_group_links
                                      .Where(w => w.f_page_group == PageGroup && w.n_sort > delelement.Single().n_sort)
                                      .Set(p => p.n_sort, p => p.n_sort - 1)
                                      .Update();
                                    delelement.Delete();
                                }
                            }                            
                        }                        
                        #endregion



                        if (GroupExist.Any())
                        {
                            foreach (var g in page.MenuGroups)
                            {
                                if (GroupExist.Where(w => w.f_page_group == g).Any())
                                {

                                }
                                else
                                {
                                    var _q = db.core_page_group_links.Where(w => w.f_page_group == g).Select(s => s.n_sort);
                                    int _sort = _q.Any() ? _q.Max() : 0;
                                    _sort++;

                                    db.core_page_group_links.Insert(() => new core_page_group_links
                                    {
                                        f_page = page.Id,
                                        f_page_group = g,
                                        n_sort= _sort
                                    });
                                }
                            }
                        }
                        else
                        {
                            //случай когда раньше элемент к никакой группе не принадлежал
                            List<core_page_group_links> groups = new List<core_page_group_links>();
                            foreach (var g in page.MenuGroups)
                            {
                                var _q = db.core_page_group_links.Where(w => w.f_page_group == g).Select(s => s.n_sort);
                                int _sort = _q.Any() ? _q.Max() : 0;
                                _sort++;

                                db.core_page_group_links.Insert(() => new core_page_group_links
                                {
                                    f_page = page.Id,
                                    f_page_group = g,
                                    n_sort = _sort
                                });                                
                            }                            
                        }
                    }
                    else
                    {
                        db.core_page_group_links.Where(w => w.f_page == page.Id).Delete();
                    }

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Удаляет эл-т карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeletePage(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    string result = null;

                    var page = db.core_pages.Where(w => w.gid == id).SingleOrDefault();
                    if (page != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = page.c_name,
                            Section = LogModule.Pages,
                            Action = LogAction.delete
                        };
                        InsertLog(log, page);


                        #region смещаем n_sort
                        //в карте сайта
                        db.core_pages
                            .Where(w => w.pgid==page.pgid && w.f_site==_siteId && w.n_sort>page.n_sort)
                            .Set(p => p.n_sort, p => p.n_sort - 1)
                            .Update();

                        //в группах меню
                        var pagegrouplist = db.core_page_group_links.Where(w => w.f_page == id).ToArray();
                        if (pagegrouplist.Any())
                        {
                            foreach (var item in pagegrouplist)
                            {
                                db.core_page_group_links
                                  .Where(w=>w.f_page_group==item.f_page_group && w.n_sort>item.n_sort)
                                  .Set(p => p.n_sort, p => p.n_sort - 1)
                                  .Update();
                            }
                        }
                        

                        #endregion

                        #region удаляем дочерние элементы
                        var childid = db.core_pages.Where(w => w.pgid == page.gid).Select(s => s.gid);
                        if (childid.Any())
                            foreach (var item in childid.ToArray())
                            {
                                DeletePage(item);
                            }
                        #endregion



                        db.Delete(page);

                        result = page.pgid != null? $"item/{page.pgid.ToString()}" : null;
                        tr.Commit();
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Возвращает хлебные крошки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GroupsModel[] GetBreadCrumbs(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                List<GroupsModel> list = new List<GroupsModel>();
                var parent = db.core_pages.Where(w => w.gid == id).Select(s => s.pgid).SingleOrDefault();
                if (parent.HasValue)
                {
                    var item = GetBreadCrumb(parent.Value, db);
                    while (item != null && item.Id != Guid.Empty)
                    {
                        list.Add(item);
                        item = GetBreadCrumb(item.Parent, db);
                    }

                    list.Reverse();
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Возвращает эл-т хлебной крошки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private GroupsModel GetBreadCrumb(Guid? id, CMSdb db)
        {
            return db.core_pages
                .Where(w => w.gid == id)
                .Select(s => new GroupsModel
                {
                    Id = s.gid,
                    Title = s.c_name,
                    Parent = s.pgid
                }).SingleOrDefault();
        }

        /// <summary>
        /// Проверяет существование эл-ты карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckPageExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_pages
                    .Where(w => w.gid == id).Any();
            }
        }
        /// <summary>
        /// Проверяем есть ли на этом уровне такой же алиас
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ChechPageAlias(string path, string alias,Guid id) {
            using (var db = new CMSdb(_context))
            {
                return db.core_pages
                         .Where(w => w.f_site == _siteId && w.c_path==path && w.c_alias==alias)
                         .Where(w=>w.gid!=id)
                         .Any();                
            }
        }

        /// <summary>
        /// Возвращает эл-ты для фильтра групп меню
        /// </summary>
        /// <returns></returns>
        public GroupsModel[] GetPageGroups()
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_page_groups
                    .Where(w => w.f_site == _siteId)
                    .OrderBy(o => o.n_sort)
                    .Select(s => new GroupsModel
                    {
                        Id = s.id,
                        Title = s.c_name
                    }).ToArray();
            }
        }

        /// <summary>
        /// Возвращает группу меню карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GroupsModel GetPageGroup(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_page_groups
                    .Where(w => w.id == id)
                    .Select(s => new GroupsModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Alias=s.c_alias
                    })
                    .SingleOrDefault();
            }
        }

        /// <summary>
        /// Сохраняет меню карты сайта
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool SavePageGroup(GroupsModel item)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_page_groups
                    .Where(w => w.f_site == _siteId);

                if (query.Where(w => w.id == item.Id).Any())
                {
                    return query
                        .Set(s => s.c_name, item.Title)
                        .Set(s => s.c_alias, item.Alias.ToLower())
                        .Update() > 0;
                }
                else
                {
                    int sort = query.Any() ? query.Select(s => s.n_sort).Max() : 1;
                    sort++;
                    return db.core_page_groups.Insert(() => new core_page_groups
                    {
                        id = item.Id,
                        c_name = item.Title,
                        c_alias=item.Alias.ToLower(),
                        f_site = _siteId,
                        n_sort = sort
                    }) > 0;
                }
            }
        }

        /// <summary>
        /// Удаляет меню карты сайта
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePageGroup(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var q = db.core_page_groups.Where(w => w.id == id);

                    db.core_page_groups.Where(w=>w.f_site==_siteId && w.n_sort>q.Single().n_sort)
                      .Set(p => p.n_sort, p => p.n_sort - 1)
                      .Update();

                    var result= q.Delete() > 0;
                    tr.Commit();

                    return result;
                }                    
            }
        }
        public bool ChangePositionPagesGroup(Guid id, int new_num, string group)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var groupid =Guid.Parse(group);
                    var q = db.core_page_group_links.Where(w => w.f_page_group == groupid && w.f_page==id);
                    if (q.Any())
                    {
                        var data = q.Single();
                        int actualNum = data.n_sort;
                        if (new_num != actualNum)
                        {
                            if (new_num > actualNum)
                            {
                                db.core_page_group_links
                                  .Where(w => w.f_page_group == groupid)
                                  .Where(w => w.n_sort > actualNum)
                                  .Where(w => w.n_sort <= new_num)
                                  .Set(s => s.n_sort, s => s.n_sort - 1)
                                  .Update();
                            }
                            else
                            {
                                db.core_page_group_links
                                  .Where(w => w.f_page_group == groupid)
                                  .Where(w => w.n_sort < actualNum)
                                  .Where(w => w.n_sort >= new_num)
                                  .Set(s => s.n_sort, s => s.n_sort + 1)
                                  .Update();
                            }
                            var result= db.core_page_group_links
                                          .Where(w => w.id == data.id)
                                          .Set(s => s.n_sort, new_num)
                                          .Update() > 0;
                            tr.Commit();
                            return result;
                        }

                    }
                    return false;
                }
            }
        }
        /// <summary>
        /// Меняет порядок сортировки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newNum"></param>
        /// <returns></returns>
        public bool ChangePositionPages(Guid id, int newNum)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = false;

                    var query = db.core_pages.Where(w => w.gid == id);
                    if (query.Any())
                    {
                        var data = query.SingleOrDefault();
                        Guid? parent = data.pgid; 
                        int actualNum = data.n_sort;
                        if (newNum != actualNum)
                        {
                            if (newNum > actualNum)
                            {
                                db.core_pages
                                    .Where(w => w.pgid == parent)
                                    .Where(w => w.n_sort > actualNum)
                                    .Where(w => w.n_sort <= newNum)
                                    .Where(w => w.f_site == _siteId)
                                    .Set(s => s.n_sort, s => s.n_sort - 1)
                                    .Update();
                            }
                            else
                            {
                                db.core_pages
                                    .Where(w => w.pgid == parent)
                                    .Where(w => w.n_sort < actualNum)
                                    .Where(w => w.n_sort >= newNum)
                                    .Where(w => w.f_site == _siteId)
                                    .Set(s => s.n_sort, s => s.n_sort + 1)
                                    .Update();
                            }
                            result = db.core_pages
                                .Where(w => w.gid == id)
                                .Set(s => s.n_sort, newNum)
                                .Update() > 0;

                        }
                    }

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Возвращает название страницы по псевдониму
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public string GetPageName(string alias)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_menu
                    .Where(w => w.c_alias.ToLower() == alias.ToLower())
                    .Select(s => s.c_title);
                   

                return data.SingleOrDefault();
            }
        }
    }
}
