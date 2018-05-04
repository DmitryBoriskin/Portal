using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с модулями системы и их шаблонами
    /// </summary>
    public partial class CmsRepository
    {
        #region Шаблоны

        /// <summary>
        /// Проверка существует запись с id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TemplateExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_views
                    .Where(t => t.id == id);

                if (data.Any())
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Список шаблонов как массив
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TemplateModel[] GetTemplatesList()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_views
                    .Where(t => t.id != Guid.Empty);

                query = query.OrderBy(t => t.n_sort);

                var list = query
                    .Select(s => new TemplateModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Controller = new ModuleModel()
                        {
                            Id = s.f_controller
                        },
                        ViewPath = s.c_path,
                        Image = s.c_img,

                    });

                return list.ToArray();
            }
        }

        /// <summary>
        /// Возвращает постраничный список шаблонов для контроллеров
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<TemplateModel> GetTemplates(TemplateFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_views
                    .Where(t => t.id != Guid.Empty);

                if (filter.Controller.HasValue)
                    query = query.Where(t => t.f_controller == filter.Controller.Value);

                if (!string.IsNullOrEmpty(filter.SearchText))
                    query = query.Where(t => t.c_name.ToLower().Contains(filter.SearchText.ToLower()));

                query = query.OrderBy(t => t.n_sort);

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new TemplateModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Controller = new ModuleModel()
                        {
                            Id = s.f_controller,
                            Title = db.core_controllers
                                                .Where(c => c.id == s.f_controller)
                                                .First()
                                                .c_name,
                        },
                        ViewPath = s.c_path,
                        Image = s.c_img,

                    });

                return new Paged<TemplateModel>()
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
        }

        /// <summary>
        /// Возвращает постраничный список шаблонов для контроллеров
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TemplateModel GetTemplate(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_views
                    .Where(t => t.id == id);

                var data = query
                    .Select(s => new TemplateModel
                    {
                        Id = s.id,
                        Title = s.c_name,
                        Controller = new ModuleModel()
                        {
                            Id = s.f_controller,
                            Title = db.core_controllers
                                                .Where(c => c.id == s.f_controller)
                                                .First()
                                                .c_name,
                        },
                        ViewPath = s.c_path,
                        Image = s.c_img,

                    });

                return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет запись о шаблоне
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public bool InsertTemplate(TemplateModel template)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var cdTemplate = new core_views
                    {
                        id = template.Id,
                        c_name = template.Title,
                        c_path = template.ViewPath,
                        c_img = template.Image,
                        f_controller = Guid.Empty
                    };
                    db.Insert(cdTemplate);

                    var log = new LogModel
                    {
                        PageId = Guid.NewGuid(),
                        PageName = template.Title,
                        Section = LogModule.Templates,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Изменения модуля
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool UpdateTemplate(TemplateModel template)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_views
                        .Where(s => s.id == template.Id);

                    if (data.Any())
                    {
                        var cdTemplate = data.SingleOrDefault();
                        cdTemplate.c_name = template.Title;
                        cdTemplate.c_path = template.ViewPath;
                        cdTemplate.c_img = template.Image;
                        cdTemplate.f_controller = (template.Controller != null) ? template.Controller.Id : Guid.Empty;

                        db.Update(cdTemplate);

                        var log = new LogModel
                        {
                            PageId = template.Id,
                            PageName = template.Title,
                            Section = LogModule.Templates,
                            Action = LogAction.update
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    };
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаляет шаблон
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteTemplate(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_views
                        .Where(s => s.id == id);

                    if (data.Any())
                    {
                        var cdTemplate = data.Single();
                        db.Delete(cdTemplate);

                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = String.Format("{0} ({1})", cdTemplate.c_name, cdTemplate.c_path),
                            Section = LogModule.Modules,
                            Action = LogAction.delete
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }
        #endregion

        #region Модуль

        /// <summary>
        /// Проверка существует запись с id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ModuleExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_controllers
                    .Where(t => t.id == id);

                if (data.Any())
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Возвращает список модулей как массив
        /// </summary>
        /// <returns></returns>
        public ModuleModel[] GetModulesList()
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_controllers
                     .Where(t => t.id != Guid.Empty);

                query = query.OrderBy(t => t.n_sort);

                var list = query
                    .Select(s => new ModuleModel
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Title = s.c_name,
                        Controller = s.c_controller_name,
                        Action = s.c_action_name,
                        View = s.c_default_view
                    });

                return list.ToArray();
            }
        }

        /// <summary>
        /// Возвращает постраничный список модулей
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<ModuleModel> GetModules(ModuleFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_controllers
                     .Where(t => t.id != Guid.Empty)
                     .Where(t => t.pid != null);

                if (!string.IsNullOrEmpty(filter.SearchText))
                    query = query.Where(t => t.c_name.ToLower().Contains(filter.SearchText) || t.c_desc.ToLower().Contains(filter.SearchText));

                query = query.OrderBy(t => t.n_sort);

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new ModuleModel
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Title = s.c_name,
                        Controller = s.c_controller_name,
                        Action = s.c_action_name,
                        View = s.c_default_view,
                        Desc = s.c_desc
                    });

                return new Paged<ModuleModel>()
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
        }

        /// <summary>
        /// Возвращает модуль
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ModuleModel GetModule(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_controllers
                    .Where(s => s.id == id)
                   .Select(s => new ModuleModel
                   {
                       Id = s.id,
                       ParentId = s.id,
                       Title = s.c_name,
                       Controller = s.c_controller_name,
                       Action = s.c_action_name,
                       View = s.c_default_view,
                       Desc = s.c_desc
                   });
                return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет запись о модуле
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool InsertModule(ModuleModel module)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var cdController = new core_controllers
                    {
                        id = module.Id,
                        pid = module.ParentId,
                        c_name = module.Title,
                        c_controller_name = module.Controller,
                        c_action_name = module.Action,
                        c_default_view = module.View,
                        c_desc = module.Desc
                    };
                    db.Insert(cdController);

                    var log = new LogModel
                    {
                        PageId = module.Id,
                        PageName = module.Title,
                        Section = LogModule.Modules,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Изменения модуля
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool UpdateModule(ModuleModel module)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_controllers
                        .Where(s => s.id == module.Id);

                    if (data.Any())
                    {
                        var cdController = data.SingleOrDefault();
                        cdController.pid = module.ParentId;
                        cdController.c_name = module.Title;
                        cdController.c_controller_name = module.Controller;
                        cdController.c_action_name = module.Action;
                        cdController.c_default_view = module.View;
                        cdController.c_desc = module.Desc;

                        db.Update(cdController);

                        //Если выбрана вьюха, мы ей присваиваем контроллер
                        if (module.View != Guid.Empty)
                        {
                            var cdView = db.core_views
                                  .Where(v => v.id == module.View)
                                  .Single();

                            cdView.f_controller = module.Id;

                            db.Update(cdView);
                        }

                        var log = new LogModel
                        {
                            PageId = module.Id,
                            PageName = module.Title,
                            Section = LogModule.Modules,
                            Action = LogAction.update
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    };
                    return false;
                }
            }
        }

        /// <summary>
        /// Удаляет модуль
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteModule(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {

                    var data = db.core_controllers
                        .Where(s => s.id == id);

                    if (data.Any())
                    {
                        var cdController = data.Single();
                        db.Delete(cdController);

                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = String.Format("{0}/{1}", cdController.c_controller_name, cdController.c_action_name),
                            Section = LogModule.Modules,
                            Action = LogAction.delete,
                            Comment = "Удален модуль" + String.Format("{0}/{1}", cdController.c_controller_name, cdController.c_action_name)
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public SiteModuleModel[] GetSiteModulesList(Guid siteId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_controllers
                     .Where(t => t.id != Guid.Empty);

                var list = query
                    .Select(s => new SiteModuleModel()
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Title = s.c_name,
                        Controller = s.c_controller_name,
                        Action = s.c_action_name,
                        View = (GetSiteModule(siteId,s.id) != null) ? GetSiteModule(siteId,s.id).View : s.c_default_view,
                        Desc = s.c_desc,
                        SiteModuleId = (GetSiteModule(siteId, s.id) != null) ? GetSiteModule(siteId, s.id).Id : (Guid?)null,
                        Checked = (GetSiteModule(siteId,s.id) != null) ? true : false
                    });

                return list.ToArray();
            }
        }

        /// <summary>
        /// Модуль сайта
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public SiteModuleModel GetSiteModule(Guid siteId, Guid moduleId)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_site_controllers
                    .Where(s => s.f_site == siteId)
                    .Where(s => s.f_controller == moduleId)
                     .AsQueryable();

                var data = query
                    .Select(s => new SiteModuleModel()
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Title = s.fksitecontrollerscontrollers.c_name,
                        Controller = s.fksitecontrollerscontrollers.c_controller_name,
                        Action = s.fksitecontrollerscontrollers.c_action_name,
                        View = (s.f_view != null) ? s.f_view.Value : s.fksitecontrollerscontrollers.c_default_view,
                        Desc = s.fksitecontrollerscontrollers.c_desc,
                        Checked = true
                    });

                return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавление Модуля сайту
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public bool BindSiteModule(Guid siteId, Guid moduleId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_site_controllers
                        .Where(s => s.f_site == siteId)
                        .Where(s => s.f_controller == moduleId);

                    if (!data.Any())
                    {
                        var cdSiteController = new core_site_controllers()
                        {
                            id = Guid.NewGuid(),
                            f_site = siteId,
                            f_controller = moduleId
                        };
                        db.Insert(cdSiteController);

                        var module = db.core_controllers
                                        .Where(m => m.id == moduleId)
                                        .Single();

                        var log = new LogModel
                        {
                            PageId = siteId,
                            PageName = module.c_name,
                            Section = LogModule.Sites,
                            Action = LogAction.update,
                            Comment = "Сайту включен модуль " + module.c_controller_name + "/" + module.c_action_name
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public bool UnBindSiteModule(Guid siteId, Guid moduleId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_site_controllers
                        .Where(s => s.f_site == siteId)
                        .Where(s => s.f_controller == moduleId);

                    if (data.Any())
                    {
                        var cdSiteModule = data.Single();
                        db.Delete(cdSiteModule);

                        var module = db.core_controllers
                                       .Where(m => m.id == moduleId)
                                       .Single();

                        var log = new LogModel
                        {
                            PageId = siteId,
                            PageName = module.c_name,
                            Section = LogModule.Sites,
                            Action = LogAction.update,
                            Comment = "Сайту отключен модуль " + module.c_controller_name + "/" + module.c_action_name
                        };
                        InsertLog(log);

                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        #endregion
    }
}
