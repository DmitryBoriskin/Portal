using System;

namespace PgDbase.entity.cms
{
    /// <summary>
    /// Лог
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        public Guid User { get; set; }

        /// <summary>
        /// Страница
        /// </summary>
        public Guid Page { get; set; }

        /// <summary>
        /// Название страницы
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Секция
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// Ip-адрес
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Домен
        /// </summary>
        public Guid Site { get; set; }
    }
}
