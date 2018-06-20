using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageModule.Areas.Admin.Models
{
    public class MessagesViewModel : CoreViewModel
    {
        public MessagesModel Item { get; set; }
        public Paged<PaymentModel> List { get; set; }
    }
}