using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Авторизованный пользователь
    /// </summary>
    public class AccountModel
    {
    }
    /// <summary>
    /// Домен
    /// </summary>
    public class DomainList
    {
        /// <summary>
        /// Сортировка
        /// </summary>
        public int Permit { get; set; }

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Название домена
        /// </summary>
        public string DomainName { get; set; }
    }
}
