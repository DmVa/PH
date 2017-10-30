using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data.JiraData
{
    public class IssueFieldsJira
    {
        public int Timespent { get; set; }
        public double  Customfield_10004 { get; set; }
        public string Summary { get; set; }
        public IssueStatusJira Status { get; set; }
    }
}
