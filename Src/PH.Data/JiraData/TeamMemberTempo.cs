using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data.JiraData
{
    public class TeamMemberTempo
    {
        public string TeamMemberId { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Displayname { get; set; }
        public bool ActiveInJira { get; set; }

    }
}
