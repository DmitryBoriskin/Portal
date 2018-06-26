using PgDbase.entity;
using PgDbase.Entity.modules.vote;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VoteModule.Areas.Admin.Models
{
    public class VoteViewModel: CoreViewModel
    {
        public Paged<VoteModel> List { get; set; }
        public VoteModel Item { get; set; }
        public AnswerModel Answer { get; set; }
    }
}