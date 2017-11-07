using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Config
{
    public class TeamSettings
    {
        public TeamSettings()
        {
            TeamMembersSettings = new List<UserSetting>();
        }

        public int TeamId { get; set; }
        public int ReservedDaysForAllTeam { get; set; }

        public List<UserSetting> TeamMembersSettings { get; set; }
    }
}
