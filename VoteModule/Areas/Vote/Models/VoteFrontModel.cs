using PgDbase.entity;
using PgDbase.Entity.modules.vote;
using Portal.Models;

namespace VoteModule.Areas.Vote.Models
{
    public class VoteFrontModel : LayoutFrontModel
    {        
        public VoteModel Item{get;set;}
        public Paged<VoteModel> List { get; set; }
    }
}