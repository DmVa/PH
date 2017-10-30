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
        public string Status { get; set; }
        public string Summary { get; set; }

        public string TimeSpentHours { get; set; }
        public string OriginalEstimateHours { get; set; }

    }
}
