using PgDbase.entity;
using PgDbase.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Repository.cms
{
    public partial class CmsRepository
    {
        public Paged<MessagesModel> GetMessages(FilterModel filter)
        {
            Paged<MessagesModel> result = new Paged<MessagesModel>();
            using (var db = new CMSdb(_context))
            {
                var query = db.msg_messages.Where(w => w.f_parent == null && w.f_site==_siteId);

                query = query.OrderByDescending(o => o.b_date);

                var list = query
                .Skip(filter.Size * (filter.Page - 1))
                .Take(filter.Size)
                .Select(s => new MessagesModel
                {
                    Id = s.id,
                    //Date=s.d_d,
                    
                   
                }).ToArray();

            }
            return null;
        }
    }
}
