using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessagesModule.Areas.Admin.Models
{
    public class MessagesViewModel : CoreViewModel
    {
        public MessagesModel Item { get; set; }
        public Paged<MessagesModel> List { get; set; }
    }
}