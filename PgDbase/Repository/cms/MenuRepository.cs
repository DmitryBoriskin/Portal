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
                                Alias = s.c_alias,
                                GroupName = s.c_title,
                                GroupItems = s.fk_menu_parent_BackReferences.Select(m => new CmsMenuItem()
                                {
                                    Alias = m.c_alias,
                                    Title = m.c_title,
                                    Class = m.c_class
                                }).ToArray()
                            });
                if (data.Any()) return data.ToArray();
                return null;
            }
        }
    }
}
