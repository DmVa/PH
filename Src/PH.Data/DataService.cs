using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PH.Config;
using Atlassian.Jira;
using RestSharp;
using PH.Data.JiraData;

namespace PH.Data
{
    public class DataService
    {

        private ConfigManager _configManager;
        private DataLoadSettings _loadSettings;
        public void Init(PHConfigSection settings)
        {
            _configManager = new ConfigManager(settings);
            _configManager.Load();
            _loadSettings = _configManager.Settings;
        }

        public DateTime GetDefaultDate()
        {
            DateTime date = DateTime.Now.Date;
            // get previous friday;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    date = date.AddDays(-3); 
                    break;
                case DayOfWeek.Sunday:
                    date = date.AddDays(-2);
                    break;
                default:
                    date = date.AddDays(-1);
                    break;
            }

            return date;
        }
        private Jira GetClient()
        {
            var jira = Jira.CreateRestClient(_loadSettings.Credentials.UrlBase, _loadSettings.Credentials.UserName, _loadSettings.Credentials.UserPassword);
            return jira;
        }

      

        public List<UserData> LoadUsers()
        {
            var result = new List<UserData>();
            var jira = GetClient();
            var usersTask = jira.RestClient.ExecuteRequestAsync<List<UserTempo>>(Method.GET, $"/rest/tempo-teams/2/team/{_loadSettings.TeamId}/member");
       
            usersTask.Wait();
            foreach (var user in usersTask.Result)
            {
                if (!user.Member?.ActiveInJira ?? false)
                    continue;
                if (!string.IsNullOrEmpty(user.Membership?.DateToANSI))
                    continue;

                if (user.Member.ActiveInJira )
                {
                    var userData = new UserData();
                    userData.Key = user?.Member?.Key ?? "";
                    userData.DisplayName = user?.Member?.Displayname ?? "";
                    result.Add(userData);
                }
            }
            return result;
        }

        // key -user key
        public Dictionary<string, List<WorklogLine>> GetTeamWorklog(DateTime dateFrom, DateTime dateTo)
        {
            var result = new Dictionary<string, List<WorklogLine>>();
            var worklogs = GetWorklog(dateFrom, dateTo);
            var issueCache = new Dictionary<string, IssueData>();// key ->issue key
            foreach (var worklog in worklogs)
            {
                List<WorklogLine> userLines = null;
                if (result.ContainsKey(worklog.AuthorKey))
                {
                    userLines = result[worklog.AuthorKey];
                }
                else
                {
                    userLines = new List<WorklogLine>();
                    result.Add(worklog.AuthorKey, userLines);
                }
                var line = new WorklogLine();
                line.WorklogUserKey = worklog.AuthorKey;
                line.Worklog = worklog;
                IssueData issue = null;
                if (issueCache.ContainsKey(worklog.IssueKey))
                {
                    issue = issueCache[worklog.IssueKey];
                }
                else
                {
                    issue = GetIssue(worklog.IssueKey);
                    issueCache.Add(worklog.IssueKey, issue);
                }
                line.Issue = issue;
                userLines.Add(line);
            }
            return result;
        }

        public List<WorklogData> GetWorklog(DateTime dateFrom, DateTime dateTo)
        {
            var result = new List<WorklogData>();
            var jira = GetClient();
            string dateFromStr = dateFrom.ToString("yyyy-MM-dd");
            string dateToStr = dateTo.ToString("yyyy-MM-dd");
            var task = jira.RestClient.ExecuteRequestAsync<List<WorklogTempo>>(Method.GET, $"/rest/tempo-timesheets/3/worklogs?dateFrom={dateFromStr}&dateTo={dateToStr}&teamId={_loadSettings.TeamId}");

            task.Wait();
            foreach (var worklog in task.Result)
            {
                WorklogData data = new WorklogData();
                data.AuthorKey = worklog.Author?.Key;
                data.BookedComments = worklog.Comment;
                data.BookedHours = ((double)worklog.TimeSpentSeconds / 60 / 60).ToString("F1");
                data.IssueKey = worklog.Issue?.Key;
                data.WorklogDate = worklog.DateStarted.Substring(0, Math.Min(10, worklog.DateStarted.Length));
                result.Add(data);
            }
            return result;
        }

        public IssueData GetIssue(string key)
        {
            IssueData result = null;
            var jira = GetClient();
            var task = jira.RestClient.ExecuteRequestAsync<IssueJira>(Method.GET, $"/rest/api/2/issue/{key}");

            task.Wait();
            IssueJira issue = task.Result;
            result = IssueJiraToIssuesData(issue);
            

       
            return result;
        }

        private IssueData IssueJiraToIssuesData(IssueJira issue)
        {


            if (issue == null)
                return null;

            IssueData result = new IssueData();
            result.Key = issue.Key;
            result.Status = issue.Fields?.Status?.Name ?? "";
            result.Summary = issue.Fields?.Summary;
            result.OriginalEstimateHours = (issue.Fields?.Customfield_10004 * 4).ToString();
            result.TimeSpentHours = ((double)issue.Fields?.Timespent / 60 / 60).ToString("F1");

            return result;
        }

        public List<IssueData> GetIssues(int sprintid)
        {
            int startAt = 0;
            bool isAllGet = false;
            List<IssuesListJira> pages = new List<IssuesListJira>();
            while (!isAllGet)
            {
                IssuesListJira issuesPage = GetIssues(sprintid, startAt);
                pages.Add(issuesPage);
                bool isAllget = (issuesPage.StartAt + issuesPage.MaxResults) >= issuesPage.Total;
                startAt = startAt + issuesPage.MaxResults;
            }
            List<IssueData> result = new List<IssueData>();

            foreach (var issuePage in pages)
            {
                if (issuePage.Issues != null)
                {
                    foreach (var issue in issuePage.Issues)
                    {
                        IssueData dataItem = IssueJiraToIssuesData(issue);
                        if (dataItem != null)
                            result.Add(dataItem);
                    }

                }
            }

            return result;
        }

        private IssuesListJira GetIssues(int sprintid, int startAt)
        {
            IssuesListJira result = null;
            var jira = GetClient();
            var task = jira.RestClient.ExecuteRequestAsync<IssuesListJira>(Method.GET, $"/rest/api/2/search?startAt={startAt}jql=Sprint={sprintid}");

            task.Wait();

            IssuesListJira issue = task.Result;
          

            return result;
        }
    }
}
