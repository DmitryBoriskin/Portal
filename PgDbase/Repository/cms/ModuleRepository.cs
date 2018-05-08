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
                    .Select(s => new TemplateModel()
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
                    .Take(filter.Size).Select(s => new TemplateModel()
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
                    .Select(s => new TemplateModel()
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
                    var cdTemplate = new core_views()
                    {
                        id = template.Id,
                        c_name = template.Title,
                        c_path = template.ViewPath,
                        c_img = template.Image,
                        f_controller = Guid.Empty
                    };
                    db.Insert(cdTemplate);

                    var log = new LogModel()
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

                        var log = new LogModel()
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

                        var log = new LogModel()
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

        #region Модуль и его компоненты

        /// <summary>
        /// Проверка: существует ли модуль или компонент с id
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
                     .Where(t => t.id != Guid.Empty)
                     .Where(t => t.f_parent == null);

                query = query.OrderBy(t => t.n_sort);

                var list = query
                    .Select(s => new ModuleModel()
                    {
                        Id = s.id,
                        Title = s.c_name,
                        ControllerName = s.c_controller_name,
                        ModuleParts = db.core_controllers
                                        .Where(m => m.f_parent == s.id)
                                        .Select(m => new ModuleModel()
                                        {
                                            Id = m.id,
                                            Title = m.c_name,
                                            ControllerName = m.c_controller_name,
                                            ActionName = m.c_action_name,
                                            ParentId = m.f_parent,
                                            Desc = m.c_desc,
                                            View = m.c_default_view
                                        }).ToArray()
                    });

                return list.ToArray();
            }
        }

        /// <summary>
        /// Возвращает постраничный список модулей с компонентами
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<ModuleModel> GetModules(ModuleFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_controllers
                     .Where(t => t.id != Guid.Empty)
                     .Where(t => t.f_parent == null);

                if (!string.IsNullOrEmpty(filter.SearchText))
                    query = query.Where(t => t.c_name.ToLower().Contains(filter.SearchText) || t.c_desc.ToLower().Contains(filter.SearchText));

                query = query.OrderBy(t => t.n_sort);

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new ModuleModel()
                    {
                        Id = s.id,
                        Title = s.c_name,
                        ControllerName = s.c_controller_name,
                        ModuleParts = db.core_controllers
                                        .Where(m => m.f_parent == s.id)
                                        .Select(m => new ModuleModel()
                                        {
                                            Id = m.id,
                                            Title = m.c_name,
                                            ControllerName = m.c_controller_name,
                                            ActionName = m.c_action_name,
                                            ParentId = m.f_parent,
                                            Desc = m.c_desc,
                                            View = m.c_default_view
                                        }).ToArray()
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
        /// Возвращает модуль, либо компонент
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ModuleModel GetModule(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_controllers
                    .Where(s => s.id == id)
                   .Select(s => new ModuleModel()
                   {
                       Id = s.id,
                       ParentId = s.f_parent,
                       Title = s.c_name,
                       ControllerName = s.c_controller_name,
                       ActionName = s.c_action_name,
                       View = s.c_default_view,
                       Desc = s.c_desc,
                       ModuleParts = db.core_controllers
                                        .Where(m => m.f_parent == s.id)
                                        .Select(m => new ModuleModel()
                                        {
                                            Id = m.id,
                                            Title = m.c_name,
                                            ControllerName = m.c_controller_name,
                                            ActionName = m.c_action_name,
                                            ParentId = m.id,
                                            Desc = m.c_desc,
                                            View = m.c_default_view
                                        }).ToArray()
                   });
                return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет модуль или компонент
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool InsertModule(ModuleModel module)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var cdModule = new core_controllers()
                    {
                        id = module.Id,
                        c_name = module.Title,
                        c_controller_name = module.ControllerName
                    };

                    //Если это компонент модуля
                    if (module.ParentId.HasValue)
                    {
                        //Инфо о модуле, которому принадлежит компонент
                        var parentModuleData = db.core_controllers
                            .Where(m => m.id == module.ParentId.Value);

                        if (!parentModuleData.Any())
                            return false;

                        var parentModule = parentModuleData.Single();

                        cdModule.f_parent = module.ParentId;
                        cdModule.c_action_name = module.ActionName;
                        cdModule.c_default_view = module.View;
                        cdModule.c_desc = module.Desc;

                        //Не разрешаем вводить имя контроллера, отличное от родительского
                        cdModule.c_controller_name = parentModule.c_controller_name;


                        //Устанавливаем выбранному шаблону тип
                        #region

                        var cdViewData = db.core_views
                              .Where(v => v.id == module.View);

                        if (!cdViewData.Any())
                            return false;

                        var cdView = cdViewData.Single();
                        cdView.f_controller = module.Id;

                        db.Update(cdView);

                        #endregion

                        //Дополнительное логирование
                        #region

                        var parentlog = new LogModel()
                        {
                            PageId = parentModule.id,
                            PageName = module.Title,
                            Section = LogModule.Modules,
                            Action = LogAction.insert,
                            Comment = $"Добавлен компонент '{module.Title}' ({module.ControllerName}/{module.ActionName})"
                                        + " к модулю '{parentModule.c_name}' ({parentModule.c_controller_name})"
                        };
                        InsertLog(parentlog);
                        #endregion

                    }

                    db.Insert(cdModule);

                    //Логируем
                    #region

                    var log = new LogModel()
                    {
                        PageId = module.Id,
                        PageName = module.Title,
                        Section = LogModule.Modules,
                        Action = LogAction.insert,
                        Comment = $"Добавлен модуль '{module.Title}' ({module.ControllerName})"
                    };
                    InsertLog(log);

                    #endregion

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Изменение модуля или компонента
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

                        cdController.c_name = module.Title;
                        cdController.c_controller_name = module.ControllerName;

                        if (module.ParentId.HasValue)
                        {
                            //Поля которые нельзя менять для компонента модуля
                            //cdController.f_parent;
                            //cdController.c_controller_name;

                            cdController.c_name = module.Title;
                            cdController.c_action_name = module.ActionName;
                            cdController.c_default_view = module.View;
                            cdController.c_desc = module.Desc;


                            //Устанавливаем выбранному шаблону тип модуля
                            #region

                            var cdViewData = db.core_views
                                  .Where(v => v.id == module.View);

                            if (!cdViewData.Any())
                                return false;

                            var cdView = cdViewData.Single();
                            cdView.f_controller = module.Id;

                            db.Update(cdView);

                            #endregion

                            //Дополнительное логирование
                            #region

                            //Инфо о модуле, которому принадлежит компонент
                            var parentModuleData = db.core_controllers
                                .Where(m => m.id == module.ParentId.Value);

                            if (parentModuleData.Any())
                            {
                                var parentModule = parentModuleData.Single();

                                var parentlog = new LogModel()
                                {
                                    PageId = parentModule.id,
                                    PageName = module.Title,
                                    Section = LogModule.Modules,
                                    Action = LogAction.insert,
                                    Comment = $"Изменен компонент '{module.Title}' ({module.ControllerName}/{module.ActionName})"
                                                + " к модулю '{parentModule.c_name}' ({parentModule.c_controller_name})"
                                };
                                InsertLog(parentlog);
                            }

                            #endregion
                        }

                        db.Update(cdController);

                        //Логирование
                        #region

                        var log = new LogModel()
                        {
                            PageId = module.Id,
                            PageName = module.Title,
                            Section = LogModule.Modules,
                            Action = LogAction.update,
                            Comment = $"Изменен модуль '{module.Title}' ({module.ControllerName})"
                        };
                        InsertLog(log);

                        #endregion

                        tran.Commit();
                        return true;
                    }

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
                        //Не удаляем модуль, пока у него есть компоненты
                        if (data.Any(s => s.id == s.fkcontrollerparentid.f_parent))
                            return false;

                        var cdController = data.Single();

                        //При удалении модуля, у всех привязанных шаблонов изменяем тип на дефолтный
                        #region

                        var cdViewData = db.core_views
                              .Where(v => v.id == cdController.c_default_view)
                              .Set(v => v.f_controller, Guid.Empty)
                              .Update();

                        #endregion

                        //Дополнительное логирование
                        #region

                        //Инфо о модуле, которому принадлежит компонент
                        var parentModuleData = db.core_controllers
                            .Where(m => m.id == cdController.f_parent);

                        if (parentModuleData.Any())
                        {
                            var parentModule = parentModuleData.Single();

                            var parentlog = new LogModel()
                            {
                                PageId = parentModule.id,
                                PageName = cdController.c_name,
                                Section = LogModule.Modules,
                                Action = LogAction.insert,
                                Comment = $"Удален компонент '{cdController.c_name}' ({cdController.c_controller_name}/{cdController.c_action_name})"
                                        + " модуля '{parentModule.c_name}'({ parentModule.c_controller_name })"
                            };
                            InsertLog(parentlog);
                        }

                        #endregion

                        db.Delete(cdController);

                        //Логируем
                        #region

                        var log = new LogModel()
                        {
                            PageId = id,
                            PageName = cdController.c_name,
                            Section = LogModule.Modules,
                            Action = LogAction.delete,
                            Comment = $"Удален модуль '{cdController.c_name}' ({cdController.c_controller_name})"
                        };
                        InsertLog(log);
                        #endregion

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
                var query = db.core_site_controllers
                     .Where(t => t.f_site == siteId)
                     .Where(t => t.fksitecontrollerscontrollers.f_parent == null);

                var list = query
                    .Select(s => new SiteModuleModel()
                    {
                        Id = s.id,
                        SiteId = siteId,
                        ModuleId = s.f_controller,
                        Title = s.fksitecontrollerscontrollers.c_name,
                        ControllerName = s.fksitecontrollerscontrollers.c_controller_name,
                        ActionName = s.fksitecontrollerscontrollers.c_action_name,
                        Desc = s.fksitecontrollerscontrollers.c_desc,
                        View = (s.f_view != null) ? s.f_view.Value : s.fksitecontrollerscontrollers.c_default_view,
                        ModuleParts = db.core_site_controllers
                                        .Where(m => m.fksitecontrollerscontrollers.f_parent == s.fksitecontrollerscontrollers.id)
                                        .Where(m => m.f_site == s.f_site)
                                        .Select(m => new ModuleModel()
                                        {
                                            Id = m.id,
                                            Title = m.fksitecontrollerscontrollers.c_name,
                                            ControllerName = m.fksitecontrollerscontrollers.c_controller_name,
                                            ActionName = m.fksitecontrollerscontrollers.c_action_name,
                                            ParentId = m.fksitecontrollerscontrollers.f_parent,
                                            Desc = m.fksitecontrollerscontrollers.c_desc,
                                            View = (m.f_view != null) ? m.f_view.Value : m.fksitecontrollerscontrollers.c_default_view
                                        }).ToArray()
                    });

                return list.ToArray();
            }
        }

        /// <summary>
        /// Модуль сайта
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public SiteModuleModel GetSiteModule(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_site_controllers
                    .Where(s => s.id == id);

                var data = query
                    .Select(s => new SiteModuleModel()
                    {
                        Id = s.id,
                        SiteId = s.f_site,
                        ModuleId = s.f_controller,
                        Title = s.fksitecontrollerscontrollers.c_name,
                        ControllerName = s.fksitecontrollerscontrollers.c_controller_name,
                        ActionName = s.fksitecontrollerscontrollers.c_action_name,
                        Desc = s.fksitecontrollerscontrollers.c_desc,
                        View = (s.f_view != null)? s.f_view.Value : s.fksitecontrollerscontrollers.c_default_view,
                        ModuleParts = db.core_site_controllers
                                        .Where(m => m.fksitecontrollerscontrollers.f_parent == s.fksitecontrollerscontrollers.id)
                                        .Where(m => m.f_site == s.f_site)
                                        .Select(m => new ModuleModel()
                                        {
                                            Id = m.id,
                                            Title = m.fksitecontrollerscontrollers.c_name,
                                            ControllerName = m.fksitecontrollerscontrollers.c_controller_name,
                                            ActionName = m.fksitecontrollerscontrollers.c_action_name,
                                            ParentId = m.fksitecontrollerscontrollers.id,
                                            Desc = m.fksitecontrollerscontrollers.c_desc,
                                            View = (m.f_view != null) ? m.f_view.Value : m.fksitecontrollerscontrollers.c_default_view
                                        }).ToArray()

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
        public bool InsertSiteModuleLink(Guid siteId, Guid moduleId)
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
                        var cdSiteModule = new core_site_controllers()
                        {
                            id = Guid.NewGuid(),
                            f_site = siteId,
                            f_controller = moduleId
                        };
                        db.Insert(cdSiteModule);

                        //Переносим компоненты модуля
                        var cdSiteModuleParts = db.core_controllers
                            .Where(p => p.f_parent == moduleId)
                            .Select(p => new core_site_controllers() {
                                id = Guid.NewGuid(),
                                f_site = siteId,
                                f_controller = p.id
                            });

                        if(cdSiteModuleParts.Any())
                            foreach(var modulePart in cdSiteModuleParts.ToArray())
                            {
                                db.Insert(modulePart);
                            }

                        //Доп инфа для логирования
                        var module = db.core_controllers
                                        .Where(m => m.id == moduleId)
                                        .Single();

                        var log = new LogModel()
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
        /// Изменение модуля или компонента
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool SetSiteModuleTemplateDefault(Guid id, Guid templateId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_site_controllers
                        .Where(s => s.id == id);

                    if (data.Any())
                    {
                        var cdSiteController = data.SingleOrDefault();
                        cdSiteController.f_view = templateId;

                        db.Update(cdSiteController);

                        //Логирование
                        #region

                        //var log = new LogModel()
                        //{
                        //    PageId = id,
                        //    PageName = module.Title,
                        //    Section = LogModule.Modules,
                        //    Action = LogAction.update,
                        //    Comment = $"Изменен модуль '{module.Title}' ({module.ControllerName})"
                        //};
                        //InsertLog(log);

                        #endregion

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
        public bool DeleteSiteModuleLink(Guid linkId)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_site_controllers
                        .Where(s => s.id == linkId);

                    if (data.Any())
                    {
                        var cdSiteModule = data.Single();
                        db.Delete(cdSiteModule);

                        //Удаляем компоненты модуля
                        var cdSiteModulePartsId = db.core_controllers
                            .Where(p => p.f_parent == cdSiteModule.f_controller)
                            .Select(p => p.id);

                        if (cdSiteModulePartsId.Any())
                            db.core_site_controllers
                                .Where(p => p.f_site == cdSiteModule.f_site)
                                .Where(p => cdSiteModulePartsId.Contains(p.f_controller))
                                .Delete();

                       //Доп инфа для логирования
                        var module = db.core_controllers
                                       .Where(m => m.id == cdSiteModule.f_controller)
                                       .Single();

                        var log = new LogModel()
                        {
                            PageId = cdSiteModule.f_site,
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
