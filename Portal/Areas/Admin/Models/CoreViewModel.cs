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
        /// Текущий домен
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Настройки сайта
        /// </summary>
        public SettingsModel Settings { get; set; }

        /// <summary>
        /// Подключенный пользователь
        /// </summary>
        public AccountModel Account { get; set; }

        /// <summary>
        /// Права пользователя
        /// </summary>
        //public ResolutionsModel UserResolution { get; set; }

        /// <summary>
        /// Меню админки из структуры CMS
        /// </summary>
        public CmsMenuModel[] Menu { get; set; }
        /// <summary>
        /// Меню модулей
        /// </summary>
        public CmsMenuItem[] MenuModul { get; set; }

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