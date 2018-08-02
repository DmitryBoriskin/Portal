using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с Меню (CmsMenu)
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает разделы меню CMS с дочерними элементами
        /// </summary>
        /// <returns></returns>
        public CmsMenuItemModel[] GetCmsMenu()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_menu
                            .Where(s => s.f_parent == null)
                            .OrderBy(s => s.n_sort)
                            .Select(s => new CmsMenuItemModel()
                            {
                                Alias = s.c_alias,
                                Name = s.c_title,
                                ShowInMenu = s.b_show,
                                Childs = s.fk_menu_parent_BackReferences
                                                .OrderBy(m => m.n_sort)
                                                .Select(m => new CmsMenuItemModel()
                                                {
                                                    Alias = m.c_alias,
                                                    Name = m.c_title,
                                                    Icon = m.c_class,
                                                    ShowInMenu = m.b_show
                                                }).ToArray()
                            });

                return data.ToArray();
            }
        }

        /// <summary>
        /// Список меню CMS без разделов
        /// </summary>
        /// <returns></returns>
        public CmsMenuItemModel[] GetCmsMenuItems()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_menu
                            .Where(s => s.f_parent != null)
                            .OrderBy(s => s.n_sort)
                            .Select(s => new CmsMenuItemModel()
                            {
                                Id = s.id,
                                Alias = s.c_alias,
                                Name = s.c_title,
                                Icon = s.c_class,
                                Sort = s.n_sort
                            });

                return data.ToArray();
            }
        }

        public CmsMenuItemModel[] GetModulesMenu(Guid UserId)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_controllers
                           .Where(w => w.f_parent == null)
                           .Where(w => w.id != Guid.Empty)
                           .OrderBy(w => w.n_sort)
                           .Select(s => new CmsMenuItemModel()
                           {
                               Alias = s.c_controller_name,
                               Name = s.c_name,
                               Icon = s.c_class
                           });
                if (data.Any())
                {
                    return data.ToArray();
                }
                return null;
            }
        }

        
        /// <summary>
        /// Единичный элемент cms menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CmsMenuItemModel GetCmsMenuItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_menu.Where(w => w.id == id);
                if (query.Any())
                {
                    var s = query.Single();
                    var data = new CmsMenuItemModel
                    {
                        Id = s.id,
                        Alias = s.c_alias,
                        Icon = s.c_class,                        
                        Name = s.c_title
                    };
                    if (s.f_parent != null)
                    {
                        data.Pid = (Guid)s.f_parent;
                    }
                    return data;
                }
                return null;
            }
        }

       
        /// <summary>
        /// Возвращает true если есть элемент с таким id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckMenuExist(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.core_menu.Where(w => w.id == id).Any();
            }            
        }


        public bool UpdateMenu(CmsMenuItemModel menu)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = menu.Id,
                        PageName = menu.Name,
                        Section = LogModule.Menu,
                        Action = LogAction.update
                    });
                    var q = db.core_menu.Where(w => w.id == menu.Id);
                    if (q.Any())
                    {
                        //влияние n_sort при смене/назначении группы 
                        if(q.Single().f_parent!= menu.Pid){
                            if (q.Single().f_parent != null)
                            {
                                db.core_menu.Where(w => w.f_parent == q.Single().f_parent && w.n_sort>q.Single().n_sort)
                                            .Set(p => p.n_sort, p => p.n_sort - 1)
                                            .Update();
                            }
                        }

                        //определим новый парметр сортровки для текущего значения
                        int newsort = 0;
                        var q2 = db.core_menu.Where(w => w.f_parent == menu.Pid);
                        if (q2.Any())
                        {
                            newsort = (int)q2.Select(s => s.n_sort).Max();
                        }
                        newsort++;

                        bool result = db.core_menu
                                        .Where(w => w.id == menu.Id)
                                        .Set(s => s.c_title, menu.Name)
                                        .Set(s => s.c_alias, menu.Alias)
                                        .Set(s => s.c_class, menu.Icon)
                                        .Set(s => s.f_parent, menu.Pid)
                                        .Set(s => s.n_sort, (q.Single().f_parent==null)? q.Single().n_sort : newsort)
                                        .Update() > 0;
                        tr.Commit();
                        return true;
                    }
                    return false;                    
                }
            }
        }
        public bool InsertMenu(CmsMenuItemModel menu)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr= db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = menu.Id,
                        PageName = menu.Name,
                        Section = LogModule.Menu,
                        Action = LogAction.insert
                    });

                    int sort = 1;
                    if (menu.Pid != null)
                    {
                        var q = db.core_menu.Where(w => w.f_parent == menu.Pid);
                        if (q.Any())                        
                        sort = (int)q.Select(s => s.n_sort).Max() + 1;                                                
                    }
                        
                    bool result=db.core_menu
                                  .Insert(
                                  ()=>new core_menu {
                                      id=menu.Id,
                                      c_title=menu.Name,
                                      c_alias=menu.Alias,
                                      c_class=menu.Icon,
                                      n_sort=sort,
                                      f_parent=(menu.Pid!=null)? menu.Pid:null
                                  })> 0;
                    tr.Commit();
                    return result;
                }
            }
        }

        public bool DeleteMenu(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query = db.core_menu.Where(w => w.id == id);
                    if (query.Any())
                    {
                        var data = query.Single();
                        InsertLog(new LogModel
                        {
                            PageId = id,
                            PageName = data.c_title,
                            Section = LogModule.Menu,
                            Action = LogAction.delete,
                            Comment = "Удален пункт меню" + String.Format("{0}/{1}", data.c_title, data.c_alias)
                        }, data);


                        //смещаем n_sort
                        db.core_menu
                          .Where(w => w.f_parent == query.Single().f_parent && w.n_sort > query.Single().n_sort)
                          .Set(p => p.n_sort, p => p.n_sort - 1)
                          .Update();
                        
                        query.Delete();
                        tr.Commit();
                        return true;
                    }
                }                
            }
            return false;
        }

       

        public bool ChangePositionMenu(Guid id,int new_num)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var query = db.core_menu.Where(w => w.id == id);
                    if (query.Any())
                    {
                        var data = query.Single();
                        Guid Parent = (Guid)data.f_parent;
                        int actual_num = (int)data.n_sort;
                        if(new_num != actual_num)
                        {
                            if (new_num > actual_num)
                            {
                                db.core_menu
                                    .Where(w => w.f_parent == Parent && w.n_sort > actual_num && w.n_sort <= new_num)
                                    .Set(p => p.n_sort, p => p.n_sort - 1)
                                    .Update();
                            }
                            else
                            {
                                db.core_menu
                                    .Where(w => w.f_parent == Parent && w.n_sort < actual_num && w.n_sort >= new_num)
                                    .Set(p => p.n_sort, p => p.n_sort + 1)
                                    .Update();
                            }
                            db.core_menu.Where(w => w.id == id).Set(s => s.n_sort, new_num).Update();
                        }
                        tr.Commit();
                        return true;
                    }                    
                    return false;
                }                    
            }
        }
    }
}