using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data
{
    public class WorklogData
    {
        public string AuthorKey { get; set; }
        public string IssueKey { get; set; }
        public string WorklogDate { get; set; }
        public string BookedHours { get; set; }
        public string BookedComments { get; set; }
    }
}
