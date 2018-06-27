using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Entity.modules.vote
{
    public class VoteModel
    {
        public Guid Id { get; set; }

        public bool TypeMulti { get; set;}
        public bool Disabled { get; set; }        
        public bool Important { get; set; }

        /// <summary>
        /// если true показываем данные статистики иначе инфу для голосования
        /// </summary>
        public bool ShowStat { get; set; } 

        public string Title { get; set; }
        public string Text { get; set; }



        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public List<AnswerModel> List { get; set; }
        public List<AnswerAndStat> ListStat { get; set; }
    }
    public class AnswerModel
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Variant { get; set; }
        public int  Sort { get; set; }
    }
    public class AnswerAndStat : AnswerModel
    {
        public int AllCount { get; set; }
        public int CurrentCount { get; set; }
        public double Proc
        {
            get
            {
                return CurrentCount * 100 / AllCount;
            }            
        }
    }
}
