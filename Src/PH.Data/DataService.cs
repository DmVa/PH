using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PH.Config;
using Atlassian.Jira;
using RestSharp;
using PH.Data.JiraData;
using System.Threading;

namespace PH.Data
{
    public class DataService
    {
        private DataLoadSettings _loadSettings;
        public const string VIRTUAL_TEAMUSER_KEY = "team";
        public const string VIRTUAL_NOTASSIGNED_KEY = "Not Assigned";

        public void Init(DataLoadSettings loadSettings)
        {
            _loadSettings = loadSettings;
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
            var usersTask = jira.RestClient.ExecuteRequestAsync<List<UserTempo>>(Method.GET, $"/rest/tempo-teams/2/team/{_loadSettings.Team.TeamId}/member");
       
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

            result.Add(new UserData() { Key = VIRTUAL_NOTASSIGNED_KEY, DisplayName = "Not Assigned" });
            result.Add(new UserData() { Key = VIRTUAL_TEAMUSER_KEY, DisplayName = "Team" });
            
            return result;
        }

        public Task<List<UserData>> LoadUsersAsync()
        {
            return Task.Run(() => { return LoadUsers(); });
        }

        // key -user key
        public Dictionary<string, List<WorklogLine>> GetTeamWorklog(List<WorklogData> worklogs, List<IssueData> issues)
        {
            var result = new Dictionary<string, List<WorklogLine>>();
            var issueCache = new Dictionary<string, IssueData>();// key ->issue key
            if (issues != null)
            {
                foreach(var issue in issues)
                {
                    if (issueCache.ContainsKey(issue.Key))
                        issueCache.Add(issue.Key, issue);
                }
            }

            // create groups
            foreach (var worklog in worklogs)
            {
                AddWorklogToGroup(result, worklog, worklog.AuthorKey, issueCache);
                AddWorklogToGroup(result, worklog, VIRTUAL_TEAMUSER_KEY, issueCache);
            }

            return result;
        }

        public string GetAllUsersKey()
        {
            return VIRTUAL_TEAMUSER_KEY;
        }

        // key -user key
        public Dictionary<string, List<IssueUserStoryData>> GetTeamIssues(List<IssueData> issues)
        {
            Dictionary<string, List<IssueUserStoryData>> result = new Dictionary<string, List<IssueUserStoryData>>();
            List<string> users = issues.Select(x => x.AssignedUserKey).Distinct().ToList();
            foreach(var userName in users)
            {
                List<IssueUserStoryData> userdata = GetIssuessForUser(userName, false, issues);
                result.Add(userName, userdata);
            }

            List<IssueUserStoryData> data = GetIssuessForUser(VIRTUAL_TEAMUSER_KEY, true, issues);
            result.Add(VIRTUAL_TEAMUSER_KEY, data);
            return result;
        }

