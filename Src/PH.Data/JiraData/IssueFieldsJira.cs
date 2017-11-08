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
        //storypoints;
        public double Customfield_10004 { get; set; }
        //rank
        public string Customfield_10011 { get; set; }
        public string Summary { get; set; }
        public IssueStatusJira Status { get; set; }
        public IssueTypeJira Issuetype { get; set; }
       public  IssueJira Parent { get; set; }
        public UserJira Assignee { get; set; }
    }
}
