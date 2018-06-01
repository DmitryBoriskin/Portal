using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Repository.front
{
    public partial class Repository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

    
        public Repository(string connectionString)
        {
            _context = connectionString;
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
