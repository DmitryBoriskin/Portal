using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Права пользователя
    /// </summary>
    public class UserResolution : Resolution
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Связь пользователя с сайтом
        /// </summary>
        public Guid UserSiteLink { get; set; }

        /// <summary>
        /// Контроллер
        /// </summary>
        public Guid SiteController { get; set; }
    }
}
