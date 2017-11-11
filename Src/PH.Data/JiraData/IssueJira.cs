using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data.JiraData
{
    public class IssueJira
    {
        public string Self { get; set; }
        public string Key { get; set; }
        public IssueFieldsJira Fields {get;set;}
        public double? StoryPoint { get; internal set; }
        public string Rank { get; internal set; }
    }
}
