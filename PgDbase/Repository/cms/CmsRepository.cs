using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase
{
    public class CmsRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;
        private Guid _currentUserId = Guid.Empty;
        private string _ip = string.Empty;
        private string _domain = string.Empty;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CmsRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }
        public CmsRepository(string ConnectionString, Guid UserId, string IP, string DomainUrl)
        {
            _context = ConnectionString;
            //_domain = (!string.IsNullOrEmpty(DomainUrl)) ? getSiteId(DomainUrl) : "";
            _ip = IP;
            _currentUserId = UserId;

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

    }
}
