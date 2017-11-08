using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Data
{
    public class IssueUserStoryData
    {
        public IssueUserStoryData()
        {
            SubIssues = new List<IssueData>();
            Issue = new IssueData();
        }

        public string Key { get; set; }
        public string Name { get; set; }

        public IssueData Issue { get; set; }
        public List<IssueData> SubIssues { get; set; }
        public double StoryPointsBySubIssues { get; set; }
        public double SpentHoursBySubIssues { get; set; }
        public double StoryPointCostBySubIssues { get; set; }

        internal void CalcAgregateValues()
        {
            StoryPointsBySubIssues = SubIssues.Sum(x => x.StoryPoints);
            SpentHoursBySubIssues = SubIssues.Sum(x => x.TimeSpentSeconds) / 60 / 60;
            StoryPointCostBySubIssues = 0;
            if (StoryPointsBySubIssues > 0)
                StoryPointCostBySubIssues = SpentHoursBySubIssues / StoryPointsBySubIssues;
        }
    }
}
