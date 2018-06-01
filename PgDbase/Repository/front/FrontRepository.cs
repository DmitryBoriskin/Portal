using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PgDbase.Repository.front
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


        public FrontRepository(string connectionString, Guid siteId )
        {
            _context = connectionString;
            _siteId = siteId;

            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
        }

        /// <summary>
        /// фотогаллерея
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<core_photos> GetGallery(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                var data = db.core_photos.Where(w => w.f_album == id);
                if (data.Any())
                    return data.OrderBy(o => o.n_sort).ToList<core_photos>();
                else
                    return null;
            }
        }


    }
}
