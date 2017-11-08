using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data
{
    public class IssueData
    {
        public string Key { get; set; }
        public string AssignedUserKey { get; set; }
        public string Status { get; set; }
        public string Summary { get; set; }

        public double TimeSpentHours { get; set; }
        public double OriginalEstimateHours { get; set; }
        public double StoryPoints { get;  set; }
        public string Rank { get; set; }
        public string ParentKey { get; internal set; }
        public double TimeSpentSeconds { get; internal set; }
        public double StoryPointCost
        {
            get
            {

                if (StoryPoints > 0)
                    return TimeSpentHours / StoryPoints;

                return 0;
            }
        }

    }
}
