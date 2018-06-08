using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Основная модель для представлений
    /// </summary>
    public class CoreViewModel
    {
        /// <summary>
        /// Сайт
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Название контроллера
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Название Экшена
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Название страницы
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Настройки сайта
        /// </summary>
        public SettingsModel Settings { get; set; }

        /// <summary>
        /// Сайты
        /// </summary>
        public RoleModel[] Sites { get; set; }

        /// <summary>
        /// Меню админки из структуры CMS
        /// </summary>
        public CmsMenuItemModel[] MenuCMS { get; set; }

        /// <summary>
        /// Меню модулей
        /// </summary>
        public CmsMenuItemModel[] MenuModules { get; set; }

        /// <summary>
        /// Логи, последние н записей
        /// </summary>
        //public cmsLogModel Log { get; set; }

        /// <summary>
        /// Ошибка
        /// </summary>
        public ErrorMessage ErrorInfo { get; set; }
    }
}