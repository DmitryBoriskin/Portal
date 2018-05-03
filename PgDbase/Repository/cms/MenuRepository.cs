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
        public CmsMenuModel[] GetCmsMenu()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_menu
                            .Where(s => s.f_parent == null)
                            .Select(s => new CmsMenuModel()
                            {
                                Id=s.id,
                                Alias = s.c_alias,
                                GroupName = s.c_title,
                                GroupItems = s.fk_menu_parent_BackReferences.Select(m => new CmsMenuItem()
                                {
                                    Id=m.id,
                                    Alias = m.c_alias,
                                    Title = m.c_title,
                                    Class = m.c_class
                                }).ToArray()
                            });
                if (data.Any()) return data.ToArray();
                return null;
            }
        }


        /// <summary>
        /// Единичный элемент cms menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CmsMenuItem GetCmsMenuItem(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_menu.Where(w => w.id == id);
                if (query.Any())
                {
                    var s = query.Single();
                    var data = new CmsMenuItem
                    {
                        Id = s.id,
                        Alias = s.c_alias,
                        Class = s.c_class,                        
                        Title = s.c_title
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
        public CmsMenuModel[] GetMenuGroup()
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_menu.Where(w=>w.f_parent==null);
                if (data.Any())
                {
                    return data.OrderBy(o => o.n_sort)
                               .Select(s => new CmsMenuModel
                               {
                                   Id = s.id,
                                   GroupName = s.c_title
                               }).ToArray();
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


        public bool UpdateMenu(CmsMenuItem menu)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = menu.Id,
                        PageName = menu.Title,
                        Section = LogModule.Menu,
                        Action = LogAction.update
                    });

                    bool result = db.core_menu
                                  .Where(w => w.id == menu.Id)
                                  .Set(s => s.c_title, menu.Title)
                                  .Set(s => s.c_alias, menu.Alias)
                                  .Set(s => s.c_class, menu.Class)
                                  .Set(s => s.f_parent, menu.Pid)
                                  .Update()>0;
                    tr.Commit();
                    return true;
                }
            }
        }
        public bool InsertMenu(CmsMenuItem menu)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr= db.BeginTransaction())
                {
                    InsertLog(new LogModel
                    {
                        PageId = menu.Id,
                        PageName = menu.Title,
                        Section = LogModule.Menu,
                        Action = LogAction.update
                    });
                    int sort = 1;
                    bool result=db.core_menu
                                  .Insert(
                                  ()=>new core_menu {
                                      id=menu.Id,
                                      c_title=menu.Title,
                                      c_alias=menu.Alias,
                                      c_class=menu.Class,
                                      n_sort=sort,
                                      f_parent=(menu.Pid!=null)? menu.Pid:null
                                  })> 0;
                    tr.Commit();
                    return result;
                }
            }
        }
    }
}