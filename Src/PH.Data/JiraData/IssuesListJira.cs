using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data.JiraData
{
    public class IssuesListJira
    {
         public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }
        public List<IssueJira> Issues { get; set; }
    }
}