        private List<IssueUserStoryData> GetIssuessForUser(string userKey, bool isTotal, List<IssueData> issues)
        {
            List<IssueUserStoryData> result = new List<IssueUserStoryData>();
            List<IssueData> userStories;

            if (!isTotal)
            {
                userStories = new List<IssueData>();
                List<IssueData> hisUserStories = issues.Where(x => string.IsNullOrEmpty(x.ParentKey) && x.AssignedUserKey == userKey).Select(x => x).ToList();
                List<string> nothisUserStoriesKeys = issues.Where(x => !string.IsNullOrEmpty(x.ParentKey) && x.AssignedUserKey == userKey).Select(x => x.ParentKey).Distinct().ToList();
                List<IssueData> notHisUserStories = issues.Where(x => string.IsNullOrEmpty(x.ParentKey) && x.AssignedUserKey != userKey && nothisUserStoriesKeys.Contains(x.Key)).ToList();
                userStories.AddRange(hisUserStories);
                userStories.AddRange(notHisUserStories);
            }
            else
            {
               userStories = issues.Where(x => string.IsNullOrEmpty(x.ParentKey)).Select(x => x).ToList();
            }

            List<IssueUserStoryData> userStoriesData = new List<IssueUserStoryData>();
            foreach (var userStory in userStories.OrderBy(x=>x.Rank))
            {
                IssueUserStoryData data = CreateIssueUserStoryData(userStory);
                if (!isTotal)
                {
                    data.SubIssues = issues.Where(x => x.ParentKey == data.Key && x.AssignedUserKey ==userKey).ToList();
                }
                else
                {
                    data.SubIssues = issues.Where(x => x.ParentKey == data.Key).ToList();
                }

                data.CalcAgregateValues();
                userStoriesData.Add(data);
                result.Add(data);
            }
            
            IssueUserStoryData totalLine = new IssueUserStoryData();
            totalLine.Key = userKey;
            totalLine.Name = "Total";
            if (isTotal)
            {
                totalLine.Issue.StoryPoints = userStoriesData.Sum(x => x.Issue.StoryPoints);
                totalLine.StoryPointsBySubIssues = userStoriesData.Sum(x => x.StoryPointsBySubIssues);
                totalLine.SpentHoursBySubIssues = userStoriesData.Sum(x => x.SpentHoursBySubIssues);
            }
            else
            {
                totalLine.Issue.StoryPoints = userStoriesData.Where(x => x.Issue.AssignedUserKey == userKey).Sum(x => x.SpentHoursBySubIssues);
                totalLine.StoryPointsBySubIssues = userStoriesData.Sum(x => x.StoryPointsBySubIssues);
                totalLine.SpentHoursBySubIssues = userStoriesData.Sum(x => x.SpentHoursBySubIssues);

            }
            if (totalLine.StoryPointsBySubIssues > 0)
                totalLine.StoryPointCostBySubIssues = totalLine.SpentHoursBySubIssues / totalLine.StoryPointsBySubIssues;

            result.Add(totalLine);
            return result;
        }

      
        private IssueUserStoryData CreateIssueUserStoryData(IssueData issue)
        {
            IssueUserStoryData issueuserStory = new IssueUserStoryData();
            issueuserStory.Key = issue.Key;
            issueuserStory.Issue = issue;
            issueuserStory.Name = issue.Summary;
            return issueuserStory;
        }

        private void AddWorklogToGroup(Dictionary<string, List<WorklogLine>> result, WorklogData worklog, string userKey, Dictionary<string, IssueData> issueCache)
        {
            List<WorklogLine> userLines = null;
            if (result.ContainsKey(userKey))
            {
                userLines = result[userKey];
            }
            else
            {
                userLines = new List<WorklogLine>();
                result.Add(userKey, userLines);
            }

            var line = new WorklogLine();
            line.WorklogUserKey = userKey;
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

        public List<WorklogData> GetWorklog(DateTime dateFrom, DateTime dateTo)
        {
            var result = new List<WorklogData>();
            var jira = GetClient();
            string dateFromStr = dateFrom.ToString("yyyy-MM-dd");
            string dateToStr = dateTo.ToString("yyyy-MM-dd");
            var task = jira.RestClient.ExecuteRequestAsync<List<WorklogTempo>>(Method.GET, $"/rest/tempo-timesheets/3/worklogs?dateFrom={dateFromStr}&dateTo={dateToStr}&teamId={_loadSettings.Team.TeamId}");

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
            //result.AssignedUserKey = issue.Fields?.
            result.Status = issue.Fields?.Status?.Name ?? "";
            result.Summary = issue.Fields?.Summary;
            result.StoryPoints = issue.Fields?.Customfield_10004 ?? 0;
            result.Rank = issue.Fields?.Customfield_10011;
            if (issue.Fields?.Issuetype?.Subtask == true)
                result.ParentKey = issue.Fields?.Parent?.Key;

            result.AssignedUserKey = issue.Fields?.Assignee?.Key ?? VIRTUAL_NOTASSIGNED_KEY;
            result.OriginalEstimateHours = (double)(issue.Fields?.Customfield_10004 * 4);
            result.TimeSpentSeconds = (double)issue.Fields?.Timespent;
            result.TimeSpentHours = (double)(issue.Fields?.Timespent / 60 / 60);

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
                if (issuesPage != null)
                {
                    pages.Add(issuesPage);
                    isAllGet = (issuesPage.StartAt + issuesPage.MaxResults) >= issuesPage.Total;
                    startAt = startAt + issuesPage.MaxResults;
                }
                else
                {
                    isAllGet = true;
                }
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
            var jira = GetClient();
            var task = jira.RestClient.ExecuteRequestAsync<IssuesListJira>(Method.GET, $"/rest/api/2/search?startAt={startAt}&jql=Sprint={sprintid}");
            task.Wait();
            IssuesListJira issue = task.Result;
            return issue;
        }
    }
}
