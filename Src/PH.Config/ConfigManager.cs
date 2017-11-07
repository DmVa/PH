using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PH.Config
{
    public class ConfigManager
    {
        private DataLoadSettings _settings;
        private PHConfigSection _appConfig;
        public ConfigManager(PHConfigSection appConfig)
        {
            _appConfig = appConfig;
        }

        public void Init()
        {
            if (!ReadSettings())
                _settings = GetDefaultSettings();
        }

        public DataLoadSettings Settings
        {
            get { return _settings; }
        }
        public void SaveSettings()
        {
            string path = _appConfig.ConfigFile;
            XmlSerializer serializer = new XmlSerializer(typeof(DataLoadSettings));

            using (StreamWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, _settings);
            }
        }

        public bool ReadSettings()
        {
            string path = _appConfig.ConfigFile;
            if (!File.Exists(path))
                return false;

            XmlSerializer serializer = new XmlSerializer(typeof(DataLoadSettings));
            DataLoadSettings deserialized = null;
            using (StreamReader reader = new StreamReader(path))
            {
                 deserialized = serializer.Deserialize(reader) as DataLoadSettings;
            }

            if (deserialized == null)
            {

                _settings = null;
                return false;
            }

            _settings = deserialized;
            if (_settings.Sprint.DateFrom == DateTime.MinValue)
                _settings.Sprint.DateFrom = DateTime.Now.Date;
            if (_settings.Sprint.DateTo == DateTime.MinValue)
                _settings.Sprint.DateTo = DateTime.Now.Date;

            return true;
        }

        private DataLoadSettings GetDefaultSettings()
        {
            DataLoadSettings settings = new DataLoadSettings();
            settings.Credentials.UserName = "dmytro";
            settings.Credentials.UserPassword = "bobik_2015";
            settings.Credentials.UrlBase = "https://preciseres.atlassian.net";

            settings.Team.TeamId = 2;// Dev team
           // settings.StoryPointFieldId = 10004;
            settings.Sprint.SprintId = 70;
                settings.Sprint.DateFrom = DateTime.Now;
                settings.Sprint.DateTo = DateTime.Now;
                return settings;
        }
    }
}
