using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data.JiraData
{
   public  class WorklogTempo
    {
        public int TimeSpentSeconds;
        public string DateStarted { get; set; }
        public string Comment { get; set; }
        public int TempoWorklogId { get; set; }
        public int JiraWorklogId { get; set; }
        public AuthorTempo Author{ get; set; }
        public IssueTempo Issue { get; set; }
    }
}
