using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PH.Config
{
    public class PHConfigSection :  ConfigurationSection
    {
        public const string CONFIG_SECTION_NAME = "phConfiguration";

        private const string SETTINGSFILE = "settingsFile";
        [ConfigurationProperty(SETTINGSFILE, DefaultValue = "", IsRequired = true)]
        public String ConfigFile
        {
            get { return (String)this[SETTINGSFILE]; }
            set { this[SETTINGSFILE] = value; }
        }
    }
}
