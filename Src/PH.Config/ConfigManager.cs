using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Config
{
    public class ConfigManager
    {
        private DataLoadSettings _settings;
        public ConfigManager(PHConfigSection appConfig)
        {

        }

        public void Load()
        {
            _settings = GetDefaultSettings();
        }

        public DataLoadSettings Settings
        {
            get { return _settings; }
        }
        
        private DataLoadSettings GetDefaultSettings()
        {
            DataLoadSettings settings = new DataLoadSettings();
            settings.Credentials.UserName = "dmytro";
            settings.Credentials.UserPassword = "bobik_2015";
            settings.Credentials.UrlBase = "https://preciseres.atlassian.net";
            settings.TeamId = 2;// Dev team
            settings.StoryPointFieldId = 10004;
            settings.SprintId = 70;
            return settings;
        }
    }
}
