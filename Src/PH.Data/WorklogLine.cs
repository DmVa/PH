using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data
{
    public class WorklogLine
    {
        public WorklogLine()
        {
            Worklog = new WorklogData();
            Issue = new IssueData();
        }
        public string WorklogUserKey { get; set; }
        public WorklogData Worklog { get; set; }
        public IssueData  Issue { get; set; }
    }
}
