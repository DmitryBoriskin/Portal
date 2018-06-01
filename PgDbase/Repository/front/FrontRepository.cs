using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с новостями
    /// </summary>
    public partial class FrontRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private Guid _currentUserId = Guid.Empty;

        /// <summary>
        /// ip-адрес
        /// </summary>
        private string _ip = string.Empty;

        /// <summary>
        /// Идентифкатор сайта
        /// </summary>
        private Guid _siteId;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FrontRepository()
        {
            _context = "defaultConnection";
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


        public FrontRepository(string connectionString, Guid siteId, string ip, Guid userId )
        {
            _context = connectionString;
            _siteId = siteId;
            _ip = ip;
            _currentUserId = userId;

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }


    }
}
