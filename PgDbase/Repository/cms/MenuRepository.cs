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
                                    id=m.id,
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
                        id = s.id,
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
                var data = db.core_menu;
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
    }
}