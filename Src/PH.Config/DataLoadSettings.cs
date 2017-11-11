using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Config
{
    public class DataLoadSettings
    {
        public DataLoadSettings()
        {
            Credentials = new CredentialSettings();
            Sprint = new SprintSettings();
            Team = new TeamSettings();
            Jira = new JiraSettings();
        }

        public CredentialSettings Credentials
        {
            get;set;
        }

        public SprintSettings Sprint
        {
            get; set;
        }

        public TeamSettings Team
        {
            get; set;
        }
        public JiraSettings Jira
        {
            get; set;
        }
    }
}
