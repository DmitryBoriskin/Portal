using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с модулями системы
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Возвращает постраничный список модулей
        /// </summary>
        /// <returns></returns>
        public Paged<ModuleModel> GetModules(ModuleFilter filter)
        {
            using (var db = new CMSdb(_context))
            {
                var query = db.core_controllers.AsQueryable();

                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size).Select(s => new ModuleModel
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Name = s.c_name,
                        Controller = s.c_controller_name,
                        Action = s.c_action_name,
                        View = s.c_default_view
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
                   .Select(s => new ModuleModel
                    {
                        Id = s.id,
                        ParentId = s.id,
                        Name = s.c_name,
                        Controller = s.c_controller_name,
                        Action = s.c_action_name,
                        View = s.c_default_view
                    });
                return data.SingleOrDefault();
            }
        }

        /// <summary>
        /// Добавляет пользователя
        /// </summary>
        /// <param name="user"></param>
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
                        c_name = module.Name,
                        c_controller_name = module.Controller,
                        c_action_name = module.Action,
                        c_default_view = module.View
                    };
                    db.Insert(cdController);

                    var log = new LogModel
                    {
                        PageId = Guid.NewGuid(),
                        PageName = module.Name,
                        Section = LogModule.Module,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    tran.Commit();
                    return true;
                }
            }
        }

        /// <summary>
        /// Обновляет пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateModule(ModuleModel module)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tran = db.BeginTransaction())
                {
                    var data = db.core_controllers
                        .Where(s => s.id == module.Id);

                    if(data.Any())
                    {
                        var cdController = data.SingleOrDefault();
                        cdController.id = module.Id;
                        cdController.pid = module.ParentId;
                        cdController.c_name = module.Name;
                        cdController.c_controller_name = module.Controller;
                        cdController.c_action_name = module.Action;
                        cdController.c_default_view = module.View;

                        db.Update(cdController);


                        var log = new LogModel
                        {
                            PageId = Guid.NewGuid(),
                            PageName = module.Name,
                            Section = LogModule.Module,
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
        /// Удаляет пользователя
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
                            PageId = Guid.NewGuid(),
                            PageName = String.Format("{0}/{1}", cdController.c_controller_name, cdController.c_action_name),
                            Section = LogModule.Module,
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

    }
}
