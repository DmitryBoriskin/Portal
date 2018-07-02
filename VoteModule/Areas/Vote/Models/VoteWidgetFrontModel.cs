using PgDbase.Entity.modules.vote;

namespace VoteModule.Areas.Vote.Models
{
    public class VoteWidgetFrontModel
    {
        public VoteModel Item { get; set; }
        public AnswerAndStat VoteStat { get; set; }
    }
}