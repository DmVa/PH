using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data.JiraData
{
    public class UserTempo
    {
        public string Id { get; set; }
        public TeamMemberTempo Member { get; set; }
        public TeamMembershipTempo Membership { get; set; }
    }
}
